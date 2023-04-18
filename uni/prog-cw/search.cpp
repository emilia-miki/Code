#include <iostream>
#include <vector>

using namespace std;

enum Color
{
    BLACK, RED
};

struct Node
{
    Node *left, *p, *right;
    int data;
    enum Color color;
    Node();
};

Node* nil = new Node();
Node* root = nil;

void printRBTree(Node *node, int level)
{
    if (node == nil)
    {
        return;
    }

    printRBTree(node->right, level + 1);
    for (int i = 0; i < level; i++)
    {
        cout << '\t';
    }
    cout << node->data << endl;
    printRBTree(node->left, level + 1);
}

Node::Node()
{
    left = nil;
    right = nil;
    p = nil;
    color = BLACK;
    data = 0;
}

Node* find(int obj)
{
    Node* x = root;
    while ((x != nil) && (x->data != obj))
    {
        if (obj < x->data)
        {
            x = x->left;
        }
        else
        {
            x = x->right;
        }
    }
    if (x == nil)
    {
        x = nullptr;
    }
    return x;
}

void inOrderTreeWalk(Node* x, int num1, int num2, vector<int>& vec)
{
    if (x == nil)
    {
        return;
    }

    if (x->left != nil && x->data >= num1)
    {
        inOrderTreeWalk(x->left, num1, num2, vec);
    }

    if (x->data >= num1 && x->data <= num2)
    {
        vec.push_back(x->data);
    }

    if (x->right != nil && x->data <= num2)
    {
        inOrderTreeWalk(x->right, num1, num2, vec);
    }
}

vector<int> findInRange(int num1, int num2)
{
    vector<int> vec;
    inOrderTreeWalk(root, num1, num2, vec);
    return vec;
}

void leftRotate(Node* x)
{
    if ((x == nil) || (x->right == nil))
    {
        return;
    }
    Node* node = x->right;
    x->right = node->left;
    if (node->left != nil)
    {
        node->left->p = x;
    }
    node->p = x->p;
    if (x->p == nil)
    {
        root = node;
    }
    else if (x == x->p->left)
    {
        x->p->left = node;
    }
    else
    {
        x->p->right = node;
    }
    node->left = x;
    x->p = node;
}

void rightRotate(Node* x)
{
    if ((x == nil) || (x->left == nil))
    {
        return;
    }
    Node* node = x->left;
    x->left = node->right;
    if (node->right != nil)
    {
        node->right->p = x;
    }
    node->p = x->p;
    if (x->p == nil)
    {
        root = node;
    }
    else if (x == x->p->right)
    {
        x->p->right = node;
    }
    else
    {
        x->p->left = node;
    }
    node->right = x;
    x->p = node;
}

void insertFixup(Node* x)
{
    Node* uncle;
    while (x->p->color == RED)
    {
        if (x->p == x->p->p->left)
        {
            uncle = x->p->p->right;
            if (uncle->color == RED)
            {
                x->p->color = BLACK;
                uncle->color = BLACK;
                x->p->p->color = RED;
                x = x->p->p;
            }
            else
            {
                if (x == x->p->right)
                {
                    x = x->p;
                    leftRotate(x);
                }
                x->p->color = BLACK;
                x->p->p->color = RED;
                rightRotate(x->p->p);
            }
        }
        else if (x->p == x->p->p->right)
        {
            uncle = x->p->p->left;
            if (uncle->color == RED)
            {
                x->p->color = BLACK;
                uncle->color = BLACK;
                x->p->p->color = RED;
                x = x->p->p;
            }
            else
            {
                if (x == x->p->left)
                {
                    x = x->p;
                    rightRotate(x);
                }
                x->p->color = BLACK;
                x->p->p->color = RED;
                leftRotate(x->p->p);
            }
        }
    }
    root->color = BLACK;
}

void insert(int obj)
{
    if (find(obj))
    {
        return;
    }

    Node* node, * prevNode;
    Node* newNode = new Node;
    newNode->left = nil;
    newNode->right = nil;
    newNode->p = nil;
    newNode->data = obj;
    newNode->color = RED;
    prevNode = nil;
    node = root;
    if (node == nil)
    {
        root = new Node;
        root->color = BLACK;
        root->left = nil;
        root->right = nil;
        root->p = nil;
        root->data = obj;
        return;
    }
    while (node != nil)
    {
        prevNode = node;
        if (newNode->data < node->data)
        {
            node = node->left;
        }
        else
        {
            node = node->right;
        }
    }
    newNode->p = prevNode;
    if (prevNode == nil)
    {
        root = newNode;
    }
    else if (newNode->data < prevNode->data)
    {
        prevNode->left = newNode;
    }
    else
    {
        prevNode->right = newNode;
    }
    newNode->color = RED;
    insertFixup(newNode);
}

void search()
{
    vector<int> vec;
    std::cout << "Enter the number of elements in your tree: ";
    int size;
    std::cin >> size;
    if (std::cin.fail())
    {
        std::cout << "Error: expected an integer\n";
        return;
    }
    std::cout << "Enter the numbers: ";
    int num, num1, num2;
    for (int i = 0; i < size; i++)
    {
        std::cin >> num;
        if (std::cin.fail())
        {
            std::cout << "Error: expected an integer\n";
            return;
        }
        insert(num);
    }
    std::cout << "\nHere's the tree:\n";
    printRBTree(root, 0);

    char c;
    while (true)
    {
        std::cout << "0. Back to main menu\n";
        std::cout << "1. Find a number\n";
        std::cout << "2. Find all numbers in range\n";
        std::cin >> c;
        switch (c)
        {
        case '0':
            return;
            break;
        case '1':
            std::cout << "Enter a number: ";
            std::cin >> num;
            if (std::cin.fail())
            {
                std::cout << "Error: expected an integer\n";
                exit(EXIT_FAILURE);
            }
            if (find(num))
            {
                std::cout << "The number is found\n";
            }
            else
            {
                std::cout << "The number is not found\n";
            }
            break;
        case '2':
            std::cout << "Enter the range (for ex. \"3 7\"): ";
            std::cin >> num1 >> num2;
            if (std::cin.fail())
            {
                std::cout << "Error: expected an integer\n";
                exit(EXIT_FAILURE);
            }
            vec = findInRange(num1, num2);
            for (int i : vec)
            {
                std::cout << i << " ";
            }
            std::cout << std::endl;
            break;
        default:
            std::cout << "Invalid choice\n";
            break;
        }
    }
}