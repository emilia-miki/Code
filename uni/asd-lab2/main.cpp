#include <iostream>
#include <fstream>
#include <ctime>
#include <cstdlib>

#include "hash_table.hpp"
#include "s_hash_table.hpp"

bool testHashTable();
bool testOHashTable();

int main(int argc, char **argv)
{
    srand(time(nullptr));
    if (argc == 1)
    {
        return testHashTable();
    }
    else if (argc == 2)
    {
        std::string arg = std::string(argv[1]);
        if (arg == "--open")
        {
            return testOHashTable();
        }
        else
        {
            SHashTable table;
            std::ifstream f(arg);
            if (!f.is_open())
            {
                std::cout << "File " << argv[1] << " does not exist.\n";
                return -1;
            }

            std::string str;
            std::string word;
            while (f >> str)
            {
                word = "";
                for (auto c: str)
                {
                    if (isalpha(c)) word += tolower(c);
                }
                if (!word.empty()) table.insert(word);
                
            }

            table.print();
            f.close();
        }
    }
    else
    {
        std::cout << "Invalid arguments\n";
        return -1;
    }
}