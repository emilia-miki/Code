#include <iostream>

char find_min(std::string &str, int l)
{
    char min = -1;
    for (int i = l; i < str.length(); i++)
    {
        if ((str[i] < 'J') || (str[i] > 'S')) continue;
        if ((min == -1) || (str[i] < str[min])) min = i;
    }
    return min;
}

int main(int, char **)
{
    std::string str;
    char min;
    int l = 0;

    std::cin >> str;
    for (int i = 0; i < str.length(); i++) str[i] = toupper(str[i]);

    while ((min = find_min(str, l)) != -1)
    {
        std::swap(str[l], str[min]);
        l++;    
    }
    str.resize(l);

    std::cout << str << std::endl;
}