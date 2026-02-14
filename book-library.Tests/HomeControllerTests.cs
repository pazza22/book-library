using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using book_library.Controllers;
using book_library.Models;
using book_library.Services;

namespace book_library.Tests;

[TestClass]
public class HomeControllerTests
{
    private Mock<IBookService> _mockBookService;
    private HomeController _controller;
    private List<Book> _testBooks;

    [TestInitialize]
    public void Setup()
    {
        _mockBookService = new Mock<IBookService>();
        _controller = new HomeController(_mockBookService.Object);
        
        // Create test data
        _testBooks = new List<Book>();
        for (int i = 1; i <= 25; i++)
        {
            _testBooks.Add(new Book
            {
                Id = i,
                Title = $"Test Book {i}",
                Author = $"Author {i}",
                Genre = "Fiction",
                PublicationYear = 2000 + i,
                ISBN = $"978-000000000{i}",
                Description = $"Description for book {i}"
            });
        }
    }

    [TestMethod]
    public void Index_WithNoSearchTerm_ShouldReturnFirstPage()
    {
        // Arrange
        _mockBookService.Setup(s => s.SearchBooks(null)).Returns(_testBooks);

        // Act
        var result = _controller.Index(null, 1) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        var model = result.Model as List<Book>;
        Assert.IsNotNull(model);
        Assert.AreEqual(10, model.Count); // PageSize is 10
        Assert.AreEqual("Test Book 1", model[0].Title);
        Assert.AreEqual("Test Book 10", model[9].Title);
    }

    [TestMethod]
    public void Index_WithSearchTerm_ShouldCallSearchBooks()
    {
        // Arrange
        var searchTerm = "Fiction";
        _mockBookService.Setup(s => s.SearchBooks(searchTerm)).Returns(_testBooks);

        // Act
        var result = _controller.Index(searchTerm, 1) as ViewResult;

        // Assert
        _mockBookService.Verify(s => s.SearchBooks(searchTerm), Times.Once);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Index_SecondPage_ShouldReturnCorrectBooks()
    {
        // Arrange
        _mockBookService.Setup(s => s.SearchBooks(null)).Returns(_testBooks);

        // Act
        var result = _controller.Index(null, 2) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        var model = result.Model as List<Book>;
        Assert.IsNotNull(model);
        Assert.AreEqual(10, model.Count);
        Assert.AreEqual("Test Book 11", model[0].Title);
        Assert.AreEqual("Test Book 20", model[9].Title);
    }

    [TestMethod]
    public void Index_LastPage_ShouldReturnRemainingBooks()
    {
        // Arrange
        _mockBookService.Setup(s => s.SearchBooks(null)).Returns(_testBooks);

        // Act
        var result = _controller.Index(null, 3) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        var model = result.Model as List<Book>;
        Assert.IsNotNull(model);
        Assert.AreEqual(5, model.Count); // 25 books total, 10 per page, page 3 has 5
        Assert.AreEqual("Test Book 21", model[0].Title);
        Assert.AreEqual("Test Book 25", model[4].Title);
    }

    [TestMethod]
    public void Index_ShouldSetViewBagProperties()
    {
        // Arrange
        var searchTerm = "Test";
        _mockBookService.Setup(s => s.SearchBooks(searchTerm)).Returns(_testBooks);

        // Act
        var result = _controller.Index(searchTerm, 2) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(searchTerm, result.ViewData["SearchTerm"]);
        Assert.AreEqual(2, result.ViewData["CurrentPage"]);
        Assert.AreEqual(3, result.ViewData["TotalPages"]); // 25 books / 10 per page = 3 pages
        Assert.AreEqual(25, result.ViewData["TotalBooks"]);
    }

    [TestMethod]
    public void Index_WithEmptyResults_ShouldReturnEmptyList()
    {
        // Arrange
        _mockBookService.Setup(s => s.SearchBooks("NonExistent")).Returns(new List<Book>());

        // Act
        var result = _controller.Index("NonExistent", 1) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        var model = result.Model as List<Book>;
        Assert.IsNotNull(model);
        Assert.AreEqual(0, model.Count);
        Assert.AreEqual(0, result.ViewData["TotalPages"]);
        Assert.AreEqual(0, result.ViewData["TotalBooks"]);
    }

    [TestMethod]
    public void Index_CalculatesTotalPagesCorrectly()
    {
        // Arrange - Test with exactly 20 books (should be 2 pages)
        var exactBooks = _testBooks.Take(20).ToList();
        _mockBookService.Setup(s => s.SearchBooks(null)).Returns(exactBooks);

        // Act
        var result = _controller.Index(null, 1) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.ViewData["TotalPages"]);
        Assert.AreEqual(20, result.ViewData["TotalBooks"]);
    }

    [TestMethod]
    public void Privacy_ShouldReturnView()
    {
        // Act
        var result = _controller.Privacy() as ViewResult;

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Error_ShouldReturnViewWithErrorViewModel()
    {
        // Arrange
        var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        var model = result.Model as ErrorViewModel;
        Assert.IsNotNull(model);
        Assert.IsNotNull(model.RequestId);
        Assert.AreEqual("test-trace-id", model.RequestId);
    }

    [TestMethod]
    public void Index_WithSingleBook_ShouldReturnOneBook()
    {
        // Arrange
        var singleBook = new List<Book> { _testBooks[0] };
        _mockBookService.Setup(s => s.SearchBooks("Test Book 1")).Returns(singleBook);

        // Act
        var result = _controller.Index("Test Book 1", 1) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        var model = result.Model as List<Book>;
        Assert.IsNotNull(model);
        Assert.AreEqual(1, model.Count);
        Assert.AreEqual(1, result.ViewData["TotalPages"]);
    }

    [TestMethod]
    public void Index_PageOutOfBounds_ShouldReturnEmptyList()
    {
        // Arrange
        _mockBookService.Setup(s => s.SearchBooks(null)).Returns(_testBooks);

        // Act
        var result = _controller.Index(null, 999) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        var model = result.Model as List<Book>;
        Assert.IsNotNull(model);
        Assert.AreEqual(0, model.Count);
    }

    [TestMethod]
    public void Index_ShouldSetTitleToBookLibrary()
    {
        // Arrange
        _mockBookService.Setup(s => s.SearchBooks(null)).Returns(_testBooks);

        // Act
        var result = _controller.Index(null, 1) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Book Library", result.ViewData["Title"]);
    }
}
