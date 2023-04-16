using System;
using System.Collections.Generic;
using OnionCore;

namespace ConsoleApp
{
    /// <summary>
    /// A helper class for writing data to terminal.
    /// </summary>
    public static class Writer
    {
        /// <summary>
        /// A helper method that displays available commands.
        /// </summary>
        public static void DisplayCommands(IEnumerable<string> commands)
        {
            Console.WriteLine("Possible commands:");
            foreach (var cmd in commands)
            {
                Console.WriteLine("    " + cmd);
            }

            Console.WriteLine("    help");
            Console.WriteLine("    exit");
        }

        /// <summary>
        /// A helper method that outputs <c>PersonalInformation</c>.
        /// </summary>
        /// <param name="tuple">A tuple of: user ID, user login, <c>PersonalInformation</c>.</param>
        public static void WritePersonalInformation(Tuple<int, string, PersonalInformation> tuple)
        {
            Console.WriteLine("User ID: " + tuple.Item1);
            Console.WriteLine("User login: " + tuple.Item2);
            var info = tuple.Item3;
            Console.WriteLine("Name: " + info.Name);
            Console.WriteLine("Surname: " + info.Surname);
            Console.WriteLine("Email: " + info.Email);
            Console.WriteLine("Phone number: " + info.PhoneNumber);
        }

        /// <summary>
        /// A helper method to output a list of <c>Product</c> entries.
        /// </summary>
        /// <param name="products">A list of <c>Product</c> entries.</param>
        public static void WriteProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                WriteProduct(product);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// A helper method that outputs a list of <c>Order</c> entries.
        /// </summary>
        /// <param name="orderEnumerable">A list of <c>Order</c> entries.</param>
        public static void WriteOrders(IEnumerable<Order> orderEnumerable)
        {
            foreach (var order in orderEnumerable)
            {
                WriteOrder(order);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// A helper method that outputs an <c>Order</c>.
        /// </summary>
        /// <param name="order">An <c>Order</c>.</param>
        public static void WriteOrder(Order order)
        {
            Console.WriteLine("ID: " + order.Id);
            Console.WriteLine("Customer ID: " + order.Customer.Id);
            foreach (var productAndAmount in order.ProductList)
            {
                Console.WriteLine("Product: " + productAndAmount.Product.Name);
                Console.WriteLine("Amount: " + productAndAmount.Amount);
            }

            Console.WriteLine("Status: " + order.Status);
            Console.WriteLine("Creation date: " + order.DateTime);
        }

        /// <summary>
        /// A helper method that outputs a <c>Product</c>.
        /// </summary>
        /// <param name="product">A <c>Product</c>.</param>
        public static void WriteProduct(Product product)
        {
            Console.WriteLine("ID: " + product.Id);
            Console.WriteLine("Name: " + product.Name);
            Console.WriteLine("Category: " + product.Category);
            Console.WriteLine("Description: " + product.Description);
            Console.WriteLine("Cost: " + product.Cost);
        }

        /// <summary>
        /// A helper method that outputs a list of <c>IUser</c> entries.
        /// </summary>
        /// <param name="userEnumerable">A list of <c>IUser</c> entries.</param>
        public static void WriteUsers(IEnumerable<IUser> userEnumerable)
        {
            foreach (var user in userEnumerable)
            {
                WriteUser(user);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// A helper method that outputs a <c>IUser</c> entry.
        /// </summary>
        /// <param name="user">A <c>IUser</c> entry.</param>
        public static void WriteUser(IUser user)
        {
            Console.WriteLine("ID: " + user.Id);
            Console.WriteLine("Login: " + user.Login);
            Console.WriteLine("Password: " + user.Password);
        }
    }
}