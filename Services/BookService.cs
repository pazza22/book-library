using book_library.Models;

namespace book_library.Services;

public interface IBookService
{
    List<Book> GetAllBooks();
    List<Book> SearchBooks(string searchTerm);
}

public class BookService : IBookService
{
    private readonly List<Book> _books;

    public BookService()
    {
        _books = new List<Book>
        {
            new Book { Id = 1, Title = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", PublicationYear = 1960, ISBN = "978-0061120084", Description = "A classic novel about racism and injustice in the American South." },
            new Book { Id = 2, Title = "1984", Author = "George Orwell", Genre = "Dystopian", PublicationYear = 1949, ISBN = "978-0451524935", Description = "A dystopian novel about totalitarianism and surveillance." },
            new Book { Id = 3, Title = "Pride and Prejudice", Author = "Jane Austen", Genre = "Romance", PublicationYear = 1813, ISBN = "978-0141439518", Description = "A romantic novel about manners and matrimony in Georgian England." },
            new Book { Id = 4, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Genre = "Fiction", PublicationYear = 1925, ISBN = "978-0743273565", Description = "A story of the Jazz Age and the American Dream." },
            new Book { Id = 5, Title = "The Catcher in the Rye", Author = "J.D. Salinger", Genre = "Fiction", PublicationYear = 1951, ISBN = "978-0316769488", Description = "A coming-of-age story about teenage rebellion." },
            new Book { Id = 6, Title = "Harry Potter and the Sorcerer's Stone", Author = "J.K. Rowling", Genre = "Fantasy", PublicationYear = 1997, ISBN = "978-0590353427", Description = "A young wizard discovers his magical heritage." },
            new Book { Id = 7, Title = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", PublicationYear = 1937, ISBN = "978-0547928227", Description = "An adventure of a hobbit on a quest to reclaim treasure." },
            new Book { Id = 8, Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", Genre = "Fantasy", PublicationYear = 1954, ISBN = "978-0544003415", Description = "An epic quest to destroy an all-powerful ring." },
            new Book { Id = 9, Title = "Animal Farm", Author = "George Orwell", Genre = "Satire", PublicationYear = 1945, ISBN = "978-0452284241", Description = "A satirical allegory about revolution and power." },
            new Book { Id = 10, Title = "Brave New World", Author = "Aldous Huxley", Genre = "Dystopian", PublicationYear = 1932, ISBN = "978-0060850524", Description = "A futuristic society where humans are engineered." },
            new Book { Id = 11, Title = "The Chronicles of Narnia", Author = "C.S. Lewis", Genre = "Fantasy", PublicationYear = 1950, ISBN = "978-0066238500", Description = "Children discover a magical world through a wardrobe." },
            new Book { Id = 12, Title = "Jane Eyre", Author = "Charlotte Brontë", Genre = "Romance", PublicationYear = 1847, ISBN = "978-0141441146", Description = "A governess falls in love with her mysterious employer." },
            new Book { Id = 13, Title = "Wuthering Heights", Author = "Emily Brontë", Genre = "Gothic", PublicationYear = 1847, ISBN = "978-0141439556", Description = "A passionate and destructive love story on the moors." },
            new Book { Id = 14, Title = "Moby-Dick", Author = "Herman Melville", Genre = "Adventure", PublicationYear = 1851, ISBN = "978-0142437247", Description = "Captain Ahab's obsessive quest for the white whale." },
            new Book { Id = 15, Title = "The Odyssey", Author = "Homer", Genre = "Epic", PublicationYear = -800, ISBN = "978-0140268867", Description = "The epic journey of Odysseus returning home from war." },
            new Book { Id = 16, Title = "Crime and Punishment", Author = "Fyodor Dostoevsky", Genre = "Philosophical", PublicationYear = 1866, ISBN = "978-0486415871", Description = "A psychological exploration of guilt and redemption." },
            new Book { Id = 17, Title = "The Brothers Karamazov", Author = "Fyodor Dostoevsky", Genre = "Philosophical", PublicationYear = 1880, ISBN = "978-0374528379", Description = "A philosophical novel about faith, doubt, and morality." },
            new Book { Id = 18, Title = "War and Peace", Author = "Leo Tolstoy", Genre = "Historical", PublicationYear = 1869, ISBN = "978-0199232765", Description = "An epic tale of Russian society during the Napoleonic era." },
            new Book { Id = 19, Title = "Anna Karenina", Author = "Leo Tolstoy", Genre = "Romance", PublicationYear = 1877, ISBN = "978-0143035008", Description = "A tragic love story in imperial Russia." },
            new Book { Id = 20, Title = "The Divine Comedy", Author = "Dante Alighieri", Genre = "Epic", PublicationYear = 1320, ISBN = "978-0142437223", Description = "A journey through Hell, Purgatory, and Paradise." },
            new Book { Id = 21, Title = "Don Quixote", Author = "Miguel de Cervantes", Genre = "Satire", PublicationYear = 1605, ISBN = "978-0060934347", Description = "A knight errant's delusional adventures in Spain." },
            new Book { Id = 22, Title = "Les Misérables", Author = "Victor Hugo", Genre = "Historical", PublicationYear = 1862, ISBN = "978-0451419439", Description = "A story of redemption and revolution in 19th century France." },
            new Book { Id = 23, Title = "The Count of Monte Cristo", Author = "Alexandre Dumas", Genre = "Adventure", PublicationYear = 1844, ISBN = "978-0140449266", Description = "A tale of betrayal, imprisonment, and revenge." },
            new Book { Id = 24, Title = "Frankenstein", Author = "Mary Shelley", Genre = "Gothic", PublicationYear = 1818, ISBN = "978-0486282114", Description = "A scientist creates a living creature with tragic consequences." },
            new Book { Id = 25, Title = "Dracula", Author = "Bram Stoker", Genre = "Horror", PublicationYear = 1897, ISBN = "978-0486411095", Description = "The classic vampire tale of Count Dracula." },
            new Book { Id = 26, Title = "The Picture of Dorian Gray", Author = "Oscar Wilde", Genre = "Gothic", PublicationYear = 1890, ISBN = "978-0141439570", Description = "A man remains young while his portrait ages." },
            new Book { Id = 27, Title = "Great Expectations", Author = "Charles Dickens", Genre = "Fiction", PublicationYear = 1861, ISBN = "978-0141439563", Description = "An orphan's journey from poverty to wealth." },
            new Book { Id = 28, Title = "A Tale of Two Cities", Author = "Charles Dickens", Genre = "Historical", PublicationYear = 1859, ISBN = "978-0486406510", Description = "Set against the French Revolution, a story of sacrifice." },
            new Book { Id = 29, Title = "The Adventures of Sherlock Holmes", Author = "Arthur Conan Doyle", Genre = "Mystery", PublicationYear = 1892, ISBN = "978-0486474915", Description = "Classic detective stories featuring Sherlock Holmes." },
            new Book { Id = 30, Title = "The Little Prince", Author = "Antoine de Saint-Exupéry", Genre = "Children's", PublicationYear = 1943, ISBN = "978-0156012195", Description = "A philosophical tale about a young prince from another planet." }
        };
    }

    public List<Book> GetAllBooks()
    {
        return _books;
    }

    public List<Book> SearchBooks(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return _books;
        }

        searchTerm = searchTerm.ToLower();
        return _books.Where(b => 
            b.Title.ToLower().Contains(searchTerm) || 
            b.Author.ToLower().Contains(searchTerm) ||
            b.Genre.ToLower().Contains(searchTerm) ||
            b.Description.ToLower().Contains(searchTerm)
        ).ToList();
    }
}
