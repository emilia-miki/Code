#ifndef _ALGORITHMS_HPP_INCLUDED
#define _ALGORITHMS_HPP_INCLUDED

#include <vector>
#include <fstream>

#include "structures.hpp"

using namespace std;

void findMaxVacancies(vector<Data> &vec, ifstream &ifs);
void printRequirements(vector<Data> &vec, ifstream &ifs);
void createVacanciesArray(vector<Data> &vec, ifstream &ifs);
void updateVacancies(int company, int position, int vacancies, ifstream &ifs);

#endif