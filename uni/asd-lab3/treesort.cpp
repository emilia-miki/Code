#include "priority_queue.hpp"

struct Node
{
    Homework *key;
    Node *left, *right;
    Node(Homework *key);
    void insert(Homework *key);
    void store(Homework **arr, int &i);
};

Node::Node(Homework *k)
{
    key = k;
    left = nullptr;
    right = nullptr;
}

void Node::insert(Homework *k)
{
    if (*k < *key)
    {
        if (left == nullptr)
        {
            left = new Node(k);
        }
        else
        {
            left->insert(k);
        }
    }
    else
    {
        if (right == nullptr)
        {
            right = new Node(k);
        }
        else
        {
            right->insert(k);
        }
    }
}

void Node::store(Homework **arr, int &i)
{
    if (left != nullptr)
    {
        left->store(arr, i);
    }
    arr[i] = key;
    i++;
    if (right != nullptr)
    {
        right->store(arr, i);
    }
}

void treesort(std::vector<Homework *> &array)
{
    int len = array.size();

    Node *root = new Node(array[0]);
    for (int i = 1; i < len; i++)
    {
        root->insert(array[i]);
    }

    int i = 0;
    root->store(&array[0], i);
}