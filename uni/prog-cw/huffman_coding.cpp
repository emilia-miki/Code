#include <iostream>
#include <fstream>
#include <filesystem>
#include "huffman_coding.hpp"

using namespace std;

const string kEncodedFileName = "encoded_file";
const string kDecodedFileName = "decoded_file";
const char kMagicCode[3] = {'h', 'u', 'f'};

void writeInt(int &number, ofstream &os)
{
    os.write(reinterpret_cast<char *>(&number), 4);
}

int readInt(ifstream &is)
{
    int number;
    is.read(reinterpret_cast<char *>(&number), 4);
    return number;
}

void writeShort(short &number, ofstream &os)
{
    os.write(reinterpret_cast<char *>(&number), 2);
}

void encode(ifstream &is, ofstream &os, unordered_map<char, bits> *encodings, int file_length)
{
    char outbyte;
    char length = 0;
    char ptr;
    bits current;

    for (auto c : kMagicCode)
        os << c;

    writeInt(file_length, os);
    int tablesize = encodings->size();
    writeInt(tablesize, os);
    for (auto el : *encodings)
    {
        os << el.first;
        writeShort(el.second.bits, os);
        os << el.second.length;
    }

    static int st = 0;
    outbyte = 0;
    char *buf = new char[file_length];
    char *obuf = new char[file_length];
    int optr = 0;
    is.read(buf, file_length);
    for (int i = 0; i < file_length; i++)
    {
        ptr = 0;
        current = encodings->at(buf[i]);
        st++;
        while (ptr < current.length)
        {
            outbyte |= (int((bool) (current.bits & (1 << (15 - ptr)))) << (7 - length));
            ptr++;
            length++;
            if (length == 8)
            {
                obuf[optr] = outbyte;
                optr++;
                outbyte = 0;
                length = 0;
            }
        }
    }
    obuf[optr] = outbyte;
    optr++;
    os.write(obuf, optr);
    delete[] buf;
    delete[] obuf;
}

int decode(ifstream &is, ofstream &os, int file_length)
{
    int length;
    int tablesize;
    char inbyte;
    TreeNode *tree = new TreeNode(pair<char, int>(0, 0));
    TreeNode *currentTreeNode = tree;
    short byte;
    char bytelen;

    for (auto c : kMagicCode)
    {
        is >> inbyte;
        if (inbyte != c)
            return -1;
    }

    length = readInt(is);
    tablesize = readInt(is);
    char *table = new char[tablesize * 4];
    is.read(table, tablesize * 4);

    char c;
    int st = 0;
    for (int i = 0; i < tablesize; i++)
    {
        c = table[i * 4 + 0];
        byte = *reinterpret_cast<short *>(&table[i * 4 + 1]);
        bytelen = table[i * 4 + 3];
        
        for (int j = 0; j < bytelen; j++)
        {
            if (byte & (1 << (15 - j)))
            {
                if (currentTreeNode->right == nullptr)
                    {currentTreeNode->right = new TreeNode(pair<char, int>(0, 0)); st++;}
                currentTreeNode = currentTreeNode->right;
            }
            else
            {
                if (currentTreeNode->left == nullptr)
                    {currentTreeNode->left = new TreeNode(pair<char, int>(0, 0)); st++;}
                currentTreeNode = currentTreeNode->left;
            }
        }
        currentTreeNode->data.first = c;
        currentTreeNode = tree;
    }
    delete[] table;

    int infolength = file_length - sizeof(kMagicCode) - 2 * 4 - tablesize * 4;
    char *buf = new char[infolength];
    char *obuf = new char[length];
    int original_length = length;
    int ptr = 0;
    is.read(buf, infolength);
    for (int i = 0; i < infolength; i++)
    {
        for (int j = 7; (j >= 0) && (length > 0); j--)
        {
            if (buf[i] & (1 << j))
            {
                if (currentTreeNode->right == nullptr)
                {
                    obuf[ptr] = currentTreeNode->data.first;
                    ptr++;
                    length--;
                    currentTreeNode = tree->right;
                }
                else
                {
                    currentTreeNode = currentTreeNode->right;
                }
            }
            else
            {
                if (currentTreeNode->left == nullptr)
                {
                    obuf[ptr] = currentTreeNode->data.first;
                    ptr++;
                    length--;
                    currentTreeNode = tree->left;
                }
                else
                {
                    currentTreeNode = currentTreeNode->left;
                }
            }
        }
    }
    if (length != 0) return -1;
    os.write(obuf, original_length);
    delete[] buf;
    delete[] obuf;
    return 0;
}

