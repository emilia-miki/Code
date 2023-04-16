#include <cstring>
#include "file_io.hpp"

void readInt(int &integer, ifstream &ifs) // зчитати int з файлу
{
    ifs.read(reinterpret_cast<char *>(&integer), sizeof(int));
}

void writeInt(int integer, ofstream &ofs) // записати int до файлу
{
    ofs.write(reinterpret_cast<char *>(&integer), sizeof(int));
}

void readVec(vector<string> &vec, ifstream &ifs) // прочитати масив рядків з файлу
{
    int size;
    readInt(size, ifs);
    vec.resize(0);
    char *buffer = new char[size];
    char *ptr = buffer;
    ifs.read(buffer, size);
    while (ptr - buffer < size)
    {
        vec.push_back(ptr);
        ptr += strlen(ptr) + 1;
    }
    delete[] buffer;
}

void writeVec(vector<string> &vec, ofstream &ofs) // записати масив рядків у файл
{
    int size = 0;
    for (int i = 0; i < vec.size(); i++)
    {
        size += vec[i].length() + 1;
    }
    writeInt(size, ofs);
    for (int i = 0; i < vec.size(); i++)
    {
        ofs.write(vec[i].c_str(), vec[i].length() + 1);
    }
}

Data *readWorker(ifstream &ifs) // прочитати робітника з файлу
{
    // статична змінна, щоб пам'ять не перевиділялась при кожному виклику функції
    static Data *data = new Data;
    readInt(data->company, ifs);
    int size;
    readInt(size, ifs);
    char *buf = new char[size];
    ifs.read(buf, size);
    data->worker.name = buf;
    delete[] buf;
    readInt(data->worker.salary, ifs);
    readInt(data->position.position, ifs);
    int reqSize;
    readInt(reqSize, ifs);
    data->position.requirements.resize(reqSize);
    for (int i = 0; i < reqSize; i++)
    {
        readInt(data->position.requirements[i], ifs);
    }
    readInt(data->position.vacancies, ifs);
    return data;
}

void writeWorker(Data *data, ofstream &ofs) // записати робітника в файл
{
    writeInt(data->company, ofs);
    writeInt(data->worker.name.length() + 1, ofs);
    ofs.write(data->worker.name.c_str(), data->worker.name.length() + 1);
    writeInt(data->worker.salary, ofs);
    writeInt(data->position.position, ofs);
    int reqSize = data->position.requirements.size();
    writeInt(reqSize, ofs);
    for (int i = 0; i < reqSize; i++)
    {
        writeInt(data->position.requirements[i], ofs);
    }
    writeInt(data->position.vacancies, ofs);
}