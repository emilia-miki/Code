#include <iostream>

void enter_sizes(int &N, int &M)
{
    std::cout << "Enter N and M: ";
    std::cin >> N >> M;
}

void allocate_memory(int *&X, int N)
{
    X = new int[N];
}

void enter_array(int *X, int N)
{
    for (int i = 0; i < N; i++)
    {
        std::cin >> X[i];
    }
}
int find_max_index(int *X, int N)
{
    int x_max = 0;
    for (int i = 1; i < N; i++)
    {
        if (X[i] > X[x_max])
            x_max = i;
    }
    return x_max;
}

void print_max(int *X, int x_max)
{
    std::cout << "x_max is X[" << x_max << "] = " << X[x_max] << ";\n";
}
void free_memory(int *X)
{
    delete[] X;
}

int main(int, char **)
{
    int N, M;
    int x_max, y_max;

    enter_sizes(N, M);

    int *X, *Y;
    allocate_memory(X, N);
    allocate_memory(Y, M);

    enter_array(X, N);
    enter_array(Y, M);

    x_max = find_max_index(X, N);
    y_max = find_max_index(Y, M);

    print_max(X, x_max);
    print_max(Y, y_max);

    free_memory(X);
    free_memory(Y);
}