#ifndef _BST_HPP
#define _BST_HPP

#include <string>

using namespace std;

struct Student
{
    string name;
    double score;
    double willingness;
    int copiedWorks;

    Student();
};

bool operator<(const Student &ls, const Student &rs);

enum Color
{
    BLACK,
    RED
};

template <typename T>
struct Node
{
    T data;
    Node *left, *right, *p;
    Color color;
};

template <typename T>
class BinarySearchTree;

template <typename T>
struct three
{
    BinarySearchTree<T> L;
    bool obj;
    BinarySearchTree<T> R;
};

template <typename T>
class BinarySearchTree
{
    static Node<T> *nil;
    Node<T> *root = nil;
    int treeSize = 0;
    void inorderTreeWalk(Node<T> *x);
    void transplant(Node<T> *u, Node<T> *v);
    static Node<T> *treeMinimum(Node<T> *x);
    static Node<T> *treeMaximum(Node<T> *x);
    static int getBlackHeight(Node<T> *x);
    int treeHeight(Node<T> *x, int h);
    void treeFindInRange(Node<T> *x, T &minObj, T &maxObj, int &n);
    void leftRotate(Node<T> *x);
    void rightRotate(Node<T> *x);
    void insertFixup(Node<T> *x);
    void eraseFixup(Node<T> *x);
    void inorderSizeWalk(Node<T> *x);
    void updateSize();
    static three<T> __split(BinarySearchTree<T> tree, T &obj);  
    static BinarySearchTree<T> join(BinarySearchTree<T> tree1, T &obj, BinarySearchTree<T> tree2);

public:
    void insert(T &obj);
    Node<T> *find(T &obj);
    void erase(T &obj);
    int size();
    void print();
    int height();
    int findInRange(T &minObj, T &maxObj);
    void eraseRange(T &minObj, T &maxObj);
    static BinarySearchTree<T> merge(BinarySearchTree<T> &tree1, BinarySearchTree<T> &tree2);
    static pair<BinarySearchTree<T>, BinarySearchTree<T>> split(BinarySearchTree<T> &tree, T &obj);
};

#endif