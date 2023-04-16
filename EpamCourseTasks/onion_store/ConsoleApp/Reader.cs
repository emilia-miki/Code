using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using OnionCore;

namespace ConsoleApp
{
    /// <summary>
    /// A helper class for reading data from terminal.
    /// </summary>
    public static class Reader
    {
        /// <summary>
        /// Helper method to prompt and read a string.
        /// </summary>
        /// <param name="label">A label to display in prompt.</param>
        /// <returns>The entered string.</returns>
        public static string GetString(string label)
        {
            Console.Write(label + ": ");
            var str = Console.ReadLine();
            return str!;
        }

        /// <summary>
        /// Helper method to prompt and read an integer.
        /// </summary>
        /// <param name="label">A label to display in prompt.</param>
        /// <returns>The entered integer.</returns>
        /// <exception cref="FormatException">Thrown when parsing of integer from string failed.</exception>
        public static int GetInt(string label)
        {
            Console.Write(label + ": ");
            var str = Console.ReadLine();
            return int.Parse(str!);
        }

        /// <summary>
        /// Helper method to prompt and read a decimal.
        /// </summary>
        /// <param name="label">A label to display in prompt.</param>
        /// <returns>The entered decimal.</returns>
        /// <exception cref="FormatException">Thrown when parsing of decimal from string failed.</exception>
        public static decimal GetDecimal(string label)
        {
            Console.Write(label + ": ");
            var str = Console.ReadLine();
            return decimal.Parse(str!);
        }

        /// <summary>
        /// Helper method to prompt and read an order status.
        /// </summary>
        /// <returns>The entered order status.</returns>
        /// <exception cref="FormatException">Thrown when parsing of order status from string failed.</exception>
        public static OrderStatus GetStatus()
        {
            Console.Write("Enter the new status (New, CanceledByTheAdministrator, Sent, Completed): ");
            var line = Console.ReadLine();
            if (!Enum.TryParse(line, out OrderStatus status))
            {
                throw new FormatException("The specified status does not exist");
            }

            return status;
        }

        /// <summary>
        /// Helper method to prompt and read personal information.
        /// </summary>
        /// <returns>The entered personal information.</returns>
        /// <exception cref="FormatException">Thrown when any entered field is invalid.</exception>
        public static PersonalInformation GetPersonalInformation()
        {
            var info = new PersonalInformation
            {
                Name = GetString("Name"),
                Surname = GetString("Surname"),
                Email = GetString("Email"),
                PhoneNumber = GetString("Phone number")
            };

            _ = new MailAddress(info.Email).Address; // throw FormatException if not an email

            var validatePhoneNumberRegex = new Regex("^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)" +
                                                     "?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$");
            if (!validatePhoneNumberRegex.IsMatch(info.PhoneNumber))
            {
                throw new FormatException("Not a phone number");
            }

            return info;
        }

        /// <summary>
        /// Gets a <c>ProductAndAmount</c> list for ordering.
        /// </summary>
        /// <returns>A list of <c>ProductAndAmount</c> entries.</returns>
        /// <exception cref="FormatException">Thrown when some entered field is invalid.</exception>
        public static IEnumerable<ProductAndAmount> GetProductList()
        {
            var list = new List<ProductAndAmount>();
            var id = GetInt("Enter product ID");
            var amount = GetInt("Enter product amount");
            list.Add(new ProductAndAmount
            {
                Product = new Product
                {
                    Id = id
                },
                Amount = amount
            });

            while (true)
            {
                Console.WriteLine("Do you want to order another product? (Y/n)");
                var response = Console.ReadLine();

                if (response!.ToLower() == "n")
                {
                    break;
                }

                id = GetInt("Enter product ID");
                amount = GetInt("Enter product amount");
                list.Add(new ProductAndAmount
                {
                    Product = new Product
                    {
                        Id = id
                    },
                    Amount = amount
                });
            }

            return list.AsEnumerable();
        }
    }
}