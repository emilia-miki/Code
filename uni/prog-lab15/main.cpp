#include <iostream>
#include <fstream>
#include <vector>

using namespace std;

int fileSize = 0;

void readInt(fstream &fs, int &number)
{
    fs.read(reinterpret_cast<char *>(&number), sizeof(int));
}

void writeInt(fstream &fs, int number)
{
    fs.write(reinterpret_cast<char *>(&number), sizeof(int));
}

void insertInt(fstream &fs, int number) // вставляє число в допоміжний файл
{
    int buffer;
    fs.clear();
    fs.seekg(fs.beg);
    for (int i = 0; i < fileSize; i++)
    {
        readInt(fs, buffer);
        if (buffer == 0) // якщо комірка вільна, то записати в неї
        {
            fs.seekp(i * sizeof(int));
            writeInt(fs, number);
            return;
        }
    }

    fs.clear();
    fs.seekp(fileSize * sizeof(int)); // якщо вільних комірок немає, то в кінець
    writeInt(fs, number);
    fileSize++;
}

int extractInt(fstream &fs, bool positive) // знайти у файлі число заданого знаку
{
    int number, extracted;

    fs.seekg(fs.beg);
    fs.clear();
    for (int i = 0; i < fileSize; i++)
    {
        readInt(fs, number);
        if (fs.fail())
        {
            cerr << "Something went wrong\n";
            return 0;
        }
        if (number == 0) // пропускаємо вільні комірки
        {
            continue;
        }
        // якщо знак числа співпадає зі вказаним, то повернути число та стерти з файлу
        if (positive == bool(number > 0))
        {
            fs.seekp(i * sizeof(int));
            writeInt(fs, 0);
            return number;
        }
    }

    return 0; // якщо числа потрібного знаку не знайдено, повернути 0 як помилку
}

int main(int argc, char **argv)
{
    string fname, hname, gname;
    if (argc == 3)
    {
        fname = argv[1];
        gname = argv[2];
    }
    else
    {
        cout << "Enter file f's name:\n";
        cin >> fname;
        cout << "Enter file g's name:\n";
        cin >> gname;
    }
    hname = "." + fname + ".tmp";

    ifstream ifs(fname, ios::in);
    if (!ifs.is_open())
    {
        cerr << "Cannot open file " << fname << endl;
        exit(EXIT_FAILURE);
    }
    fstream fs(hname, ios::in | ios::out | ios::trunc);
    ofstream ofs(gname, ios::out);

    // прочитати перше число та записати у файл
    int number;
    bool isPositive;
    ifs >> number;
    ofs << number << " ";
    isPositive = number > 0 ? true : false;

    int qpos = 0, qneg = 0;
    while (ifs >> number) // читаємо числа з файлу
    {
        if (number == 0)
        {
            cerr << "Invalid file.\n";
            exit(EXIT_FAILURE);
        }

        if (isPositive ^ (number > 0)) // якщо число підходить, то пишемо у вихідний файл
        {
            ofs << number << " ";
            isPositive = !isPositive;
        }
        else if (isPositive ? qneg : qpos) // якщо ні, але є правильне число у допоміжному файлі, то беремо звідти
        {
            insertInt(fs, number);
            number > 0 ? qpos++ : qneg++;

            number = extractInt(fs, !isPositive);
            number > 0 ? qpos-- : qneg--;
            ofs << number << " ";
            isPositive = !isPositive;
        }
        else // якщо число не підходить і правильного числа немає у доп. файлі, то просто записуємо число в доп. файл
        {
            insertInt(fs, number);
            number > 0 ? qpos++ : qneg++;
        }
    }

    while (qpos || qneg) // якщо залишилися невикористані числа у доп. файлі, то пишемо їх у правильному порядку в вихідний
    {
        number = extractInt(fs, !isPositive);
        isPositive = !isPositive;
        if (number == 0)
        {
            cerr << "Invalid file.\n";
            exit(EXIT_FAILURE);
        }
        number > 0 ? qpos-- : qneg--;
        ofs << number << " ";
    }

    ifs.close();
    fs.close();
    ofs.close();
}