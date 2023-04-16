using System;
using System.Collections.Generic;

namespace paca_lab1
{
    internal static class Program
    {
        private static double CalculatePolynomial(in double[] coefficients, double x)
        {
            var n = coefficients.Length - 1;
            var b = coefficients[n];
            
            for (var i = n - 1; i >= 0; i--)
            {
                b = coefficients[i] + b * x;
            }

            return b;
        }

        private static double SumOfElementsRec(in List<double[]> matrix, int 
            yFrom, int yTo, int xFrom, int xTo)
        {
            if (yFrom == yTo)
            {
                return 0;
            }
            
            if (yTo != yFrom + 1)
            {
                return SumOfElementsRec(matrix,
                           yFrom, yFrom + (yTo - yFrom) / 2, xFrom, xTo) +
                       SumOfElementsRec(matrix,
                           yFrom + (yTo - yFrom) / 2, yTo, xFrom, xTo);
            }

            if (xTo != xFrom + 1)
            {
                return SumOfElementsRec(matrix, 
                           yFrom, yTo, xFrom, xFrom + (xTo - xFrom) / 2) +
                       SumOfElementsRec(matrix, 
                           yFrom, yTo, xFrom + (xTo - xFrom) / 2, xTo);
            }
            
            return matrix[yFrom][xFrom];
        }

        private static double SumOfElements(in List<double[]> matrix)
        {
            if (matrix.Count == 0)
            {
                return 0;
            } 
            return SumOfElementsRec(
                matrix, 0, matrix.Count, 0, matrix[0].Length);
        }

        private static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("What do you want to do?\n" +
                             "0. Exit program\n" +
                             "1. Compute a polynomial\n" +
                             "2. Find the sum of elements of A\n");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out var choice) 
                    || choice is < 0 or > 2)
                {
                    Console.WriteLine("Invalid option!\n");
                    continue;
                }

                if (HandleChoice(choice)) continue;
                Console.WriteLine("Mykyta Diachyna DA-01");
                break;
            }
        }

        private static (double[], double) GetPolynomialInput()
        {
            double[] coefficients;
            var error = false;
            double x;
            while (true)
            {
                Console.WriteLine("The polynomial form is " 
                                  + "a_n * x^n + ... " 
                                  + "+ a_2 * x^2 + a_1 * x^1 + a0.\n"
                                  + "Enter the coefficients " 
                                  + "a0, a1, ..., a_n in one row:");
                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine("Invalid input!");
                    continue;
                }

                var tokens = input.Split(' ');
                coefficients = new double[tokens.Length];
                for (var i = 0; i < tokens.Length; i++)
                {
                    if (double.TryParse(tokens[i], out coefficients[i])) continue;
                    Console.WriteLine(tokens[i] + " is not a number!");
                    error = true;
                }

                if (error)
                {
                    error = false;
                    continue;
                }

                while (true)
                {
                    Console.Write("Enter x value: ");
                    input = Console.ReadLine();
                    if (double.TryParse(input, out x))
                    {
                        break;
                    }

                    Console.WriteLine(input + " is not a number!");
                }
                break;
            }

            return (coefficients, x);
        }

        private static bool HandleChoice(int choice)
        {
            var error = false;
            switch (choice)
            {
                case 0:
                    return false;
                case 1:
                    var (coefficients, x) = GetPolynomialInput();
                    Console.WriteLine("The polynomial value P(x) = " 
                                      + CalculatePolynomial(in coefficients, x));
                    break;
                case 2:
                    var matrix = new List<double[]>();
                    Console.WriteLine("Enter the matrix A line by line:");
                    var columns = int.MaxValue;
                    var firstLine = true;
                    while (true)
                    {
                        var input = Console.ReadLine();
                        if (string.IsNullOrEmpty(input))
                        {
                            break;
                        }

                        var tokens = input.Split(' ');
                        var numbers = new double[tokens.Length];
                        for (var i = 0; i < tokens.Length; i++)
                        {
                            if (double.TryParse(tokens[i], out numbers[i])) continue;
                            Console.WriteLine(tokens[i] + " is not a number!");
                            error = true;
                        }

                        if (error)
                        {
                            error = false;
                            continue;
                        }

                        if (firstLine)
                        {
                            columns = tokens.Length;
                            firstLine = false;
                        }
                        else if (numbers.Length != columns)
                        {
                            Console.WriteLine("Every row must have "
                                              + columns
                                              + " numbers, but you entered "
                                              + numbers.Length + "!");
                            continue;
                        }

                        matrix.Add(numbers);
                    }

                    Console.WriteLine("The sum of all elements of A is " 
                                      + SumOfElements(matrix));
                    break;
            }

            return true;
        }
    }
}