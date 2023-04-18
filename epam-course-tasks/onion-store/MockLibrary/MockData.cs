using System;
using System.Collections.Generic;
using OnionCore;

namespace MockLibrary
{
    /// <summary>
    /// A database mock class.
    /// </summary>
    public static class MockData
    {
        /// <summary>
        /// Gets <c>ProductAndAmount</c> entries.
        /// </summary>
        /// <returns><c>ProductAndAmount</c> entries collection.</returns>
        public static ICollection<ProductAndAmount> GetProductList()
        {
            using var enumerator = GetProducts().GetEnumerator();
            enumerator.MoveNext();
            var list = new List<ProductAndAmount>
            {
                new ProductAndAmount
                {
                    Id = 0,
                    Product = enumerator.Current,
                    Amount = 1
                }
            };
            enumerator.MoveNext();
            list.Add(new ProductAndAmount
            {
                Id = 1,
                Product = enumerator.Current,
                Amount = 3
            });
            enumerator.MoveNext();
            list.Add(new ProductAndAmount
            {
                Id = 2,
                Product = enumerator.Current,
                Amount = 5
            });
            
            return list;
        }

        /// <summary>
        /// Gets <c>PersonalInformation</c> entries.
        /// </summary>
        /// <returns>A <c>PersonalInformation</c> entries collection.</returns>
        public static ICollection<PersonalInformation> GetPersonalInformation()
        {
            return new List<PersonalInformation>
            {
                new PersonalInformation
                {
                    Id = 0,
                    Name = "James",
                    Surname = "Bond",
                    Email = "james@bond.com",
                    PhoneNumber = "380680000000"
                },
                new PersonalInformation
                {
                    Id = 1,
                    Name = "John",
                    Surname = "Jackson",
                    Email = "john@jackson.org",
                    PhoneNumber = "380681300000"
                },
                new PersonalInformation
                {
                    Id = 2,
                    Name = "Oleh",
                    Surname = "Odnookyi",
                    Email = "oleh@odnookyi.net",
                    PhoneNumber = "380761309999"
                }
            };
        }

        /// <summary>
        /// Gets a collection of <c>Product</c> entries.
        /// </summary>
        /// <returns>A collection of <c>Product</c> entries.</returns>
        public static ICollection<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = 0,
                    Name = "Pants",
                    Category = "Clothes",
                    Description = "Cotton",
                    Cost = 8
                },
                new Product
                {
                    Id = 1,
                    Name = "Skirt",
                    Category = "Clothes",
                    Description = "Synthetic",
                    Cost = 11
                },
                new Product
                {
                    Id = 2,
                    Name = "Hat",
                    Category = "Clothes",
                    Description = "Linen",
                    Cost = 4
                },
                new Product
                {
                    Id = 3,
                    Name = "iPhone 14",
                    Category = "Phones",
                    Description = "Shiny",
                    Cost = 1100
                },
                new Product
                {
                    Id = 4,
                    Name = "Xiaomi Mi 10",
                    Category = "Phones",
                    Description = "White",
                    Cost = 700
                }
            };
        }

        /// <summary>
        /// Gets a collection of <c>IUser</c> entries.
        /// </summary>
        /// <returns>A collection of <c>IUser</c> entries.</returns>
        public static ICollection<IUser> GetUsers()
        {
            using var piEnumerator = GetPersonalInformation().GetEnumerator();
            piEnumerator.MoveNext();
            var list = new List<IUser>
            {
                new Admin
                {
                    Id = 0,
                    Login = "admin",
                    Password = "admin",
                    PersonalInformation = piEnumerator.Current
                }
            };
            piEnumerator.MoveNext();
            list.Add(new RegisteredUser {
                Id = 1,
                Login = "mykytko",
                Password = "password",
                PersonalInformation = piEnumerator.Current
            });
            piEnumerator.MoveNext();
            list.Add(new RegisteredUser
            {
                Id = 2,
                Login = "tpouhuk",
                Password = "bomjour",
                PersonalInformation = piEnumerator.Current
            });
            
            return list;
        }

        /// <summary>
        /// Gets a collection of <c>Order</c> entries.
        /// </summary>
        /// <returns>A collection of <c>Order</c> entries.</returns>
        public static ICollection<Order> GetOrders()
        {
            using var pEnumerator = GetProductList().GetEnumerator();
            pEnumerator.MoveNext();
            using var enumerator = GetUsers().GetEnumerator();
            enumerator.MoveNext();
            var list1 = new List<ProductAndAmount>
            {
                pEnumerator.Current
            };
            pEnumerator.MoveNext();
            list1.Add(pEnumerator.Current);
            pEnumerator.MoveNext();
            var list = new List<Order>
            {
                new Order
                {
                    Id = 0,
                    DateTime = DateTime.Now,
                    Customer = enumerator.Current,
                    Status = OrderStatus.New, 
                    ProductList = new List<ProductAndAmount> {pEnumerator.Current}
                }
            };
            enumerator.MoveNext();
            pEnumerator.MoveNext();
            list.Add(new Order
            {
                Id = 1,
                DateTime = DateTime.Now,
                Customer = enumerator.Current,
                Status = OrderStatus.Sent,
                ProductList = list1
            });

            return list;
        }
    }
}