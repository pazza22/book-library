namespace book_library.Models;

public class Cart
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();

    public int TotalItems => Items.Sum(item => item.Quantity);

    public decimal TotalPrice => Items.Sum(item => item.TotalPrice);

    public void AddItem(Book book, int quantity = 1)
    {
        var existingItem = Items.FirstOrDefault(item => item.BookId == book.Id);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            Items.Add(new CartItem
            {
                BookId = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                Quantity = quantity,
                ImageUrl = book.ImageUrl
            });
        }
    }

    public void RemoveItem(int bookId)
    {
        var item = Items.FirstOrDefault(i => i.BookId == bookId);
        if (item != null)
        {
            Items.Remove(item);
        }
    }

    public void UpdateQuantity(int bookId, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.BookId == bookId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
        }
    }

    public void Clear()
    {
        Items.Clear();
    }
}
