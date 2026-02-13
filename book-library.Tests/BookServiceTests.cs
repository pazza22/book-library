using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using book_library.Models;
using book_library.Services;

namespace book_library.Tests;

[TestClass]
public class BookServiceTests
{
    private IBookService _bookService;

    [TestInitialize]
    public void Setup()
    {
        _bookService = new BookService();
    }

    [TestMethod]
    public void GetAllBooks_ShouldReturnAllBooks()
    {
        // Act
        var result = _bookService.GetAllBooks();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(30, result.Count);
        Assert.IsTrue(result.Any(b => b.Title == "To Kill a Mockingbird"));
        Assert.IsTrue(result.Any(b => b.Author == "George Orwell"));
    }

    [TestMethod]
    public void SearchBooks_WithNullSearchTerm_ShouldReturnAllBooks()
    {
        // Act
        var result = _bookService.SearchBooks(null);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(30, result.Count);
    }

    [TestMethod]
    public void SearchBooks_WithEmptySearchTerm_ShouldReturnAllBooks()
    {
        // Act
        var result = _bookService.SearchBooks("");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(30, result.Count);
    }

    [TestMethod]
    public void SearchBooks_WithWhitespaceSearchTerm_ShouldReturnAllBooks()
    {
        // Act
        var result = _bookService.SearchBooks("   ");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(30, result.Count);
    }

    [TestMethod]
    public void SearchBooks_ByTitle_ShouldReturnMatchingBooks()
    {
        // Act
        var result = _bookService.SearchBooks("1984");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("1984", result[0].Title);
        Assert.AreEqual("George Orwell", result[0].Author);
    }

    [TestMethod]
    public void SearchBooks_ByAuthor_ShouldReturnMatchingBooks()
    {
        // Act
        var result = _bookService.SearchBooks("George Orwell");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(b => b.Author == "George Orwell"));
        Assert.IsTrue(result.Any(b => b.Title == "1984"));
        Assert.IsTrue(result.Any(b => b.Title == "Animal Farm"));
    }

    [TestMethod]
    public void SearchBooks_ByGenre_ShouldReturnMatchingBooks()
    {
        // Act
        var result = _bookService.SearchBooks("Fantasy");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(4, result.Count);
        Assert.IsTrue(result.All(b => b.Genre == "Fantasy"));
    }

    [TestMethod]
    public void SearchBooks_ByPartialTitle_ShouldReturnMatchingBooks()
    {
        // Act
        var result = _bookService.SearchBooks("Harry Potter");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Harry Potter and the Sorcerer's Stone", result[0].Title);
    }

    [TestMethod]
    public void SearchBooks_ByDescription_ShouldReturnMatchingBooks()
    {
        // Act
        var result = _bookService.SearchBooks("wizard");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Harry Potter and the Sorcerer's Stone", result[0].Title);
    }

    [TestMethod]
    public void SearchBooks_CaseInsensitive_ShouldReturnMatchingBooks()
    {
        // Act
        var result1 = _bookService.SearchBooks("orwell");
        var result2 = _bookService.SearchBooks("ORWELL");
        var result3 = _bookService.SearchBooks("OrWeLl");

        // Assert
        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
        Assert.IsNotNull(result3);
        Assert.AreEqual(result1.Count, result2.Count);
        Assert.AreEqual(result2.Count, result3.Count);
        Assert.AreEqual(2, result1.Count);
    }

    [TestMethod]
    public void SearchBooks_NoMatches_ShouldReturnEmptyList()
    {
        // Act
        var result = _bookService.SearchBooks("NonExistentBook12345");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void SearchBooks_MultipleMatches_ShouldReturnAllMatches()
    {
        // Act
        var result = _bookService.SearchBooks("Tolkien");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(b => b.Title == "The Hobbit"));
        Assert.IsTrue(result.Any(b => b.Title == "The Lord of the Rings"));
    }

    [TestMethod]
    public void SearchBooks_ByGenreDystopian_ShouldReturnCorrectCount()
    {
        // Act
        var result = _bookService.SearchBooks("Dystopian");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(b => b.Title == "1984"));
        Assert.IsTrue(result.Any(b => b.Title == "Brave New World"));
    }
}
