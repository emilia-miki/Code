/*
Застосування бінарних дерев:
Сортування, пошук, кодування інформації – реалізація алгоритму Хафмана.
Передбачити розвинену систему меню з вибору типу задач.
*/

#include <iostream>

void treesort();
void heapsort();
void search();
void huffman_coding();

int main(int, char **)
{
    char c;

    while (true)
    {
        std::cout << "\nChoose an option:\n"
            << "1. Tree sort\n"
            << "2. Heap sort\n"
            << "3. Search\n"
            << "4. Huffman coding\n"
            << "0. Exit program\n";
        std::cin >> c;
        switch (c)
        {
            case '0':
                return 0;
                break;
            case '1':
                treesort();
                break;
            case '2':
                heapsort();
                break;
            case '3':
                search();
                break;
            case '4':
                huffman_coding();
                break;
            default:
                std::cout << "Invalid option\n";
                break;
        }
    }
}