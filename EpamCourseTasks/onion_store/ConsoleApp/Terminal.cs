using System;
using System.Collections.Generic;
using OnionCore;

namespace ConsoleApp
{
    /// <summary>
    /// Terminal class. Implements ITerminal.
    /// </summary>
    public class Terminal : ITerminal
    {
        /// <inheritdoc cref="ITerminal.Commands"/>
        public IDictionary<string, Delegate> Commands { get; set; }
        
        /// <inheritdoc cref="ITerminal.User"/>
        public IUser User { get; set; }

        /// <summary>
        /// Reads required arguments for a delegate from terminal.
        /// </summary>
        /// <param name="command">The entered command by the <c>IUser</c>.</param>
        /// <returns>An array of arguments.</returns>
        private object[] AssembleArgumentsFor(string command)
        {
            switch (command)
            {
                case "viewGoods":
                case "signOut":
                    return Array.Empty<object>();
                case "login":
                case "register":
                    User = new User
                    {
                        Login = Reader.GetString("Login"),
                        Password = Reader.GetString("Password")
                    };
                    return new object[] {User.Login, User.Password};
                case "searchProductByName":
                    return new object[] {Reader.GetString("Name")};
                case "createOrder":
                    return new object[] {User, Reader.GetProductList()};
                case "payOrder":
                    return new object[] {User, Reader.GetInt("Order ID")};
                case "cancelOrder":
                case "setOrderStatusReceived":
                    return new object[] {User, Reader.GetInt("Order ID")};
                case "reviewOrderHistory":
                case "viewUsersPersonalInformation":
                    return new object[] {User};
                case "changeCredentials":
                    return new object[] {User, Reader.GetString("New login"), 
                        Reader.GetString("New password")};
                case "changePersonalInformation":
                    return new object[] {User, Reader.GetPersonalInformation()};
                case "changeUserPersonalInformation":
                    return new object[] {User, Reader.GetInt("User ID"), Reader.GetPersonalInformation()};
                case "addNewProduct":
                    return new object[]
                    {
                        User, Reader.GetString("Name"), Reader.GetString("Category"),
                        Reader.GetString("Description"), Reader.GetDecimal("Cost")
                    };
                case "changeProductInformation":
                    return new object[]
                    {
                        User, Reader.GetInt("Product ID"), Reader.GetString("New name"),
                        Reader.GetString("New category"), Reader.GetString("New description"), 
                        Reader.GetDecimal("New cost")
                    };
                case "changeOrderStatus":
                    return new object[] {User, Reader.GetInt("Order ID"), Reader.GetStatus()};
                default:
                    return Array.Empty<object>();
            }
        }

        /// <summary>
        /// Handle the return value of a delegate.
        /// </summary>
        /// <param name="value">The returned value by the delegate.</param>
        /// <exception cref="InvalidOperationException">Thrown when the returned value is not recognized.</exception>
        private void HandleReturnValue(object value)
        {
            if (value == null)
            {
                return;
            }

            if (value is string)
            {
                Console.WriteLine(value);
                return;
            }

            if (value is IDictionary<string, Delegate> dictionary)
            {
                Commands = dictionary;
                return;
            }

            if (value is Product p)
            {
                Writer.WriteProduct(p);
                return;
            }

            if (value is IEnumerable<IUser> userEnumerable)
            {
                Writer.WriteUsers(userEnumerable);
                return;
            }

            if (value is IEnumerable<Order> orderEnumerable)
            {
                Writer.WriteOrders(orderEnumerable);
                return;
            }

            if (value is IEnumerable<Product> products)
            {
                Writer.WriteProducts(products);
                return;
            }

            if (value is Tuple<int, string, PersonalInformation> tuple)
            {
                Writer.WritePersonalInformation(tuple);
                return;
            }

            throw new InvalidOperationException();
        }

        /// <inheritdoc cref="ITerminal.Start"/>
        public void Start()
        {
            while (true)
            {
                Console.Write("$ ");
                var command = Console.ReadLine()?.Trim();
                switch (command)
                {
                    case "help":
                    {
                        Writer.DisplayCommands(Commands.Keys);
                        continue;
                    }
                    case "exit":
                        return;
                }

                if (command != null && Commands.ContainsKey(command))
                {
                    object[] arguments;
                    try
                    {
                        arguments = AssembleArgumentsFor(command);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Error: incorrect format");
                        continue;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Error: " + exception.Message);
                        continue;
                    }
                    
                    var returnValue = Commands[command].DynamicInvoke(arguments);
                    HandleReturnValue(returnValue);
                    continue;
                }

                Console.WriteLine("Invalid command!");
                Writer.DisplayCommands(Commands.Keys);
            }
        }
    }
}