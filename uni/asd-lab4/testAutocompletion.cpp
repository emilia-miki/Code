#include <iostream>
#include <fstream>
#include <vector>
#include <unordered_map>

using namespace std;

const string filename = "words_alpha.txt";

class Node
{
    bool isLeaf;
    unordered_map<char, Node *> children;

    void findRecursively(Node *x, string word, vector<string> &vec)
    {
        for (pair<char, Node *> p : x->children)
        {
            if (p.second->isLeaf)
            {
                vec.push_back(word + p.first);
            }
            findRecursively(p.second, word + p.first, vec);
        }
    }

public:
    void insert(string word)
    {
        Node *buf;
        Node *node = this;
        for (int i = 0; i < word.length(); i++)
        {
            if (!node->children.count(word[i]))
            {
                buf = new Node;
                node->children.insert(pair<char, Node *>(word[i], buf));
                node = buf;
            }
            else
            {
                node = node->children.find(word[i])->second;
            }
        }
        node->isLeaf = true;
    }

    vector<string> findByPrefix(string prefix)
    {
        vector<string> vec;
        string word;

        Node *node = this;
        for (int i = 0; i < prefix.length(); i++)
        {
            if (node->children.count(prefix[i]))
            {
                node = node->children.find(prefix[i])->second;
            }
            else
            {
                return vec;
            }
        }

        findRecursively(node, word, vec);

        return vec;
    }
};

void testAutocompletion()
{
    string str;
    vector<string> vec;
    Node trie;

    cout << "Please, wait. Loading dictionary..." << endl;
    ifstream file(filename, ios::in);
    while (file >> str)
    {
        trie.insert(str);
    }
    file.close();
    cout << "The dictionary is loaded." << endl;

    cout << "Enter any non-letter symbol to exit program." << endl;
    while (true)
    {
        cout << "> ";
        cin >> str;
        for (char ch : str)
        {
            if (!isalpha(ch))
            {
                return;
            }
        }
        vec = trie.findByPrefix(str);
        cout << "> ";
        if (vec.size() > 0)
        {
            for (int i = 0; i < vec.size() - 1; i++)
            {
                cout << str + vec[i] << ", ";
            }
            cout << str + vec[vec.size() - 1];
        }
        cout << endl;
    }
}