using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using book_library.Models;

namespace book_library.Tests;

[TestClass]
public class CartTests
{
    private Cart _cart;
    private Book _testBook1;
    private Book _testBook2;

    [TestInitialize]
    public void Setup()
    {
        _cart = new Cart();
        _testBook1 = new Book 
        { 
            Id = 1, 
            Title = "Test Book 1", 
            Author = "Author 1", 
            Price = 10.99m, 
            ImageUrl = "image1.jpg" 
        };
        _testBook2 = new Book 
        { 
            Id = 2, 
            Title = "Test Book 2", 
            Author = "Author 2", 
            Price = 15.99m, 
            ImageUrl = "image2.jpg" 
        };
    }

    [TestMethod]
    public void AddItem_NewBook_ShouldAddToCart()
    {
        // Act
        _cart.AddItem(_testBook1, 1);

        // Assert
        Assert.AreEqual(1, _cart.Items.Count);
        Assert.AreEqual(1, _cart.Items[0].Quantity);
        Assert.AreEqual("Test Book 1", _cart.Items[0].Title);
    }

    [TestMethod]
    public void AddItem_ExistingBook_ShouldIncreaseQuantity()
    {
        // Arrange
        _cart.AddItem(_testBook1, 1);

        // Act
        _cart.AddItem(_testBook1, 2);

        // Assert
        Assert.AreEqual(1, _cart.Items.Count);
        Assert.AreEqual(3, _cart.Items[0].Quantity);
    }

    [TestMethod]
    public void AddItem_MultipleBooks_ShouldAddAll()
    {
        // Act
        _cart.AddItem(_testBook1, 1);
        _cart.AddItem(_testBook2, 2);

        // Assert
        Assert.AreEqual(2, _cart.Items.Count);
        Assert.AreEqual(1, _cart.Items[0].Quantity);
        Assert.AreEqual(2, _cart.Items[1].Quantity);
    }

    [TestMethod]
    public void RemoveItem_ExistingBook_ShouldRemove()
    {
        // Arrange
        _cart.AddItem(_testBook1, 1);
        _cart.AddItem(_testBook2, 1);

        // Act
        _cart.RemoveItem(1);

        // Assert
        Assert.AreEqual(1, _cart.Items.Count);
        Assert.AreEqual(2, _cart.Items[0].BookId);
    }

    [TestMethod]
    public void RemoveItem_NonExistingBook_ShouldNotChange()
    {
        // Arrange
        _cart.AddItem(_testBook1, 1);

        // Act
        _cart.RemoveItem(999);

        // Assert
        Assert.AreEqual(1, _cart.Items.Count);
    }

    [TestMethod]
    public void UpdateQuantity_ValidQuantity_ShouldUpdate()
    {
        // Arrange
        _cart.AddItem(_testBook1, 1);

        // Act
        _cart.UpdateQuantity(1, 5);

        // Assert
        Assert.AreEqual(1, _cart.Items.Count);
        Assert.AreEqual(5, _cart.Items[0].Quantity);
    }

    [TestMethod]
    public void UpdateQuantity_ZeroQuantity_ShouldRemove()
    {
        // Arrange
        _cart.AddItem(_testBook1, 1);

        // Act
        _cart.UpdateQuantity(1, 0);

        // Assert
        Assert.AreEqual(0, _cart.Items.Count);
    }

    [TestMethod]
    public void UpdateQuantity_NegativeQuantity_ShouldRemove()
    {
        // Arrange
        _cart.AddItem(_testBook1, 1);

        // Act
        _cart.UpdateQuantity(1, -5);

        // Assert
        Assert.AreEqual(0, _cart.Items.Count);
    }

    [TestMethod]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        _cart.AddItem(_testBook1, 1);
        _cart.AddItem(_testBook2, 2);

        // Act
        _cart.Clear();

        // Assert
        Assert.AreEqual(0, _cart.Items.Count);
    }

    [TestMethod]
    public void TotalItems_ShouldReturnCorrectCount()
    {
        // Arrange
        _cart.AddItem(_testBook1, 2);
        _cart.AddItem(_testBook2, 3);

        // Act
        var total = _cart.TotalItems;

        // Assert
        Assert.AreEqual(5, total);
    }

    [TestMethod]
    public void TotalPrice_ShouldCalculateCorrectly()
    {
        // Arrange
        _cart.AddItem(_testBook1, 2); // 2 * 10.99 = 21.98
        _cart.AddItem(_testBook2, 1); // 1 * 15.99 = 15.99
        // Total = 37.97

        // Act
        var total = _cart.TotalPrice;

        // Assert
        Assert.AreEqual(37.97m, total);
    }

    [TestMethod]
    public void TotalPrice_EmptyCart_ShouldReturnZero()
    {
        // Act
        var total = _cart.TotalPrice;

        // Assert
        Assert.AreEqual(0m, total);
    }
}
