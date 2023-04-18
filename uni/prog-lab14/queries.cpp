#include <iostream>

#include "globals.hpp"
#include "queries.hpp"

using namespace std;

// зчитування змінних з клавіатури з потрібними перевірками

void queryCompany(int &company)
{
    while (true)
    {
        cout << "Choose a company:\n";
        for (int i = 0; i < Companies.size(); i++)
        {
            cout << i << ". " << Companies[i] << endl;
        }
        cin >> company;
        if ((company < 0) || (company >= Companies.size()))
        {
            cout << "Invalid company.\n";
        }
        else
        {
            break;
        }
    }
}

void queryPosition(int &position)
{
    while (true)
    {
        cout << "Choose a position:\n";
        for (int i = 0; i < Positions.size(); i++)
        {
            cout << i << ". " << Positions[i] << endl;
        }
        cin >> position;
        if ((position < 0) || (position >= Positions.size()))
        {
            cout << "Invalid position.\n";
        }
        else
        {
            break;
        }
    }
}

void querySalary(int &salary)
{
    while (true)
    {
        cout << "Enter the worker's salary: ";
        cin >> salary;
        if (salary < 0)
        {
            cout << "Invalid salary.\n";
        }
        else
        {
            break;
        }
    }
}

void queryRequirements(vector<int> &reqs)
{
    int req, reqSize;
    bool isValid;

    while (true)
    {
        cout << "How many requirements are there (up to " << Requirements.size() << ")?\n";
        cin >> reqSize;
        if ((reqSize < 0) || (reqSize >= Requirements.size()))
        {
            cout << "Invalid number.\n";
        }
        else
        {
            break;
        }
    }

    reqs.resize(reqSize);
    while (true)
    {
        cout << "Choose requirements:\n";
        for (int i = 0; i < Requirements.size(); i++)
        {
            cout << i << ". " << Requirements[i] << endl;
        }
        for (int i = 0; i < reqSize; i++)
        {
            cin >> reqs[i];
        }
        isValid = true;
        for (int i = 0; i < reqSize; i++)
        {
            if ((reqs[i] < 0) || (reqs[i] > Requirements.size()))
            {
                cout << "Invalid requirements";
                isValid = false;
                break;
            }
        }
        if (isValid)
        {
            break;
        }
    }
}

void queryVacancies(int &vacancies)
{
    while (true)
    {
        cout << "Enter the number of vacancies: ";
        cin >> vacancies;
        if (vacancies < 0)
        {
            cout << "The number must be positive.\n";
        }
        else
        {
            break;
        }
    }
}