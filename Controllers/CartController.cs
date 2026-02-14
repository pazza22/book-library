using Microsoft.AspNetCore.Mvc;
using book_library.Services;

namespace book_library.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IBookService _bookService;

    public CartController(ICartService cartService, IBookService bookService)
    {
        _cartService = cartService;
        _bookService = bookService;
    }

    public IActionResult Index()
    {
        var cart = _cartService.GetCart();
        return View(cart);
    }

    [HttpPost]
    public IActionResult AddToCart([FromBody] AddToCartRequest request)
    {
        var books = _bookService.GetAllBooks();
        var book = books.FirstOrDefault(b => b.Id == request.BookId);

        if (book == null)
        {
            return Json(new { success = false, message = "Book not found" });
        }

        _cartService.AddToCart(book, request.Quantity);
        var cartItemCount = _cartService.GetCartItemCount();

        return Json(new { success = true, message = "Book added to cart!", cartItemCount });
    }

    [HttpPost]
    public IActionResult RemoveFromCart([FromBody] RemoveFromCartRequest request)
    {
        _cartService.RemoveFromCart(request.BookId);
        var cartItemCount = _cartService.GetCartItemCount();

        return Json(new { success = true, message = "Item removed from cart", cartItemCount });
    }

    [HttpPost]
    public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
    {
        if (request.Quantity < 0)
        {
            return Json(new { success = false, message = "Invalid quantity" });
        }

        _cartService.UpdateQuantity(request.BookId, request.Quantity);
        var cart = _cartService.GetCart();
        var item = cart.Items.FirstOrDefault(i => i.BookId == request.BookId);

        return Json(new 
        { 
            success = true, 
            message = "Quantity updated",
            cartItemCount = cart.TotalItems,
            itemTotal = item?.TotalPrice ?? 0,
            cartTotal = cart.TotalPrice
        });
    }

    [HttpPost]
    public IActionResult ClearCart()
    {
        _cartService.ClearCart();
        return Json(new { success = true, message = "Cart cleared" });
    }

    public IActionResult GetCartCount()
    {
        var count = _cartService.GetCartItemCount();
        return Json(new { count });
    }
}

// Request models
public record AddToCartRequest(int BookId, int Quantity);
public record RemoveFromCartRequest(int BookId);
public record UpdateQuantityRequest(int BookId, int Quantity);
