#include <vector>
#include <ctime>
#include <cmath>
#include <cstdlib>
#include <string>
#include "priority_queue.hpp"

// #define TEST1
#define TEST2
// #define TEST3

using namespace std;

extern int cmps;

int Homework::getImportance() const
{
#if defined(TEST1) || defined(TEST2) || defined(TEST3)
    return interest;
#else
    double rating = double(interest + usefulness) / 200;
    time_t currentTime = time(nullptr);
    currentTime = currentTime - currentTime % (24 * 3600);
    int daysLeft = (deadline - currentTime) / (24 * 3600);
    if (daysLeft == 0)
    {
        return 100;
    }
    double time_importance = (double)difficulty / (daysLeft * 100);
    return round(((1 - time_importance) * rating + time_importance * time_importance) * 100);
#endif
}

Homework::Homework()
{
    const int minNameLength = 4;
    const int maxNameLength = 15;
    for (int i = 0; i < minNameLength + rand() % (maxNameLength - minNameLength); i++)
    {
        name.push_back('a' + rand() % ('z' - 'a' + 1));
    }
    interest = rand() % 101;
#ifdef TEST2
    interest = rand() % 100001;
#elif defined(TEST3)
    interest = rand() % 2;
#endif
    usefulness = rand() % 101;
    difficulty = rand() % 101;
    time_t currentTime = time(nullptr);
    currentTime = currentTime - currentTime % (24 * 3600);
    deadline = currentTime + (rand() % 14 + 1) * 24 * 60 * 60;
}

bool operator<(const Homework &lh, const Homework &rh)
{
    cmps++;
    return lh.getImportance() < rh.getImportance();
}

bool operator>(const Homework &lh, const Homework &rh)
{
    cmps++;
    return rh < lh;
}

bool operator<=(const Homework &lh, const Homework &rh)
{
    cmps++;
    return !(rh < lh);
}

bool operator>=(const Homework &lh, const Homework &rh)
{
    cmps++;
    return !(lh < rh);
}

bool operator==(const Homework &lh, const Homework &rh)
{
    cmps++;
    return lh.getImportance() == rh.getImportance();
}

bool operator!=(const Homework &lh, const Homework &rh)
{
    cmps++;
    return !(lh == rh);
}

int PriorityQueue::parent(int i)
{
    return (i - 1) / 2;
}

int PriorityQueue::left(int i)
{
    return 2 * i + 1;
}

int PriorityQueue::right(int i)
{
    return 2 * i + 2;
}

int PriorityQueue::size()
{
    return vec.size();
}

bool PriorityQueue::empty()
{
    return (bool)vec.size();
}

void PriorityQueue::siftDown(int i)
{
    int l = left(i);
    int r = right(i);
    int highest = i;
    if ((l < vec.size()) && (*vec[l] > *vec[i]))
    {
        highest = l;
    }
    if ((r < vec.size()) && (*vec[r] > *vec[highest]))
    {
        highest = r;
    }
    if (highest != i)
    {
        swap(vec[i], vec[highest]);
        siftDown(highest);
    }
}

void PriorityQueue::siftUp(int i)
{
    while ((i > 0) && (*vec[i] > *vec[parent(i)]))
    {
        swap(vec[i], vec[parent(i)]);
        i = parent(i);
    }
}

void PriorityQueue::push(Homework element)
{
    vec.push_back(new Homework(element));
    siftUp(vec.size() - 1);
}

Homework PriorityQueue::top()
{
    return *vec[0];
}

void PriorityQueue::pop()
{
    delete vec[0];
    vec[0] = vec[vec.size() - 1];
    vec.resize(vec.size() - 1);
    siftDown(0);
}

void PriorityQueue::heapSort(vector<Homework *> &v)
{
    PriorityQueue queue;
    queue.vec = v;
    int n = queue.vec.size();
    for (int i = queue.parent(n - 1); i >= 0; i--)
    {
        queue.siftDown(i);
    }

    for (int i = n - 1; i > 0; i--)
    {
        v[i] = queue.vec[0];
        queue.vec[0] = queue.vec[i];
        queue.vec.resize(i);
        queue.siftDown(0);  
    }
    v[0] = queue.vec[0];
}