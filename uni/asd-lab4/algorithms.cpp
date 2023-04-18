#include <iostream>

const int kKeyLength = 12;

std::string generateRandString(int min_length, int max_length)
{
    std::string str;
    for (int i = 0; i < min_length + rand() % (max_length - min_length + 1); i++)
    {
        str.push_back('a' + rand() % ('z' - 'a' + 1));
    }
    str[0] = toupper(str[0]);
    return str;
}

long long pow(int number, int power)
{
    long long result = 1;
    for (int i = 0; i < power; i++)
    {
        result *= number;
    }
    return result;
}

long long generateRandLong()
{
    long long key = 0;

    for (int i = 0; i < kKeyLength; i++)
    {
        key += (rand() % 10) * pow(10, i);
    }

    return key;
}