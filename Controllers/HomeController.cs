using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using book_library.Models;
using book_library.Services;

namespace book_library.Controllers;

public class HomeController : Controller
{
    private readonly IBookService _bookService;
    private const int PageSize = 10;

    public HomeController(IBookService bookService)
    {
        _bookService = bookService;
    }

    public IActionResult Index(string searchTerm, int page = 1)
    {
        ViewData["Title"] = "Pustaka Bandara";
        
        var allBooks = _bookService.SearchBooks(searchTerm);
        var totalBooks = allBooks.Count;
        var totalPages = (int)Math.Ceiling(totalBooks / (double)PageSize);

        var books = allBooks
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalBooks = totalBooks;

        return View(books);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
