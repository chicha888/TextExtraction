using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextExtraction.Controllers;
using TextExtraction.Models;
using TextExtraction.ViewModels;

namespace Text.Extraction.Tests.AccountConrollerTests
{
    public class AccountControllerTests
    {
       private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(
                new Mock<IUserStore<AppUser>>().Object, null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null);

            _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ShouldRedirectToHomeIndex()
        {
            // Arrange
            var loginVM = new LoginVM { EmailAddress = "test@example.com", Password = "ValidPassword123" };
            var user = new AppUser { Email = loginVM.EmailAddress, UserName = "testuser" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginVM.EmailAddress)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginVM.Password)).ReturnsAsync(true);
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, loginVM.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _controller.Login(loginVM);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        

        [Fact]
        public async Task Register_ValidData_ShouldRedirectToHomeIndex()
        {
            // Arrange
            var registerVM = new RegisterVM { Username = "newuser", Email = "newuser@example.com", Password = "StrongPassword123" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(registerVM.Email)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), registerVM.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerVM);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        
    }
}
