#ifndef _HUFFMAN_CODING_HPP
#define _HUFFMAN_CODING_HPP

#include <unordered_map>
#include <bitset>
#include <vector>

struct bits
{
    short bits;
    char length;
};

struct TreeNode
{
    TreeNode(std::pair<char, int> p);
    std::pair<char, int> data;
    TreeNode *right;
    TreeNode *left;
};

class PriorityQueue
{
    std::vector<TreeNode *> vec;

public:
    //PriorityQueue();
    //~PriorityQueue();
    int parent(int index);
    int left(int index);
    int right(int index);
    void minHeapify(int index);
    void insert(TreeNode *element);
    TreeNode *minimum();
    TreeNode *extractMin();

    TreeNode *constructHuffmanTree();
    std::unordered_map<char, bits> *getHuffmanEncodings();

private:
    void recursiveEncode(TreeNode *node, bits *b, std::unordered_map<char, bits> *map);
};

#endif