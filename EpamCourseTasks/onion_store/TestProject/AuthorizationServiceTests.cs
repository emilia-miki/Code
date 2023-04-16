using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceInterface.Services;
using NUnit.Framework;
using Moq;
using RepositoryInterface;
using ServiceInterface.IServices;
using MockLibrary;
using ServiceInterface.Controllers;
using OnionCore;
using ServiceInterface;

namespace TestProject
{
    [TestFixture]
    public class AuthorizationServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = MockIUserRepository.GetMock();
        private readonly Mock<IDelegateService> _delegatesMock = MockIDelegateService.GetMock(new DelegateService
        {
            ProductController = new ProductController(),
            UserController = new UserController(),
            OrderController = new OrderController()
        });
        private IAuthorizationService _authorizationService;

        [SetUp]
        public void SetUp()
        {
            _authorizationService = new AuthorizationService
            {
                UserRepository = _userRepositoryMock.Object,
                DelegateService = _delegatesMock.Object
            };
            _userRepositoryMock.Invocations.Clear();
        }

        [Test]
        public void Authorize_NullUserArgument_ReturnFalse()
        {
            var result = _authorizationService.Authorize(null, null);
            Assert.IsFalse(result);
        }

        [Test]
        public void Authorize_UserDoesNotExist_ReturnFalse()
        {
            var result = _authorizationService.Authorize(new User
            {
                Login = "",
                Password = ""
            }, new Role[] {});
            Assert.IsFalse(result);
        }

        [Test]
        public void Authorize_WrongPassword_ReturnFalse()
        {
            var admin = new Admin
            {
                Login = _userRepositoryMock.Object.GetAll().First(u => u is Admin).Login,
                Password = ""
            };
            var result = _authorizationService.Authorize(admin, new Role[] {Role.Admin});
            _userRepositoryMock.Verify(ur => ur.GetByLogin("admin"), Times.Once);
            Assert.IsFalse(result);
        }
        
        private void CheckPermissions(IDictionary<string, Delegate> dictionary, IUser user)
        {
            foreach (var (_, d) in dictionary)
            {
                _userRepositoryMock.Invocations.Clear();
                var attribute = (RolePermissionsAttribute) 
                    d.Method.GetCustomAttribute(typeof(RolePermissionsAttribute));
                var role = Role.Guest;

                if (user is RegisteredUser)
                {
                    role = Role.RegisteredUser;
                }
                
                if (user is Admin)
                {
                    role = Role.Admin;
                }

                if (attribute!.Roles.Any(r => r == role))
                {
                    continue;
                }
                
                var result = _authorizationService.Authorize(user, attribute!.Roles);

                _userRepositoryMock.Verify(m => m.GetByLogin(user.Login), Times.Once);
                if (attribute!.Roles.Any(r => r == role))
                {
                    Assert.IsTrue(result);
                }
                else
                {
                    Assert.IsFalse(result);
                }
            }
        }
        
        [Test]
        public void Authorize_Permissions_ReturnBool()
        {
            var delegates = new DelegateService
            {
                ProductController = new ProductController(),
                OrderController = new OrderController(),
                UserController = new UserController()
            };
            _authorizationService.DelegateService = delegates;

            var registeredUser = _userRepositoryMock.Object.GetAllRegisteredUsers().First();
            
            // Create a set difference (AdminDelegates - UserDelegates)
            var dictionary = delegates.AdminDelegates;
            foreach (var pair in delegates.UserDelegates)
            {
                dictionary.Remove(pair.Key);
            }
            
            CheckPermissions(dictionary, registeredUser);

            var admin = _userRepositoryMock.Object.GetAll().First(u => u is Admin);

            // Create a set difference (UserDelegates - AdminDelegates)
            dictionary = delegates.UserDelegates;
            foreach (var pair in delegates.AdminDelegates)
            {
                dictionary.Remove(pair.Key);
            }
            
            CheckPermissions(dictionary, admin);
        }
    }
}