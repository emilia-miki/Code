#ifndef _DATA_HPP_INCLUDED
#define _DATA_HPP_INCLUDED

#include "structures.hpp"

using namespace std;

void generateData(Data *data);
void replaceData(string &name, int salary, int company, int position,
                 vector<int> &requirements, int vacancies, ifstream &ifs);
void addData(string &name, int salary, int company, int position, ifstream &ifs);
int removeData(string &name, ifstream &ifs);
void viewData(ifstream &ifs);

#endif