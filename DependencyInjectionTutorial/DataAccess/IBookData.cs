using DependencyInjectionTutorial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionTutorial.DataAccess
{
    public interface IBookData
    {
        IEnumerable<Book> Books { get; }
        Book Create(Book b);
    }
}
