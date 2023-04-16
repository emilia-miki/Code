#include <iostream>
#include <cstdlib>
#include <vector>
#include "BST.hpp"

void testMergeSplit()
{
    const int iters = 10;
    BinarySearchTree<Student> x, y;
    Student st;
    vector<string> str1 = { "A", "B", "C", "D" };
    vector<string> str2 = { "C", "D", "E", "F" };
    for (int i = 0; i < str1.size(); i++)
    {
        st = Student();
        st.name = str1[i];
        x.insert(st);
    }
    for (int i = 0; i < str2.size(); i++)
    {
        st = Student();
        st.name = str2[i];
        y.insert(st);
    }
    std::cout << "X: ";
    x.print();
    std::cout << "Y: ";
    y.print();
    std::cout << "X∪Y: ";
    x = BinarySearchTree<Student>::merge(x, y);
    x.print();
    st.name = "D";
    std::cout << "Split by " << st.name << ": " << endl;
    auto p = BinarySearchTree<Student>::split(x, st);
    p.first.print();
    std::cout << "Y: ";
    p.second.print();
}