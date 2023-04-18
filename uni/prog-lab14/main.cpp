#include <ctime>
#include <cstdlib>
#include <iostream>
#include <fstream>
#include <vector>

#include "globals.hpp"
#include "structures.hpp"
#include "file_io.hpp"
#include "queries.hpp"
#include "data.hpp"
#include "algorithms.hpp"

using namespace std;

const string filename = "prog_lab_14_file.bin"; // ім'я файлу з базою даних
const string filename_temp = ".prog_lab_14_file.bin.tmp"; // ім'я тимчасового файлу
// глобальні списки можливих компаній, посад та вимог. Надалі всі компанії, посади та вимоги
// зберігаються як цілі числа, що є індексами елементів цих масивів
vector<string> Companies = {"Google", "Apple", "Amazon", "Microsoft", "Facebook", "EPAM", "Genesis", "Spotify"};
vector<string> Positions = {"junior", "middle", "senior", "tech lead", "team lead", "QA", "PM"};
vector<string> Requirements = {"C++", "Java", "C#", "Python", "C", "JS", "TypeScript", "Node JS", "PHP"};
int vecsize; // розмір бази даних
streampos seekBeg; // буде вказувати на початок бази даних одразу після глобальних масивів на початку файлу

int main(int, char **)
{
    int query;
    vector<Data> vec;
    Data *data;
    string name;
    vector<int> requirements;
    int salary, position, company, vacancies;
    int reqSize;
    bool workerExists;
    bool reqFound;
    char *buffer = new char[BUFSIZ];
    char *ptr = buffer;

    srand(time(nullptr));
    ifstream ifs(filename, ios::in);
    if (!ifs.is_open()) // якщо файл бази даних не існує, то автоматично генерується база даних
    {
        while (true)
        {
            cout << "No database found. Generating\n";
            cout << "How many entries do you need?\n";
            cin >> vecsize;
            if (vecsize < 0)
            {
                cout << "Invalid number.\n";
            }
            else
            {
                break;
            }
        }
        ofstream ofs(filename, ios::out);
        // на початку файлу пишуться глобальні масиви та розмір бази даних
        writeVec(Companies, ofs);
        writeVec(Positions, ofs);
        writeVec(Requirements, ofs);
        writeInt(vecsize, ofs);
        data = new Data;
        for (int i = 0; i < vecsize; i++)
        {
            generateData(data);
            writeWorker(data, ofs);
        }
        delete data;
        ofs.close();
        ifs.open(filename, ios::in);
    }

    // зчитування глобальних масивів та розміру бази даних
    readVec(Companies, ifs);
    readVec(Positions, ifs);
    readVec(Requirements, ifs);
    readInt(vecsize, ifs);
    seekBeg = ifs.tellg();

    bool err;
    while (true) // меню
    {
        if (!ifs.is_open())
        {
            ifs.open(filename, ios::in);
        }
        ifs.clear();
        ifs.seekg(seekBeg);
        cout << "MENU\nChoose an option:\n";
        cout << "0. Exit program\n";
        cout << "1. Find companies with the maximum number of vacancies for a position\n";
        cout << "2. Print the list of requirements for a position in every company\n";
        cout << "3. Create an array of vacancies and salaries\n";
        cout << "4. Add data to the database\n";
        cout << "5. Replace data in the database\n";
        cout << "6. Delete data in the database\n";
        cout << "7. View data\n";
        cin >> query;
        switch (query)
        {
        case 0:
            return 0;

        case 1:
            findMaxVacancies(vec, ifs);
            break;

        case 2:
            printRequirements(vec, ifs);
            break;

        case 3:
            createVacanciesArray(vec, ifs);
            break;

        case 4:
            cout << "Enter the worker's name:\n";
            cin >> name;
            err = false;
            for (int i = 0; i < vecsize; i++)
            {
                data = readWorker(ifs);
                if (name == data->worker.name)
                {
                    cout << "This worker already exists.\n";
                    err = true;
                    break;
                }
            }
            if (err)
            {
                continue;
            }
            querySalary(salary); // введення відповідних змінних з клавіатури
            queryCompany(company);
            queryPosition(position);
            addData(name, salary, company, position, ifs);
            cout << "The worker was successfully added to the database!\n";
            break;

        case 5:
            while (true)
            {
                cout << "What do you want to replace?\n";
                cout << "0. Go back\n";
                cout << "1. Replace a worker's data\n";
                cout << "2. Change vacancies number for a position in a company\n";
                cin >> query;
                if ((query < 0) || (query > 2))
                {
                    cout << "Invalid choice.\n";
                }
                else
                {
                    break;
                }
            }

            switch (query)
            {
            case 0:
                continue;
                break;

            case 1:
                cout << "Enter the worker's name:\n";
                cin >> name;
                workerExists = false;
                reqFound = false;
                // перевірка на існування робітника з таким ім'ям, а також пошук відповідних позиції вимог та вакансій
                for (int i = 0; i < vecsize; i++)
                {
                    data = readWorker(ifs);
                    if (data->worker.name == name)
                    {
                        workerExists = true;
                    }
                    if ((!reqFound) && (data->position.position == position) && (data->company == company))
                    {
                        requirements = data->position.requirements;
                        vacancies = data->position.vacancies;
                        reqFound = true;
                    }
                }
                if (!workerExists)
                {
                    cout << "There is no such worker.\n";
                    continue;
                }
                cout << "Enter the new data:\n";
                querySalary(salary);
                queryCompany(company);
                queryPosition(position);
                if (!reqFound) // якщо вимоги та вакансії на цю посаду не знайдено, то ввести з клавіатури
                {
                    queryRequirements(requirements);
                    queryVacancies(vacancies);
                }
                replaceData(name, salary, company, position, requirements, vacancies, ifs);
                break;
            case 2:
                queryCompany(company);
                queryPosition(position);
                queryVacancies(vacancies);
                updateVacancies(company, position, vacancies, ifs);
                break;
            }
            cout << "The data was successfully updated.\n";
            break;

        case 6:
            cout << "Enter the name of the worker you want to delete: ";
            cin >> name;
            if (removeData(name, ifs) == -1)
            {
                cout << "This worker does not exist in the database.\n";
                break;
            }
            cout << "The entry was deleted successfully.\n";
            break;

        case 7:
            viewData(ifs);
            break;

        default:
            cout << "Invalid query.\n";
            break;
        }
    close:
        ifs.close();
    }
}
