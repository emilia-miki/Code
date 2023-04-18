#ifndef _S_HASH_TABLE_HPP_INCLUDED
#define _S_HASH_TABLE_HPP_INCLUDED

#include <iostream>
#include "nodes.hpp"

struct SHashTable
{
    SLinkedList *bucketsArray;
    int table_size;
    int m;

    SHashTable();
    int hash(std::string key, int m);
    void insert(std::string key);
    SHashNode *find(std::string key);
    void erase(std::string key);
    int size();
    void print();
};

#endif