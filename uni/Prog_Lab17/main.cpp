// y = x * arctan(x)
#include <cmath>
#include <string>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>

double f(double x) // функція з умови
{
    return x * atan(x);
}
#include <iostream>
double findStep(double number) // знайти оптимальний крок для поділок
{
    // int i = 0;
    // while ((int)number == 0)
    // {
    //     number *= 10;
    //     i++;
    // }
    // number = round(number) / pow(10, i);
    return number;
}

std::string toString(double number) // округлити дійсне число та привести до типу string
{
    char str[BUFSIZ];
    int i = 0;
    while ((int)number == 0)
    {
        number *= 10;
        i++;
    }
    number *= 100;
    number = round(number);
    number /= pow(10, i + 2);
    sprintf(str, "%.2f", number);
    return std::string(str);
}

sf::RenderWindow window;          // вікно програми
sf::Text text;                    // об'єкт для виведення поділок
const int charsize = 20;          // розмір шрифту
double minx = -2.0f, maxx = 2.0f; // видима на вікні область визначення функції
double miny = 0, maxy = f(maxx);  // відповідна область значень функції // TODO: find minx and maxx computationally
int resx = 800, resy = 600;       // роздільність вікна
// крок для переміщення, приближення та віддалення графіку
double emx = (maxx - minx) / 10, emy = (maxy - miny) / 10;
// невеликі дельти для врахування похибок типу double
double dx = (maxx - minx) / resx;
double dy = (maxy - miny) / resy;

double xtopx(double x) // знайти аргумент функції в координатах вікна
{
    return minx + (resx / (maxx - minx)) * (x - minx);
}

double pxtox(double px) // знайти значення функції, що відповідає заданій координаті
{
    return minx + ((maxx - minx) / resx) * px;
}

double ytopx(double y) // знайти значення функції в координатах вікна
{
    return resy - miny - (resy / (maxy - miny)) * (y - miny);
}

double pxtoy(double px) // знайти аргумент функції, що відповідає заданій координаті
{
    return miny + ((maxy - miny) / resy) * (resy - px);
}

void renderXAxis()
{
    // знайти нулі функції в координатах вікна
    double x = xtopx(0);
    double y = ytopx(0);

    // якщо нуль функції знаходиться за межами видимої області, то малювати вісь біля краю вікна
    if ((x < 30) || (x - resx > -30))
    {
        x = 0;
    }
    if ((y < 30) || (y - resy > -30))
    {
        y = (double)resy - 1.0;
    }

    // провести чорну горизонтальну пряму по краю вікна
    sf::VertexArray lines(sf::Lines, 2);
    lines[0].position.x = 1.0f;
    lines[0].position.y = y;
    lines[0].color = sf::Color::Black;
    lines[1].position.x = (double)resx - 1.0f;
    lines[1].position.y = y;
    lines[1].color = sf::Color::Black;
    window.draw(lines);

    double step = findStep((maxx - minx) / 16); // знайти оптимальний крок для поділок

    // зробити нульову поділку
    if ((x > 30) && (x - resx < -30))
    {
        lines[0].position.x = x;
        lines[0].position.y = y - resy / 200;
        lines[1].position.x = x;
        lines[1].position.y = y + resy / 200;
        window.draw(lines);
    }

    // зробити поділки від нуля функції до нижнього краю
    double steptopx = xtopx(step) - xtopx(0);
    for (double i = x + steptopx; i < resx - 30; i += steptopx)
    {
        // вивести відповідне значення функції біля поділки
        text.setString(toString(pxtox(i)));
        text.setPosition(sf::Vector2f(i - charsize, y - charsize - charsize / 2));
        window.draw(text);
        // намалювати поділку
        lines[0].position.x = i;
        lines[0].position.y = y - resx / 200;
        lines[1].position.x = i;
        lines[1].position.y = y + resx / 200;
        window.draw(lines);
    }

    for (double i = x - steptopx; i >= 30; i -= steptopx) // зробити поділки від нуля функції до верхнього краю
    {
        // вивести відповідне значення функції біля поділки
        text.setString(toString(pxtox(i)));
        text.setPosition(sf::Vector2f(i - charsize, y - charsize - charsize / 2));
        window.draw(text);
        // намалювати поділку
        lines[0].position.x = i;
        lines[0].position.y = y - resx / 200;
        lines[1].position.x = i;
        lines[1].position.y = y + resx / 200;
        window.draw(lines);
    }

    // намалювати стрілочку
    lines[0].position.x = (double)resx - 1.0f;
    lines[0].position.y = y;
    lines[1].position.x = (double)resx - 1.0f - resx / 100;
    lines[1].position.y = y - resy / 100;
    window.draw(lines);
    lines[1].position.x = (double)resx - 1.0f - resx / 100;
    lines[1].position.y = y + resy / 100;
    window.draw(lines);
}

