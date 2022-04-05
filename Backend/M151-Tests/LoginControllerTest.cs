using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using m151_backend.Controllers;
using m151_backend.DTOs;
using m151_backend.Entities;
using M151_Tests.TestSetup;
using Microsoft.EntityFrameworkCore;
using Moq;
using Students.Data;
using Xunit;

namespace M151_Tests
{
    public class LoginControllerTest
    {
        private LoginController LoginControllerSetup()
        {
            var data = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "eric",
                    PasswordHash =
                        Encoding.ASCII.GetBytes(
                            "0x69A160B7375FB0841138536EC22C205FC0E355634B4BCF184A30EDEE9815274842C96A" +
                            "0074CA5AFF4172F86E6983131C4879C8DFDCD9B70F378A16826BF0CDEB"),
                    PasswordSalt = Encoding.ASCII.GetBytes(
                        "0xA36EEA119F2BA918BD3E41FD267FF6ED883FA2C48F0E9A02D70B24504F32B5E2D4CC6AD73C0E04497197304F7EAE3C969A0FF37212A4CA" +
                        "1D01A1F0BF473D779C3091F0CBC430A12F0437D1E26EFBF8EEAB1C5FCDB6E8805D87EA7A7" +
                        "7F20DB9C47701BB28C799D635C669F839027C7545E60E0C30D5D3E816BCB81F0A010C9AD8")
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<User>(data.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<User>(data.Provider));

            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<DataContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var service = new LoginController(mockContext.Object);
            return service;
        }

        [Fact]
        public async Task LoginCorrectUser()
        {
            // Arrange
            var api = LoginControllerSetup();
            var user = new LoginDTO
            {
                Username = "eric",
                Password = "1234"
            };

            // Act
            var response = await api.LoginUser(user);

            // Assert
            Assert.NotNull(response.Value.Token);
            Assert.NotNull(response.Value.RefreshToken);
            Assert.True(response.Value.RefreshExpires > DateTime.Now);
        }

        [Fact]
        public async Task LoginEmptyFields()
        {
            // Arrange
            var api = LoginControllerSetup();
            var user = new LoginDTO
            {
                Username = null,
                Password = null
            };

            // Act
            var response = await api.LoginUser(user);

            // Assert
            Assert.Contains(response.Value.ToString(), "not valid");
        }

        [Fact]
        public async Task LoginWrongUser()
        {
            // Arrange
            var api = LoginControllerSetup();
            var user = new LoginDTO
            {
                Username = "daniel",
                Password = "1234"
            };

            // Act
            var response = await api.LoginUser(user);

            // Assert
            Assert.Contains(response.Value.ToString(), "Login_WrongUser");
        }

        [Fact]
        public async Task LoginWrongPassword()
        {
            // Arrange
            var api = LoginControllerSetup();
            var user = new LoginDTO
            {
                Username = "eric",
                Password = "5678"
            };

            // Act
            var response = await api.LoginUser(user);

            // Assert
            Assert.Contains(response.Value.ToString(), "Login_WrongPassword");
        }
    }
}