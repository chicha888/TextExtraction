using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using MockQueryable.Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TextExtraction.Controllers;
using TextExtraction.Models;
using TextExtraction.ViewModels;
using Xunit;
using MockQueryable;

public class ProfileControllerTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;

    public ProfileControllerTests()
    {
        var store = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task Index_Returns_NotFound_When_User_Not_Found()
    {
        // Настраиваем пустой IQueryable
        var users = new List<AppUser>().AsQueryable().BuildMock();
        _userManagerMock.Setup(um => um.Users).Returns(users);

        var controller = new ProfileController(_userManagerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                    }))
                }
            }
        };

        var result = await controller.Index();

        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().Be("User not found");
    }

    [Fact]
    public async Task Index_Returns_ViewResult_With_ProfileVM()
    {
        // Создаём тестового пользователя
        var user = new AppUser
        {
            Id = "test-user-id",
            UserName = "TestUser",
            Email = "test@example.com",
            FileHistory = new List<FileHistory>
            {
                new FileHistory { FileName = "file1.txt", UploadedAt = DateTime.UtcNow }
            }
        };

        // Настраиваем IQueryable с пользователем
        var users = new List<AppUser> { user }.AsQueryable().BuildMock();
        _userManagerMock.Setup(um => um.Users).Returns(users);

        var controller = new ProfileController(_userManagerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                    }))
                }
            }
        };

        var result = await controller.Index();

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<ProfileVM>()
            .Which.Username.Should().Be("TestUser");
    }
}