void renderYAxis()
{
    // знайти нулі функції в координатах вікна
    double x = xtopx(0);
    double y = ytopx(0);

    // якщо нуль функції знаходиться за межами видимої області, то малювати вісь біля краю вікна
    if ((x < 30) || (x - resx > -30))
    {
        x = 1.0;
    }
    if ((y < 30) || (y - resy > -30))
    {
        y = 0;
    }

    // провести чорну вертикальну пряму по краю вікна
    sf::VertexArray lines(sf::Lines, 2);
    lines[0].position.x = x;
    lines[0].position.y = 1.0f;
    lines[0].color = sf::Color::Black;
    lines[1].position.x = x;
    lines[1].position.y = (double)resy - 1.0f;
    lines[1].color = sf::Color::Black;
    window.draw(lines);

    double step = findStep((maxx - minx) / 16); // знайти оптимальний крок для поділок

    // зробити нульову поділку
    if ((y > 30) && (y - resy < -30))
    {
        lines[0].position.x = x - resx / 200;
        lines[0].position.y = y;
        lines[1].position.x = x + resx / 200;
        lines[1].position.y = y;
        window.draw(lines);
    }

    // зробити поділки від нуля функції до нижнього краю
    double steptopx = xtopx(step) - xtopx(0);
    for (double i = y + steptopx; i < resy - 30; i += steptopx)
    {
        // вивести відповідне значення функції біля поділки
        text.setString(toString(pxtoy(i)));
        text.setPosition(sf::Vector2f(x + charsize / 2, i - charsize));
        window.draw(text);
        // намалювати поділку
        lines[0].position.x = x - resx / 200;
        lines[0].position.y = i;
        lines[1].position.x = x + resx / 200;
        lines[1].position.y = i;
        window.draw(lines);
    }

    for (double i = y - steptopx; i >= 30; i -= steptopx) // зробити поділки від нуля функції до верхнього краю
    {
        // вивести відповідне значення функції біля поділки
        text.setString(toString(pxtoy(i)));
        text.setPosition(sf::Vector2f(x + charsize / 2, i - charsize));
        window.draw(text);
        // намалювати поділку
        lines[0].position.x = x - resx / 200;
        lines[0].position.y = i;
        lines[1].position.x = x + resx / 200;
        lines[1].position.y = i;
        window.draw(lines);
    }

    // намалювати стрілочку
    lines[0].position.x = x;
    lines[0].position.y = 1.0f;
    lines[1].position.x = x - resx / 100;
    lines[1].position.y = 1.0f + resy / 100;
    window.draw(lines);
    lines[1].position.x = x + resx / 100;
    lines[1].position.y = 1.0f + resy / 100;
    window.draw(lines);
}

