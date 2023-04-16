#include <iostream>
#include <cstdlib>
#include <ctime>

#include "BST.hpp"

using namespace std;

bool testBinarySearchTree();
void testAutocompletion();
void testMergeSplit();

int main(int, char **)
{
    srand(time(nullptr));
    int c;
    while (true)
    {
        cout << "MENU\n";
        cout << "0. Exit program.\n";
        cout << "1. Test BST.\n";
        cout << "2. Test autocompletion.\n";
        cout << "3. Test merge and split.\n";
        cin >> c;
        switch (c)
        {
        case 0:
            return 0;
            break;

        case 1:
            testBinarySearchTree();
            return 0;
            break;

        case 2:
            testAutocompletion();
            return 0;
            break;

        case 3:
            testMergeSplit();
            return 0;
            break;
        
        default:
            cout << "Invalid input.\n";
            break;
        }
    }
}
