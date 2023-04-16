/*
Створити текстовий файл, перший рядок якого містить значення n та m,
які визначають кількість рядків та стовпців матриці. Наступні n рядків
містять по m чисел, що є елементами матриці. Визначити максимальне
значення в кожному рядку матриці та дописати їх в кожний рядок файлу,
починаючи з другого.
*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>
#include <time.h>

#define N 5
#define M 5
#define MAX 10

int main(int, char **)
{
    int n, m;
    char buffer[BUFSIZ];
    FILE *f;

    printf("Enter file name: ");
    scanf("%s", buffer);
    char *filename = (char *)malloc(strlen(buffer) + 1);
    strcpy(filename, buffer);

    f = fopen(filename, "r");
    // Generate matrix and write into file
    if (f == NULL)
    {
        srand(time(NULL));
        n = rand() % N + 1;
        m = rand() % M + 1;
        f = fopen(filename, "w");
        fprintf(f, "%d %d\n", n, m);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                fprintf(f, "%d ", rand() % MAX);
            }
            fprintf(f, "\n");
        }
        freopen(filename, "r", f);
    }

    // Read the matrix from file and write a new file
    char *temp_filename = (char *)malloc(strlen(filename) + 5);
    strcpy(temp_filename, filename);
    memcpy(temp_filename + strlen(filename), ".tmp", 5);
    FILE *temp = fopen(temp_filename, "w");
    
    if (fscanf(f, "%d %d", &n, &m) == EOF)
    {
        fprintf(stderr, "Invalid file\n");
        return -1;
    }
    fprintf(temp, "%d %d\n", n, m);
    int buf, max;
    for (int i = 0; i < n; i++)
    {
        max = INT_MIN;
        for (int j = 0; j < m; j++)
        {
            if (fscanf(f, "%d", &buf) == EOF)
            {
                fprintf(stderr, "Invalid file\n");
                return -1;
            }
            if (buf > max) max = buf;
            fprintf(temp, "%d ", buf);
        }
        fprintf(temp, "%d\n", max);
    }

    fclose(f);
    fclose(temp);
    remove(filename);
    rename(temp_filename, filename);
    free(temp_filename);
    free(filename);
}
