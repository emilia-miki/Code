#include <iostream>

int len;
int *heap;

int parent(int i)
{
    return (i - 1) / 2;
}

int left(int i)
{
    return i * 2 + 1;
}

int right(int i)
{
    return i * 2 + 2;
}

void siftDown(int *arr, int n, int i)
{
    int l = left(i);
    int r = right(i);
    int smallest;
    if ((l < n) && (arr[l] < arr[i]))
        smallest = l;
    else
        smallest = i;
    if ((r < n) && (arr[r] < arr[smallest]))
        smallest = r;
    if (smallest != i)
    {
        std::swap(arr[i], arr[smallest]);
        siftDown(arr, n, smallest);
    }
}

void printHeap(int index, int level)
{
    if (index >= len)
    {
        return;
    }

    printHeap(right(index), level + 1);
    for (int i = 0; i < level; i++)
    {
        std::cout << '\t';
    }
    std::cout << heap[index] << std::endl;
    printHeap(left(index), level + 1);
}

void heapsort()
{
    std::cout << "How long is your array of integers?\n";
    std::cin >> len;
    if (std::cin.fail())
    {
        std::cout << "Error: expected an integer\n";
        exit(EXIT_FAILURE);
    }
    heap = new int[len];
    std::cout << "Enter the array:\n";
    for (int i = 0; i < len; i++)
    {
        std::cin >> heap[i];
        if (std::cin.fail())
        {
            std::cout << "Error: expected an integer\n";
            exit(EXIT_FAILURE);
        }
    }
    for (int i = (len - 1) / 2; i >= 0; i--)
    {
        siftDown(heap, len, i);
    }
    for (int i = len - 1; i > 0; i--)
    {
        std::swap(heap[i], heap[0]);
        siftDown(heap, i, 0);
    }

    std::cout << "\nHere's the heap:\n";
    printHeap(0, 0);
    std::cout << "The sorted array is:\n";
    for (int i = len - 1; i >= 0; i--)
    {
        std::cout << heap[i] << " ";
    }
    std::cout << std::endl;
}