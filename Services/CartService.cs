using book_library.Models;
using Microsoft.Extensions.Caching.Memory;

namespace book_library.Services;

public interface ICartService
{
    Cart GetCart();
    void AddToCart(Book book, int quantity = 1);
    void RemoveFromCart(int bookId);
    void UpdateQuantity(int bookId, int quantity);
    void ClearCart();
    int GetCartItemCount();
}

public class CartService : ICartService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBookService _bookService;
    private const string CartCacheKeyPrefix = "ShoppingCart_";

    public CartService(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, IBookService bookService)
    {
        _memoryCache = memoryCache;
        _httpContextAccessor = httpContextAccessor;
        _bookService = bookService;
    }

    private string GetCartCacheKey()
    {
        // Use session ID as part of the cache key to ensure each user has their own cart
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session != null)
        {
            // Ensure session is started
            if (string.IsNullOrEmpty(session.Id))
            {
                session.SetString("_init", "1");
            }
            return $"{CartCacheKeyPrefix}{session.Id}";
        }
        
        // Fallback to a default key if session is not available (shouldn't happen in normal flow)
        return $"{CartCacheKeyPrefix}default";
    }

    public Cart GetCart()
    {
        var cacheKey = GetCartCacheKey();
        
        if (_memoryCache.TryGetValue(cacheKey, out Cart? cart) && cart != null)
        {
            return cart;
        }

        return new Cart();
    }

    private void SaveCart(Cart cart)
    {
        var cacheKey = GetCartCacheKey();
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };

        _memoryCache.Set(cacheKey, cart, cacheOptions);
    }

    public void AddToCart(Book book, int quantity = 1)
    {
        var cart = GetCart();
        cart.AddItem(book, quantity);
        SaveCart(cart);
    }

    public void RemoveFromCart(int bookId)
    {
        var cart = GetCart();
        cart.RemoveItem(bookId);
        SaveCart(cart);
    }

    public void UpdateQuantity(int bookId, int quantity)
    {
        var cart = GetCart();
        cart.UpdateQuantity(bookId, quantity);
        SaveCart(cart);
    }

    public void ClearCart()
    {
        var cart = new Cart();
        SaveCart(cart);
    }

    public int GetCartItemCount()
    {
        var cart = GetCart();
        return cart.TotalItems;
    }
}
