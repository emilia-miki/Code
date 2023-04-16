#ifndef _O_HASH_TABLE_HPP_INCLUDED
#define _O_HASH_TABLE_HPP_INCLUDED

#include <iostream>
#include "nodes.hpp"

struct OHashTable
{
    int table_size;
    int m;
    OHashNode *bucketsArray;

    OHashTable();
    long long hash(long long key, int m);
    void insert(long long key, Data value);
    OHashNode *find(long long key);
    void erase(long long key);
    int size();
};

#endif