void printHuffmanTree(TreeNode *node, int level)
{
    if (node == nullptr)
    {
        return;
    }

    printHuffmanTree(node->right, level + 1);
    for (int i = 0; i < level; i++)
    {
        std::cout << '\t';
    }
    std::cout << node->data.first << " (" << node->data.second << ")" << std::endl;
    printHuffmanTree(node->left, level + 1);
}

void huffman_coding()
{
    unordered_map<char, int> map;

    string filename;
    cout << "Enter the name of the file to be encoded:\n";
    cin.get();
    getline(cin, filename);

    // open the original file
    ifstream originalFile;
    originalFile.open(filename);
    if (!originalFile.is_open())
    {
        cout << "Error: this file does not exist.\n";
        return;
    }
    int file_length = filesystem::file_size(filename);
    cout << filename << " is open\n";
    cout << file_length << endl;

    // read the file byte by byte and count frequencies of each byte
    char *buf;
    buf = new char[file_length];
    originalFile.read(buf, file_length);
    for (int i = 0; i < file_length; i++)
    {
        if (!map.count(buf[i]))
        {
            map.insert(pair<char, int>(buf[i], 1));
        }
        else
        {
            map.at(buf[i])++;
        }
    }
    delete[] buf;
    cout << "File read and mapped. " << map.size() << " unique bytes.\n";

    // transfer all the resulting bytes and frequencies into a priority queue
    PriorityQueue queue;
    TreeNode *node;
    for (auto el : map)
    {
        node = new TreeNode(el);
        queue.insert(node);
    }
    cout << "Priority queue created.\n";

    // construct Huffman tree and get the encodings in a hash table
    queue.constructHuffmanTree();
    cout << "Huffman tree constructed:\n";
    printHuffmanTree(queue.minimum(), 0);
    unordered_map<char, bits> *encodings = queue.getHuffmanEncodings();
    cout << "Huffman encodings read.\n";

    // go to the beginning of the file to read it again (for encoding)
    originalFile.clear();
    originalFile.seekg(0, originalFile.beg);

    // open for writing the future encoded file
    ofstream encodedFileO;
    encodedFileO.open(kEncodedFileName);
    if (!encodedFileO.is_open())
    {
        cout << "Error opening encoded file stream.\n";
        return;
    }

    // encode the original file into the encoded file
    encode(originalFile, encodedFileO, encodings, file_length);
    cout << "File encoded.\n";
    originalFile.close();
    encodedFileO.close();

    // open encodedFile for reading and decodedFile for writing
    ifstream encodedFile;
    encodedFile.open(kEncodedFileName);
    if (!encodedFile.is_open())
    {
        cout << "Error opening encoded file.\n";
        return;
    }
    int encodedFileSize = filesystem::file_size(kEncodedFileName);

    ofstream decodedFile;
    decodedFile.open(kDecodedFileName);
    if (!decodedFile.is_open())
    {
        cout << "Error opening decoded file.\n";
        return;
    }

    // decode the file back
    if (decode(encodedFile, decodedFile, encodedFileSize) != -1)
        cout << "File decoded.\n";
    else
        cout << "Invalid encoded file.\n";
    encodedFile.close();
    decodedFile.close();
}