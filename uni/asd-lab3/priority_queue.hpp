#ifndef _PRIORITY_QUEUE_HPP
#define _PRIORITY_QUEUE_HPP

#include <string>
#include <vector>
#include <ctime>

class Homework
{
    int getImportance() const;

public:
    std::string name;
    int interest;
    int usefulness;
    int difficulty;
    time_t deadline;
    Homework();
    friend bool operator<(const Homework &lh, const Homework &rh);
    friend bool operator>(const Homework &lh, const Homework &rh);
    friend bool operator<=(const Homework &lh, const Homework &rh);
    friend bool operator>=(const Homework &lh, const Homework &rh);
    friend bool operator==(const Homework &lh, const Homework &rh);
    friend bool operator!=(const Homework &lh, const Homework &rh);
};

class PriorityQueue
{
    std::vector<Homework *> vec;

    int parent(int index);
    int left(int index);
    int right(int index);
    void siftUp(int index);
    void siftDown(int index);

public:
    int size();
    bool empty();
    void push(Homework element);
    Homework top();
    void pop();

    static void heapSort(std::vector<Homework *> &v);
};

#endif