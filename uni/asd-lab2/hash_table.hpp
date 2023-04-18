#ifndef _HASH_TABLE_HPP_INCLUDED
#define _HASH_TABLE_HPP_INCLUDED

#include <iostream>
#include "nodes.hpp"

struct HashTable
{
    LinkedList *bucketsArray;
    int table_size;
    int m;

    HashTable();
    int hash(long long key, int m);
    void insert(long long key, Data value);
    HashNode *find(long long key);
    void erase(long long key);
    int size();
};

#endif