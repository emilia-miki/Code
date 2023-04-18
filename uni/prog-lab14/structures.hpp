#ifndef _STRUCTURES_HPP_INCLUDED
#define _STRUCTURES_HPP_INCLUDED

#include <vector>
#include <string>

struct Worker
{
    std::string name;
    int salary;
};

struct Position
{
    int position; // індекс для масиву Positions
    int vacancies;
    std::vector<int> requirements; // індекс для масиву Requirements
};

struct Data
{
    int company; // індекс для масиву Companies
    Worker worker;
    Position position;
};

struct VacantData
{
    int position; // індекс для масиву Positions
    int salary;
};

#endif