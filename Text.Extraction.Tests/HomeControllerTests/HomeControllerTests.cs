using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TextExtraction.Controllers;
using TextExtraction.Interfaces;
using Xunit;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly Mock<IHistoryRepository> _historyRepositoryMock;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        _historyRepositoryMock = new Mock<IHistoryRepository>();

        _environmentMock.Setup(e => e.WebRootPath).Returns("wwwroot");
        _environmentMock.Setup(e => e.ContentRootPath).Returns("content");
    }

    [Fact]
    public void Index_Returns_ViewResult()
    {
        var controller = new HomeController(_loggerMock.Object, _environmentMock.Object, _historyRepositoryMock.Object);

        var result = controller.Index();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_Returns_ViewResult()
    {
        var controller = new HomeController(_loggerMock.Object, _environmentMock.Object, _historyRepositoryMock.Object);

        var result = controller.Privacy();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task ExtractText_WithNoFiles_RedirectsToIndexWithErrorMessage()
    {
        var controller = new HomeController(_loggerMock.Object, _environmentMock.Object, _historyRepositoryMock.Object)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };

        var result = await controller.ExtractText(new List<IFormFile>());

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");

        controller.TempData["ErrorMessage"].Should().Be("Please upload at least one valid image.");
    }
}
