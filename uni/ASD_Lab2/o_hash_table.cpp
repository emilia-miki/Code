#include "o_hash_table.hpp"

const float loadFactor = 0.5f;
const int default_m = 8;

OHashNode::OHashNode()
{
    key = -1;
    isDeleted = false;
}

OHashNode::OHashNode(long long k, Data value)
{
    key = k;
    data = value;
    isDeleted = false;
}

OHashTable::OHashTable()
{
    table_size = 0;
    m = default_m;
    bucketsArray = new OHashNode[m];
}

long long OHashTable::hash(long long key, int m)
{
    const long long p = 9149658775000477;
    const int a = 38;
    const int b = 74;
    return (a * key + b) % p;
}

void OHashTable::insert(long long key, Data value)
{
    OHashNode *node = find(key);
    if (find(key) != nullptr)
    {
        node->data = value;
        return;
    }

    long long h = hash(key, m);
    int i, n;
    for (i = 0; (bucketsArray[n = ((h + i) % m)].key != -1)
        && (!bucketsArray[n].isDeleted); i++);
    bucketsArray[n] = OHashNode(key, value);
    table_size++;

    if ((float)table_size / m >= loadFactor)
    {
        OHashNode *oldBucketsArray = bucketsArray;
        int old_m = m;
        m *= 2;
        table_size = 0;
        bucketsArray = new OHashNode[m];
        for (int i = 0; i < old_m; i++)
        {
            if ((oldBucketsArray[i].key != -1) && (!oldBucketsArray[i].isDeleted))
            {
                insert(oldBucketsArray[i].key, oldBucketsArray[i].data);
            }
        }
        delete[] oldBucketsArray;
    }
}

OHashNode *OHashTable::find(long long key)
{
    long long h = hash(key, m);
    int i = 0;
    int n = (h + i) % m;
    while (bucketsArray[n].key != -1)
    {
        if ((!bucketsArray[n].isDeleted) && (bucketsArray[n].key == key))
        {
            return &bucketsArray[n];
        }
        i++;
        n = (h + i) % m;
    }
    return nullptr;
}

void OHashTable::erase(long long key)
{
    OHashNode *node = find(key);
    if (node != nullptr)
    {
        node->isDeleted = true;
        table_size--;
    }
}

int OHashTable::size()
{
    return table_size;
}