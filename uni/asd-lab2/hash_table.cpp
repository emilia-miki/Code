#include "hash_table.hpp"

const float kLoadFactor = 1.5f;
const int default_m = 8;
const int kExpand = 2;

const int kMaxID = 100;
const int kMaxStudents = 70;
const int kNameMinLength = 5;
const int kNameMaxLength = 10;

std::string generateRandString(int, int);

Data::Data()
{
    id = rand() % kMaxID;
    students_present = rand() % kMaxStudents;
    teachers_name = generateRandString(kNameMinLength, kNameMaxLength);
}

HashNode::HashNode(long long k, Data value)
{
    key = k;
    data = value;
    next = nullptr;
}

HashTable::HashTable()
{
    table_size = 0;
    m = default_m;
    bucketsArray = new LinkedList[m];
}

int HashTable::hash(long long key, int m)
{
    const long long p = 9149658775000477;
    const int a = 38;
    const int b = 74;
    return ((a * key + b) % p) % m;
}

void HashTable::insert(long long key, Data value)
{
    HashNode *node = find(key);
    if (node != nullptr)
    {
        node->data = value;
        return;
    }

    if ((float)table_size / m >= kLoadFactor)
    {
        LinkedList *oldBucketsArray = bucketsArray;
        int old_m = m;
        m = kExpand * table_size;
        bucketsArray = new LinkedList[m];
        table_size = 0;

        for (int i = 0; i < old_m; i++)
        {
            HashNode *ptr = oldBucketsArray[i].root;
            HashNode *old_ptr;
            while (ptr != nullptr)
            {
                insert(ptr->key, ptr->data);
                old_ptr = ptr;
                ptr = ptr->next;
                delete old_ptr;
            }
        }

        delete[] oldBucketsArray;
    }

    node = new HashNode(key, value);
    bucketsArray[hash(key, m)].push_front(node);
    table_size++;
}

HashNode *HashTable::find(long long key)
{
    return bucketsArray[hash(key, m)].find(key);
}

void HashTable::erase(long long key)
{
    if (bucketsArray[hash(key, m)].erase(key) != -1)
        table_size--;
}

int HashTable::size()
{
    return table_size;
}