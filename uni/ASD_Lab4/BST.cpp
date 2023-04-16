#include <cstdlib>
#include <string>
#include <iostream>
#include "BST.hpp"

using namespace std;

string generateRandString(int min_length, int max_length);

Student::Student()
{
    name = generateRandString(4, 12);
    score = (double)rand() / RAND_MAX;
    willingness = (double)rand() / RAND_MAX;
    copiedWorks = rand() % 20;
}

bool operator<(const Student &ls, const Student &rs)
{
    return ls.name < rs.name;
}

bool operator>(const Student &ls, const Student &rs)
{
    return ls.name > rs.name;
}

bool operator<=(const Student &ls, const Student &rs)
{
    return !(ls > rs);
}

bool operator>=(const Student &ls, const Student &rs)
{
    return !(ls < rs);
}

bool operator==(const Student &ls, const Student &rs)
{
    return ls.name == rs.name;
}

bool operator!=(const Student &ls, const Student &rs)
{
    return !(ls.name == rs.name);
}

ostream &operator<<(ostream &os, const Student &st)
{
    os << st.name << endl;
    return os;
}

template <typename T>
Node<T> *BinarySearchTree<T>::nil = new Node<T>;

template <typename T>
void BinarySearchTree<T>::leftRotate(Node<T> *x)
{
    if ((x == nil) || (x->right == nil))
    {
        return;
    }
    Node<T> *node = x->right;
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

template <typename T>
void BinarySearchTree<T>::rightRotate(Node<T> *x)
{
    if ((x == nil) || (x->left == nil))
    {
        return;
    }
    Node<T> *node = x->left;
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

template <typename T>
void BinarySearchTree<T>::insert(T &obj)
{
    if (find(obj))
    {
        return;
    }

    Node<T> *node, *prevNode;
    Node<T> *newNode = new Node<T>;
    newNode->left = nil;
    newNode->right = nil;
    newNode->p = nil;
    newNode->data = obj;
    prevNode = nil;
    node = root;
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

    treeSize++;
}

template <typename T>
void BinarySearchTree<T>::insertFixup(Node<T> *x)
{
    Node<T> *uncle;
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

template <typename T>
Node<T> *BinarySearchTree<T>::find(T &obj)
{
    Node<T> *x = root;
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

template <typename T>
void BinarySearchTree<T>::transplant(Node<T> *u, Node<T> *v)
{
    if (u->p == nil)
    {
        root = v;
    }
    else if (u == u->p->left)
    {
        u->p->left = v;
    }
    else
    {
        u->p->right = v;
    }

    v->p = u->p;

    if (v != nil)
    {
        v->p = u->p;
    }
}

template <typename T>
Node<T> *BinarySearchTree<T>::treeMinimum(Node<T> *x)
{
    while (x->left != nil)
    {
        x = x->left;
    }
    return x;
}

template <typename T>
Node<T> *BinarySearchTree<T>::treeMaximum(Node<T> *x)
{
    while (x->right != nil)
    {
        x = x->right;
    }
    return x;
}

template <typename T>
void BinarySearchTree<T>::erase(T &obj)
{
    Node<T> *x = find(obj);
    if (x == nullptr)
    {
        return;
    }
    if (x->left == nil)
    {
        transplant(x, x->right);
    }
    else if (x->right == nil)
    {
        transplant(x, x->left);
    }
    else
    {
        Node<T> *min = treeMinimum(x->right);
        if (min->p != x)
        {
            transplant(min, min->right);
            min->right = x->right;
            min->right->p = min;
        }
        transplant(x, min);
        min->left = x->left;
        min->left->p = min;
    }
    delete x;
    treeSize--;
}

template <typename T>
void BinarySearchTree<T>::eraseFixup(Node<T> *x)
{
    Node<T> *sibling;
    while ((x != root) && (x->color == BLACK))
    {
        if (x == x->p->left)
        {
            sibling = x->p->right;
            if (sibling->color == RED)
            {
                sibling->color = BLACK;
                x->p->color = RED;
                leftRotate(x->p);
                sibling = x->p->right;
            }
            if ((sibling->left->color == BLACK) && (sibling->right->color == BLACK))
            {
                sibling->color = RED;
                x = x->p;
            }
            else
            {
                if (sibling->right->color == BLACK)
                {
                    sibling->left->color = BLACK;
                    sibling->color = RED;
                    rightRotate(sibling);
                    sibling = x->p->right;
                }
                sibling->color = x->p->color;
                x->p->color = BLACK;
                sibling->right->color = BLACK;
                leftRotate(x->p);
                x = root;
            }
        }
        else if (x == x->p->right)
        {
            sibling = x->p->left;
            if (sibling->color == RED)
            {
                sibling->color = BLACK;
                x->p->color = RED;
                rightRotate(x->p);
                sibling = x->p->left;
            }
            if ((sibling->left->color == BLACK) && (sibling->right->color == BLACK))
            {
                sibling->color = RED;
                x = x->p;
            }
            else
            {
                if (sibling->left->color == BLACK)
                {
                    sibling->right->color = BLACK;
                    sibling->color = RED;
                    leftRotate(sibling);
                    sibling = x->p->left;
                }
                sibling->color = x->p->color;
                x->p->color = BLACK;
                sibling->left->color = BLACK;
                rightRotate(x->p);
                x = root;
            }
        }
    }
    x->color = BLACK;
}

template <typename T>
int BinarySearchTree<T>::size()
{
    return treeSize;
}

template <typename T>
void BinarySearchTree<T>::inorderTreeWalk(Node<T> *x)
{
    if (x != nil)
    {
        inorderTreeWalk(x->left);
        printf(", %s", x->data.name.c_str());
        inorderTreeWalk(x->right);
    }
}

template <typename T>
void BinarySearchTree<T>::inorderSizeWalk(Node<T> *x)
{
    if (x != nil)
    {
        treeSize++;
        inorderSizeWalk(x->left);
        inorderSizeWalk(x->right);
    }
}

template <typename T>
void BinarySearchTree<T>::updateSize()
{
    treeSize = 0;
    if (root != nil)
    {
        treeSize++;
        inorderSizeWalk(root->left);
        inorderSizeWalk(root->right);
    }
}

template <typename T>
void BinarySearchTree<T>::print()
{
    if (root == nil)
    {
        return;
    }
    printf("%s", root->data.name.c_str());
    inorderTreeWalk(root->left);
    inorderTreeWalk(root->right);
    cout << endl;
}

template <typename T>
int BinarySearchTree<T>::treeHeight(Node<T> *x, int h)
{
    if (x == nil)
    {
        return h;
    }
    int l = treeHeight(x->left, h + 1);
    int r = treeHeight(x->right, h + 1);
    return ((r > l) ? r : l);
}

template <typename T>
int BinarySearchTree<T>::height()
{
    if (root == nil)
    {
        return 0;
    }
    return treeHeight(root, 1);
}

template <typename T>
void BinarySearchTree<T>::treeFindInRange(Node<T> *x, T &minObj, T &maxObj, int &n)
{
    if (x == nil)
    {
        return;
    }
    if (minObj < x->data)
    {
        treeFindInRange(x->left, minObj, maxObj, n);
    }
    if ((minObj <= x->data) && (x->data <= maxObj))
    {
        n++;
    }
    if (x->data < maxObj)
    {
        treeFindInRange(x->right, minObj, maxObj, n);
    }
}

template <typename T>
int BinarySearchTree<T>::findInRange(T &minObj, T &maxObj)
{
    int n = 0;
    treeFindInRange(root, minObj, maxObj, n);
    return n;
}

template <typename T>
void BinarySearchTree<T>::eraseRange(T &minObj, T &maxObj)
{
    auto p = split(*this, minObj);
    BinarySearchTree<T> l = p.first;
    auto t = __split(p.second, maxObj);
    BinarySearchTree<T> r = t.R;
    *this = merge(l, r);
}

template <typename T>
BinarySearchTree<T> BinarySearchTree<T>::join(BinarySearchTree<T> tree1, T &obj, BinarySearchTree<T> tree2)
{
    Node<T> *root1 = tree1.root;
    Node<T> *root2 = tree2.root;
    Node<T> *c, *temp_ptr;
    if ((root1 == nil) && (root2 == nil))
    {
        tree1.root = new Node<T>;
        tree1.root->color = BLACK;
        tree1.root->p = nil;
        tree1.root->left = nil;
        tree1.root->right = nil;
        tree1.root->data = obj;
        return tree1;
    }
    if (root1 == nil)
    {
        tree2.insert(obj);
        return tree2;
    }
    if (root2 == nil)
    {
        tree1.insert(obj);
        return tree1;
    }
    int initialblackheight1 = getBlackHeight(root1);
    int initialblackheight2 = getBlackHeight(root2);
    if (initialblackheight1 >= initialblackheight2)
    {
        c = root1;
        for (int i = initialblackheight1; i > initialblackheight2; i--)
        {
            if (c->right->color == BLACK)
            {
                c = c->right;
            }
            else
            {
                c = c->right->right;
            }
        }

        temp_ptr = new Node<T>; // create a new node for obj
        temp_ptr->data = obj;
        temp_ptr->color = RED;
        temp_ptr->p = c->p;
        c->p = temp_ptr;
        root2->p = temp_ptr;
        temp_ptr->left = c;
        temp_ptr->right = root2;

        if (temp_ptr->p != nil)
        {
            temp_ptr->p->right = temp_ptr; // connect it to the tree
        }
        else
        {
            tree1.root = temp_ptr;
        }

        c = temp_ptr->p;
        if (c != nil)
        {
            if (temp_ptr->left->color == RED)
            {
                tree1.insertFixup(temp_ptr->left);
            }
            if (c->color == RED)
            {
                tree1.insertFixup(c->left);
            }
        }
        else
        {
            tree1.insertFixup(temp_ptr);
        }
        tree1.updateSize();
        return tree1;
    }
    else
    {
        c = root2;
        for (int i = initialblackheight2; i > initialblackheight1; i--)
        {
            if (c->left->color == BLACK)
            {
                c = c->left;
            }
            else
            {
                c = c->left->left;
            }
        }

        temp_ptr = new Node<T>; // create a new node for obj
        temp_ptr->data = obj;
        temp_ptr->color = RED;
        temp_ptr->p = c->p;
        temp_ptr->left = root1;
        temp_ptr->right = c;
        c->p = temp_ptr;
        root1->p = temp_ptr;

        if (temp_ptr->p != nil)
        {
            temp_ptr->p->left = temp_ptr; // connect it to the tree
        }
        else
        {
            tree2.root = temp_ptr;
        }

        c = temp_ptr->p;
        if (temp_ptr->right->color == RED)
        {
            tree2.insertFixup(temp_ptr->right);
        }
        if (c->color == RED)
        {
            tree2.insertFixup(c->right);
        }
        tree2.updateSize();
        return tree2;
    }
}

template <typename T>
int BinarySearchTree<T>::getBlackHeight(Node<T> *x)
{
    int counter = 0;
    while (x != nil)
    {
        if (x->color == BLACK)
        {
            counter++;
        }
        x = x->left;
    }
    return counter;
}

template <typename T>
BinarySearchTree<T> BinarySearchTree<T>::merge(BinarySearchTree<T> &tree1, BinarySearchTree<T> &tree2)
{
    if (tree1.root == nil)
    {
        return tree2;
    }
    if (tree2.root == nil)
    {
        return tree1;
    }
    auto t = __split(tree2, tree1.root->data);
    BinarySearchTree<T> l;
    BinarySearchTree<T> r;
    l.root = tree1.root->left;
    l.root->p = nil;
    r.root = tree1.root->right;
    r.root->p = nil;
    return join(merge(l, t.L), tree1.root->data, merge(r, t.R));
}

template <typename T>
three<T> BinarySearchTree<T>::__split(BinarySearchTree<T> tree, T &obj)
{
    three<T> p, old_p;

    if (tree.root == nil)
    {
        p.L.root = nil;
        p.obj = false;
        p.R.root = nil;
        return p;
    }

    Node<T> *node = tree.root;
    node->left->p = nil;
    node->left->color = BLACK;
    p.L.root = node->left;

    node->right->p = nil;
    node->right->color = BLACK;
    p.R.root = node->right;

    if (obj < node->data)
    {
        old_p = p;
        p = __split(p.L, obj);
        p.R = join(p.R, node->data, old_p.R);
        return p;
    }
    else if (obj > node->data)
    {
        old_p = p;
        p = __split(p.R, obj);
        p.L = join(old_p.L, node->data, p.L);
        p.R = old_p.R;
        return p;
    }
    else
    {
        p.obj = true;
        return p;
    }
}

template <typename T>
pair<BinarySearchTree<T>, BinarySearchTree<T>> BinarySearchTree<T>::split(BinarySearchTree<T> &tree, T &obj)
{
    three<T> t = __split(tree, obj);
    pair<BinarySearchTree<T>, BinarySearchTree<T>> p;
    p.first = t.L;
    p.first.updateSize();
    p.second = t.R;
    p.second.updateSize();
    if (t.obj)
    {
        p.second.insert(obj);
    }
    return p;
}

template struct three<Student>;
template struct Node<Student>;
template class BinarySearchTree<Student>;