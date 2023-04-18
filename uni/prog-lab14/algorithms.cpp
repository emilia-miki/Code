#include <iostream>
#include <algorithm>

#include "globals.hpp"
#include "algorithms.hpp"
#include "queries.hpp"
#include "file_io.hpp"

using namespace std;

// перша задача з умови: знайти посади з максимальною кількістю вакансій
void findMaxVacancies(vector<Data> &vec, ifstream &ifs)
{
    int pos;
    Data *data;

    queryPosition(pos);
    int compsize = Companies.size();
    pair<int, int> *vacancies = new pair<int, int>[compsize];
    for (int i = 0; i < compsize; i++)
    {
        vacancies[i].second = -1;
    }
    ifs.clear();
    ifs.seekg(seekBeg);

    // перший елемент пари - кількість вакансій, другий - номер компанії
    int counter = 0;
    for (int i = 0; i < vecsize; i++)
    {
        data = readWorker(ifs);
        if ((data->position.position != pos) || (vacancies[data->company].second != -1))
        {
            continue;
        }
        vacancies[data->company].first = data->position.vacancies;
        vacancies[data->company].second = data->company;
        counter++;
    }
    // сортування масиву пар за першим елементом (кількістю вакансій)
    sort(vacancies, vacancies + compsize);
    printf("%20s Vacancies\n", "Company");
    int index;
    for (int i = 0; i < counter; i++)
    {
        // зчитуємо у зворотньому порядку (від більшої до меншої кількості вакансій)
        index = vacancies[compsize - i - 1].second;
        printf("%20s %5d\n", Companies[index].c_str(), vacancies[compsize - i - 1].first);
    }
    delete[] vacancies;
}

// друга задача умови: вивести вимоги на деяку посаду
void printRequirements(vector<Data> &vec, ifstream &ifs)
{
    Data *data;
    int pos;
    vector<bool> isPrinted = vector<bool>(Companies.size());

    queryPosition(pos);
    for (int i = 0; i < vecsize; i++) // пошук введеної посади в базі даних
    {
        data = readWorker(ifs);
        if (pos != data->position.position)
        {
            continue;
        }
        // якщо вимоги на цю посаду ще не виведено на екран, то вивести їх
        if (!isPrinted[data->company])
        {
            cout << Companies[data->company] << ":\n";
            cout << data->position.requirements.size() << endl;
            for (int i = 0; i < data->position.requirements.size(); i++)
            {
                cout << Requirements[data->position.requirements[i]] << " ";
            }
            cout << endl;
            isPrinted[data->company] = true;
        }
    }
}

// третя задача з умови: створити масив вакансій
void createVacanciesArray(vector<Data> &vec, ifstream &ifs)
{
    Data *data;
    int compsize = Companies.size();
    int possize = Positions.size();
    int *salariesTable = new int[compsize * possize * 2];
    ifs.clear();
    ifs.seekg(seekBeg);
    // двовимірна таблиця з 2 числами у кожній комірці: кількість зарплат та сума всіх зарплат
    int i, j; // індекси таблиці
    for (int i = 0; i < compsize * possize * 2; i++)
    {
        salariesTable[i] = 0;
    }
    for (int iter = 0; iter < vecsize; iter++) // додаємо кожну зарплату в таблицю
    {
        data = readWorker(ifs);
        i = data->position.position;
        j = data->company;
        salariesTable[2 * (i * possize + j)]++;
        salariesTable[2 * (i * possize + j) + 1] += data->worker.salary;
        cout << data->worker.name << " " << Companies[data->company] << " "
             << Positions[data->position.position] << " " << data->worker.salary << endl;
    }
    printf("%20s Average salary\n", "Position");
    // створюємо масив вакансій (умова задачі, сам масив по суті не потрібен, оскільки
    // він виводиться на екран)
    vector<pair<int, int>> *arr = new vector<pair<int, int>>(Positions.size());
    for (int k = 0; k < possize; k++)
    {
        float averageSalary = 0;
        int salariesCounter = 0;
        int curCompanySum, curCompanyCounter;
        for (int l = 0; l < compsize; l++) // підрахунок середньої зарплатні для даної посади
        {
            curCompanyCounter = salariesTable[2 * (k * possize + l)];
            if (curCompanyCounter == 0)
            {
                continue;
            }
            curCompanySum = salariesTable[2 * (k * possize + l) + 1];
            averageSalary += (float)curCompanySum / curCompanyCounter;
            salariesCounter++;
        }
        if (salariesCounter == 0)
        {
            printf("%20s No data\n", Positions[k].c_str());
            continue;
        }
        averageSalary /= salariesCounter; // ділимо суму зарплат на їх кількість націло
        printf("%20s %10d\n", Positions[k].c_str(), (int)averageSalary);
        arr->push_back(pair<int, int>(k, (int)averageSalary));
    }
    delete[] salariesTable;
    delete arr;
}

// оновлення вакансій на задану компанію та посаду
void updateVacancies(int company, int position, int vacancies, ifstream &ifs)
{
    Data *data;
    ofstream ofs(filename_temp, ios::out); // відкриття тимчасового файлу для запису
    ifs.clear();
    ifs.seekg(seekBeg);
    writeVec(Companies, ofs);
    writeVec(Positions, ofs);
    writeVec(Requirements, ofs);
    writeInt(vecsize, ofs);
    // якщо посада та компанія співпадають, то змінюємо кількість вакансій
    for (int i = 0; i < vecsize; i++)
    {
        data = readWorker(ifs);
        if ((data->position.position == position) && (data->company == company))
        {
            data->position.vacancies = vacancies;
        }
        writeWorker(data, ofs);
    }
    ofs.close();
    ifs.close();
    remove(filename.c_str());
    rename(filename_temp.c_str(), filename.c_str()); // заміна бази даних тимчасовим файлом
    ifs.open(filename, ios::in);
}