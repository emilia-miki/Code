#ifndef _FILE_IO_HPP_INCLUDED
#define _FILE_IO_HPP_INCLUDED

#include <fstream>
#include <vector>

#include "structures.hpp"

using namespace std;

void writeInt(int integer, ofstream &ofs);
void readInt(int &integer, ifstream &ifs);
void writeVec(vector<string> &vec, ofstream &ofs);
void readVec(vector<string> &vec, ifstream &ifs);
Data *readWorker(ifstream &ifs);
void writeWorker(Data *data, ofstream &ofs);

#endif