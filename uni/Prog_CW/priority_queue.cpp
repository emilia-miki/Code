#include <algorithm>
#include "huffman_coding.hpp"

TreeNode::TreeNode(std::pair<char, int> p)
{
    right = nullptr;
    left = nullptr;
    data = p;
}

int PriorityQueue::parent(int i)
{
    return (i - 1) / 2;
}

int PriorityQueue::left(int i)
{
    return 2 * i + 1;
}

int PriorityQueue::right(int i)
{
    return 2 * i + 2;
}

void PriorityQueue::minHeapify(int i)
{
    int l = left(i);
    int r = right(i);
    int smallest;
    if ((l < vec.size()) && (vec[l]->data.second < vec[i]->data.second))
        smallest = l;
    else
        smallest = i;
    if ((r < vec.size()) && (vec[r]->data.second < vec[smallest]->data.second))
        smallest = r;
    if (smallest != i)
    {
        std::swap(vec[i], vec[smallest]);
        minHeapify(smallest);
    }
}

void PriorityQueue::insert(TreeNode *element)
{
    vec.push_back(element);
    int i = vec.size() - 1;
    while ((i > 0) && (vec[parent(i)]->data.second > vec[i]->data.second))
    {
        std::swap(vec[i], vec[parent(i)]);
        i = parent(i);
    }
}

TreeNode *PriorityQueue::minimum()
{
    return vec[0];
}

TreeNode *PriorityQueue::extractMin()
{
    if (vec.size() < 1)
    {
        return nullptr;
    }
    
    TreeNode *min = vec[0];
    vec[0] = vec[vec.size() - 1];
    vec.resize(vec.size() - 1);
    minHeapify(0);
    return min;
}

TreeNode *PriorityQueue::constructHuffmanTree()
{
    TreeNode *node;
    TreeNode *first;
    TreeNode *second;
    while (vec.size() > 1)
    {
        first = extractMin();
        second = extractMin();
        node = new TreeNode(std::pair<char, int>(0, first->data.second + second->data.second));
        node->left = first;
        node->right = second;
        insert(node);
    }
    return minimum();
}

std::unordered_map<char, bits> *PriorityQueue::getHuffmanEncodings()
{
    std::unordered_map<char, bits> *map = new std::unordered_map<char, bits>();

    TreeNode *currentNode = vec[0];
    bits *b = new bits;
    b->bits = 0;
    b->length = 0;

    // can the bitset length be more than 8?
    recursiveEncode(vec[0], b, map);

    return map;
}

void PriorityQueue::recursiveEncode(TreeNode *node, bits *b, std::unordered_map<char, bits> *map)
{
    if ((node->left == nullptr) && (node->right == nullptr))
    {
        map->insert(std::pair<char, bits>(node->data.first, *b));
        return;
    }
    if (node->left != nullptr)
    {
        b->length++;
        recursiveEncode(node->left, b, map);
        b->length--;
    }
    if (node->right != nullptr)
    {
        b->bits |= (1 << (15 - b->length));
        b->length++;
        recursiveEncode(node->right, b, map);
        b->length--;
        b->bits &= ~(1 << (15 - b->length));
    }
    return;
}