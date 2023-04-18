#include <unordered_map>
#include <fstream>

#include "data.hpp"
#include "globals.hpp"
#include "file_io.hpp"
#include "queries.hpp"

// генерація робітника
void generateData(Data *data)
{
    const int nameMinLen = 4; // мінімальна довжина імені
    const int nameMaxLen = 12; // максимальна довжина імені
    const int maxSalary = 50000; // максимальна зарплатня
    const int maxVacancies = 50; // максимальна кількість вакансій
    static unordered_map<string, bool> namesMap; // хеш-таблиця імен (щоб не було дублікатів)
    static unordered_map<int, pair<vector<int>, int>> vacanciesMap; // хеш-таблиця вакансій
    // хеш-таблиця вакансій потрібна для того, щоб кількість вакансій на одну посаду
    // в одній компанії всюди була однаковою
    char buf[nameMaxLen + 1];

    while (true) // генерація унікального імені
    {
        int nameLen = nameMinLen + rand() % (nameMaxLen - nameMinLen);
        for (int i = 0; i < nameLen; i++)
        {
            buf[i] = 'a' + rand() % ('z' - 'a' + 1);
        }
        buf[nameLen] = '\0';

        if (!namesMap.count(buf))
        {
            break;
        }
    }

    // додаємо ім'я в структуру та генеруємо зарплатню 
    data->worker.name = buf;
    namesMap.insert(pair<string, bool>(data->worker.name, true));
    data->worker.salary = rand() % maxSalary;

    // генерація решти значень
    data->company = rand() % Companies.size();
    data->position.position = rand() % Positions.size();
    // індекс для двовимірної таблиці компаній та посад. Насправді такої таблиці не існує,
    // а цей індекс використовується як унікальний ідентифікатор для хеш-таблиці
    int index = data->company * Positions.size() + data->position.position;
    pair<vector<int>, int> *p;
    // якщо на цю посаду та компанію вже згенеровано вакансії, то записати їх
    if (vacanciesMap.count(index))
    {
        p = &vacanciesMap.at(index);
        data->position.requirements = p->first;
        data->position.vacancies = p->second;
    }
    else // а інакше згенерувати їх
    {
        data->position.requirements.resize(0);
        for (int i = 0; i < Requirements.size(); i++)
        {
            if (rand() % 2)
            {
                data->position.requirements.push_back(i);
            }
        }
        data->position.vacancies = rand() % maxVacancies;
        vacanciesMap.insert(pair<int, pair<vector<int>, int>> (index,
            pair<vector<int>, int>(data->position.requirements, data->position.vacancies)));
    }
}

// додавання робітника в базу даних
void addData(string &name, int salary, int company, int position, ifstream &ifs)
{
    Data *newData = new Data;
    Data *data;
    newData->worker.name = name;
    newData->worker.salary = salary;
    newData->company = company;
    newData->position.position = position;
    bool reqsFound = false;
    ofstream ofs(filename_temp, ios::out);
    ifs.clear();
    ifs.seekg(seekBeg);
    writeVec(Companies, ofs);
    writeVec(Positions, ofs);
    writeVec(Requirements, ofs);
    writeInt(vecsize + 1, ofs);

    for (int i = 0; i < vecsize; i++) // якщо такий робітник вже існує, то return
    {
        data = readWorker(ifs);
        if (data->worker.name == name)
        {
            ofs.close();
            remove(filename_temp.c_str());
            return;
        }
        if ((!reqsFound) && (data->company == company) && (data->position.position == position))
        { // пошук вимог на задану компанію та посаду
            newData->position.requirements = data->position.requirements;
            newData->position.vacancies = data->position.vacancies;
            reqsFound = true;
        }
        writeWorker(data, ofs); // переписування бази даних у тимчасовий файл
    }
    if (!reqsFound) // якщо вимоги та к-сть вакансій не знайдено, то ввести з клавіатури
    {
        queryRequirements(newData->position.requirements);
        queryVacancies(newData->position.vacancies);
    }
    writeWorker(newData, ofs); // записати робітника у тимчасовий файл

    ofs.close();
    ifs.close();
    remove(filename.c_str());
    rename(filename_temp.c_str(), filename.c_str()); // замінити базу даних тимчасовим файлом
    ifs.open(filename, ios::in);
    vecsize++;
    delete newData;
}

// видалити робітника з бази даних
int removeData(string &name, ifstream &ifs)
{
    Data *data;
    bool isDeleted = false;
    ofstream ofs(filename_temp, ios::out);
    ifs.clear();
    ifs.seekg(seekBeg);
    writeVec(Companies, ofs);
    writeVec(Positions, ofs);
    writeVec(Requirements, ofs);
    writeInt(vecsize - 1, ofs);

    // переписування всіх робітників крім того, якого треба видалити
    for (int i = 0; i < vecsize; i++)
    {
        data = readWorker(ifs);
        if (data->worker.name != name)
        {
            writeWorker(data, ofs);
        }
        else
        {
            isDeleted = true;
        }
    }
    ifs.close();
    ofs.close();
    if (isDeleted)
    {
        remove(filename.c_str());
        rename(filename_temp.c_str(), filename.c_str());
        vecsize--;
        return 0;
    }
    else
    {
        remove(filename_temp.c_str());
        return -1; // повернути -1, якщо робітника з таким ім'ям не знайдено
    }
    ifs.open(filename, ios::in);
}

// замінити робітника
void replaceData(string &name, int salary, int company, int position,
                 vector<int> &requirements, int vacancies, ifstream &ifs)
{
    Data *data;
    Data *newData = new Data;
    newData->company = company;
    newData->worker.name = name;
    newData->worker.salary = salary;
    newData->position.position = position;
    newData->position.requirements = requirements;
    newData->position.vacancies = vacancies;
    ifs.clear();
    ifs.seekg(seekBeg);
    ofstream ofs(filename_temp, ios::out);
    writeVec(Companies, ofs);
    writeVec(Positions, ofs);
    writeVec(Requirements, ofs);
    writeInt(vecsize, ofs);

    for (int i = 0; i < vecsize; i++)
    {
        data = readWorker(ifs);
        if (data->worker.name != name)
        {
            writeWorker(data, ofs);
        }
        else
        {
            writeWorker(newData, ofs);
        }
    }
    ofs.close();
    ifs.close();
    remove(filename.c_str());
    rename(filename_temp.c_str(), filename.c_str());
    ifs.open(filename, ios::in);
}

// вивести базу даних
void viewData(ifstream &ifs)
{
    Data *data;
    ifs.clear();
    ifs.seekg(seekBeg);
    printf("%15s %15s %10s %7s %10s\n", "Name", "Company", "Position", "Salary", "Vacancies");
    for (int i = 0; i < vecsize; i++)
    {
        data = readWorker(ifs);
        printf("%15s %15s %10s %7d %10d\n", data->worker.name.c_str(),
            Companies[data->company].c_str(), Positions[data->position.position].c_str(),
            data->worker.salary, data->position.vacancies);
    }
}