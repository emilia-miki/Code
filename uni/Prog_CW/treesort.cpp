#include <iostream>

struct Node
{
    int key;
    Node *left, *right;
    Node(int key);
    void insert(int key);
    void store(int *arr, int &i);
};

Node::Node(int k)
{
    key = k;
    left = right = nullptr;
}

void printTree(Node *node, int level)
{
    if (node == nullptr)
    {
        return;
    }

    printTree(node->right, level + 1);
    for (int i = 0; i < level; i++)
    {
        std::cout << '\t';
    }
    std::cout << node->key << std::endl;
    printTree(node->left, level + 1);
}

void Node::insert(int k)
{
    if (k < key)
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

void Node::store(int *arr, int &i)
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

void treesort()
{
    int len;
    std::cout << "How long is your array of integers?\n";
    std::cin >> len;
    if (std::cin.fail())
    {
        std::cout << "Error: expected an integer\n";
        exit(EXIT_FAILURE);
    }
    int *array = new int[len];
    std::cout << "Enter the array:\n";
    for (int i = 0; i < len; i++)
    {
        std::cin >> array[i];
        if (std::cin.fail())
        {
            std::cout << "Error: expected an integer\n";
            exit(EXIT_FAILURE);
        }
    }

    Node *root = new Node(array[0]);
    for (int i = 1; i < len; i++)
    {
        root->insert(array[i]);
    }

    std::cout << "\nHere's the tree:\n";
    printTree(root, 0);

    int i = 0;
    root->store(array, i);

    std::cout << "Sorted array:\n";
    for (int i = 0; i < len; i++)
    {
        std::cout << array[i] << " ";
    }
    std::cout << std::endl;
}