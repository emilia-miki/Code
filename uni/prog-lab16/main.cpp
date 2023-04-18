#include <stdio.h>

#define N 2

#define PRINTRES(a) printf("The result is %d.\n", a)

#if N == 1 // перший варіант логічного виразу
#define PRTLOGEXP() printf("((x and y) and (x or z)) and (not x xor y)\n")
#define LOGEXP(x, y, z) ((((x) & (y)) & ((x) | (z))) & (!(x) ^ (y)))
#elif N == 2 // другий варіант логічного виразу
#define PRTLOGEXP() printf("((x or y) and (z or y)) and (z or x)\n")
#define LOGEXP(x, y, z) (((x) | (y)) & ((z) | (y)) & ((z) | (x)))
#endif

#if (N != 1) && (N != 2) // якщо логічний вираз не обрано, то не компілювати
#error No expression chosen
#else

int main(int, char**) {
    int x, y, z, w;

    // введення  змінних x, y, z
    printf("Enter x, y and z: ");
    scanf("%d %d %d", &x, &y, &z);

    PRTLOGEXP(); // вивести логічний вираз
    w = LOGEXP(x, y, z); // обчислити логічний вираз
    PRINTRES(w); // вивести результат
}

#endif