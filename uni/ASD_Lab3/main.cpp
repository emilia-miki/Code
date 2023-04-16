#include <queue>
#include <cstdlib>
#include <ctime>
#include <iostream>
#include <algorithm>

#include "priority_queue.hpp"

using namespace std;

typedef Homework Data;

int cmps;

void treesort(vector<Data *> &vec);

template <typename T>
float testPriorityQueueSpeed(T &&priorityQueue)
{
    const int iters = 100000;

    clock_t timeStart = clock();
    for (int i = 0; i < iters; i++)
    {
        int insertDataAmount = rand() % 6 + 5;
        for (int j = 0; j < insertDataAmount; j++)
        {
            priorityQueue.push(Data());
        }

        priorityQueue.top();
        priorityQueue.pop();
    }
    clock_t timeEnd = clock();
    float time = (float(timeEnd - timeStart)) / CLOCKS_PER_SEC;

    return time;
}

bool testPriorityQueue()
{
    const int iters = 200;

    PriorityQueue myPriorQueue;
    priority_queue<Data> stlPriorQueue;

    bool isDataEqual = true;
    for (int i = 0; i < iters; i++)
    {
        int insertDataAmount = rand() % 6 + 5;
        for (int j = 0; j < insertDataAmount; j++)
        {
            Data randData = Data();
            myPriorQueue.push(randData);
            stlPriorQueue.push(randData);
        }

        if (!(myPriorQueue.top() == stlPriorQueue.top()))
        {
            isDataEqual = false;
            cerr << "Comparing failed on iteration " << i << endl
                 << endl;
            break;
        }

        int removeDataAmount = rand() % insertDataAmount;
        for (int j = 0; j < removeDataAmount; j++)
        {
            myPriorQueue.pop();
            stlPriorQueue.pop();
        }
    }

    int myQueueSize = myPriorQueue.size();
    int stlQueueSize = stlPriorQueue.size();

    float stlTime = testPriorityQueueSpeed<priority_queue<Data>>(priority_queue<Data>());
    float myTime = testPriorityQueueSpeed<PriorityQueue>(PriorityQueue());

    cout << "My PriorityQueue:" << endl;
    cout << "Time: " << myTime << ", size: " << myQueueSize << endl;
    cout << "STL priority_queue:" << endl;
    cout << "Time: " << stlTime << ", size: " << stlQueueSize << endl
         << endl;

    if (isDataEqual && myQueueSize == stlQueueSize)
    {
        cout << "The lab is completed" << endl
             << endl;
        return true;
    }

    cerr << ":(" << endl
         << endl;
    return false;
}

int compar(const void *ld, const void *rd)
{
    cmps++;
    if (**(Data **)ld < **(Data **)rd)
    {
        return -1;
    }
    if (**(Data **)ld > **(Data **)rd)
    {
        return 1;
    }
    return 0;
}

bool testHeapSort()
{
    srand(time(nullptr));

    const int testNumber = 10000;
    vector<Data *> vec1(testNumber);
    for (int i = 0; i < testNumber; i++)
    {
        vec1[i] = new Data();
    }
    vector<Data *> vec2 = vec1;
    vector<Data *> vec3 = vec1;
    vector<Data *> vec4 = vec1;

    cmps = 0;
    clock_t mySortStart = clock();
    PriorityQueue::heapSort(vec1);
    clock_t mySortEnd = clock();
    int heapcmps = cmps;

    struct
    {
        bool operator()(Data *ld, Data *rd) const
        {
            cmps++;
            return *ld > *rd;
        }
    } cmp;

    cmps = 0;
    clock_t stlSortStart = clock();
    sort(vec2.begin(), vec2.end(), cmp);
    clock_t stlSortEnd = clock();
    int stlcmps = cmps;

    cmps = 0;
    clock_t quicksortStart = clock();
    qsort(&vec3[0], testNumber, sizeof(Data *), compar);
    clock_t quicksortEnd = clock();
    int quickcmps = cmps;

    const bool TS = true;
    clock_t treesortStart, treesortEnd;
    int treecmps;
    if (TS)
    {
        cmps = 0;
        treesortStart = clock();
        treesort(vec4);
        treesortEnd = clock();
        treecmps = cmps;
    }

    for (int i = 0; i < testNumber; i++)
    {
        if ((*vec1[i] != *vec2[testNumber - 1 - i]) || (*vec1[i] != *vec3[i]) || (TS && (*vec1[i] != *vec4[i])))
        {
            cerr << "Heap sort is not working.\n";
            return false;
        }
    }

    cout << "My heap sort time: " << (float(mySortEnd - mySortStart) / CLOCKS_PER_SEC) << endl;
    cout << "STL sort time: " << (float(stlSortEnd - stlSortStart) / CLOCKS_PER_SEC) << endl;
    cout << "Quicksort time: " << (float(quicksortEnd - quicksortStart) / CLOCKS_PER_SEC) << endl;
    if (TS) cout << "Treesort time: " << (float(treesortEnd - treesortStart) / CLOCKS_PER_SEC) << endl;
    cout << "Amount of comparisons:\n";
    printf("%10s %10d\n", "heapsort", heapcmps);
    printf("%10s %10d\n", "std::sort", stlcmps);
    printf("%10s %10d\n", "quicksort", quickcmps);
    if (TS) printf("%10s %10d\n", "treesort", treecmps);

    return true;
}

int main()
{
    testPriorityQueue();
    testHeapSort();
}