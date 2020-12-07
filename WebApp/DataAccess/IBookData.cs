namespace WebApp.DataAccess
{
    using WebApp.Models;
    using System.Collections.Generic;
    
    public interface IBookData
    {
        IEnumerable<Book> Books { get; }
        Book Create(Book b);
    }
}
