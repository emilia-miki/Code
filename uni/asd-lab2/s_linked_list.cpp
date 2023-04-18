#include "nodes.hpp"

SLinkedList::SLinkedList()
{
    root = NULL;
}

void SLinkedList::push_front(SHashNode *element)
{
    element->next = root;
    root = element;
}

int SLinkedList::erase(std::string key)
{
    SHashNode *ptr = root;
    if (ptr == NULL)
        return -1;
    if (ptr->key == key)
    {
        SHashNode *new_root = root->next;
        delete root;
        root = new_root;
    }
    else
    {
        while ((ptr->next != NULL) && (ptr->next->key != key))
            ptr = ptr->next;
        if (ptr->next == NULL)
            return -1;
        else
        {
            SHashNode *new_ptr_next = ptr->next->next;
            delete ptr->next;
            ptr->next = new_ptr_next;
        }
    }
    return 0;
}

SHashNode *SLinkedList::find(std::string key)
{
    SHashNode *ptr = root;
    while ((ptr != NULL) && (ptr->key != key))
        ptr = ptr->next;
    return ptr;
}