void renderFunction() // оновлення графіку функції
{
    sf::VertexArray lines(sf::LineStrip, 2); // LineStrip - кінець попередньої лінії є початком наступної
    // встановити першу точку графіку
    lines[1].position.x = 0.0f;
    lines[1].position.y = ytopx(f(minx));
    lines[1].color = sf::Color::Black;
    double step = (maxx - minx) / resx;                // порахувати крок так, щоб функція була підрахована для кожного пікселя
    for (double i = minx + step; i <= maxx; i += step) // заповнити всю видиму область визначення
    {
        lines[0] = lines[1];               // кінець попередньої лінії стає початком наступної
        lines[1].position.x = xtopx(i);    // перевести аргумент i у пікселі
        lines[1].position.y = ytopx(f(i)); // знайти значення функції
        window.draw(lines);                // намалювати лінію
    }
}

void renderZero()
{
    // знайти нуль функції
    double x = xtopx(0);
    double y = ytopx(0);

    // перевірити, чи попадає він на графік
    sf::VertexArray lines(sf::Lines, 2);
    if (((x < 0) || (x > resx)) && ((y < 0) || (y > resy)))
    {
        return;
    }

    // розглянути три випадки:
    // * потрібно вивести спільний нуль для обох осей
    // * потрібно вивести нуль для осі Х
    // * потрібно вивести нуль для осі У
    if ((y < 0) || (y > resy))
    {
        y = (double)resy;
    }
    else if ((x < 0) || (x > resx))
    {
        x = 0;
    }

    x += charsize / 2;
    y -= charsize + charsize / 2;

    text.setString("0");
    text.setPosition(sf::Vector2f(x, y));
    window.draw(text);
}

int main(int, char **)
{
    sf::Font font;
    if (!font.loadFromFile("../times new roman.ttf")) // загрузити шрифт з файлу
    {
        exit(EXIT_FAILURE);
    }
    text.setFont(font);                  // встановити шрифт тексту
    text.setCharacterSize(charsize);     // встановити розмір тексту
    text.setFillColor(sf::Color::Black); // встановити колір тексту

    window.create(sf::VideoMode(resx, resy), "y = x * arctan(x)"); // створити вікно

    while (window.isOpen()) // основний цикл програми
    {
        sf::Event event;
        while (window.pollEvent(event)) // цикл, який "ловить" події
        {
            switch (event.type)
            {
            case sf::Event::Closed: // закриття вікна
                window.close();
                break;

            case sf::Event::Resized: // якщо змінено розмір вікна
                resx = event.size.width;
                resy = event.size.height;
                window.setView(sf::View(sf::FloatRect(0, 0, resx, resy))); // розширити область видимості вікна
                dx = (maxx - minx) / resx;
                dy = (maxy - miny) / resy;
                break;

            case sf::Event::KeyPressed:   // якщо нажато клавішу
                // emx = (maxx - minx) / 10; // підрахувати крок для зміщень
                // emy = (maxy - miny) / 10;
                switch (event.key.code) // перевірити, яку клавішу було нажато
                {
                case sf::Keyboard::PageUp: // розширити область
                case sf::Keyboard::Equal:
                    maxx -= emx;
                    minx += emx;
                    miny += emy;
                    maxy -= emy;
                    break;

                case sf::Keyboard::PageDown: // звузити область
                case sf::Keyboard::Dash:
                    maxx += emx;
                    minx -= emx;
                    miny -= emy;
                    maxy += emy;
                    break;

                case sf::Keyboard::Up: // змістити область донизу
                    miny += emy;
                    maxy += emy;
                    break;

                case sf::Keyboard::Down: // змістити область доверху
                    miny -= emy;
                    maxy -= emy;
                    break;

                case sf::Keyboard::Left: // змістити область вліво
                    minx -= emx;
                    maxx -= emx;
                    break;

                case sf::Keyboard::Right: // змістити область вправо
                    minx += emx;
                    maxx += emx;
                    break;

                default:
                    break;
                }
                break;

            default:
                break;
            }
        }

        window.clear(sf::Color::White); // очистити вікно та залити білим кольором

        renderFunction(); // оновити графік
        renderYAxis();    // оновити вісь Y
        renderXAxis();    // оновити вісь Х
        renderZero();     // вивести число нуль біля потрібної поділки

        window.display(); // оновити вікно
    }

    return 0;
}
