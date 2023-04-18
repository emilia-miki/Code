#include <vector>
#include <algorithm>
#include "s_hash_table.hpp"

const float loadFactor = 13.5f;
const int default_m = 8;

std::string generateRandString(int, int);

SHashNode::SHashNode(std::string k)
{
    key = k;
    data = 1;
    next = nullptr;
}

SHashTable::SHashTable()
{
    table_size = 0;
    m = default_m;
    bucketsArray = new SLinkedList[m];
}

int SHashTable::hash(std::string key, int m)
{
    const int a = 7;
    long long h = 0;

    for (int i = 0; i < key.length(); i++)
    {
        h = a * h + key[i];
    }

    return h % m;
}

void SHashTable::insert(std::string key)
{
    SHashNode *node = find(key);
    if (node != nullptr)
    {
        node->data++;
        return;
    }

    if ((float) table_size / m >= loadFactor)
    {
        SLinkedList *oldBucketsArray = bucketsArray;
        int old_m = m;
        m = 2 * table_size;
        bucketsArray = new SLinkedList[m];
        table_size = 0;

        for (int i = 0; i < old_m; i++)
        {
            SHashNode *ptr = oldBucketsArray[i].root;
            SHashNode *old_ptr;
            while (ptr != nullptr)
            {
                insert(ptr->key);
                old_ptr = ptr;
                ptr = ptr->next;
                delete old_ptr;
            }
        }
        
        delete[] oldBucketsArray;
    }

    node = new SHashNode(key);
    bucketsArray[hash(key, m)].push_front(node);
    table_size++;
}

SHashNode *SHashTable::find(std::string key)
{
    return bucketsArray[hash(key, m)].find(key);
}

void SHashTable::erase(std::string key)
{
    if (bucketsArray[hash(key, m)].erase(key) != -1) table_size--;
}

int SHashTable::size()
{
    return table_size;
}

void SHashTable::print()
{
    std::pair<int, std::string> pairs[table_size];
    
    int j = 0;
    for (int i = 0; i < m; i++)
    {
        SHashNode *ptr = bucketsArray[i].root;
        while (ptr != nullptr)
        {
            pairs[j].first = ptr->data;
            pairs[j].second = ptr->key;
            j++;
            ptr = ptr->next;
        }
    }

    std::sort(pairs, pairs + table_size);

    std::cout << "occurencies, word\n";
    for (int i = table_size - 1; i >= 0; i--)
    {
        std::cout << pairs[i].first << " " << pairs[i].second << std::endl;
    }
}