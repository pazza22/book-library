using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using book_library.Controllers;
using book_library.Models;
using book_library.Services;

namespace book_library.Tests;

[TestClass]
public class CartControllerTests
{
    private Mock<ICartService> _mockCartService;
    private Mock<IBookService> _mockBookService;
    private CartController _controller;
    private List<Book> _testBooks;

    [TestInitialize]
    public void Setup()
    {
        _mockCartService = new Mock<ICartService>();
        _mockBookService = new Mock<IBookService>();
        _controller = new CartController(_mockCartService.Object, _mockBookService.Object);

        // Create test books
        _testBooks = new List<Book>
        {
            new Book { Id = 1, Title = "Test Book 1", Author = "Author 1", Price = 10.99m, ImageUrl = "image1.jpg" },
            new Book { Id = 2, Title = "Test Book 2", Author = "Author 2", Price = 15.99m, ImageUrl = "image2.jpg" },
            new Book { Id = 3, Title = "Test Book 3", Author = "Author 3", Price = 20.99m, ImageUrl = "image3.jpg" }
        };
    }

    [TestMethod]
    public void Index_ShouldReturnViewWithCart()
    {
        // Arrange
        var cart = new Cart();
        cart.AddItem(_testBooks[0], 1);
        _mockCartService.Setup(s => s.GetCart()).Returns(cart);

        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        var model = result.Model as Cart;
        Assert.IsNotNull(model);
        Assert.AreEqual(1, model.Items.Count);
    }

    [TestMethod]
    public void AddToCart_ValidBook_ShouldReturnSuccess()
    {
        // Arrange
        var bookId = 1;
        var book = _testBooks[0];
        var request = new AddToCartRequest(bookId, 1);
        _mockBookService.Setup(s => s.GetAllBooks()).Returns(_testBooks);
        _mockCartService.Setup(s => s.AddToCart(It.IsAny<Book>(), It.IsAny<int>()));
        _mockCartService.Setup(s => s.GetCartItemCount()).Returns(1);

        // Act
        var result = _controller.AddToCart(request) as JsonResult;

        // Assert
        Assert.IsNotNull(result);
        var json = JsonSerializer.Serialize(result.Value);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.IsNotNull(data);
        Assert.IsTrue(data["success"].GetBoolean());
        Assert.AreEqual("Book added to cart!", data["message"].GetString());
        Assert.AreEqual(1, data["cartItemCount"].GetInt32());
        _mockCartService.Verify(s => s.AddToCart(It.IsAny<Book>(), 1), Times.Once);
    }

    [TestMethod]
    public void AddToCart_InvalidBook_ShouldReturnFailure()
    {
        // Arrange
        var bookId = 999;
        var request = new AddToCartRequest(bookId, 1);
        _mockBookService.Setup(s => s.GetAllBooks()).Returns(_testBooks);

        // Act
        var result = _controller.AddToCart(request) as JsonResult;

        // Assert
        Assert.IsNotNull(result);
        var json = JsonSerializer.Serialize(result.Value);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.IsNotNull(data);
        Assert.IsFalse(data["success"].GetBoolean());
        Assert.AreEqual("Book not found", data["message"].GetString());
        _mockCartService.Verify(s => s.AddToCart(It.IsAny<Book>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void AddToCart_WithQuantity_ShouldAddMultipleItems()
    {
        // Arrange
        var bookId = 1;
        var quantity = 3;
        var book = _testBooks[0];
        var request = new AddToCartRequest(bookId, quantity);
        _mockBookService.Setup(s => s.GetAllBooks()).Returns(_testBooks);
        _mockCartService.Setup(s => s.AddToCart(It.IsAny<Book>(), quantity));
        _mockCartService.Setup(s => s.GetCartItemCount()).Returns(3);

        // Act
        var result = _controller.AddToCart(request) as JsonResult;

        // Assert
        Assert.IsNotNull(result);
        var json = JsonSerializer.Serialize(result.Value);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.IsNotNull(data);
        Assert.IsTrue(data["success"].GetBoolean());
        _mockCartService.Verify(s => s.AddToCart(It.IsAny<Book>(), quantity), Times.Once);
    }

    [TestMethod]
    public void RemoveFromCart_ValidBookId_ShouldReturnSuccess()
    {
        // Arrange
        var bookId = 1;
        var request = new RemoveFromCartRequest(bookId);
        _mockCartService.Setup(s => s.RemoveFromCart(bookId));
        _mockCartService.Setup(s => s.GetCartItemCount()).Returns(0);

        // Act
        var result = _controller.RemoveFromCart(request) as JsonResult;

        // Assert
        Assert.IsNotNull(result);
        var json = JsonSerializer.Serialize(result.Value);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.IsNotNull(data);
        Assert.IsTrue(data["success"].GetBoolean());
        Assert.AreEqual("Item removed from cart", data["message"].GetString());
        _mockCartService.Verify(s => s.RemoveFromCart(bookId), Times.Once);
    }

    [TestMethod]
    public void UpdateQuantity_ValidQuantity_ShouldReturnSuccess()
    {
        // Arrange
        var bookId = 1;
        var quantity = 5;
        var request = new UpdateQuantityRequest(bookId, quantity);
        var cart = new Cart();
        var book = _testBooks[0];
        cart.AddItem(book, quantity);
        
        _mockCartService.Setup(s => s.UpdateQuantity(bookId, quantity));
        _mockCartService.Setup(s => s.GetCart()).Returns(cart);

        // Act
        var result = _controller.UpdateQuantity(request) as JsonResult;

        // Assert
        Assert.IsNotNull(result);
        var json = JsonSerializer.Serialize(result.Value);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.IsNotNull(data);
        Assert.IsTrue(data["success"].GetBoolean());
        Assert.AreEqual("Quantity updated", data["message"].GetString());
        _mockCartService.Verify(s => s.UpdateQuantity(bookId, quantity), Times.Once);
    }

    [TestMethod]
    public void UpdateQuantity_NegativeQuantity_ShouldReturnFailure()
    {
        // Arrange
        var bookId = 1;
        var quantity = -1;
        var request = new UpdateQuantityRequest(bookId, quantity);

        // Act
        var result = _controller.UpdateQuantity(request) as JsonResult;

        // Assert
        Assert.IsNotNull(result);
        var json = JsonSerializer.Serialize(result.Value);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.IsNotNull(data);
        Assert.IsFalse(data["success"].GetBoolean());
        Assert.AreEqual("Invalid quantity", data["message"].GetString());
        _mockCartService.Verify(s => s.UpdateQuantity(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void ClearCart_ShouldReturnSuccess()
    {
        // Arrange
        _mockCartService.Setup(s => s.ClearCart());

        // Act
        var result = _controller.ClearCart() as JsonResult;

        // Assert
        Assert.IsNotNull(result);
        var json = JsonSerializer.Serialize(result.Value);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.IsNotNull(data);
        Assert.IsTrue(data["success"].GetBoolean());
        Assert.AreEqual("Cart cleared", data["message"].GetString());
        _mockCartService.Verify(s => s.ClearCart(), Times.Once);
    }

    [TestMethod]
    public void GetCartCount_ShouldReturnCount()
    {
        // Arrange
        _mockCartService.Setup(s => s.GetCartItemCount()).Returns(5);

        // Act
        var result = _controller.GetCartCount() as JsonResult;

        // Assert
        Assert.IsNotNull(result);
        var json = JsonSerializer.Serialize(result.Value);
        var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        Assert.IsNotNull(data);
        Assert.AreEqual(5, data["count"].GetInt32());
    }
}
