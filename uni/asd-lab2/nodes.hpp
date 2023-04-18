#ifndef _NODES_HPP_INCLUDED
#define _NODES_HPP_INCLUDED

#include <iostream>

struct Data
{
    std::string teachers_name;
    int id;
    int students_present;

    Data();
};

struct HashNode
{
    long long key;
    Data data;
    HashNode *next;

    HashNode(long long k, Data value);
};

struct LinkedList
{
    int size;
    HashNode *root;

    LinkedList();
    void push_front(HashNode *element);
    int erase(long long key);
    HashNode *find(long long key);
};

struct SHashNode
{
    std::string key;
    int data;
    SHashNode *next;

    SHashNode(std::string key);
};

struct SLinkedList
{
    SHashNode *root;

    SLinkedList();
    void push_front(SHashNode *element);
    int erase(std::string key);
    SHashNode *find(std::string key);
};

struct OHashNode
{
    long long key;
    Data data;
    bool isDeleted;

    OHashNode();
    OHashNode(long long key, Data data);
};

#endif