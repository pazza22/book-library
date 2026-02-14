using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using book_library.Models;
using book_library.Services;

namespace book_library.Tests;

[TestClass]
public class CartServiceTests
{
    private Mock<IMemoryCache> _mockMemoryCache;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private Mock<IBookService> _mockBookService;
    private Mock<ISession> _mockSession;
    private Mock<HttpContext> _mockHttpContext;
    private ICartService _cartService;
    private List<Book> _testBooks;
    private Dictionary<object, object> _cacheStorage;

    [TestInitialize]
    public void Setup()
    {
        _mockMemoryCache = new Mock<IMemoryCache>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockBookService = new Mock<IBookService>();
        _mockSession = new Mock<ISession>();
        _mockHttpContext = new Mock<HttpContext>();
        _cacheStorage = new Dictionary<object, object>();

        // Setup session
        _mockSession.Setup(s => s.Id).Returns("test-session-id");
        _mockHttpContext.Setup(x => x.Session).Returns(_mockSession.Object);
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

        // Setup memory cache with a simple dictionary-based implementation
        _mockMemoryCache
            .Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
            .Returns((object key, out object value) =>
            {
                return _cacheStorage.TryGetValue(key, out value);
            });

        _mockMemoryCache
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns((object key) =>
            {
                var mockEntry = new Mock<ICacheEntry>();
                mockEntry.SetupProperty(e => e.Value);
                mockEntry.SetupProperty(e => e.AbsoluteExpirationRelativeToNow);
                mockEntry.SetupProperty(e => e.SlidingExpiration);
                mockEntry.Setup(e => e.Dispose()).Callback(() =>
                {
                    _cacheStorage[key] = mockEntry.Object.Value;
                });
                return mockEntry.Object;
            });

        _cartService = new CartService(_mockMemoryCache.Object, _mockHttpContextAccessor.Object, _mockBookService.Object);

        // Create test books
        _testBooks = new List<Book>
        {
            new Book { Id = 1, Title = "Test Book 1", Author = "Author 1", Price = 10.99m, ImageUrl = "image1.jpg" },
            new Book { Id = 2, Title = "Test Book 2", Author = "Author 2", Price = 15.99m, ImageUrl = "image2.jpg" },
            new Book { Id = 3, Title = "Test Book 3", Author = "Author 3", Price = 20.99m, ImageUrl = "image3.jpg" }
        };
    }

    [TestMethod]
    public void GetCart_WithEmptyCache_ShouldReturnEmptyCart()
    {
        // Act
        var result = _cartService.GetCart();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Items.Count);
        Assert.AreEqual(0, result.TotalItems);
        Assert.AreEqual(0m, result.TotalPrice);
    }

    [TestMethod]
    public void AddToCart_NewItem_ShouldAddItemToCart()
    {
        // Arrange
        var book = _testBooks[0];

        // Act
        _cartService.AddToCart(book, 1);

        // Assert
        _mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [TestMethod]
    public void AddToCart_ExistingItem_ShouldIncreaseQuantity()
    {
        // Arrange
        var cart = new Cart();
        cart.AddItem(_testBooks[0], 1);
        var cacheKey = "ShoppingCart_test-session-id";
        _cacheStorage[cacheKey] = cart;

        // Act
        _cartService.AddToCart(_testBooks[0], 2);

        // Assert
        _mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [TestMethod]
    public void RemoveFromCart_ExistingItem_ShouldRemoveItem()
    {
        // Arrange
        var cart = new Cart();
        cart.AddItem(_testBooks[0], 1);
        cart.AddItem(_testBooks[1], 1);
        var cacheKey = "ShoppingCart_test-session-id";
        _cacheStorage[cacheKey] = cart;

        // Act
        _cartService.RemoveFromCart(1);

        // Assert
        _mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [TestMethod]
    public void UpdateQuantity_ValidQuantity_ShouldUpdateQuantity()
    {
        // Arrange
        var cart = new Cart();
        cart.AddItem(_testBooks[0], 1);
        var cacheKey = "ShoppingCart_test-session-id";
        _cacheStorage[cacheKey] = cart;

        // Act
        _cartService.UpdateQuantity(1, 5);

        // Assert
        _mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [TestMethod]
    public void UpdateQuantity_ZeroQuantity_ShouldRemoveItem()
    {
        // Arrange
        var cart = new Cart();
        cart.AddItem(_testBooks[0], 1);
        var cacheKey = "ShoppingCart_test-session-id";
        _cacheStorage[cacheKey] = cart;

        // Act
        _cartService.UpdateQuantity(1, 0);

        // Assert
        _mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [TestMethod]
    public void ClearCart_ShouldClearAllItems()
    {
        // Arrange
        var cart = new Cart();
        cart.AddItem(_testBooks[0], 1);
        cart.AddItem(_testBooks[1], 2);
        var cacheKey = "ShoppingCart_test-session-id";
        _cacheStorage[cacheKey] = cart;

        // Act
        _cartService.ClearCart();

        // Assert
        _mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [TestMethod]
    public void GetCartItemCount_WithItems_ShouldReturnCorrectCount()
    {
        // Arrange
        var cart = new Cart();
        cart.AddItem(_testBooks[0], 2);
        cart.AddItem(_testBooks[1], 3);
        var cacheKey = "ShoppingCart_test-session-id";
        _cacheStorage[cacheKey] = cart;

        // Act
        var result = _cartService.GetCartItemCount();

        // Assert
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void GetCartItemCount_WithEmptyCart_ShouldReturnZero()
    {
        // Act
        var result = _cartService.GetCartItemCount();

        // Assert
        Assert.AreEqual(0, result);
    }
}
