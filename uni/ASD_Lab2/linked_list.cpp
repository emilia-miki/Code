#include "nodes.hpp"

LinkedList::LinkedList()
{
    root = nullptr;
}

void LinkedList::push_front(HashNode *element)
{
    element->next = root;
    root = element;
}

int LinkedList::erase(long long key)
{
    HashNode *ptr = root;
    if (ptr == nullptr)
        return -1;
    if (ptr->key == key)
    {
        HashNode *new_root = root->next;
        delete root;
        root = new_root;
    }
    else
    {
        while ((ptr->next != nullptr) && (ptr->next->key != key))
            ptr = ptr->next;
        if (ptr->next == nullptr)
            return -1;
        else
        {
            HashNode *new_ptr_next = ptr->next->next;
            delete ptr->next;
            ptr->next = new_ptr_next;
        }
    }
    return 0;
}

HashNode *LinkedList::find(long long key)
{
    HashNode *ptr = root;
    while ((ptr != nullptr) && (ptr->key != key))
        ptr = ptr->next;
    return ptr;
}