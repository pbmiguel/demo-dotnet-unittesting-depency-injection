namespace WebApp.DataAccess
{
    using System;
    using System.Collections.Generic;
    using Models;
    
    public class InMemoryBookData : IBookData
    {
        private static Dictionary<int, Book> _data;

        public InMemoryBookData()
        {
            Book[] books = new Book[]
            {
                new Book { Id = 1, Title  ="The Grouchy Ladybug", Author = "Eric Carle", PublishDate = new DateTime(1999, 9, 8), PageLength = 44 },
                new Book { Id = 2, Title  ="A Million Chameleons", Author = "James Young", PublishDate = new DateTime(1990, 9, 1), PageLength = 32 },
                new Book { Id = 3, Title  ="Goodnight Moon", Author = "Margaret Wise Brown", PublishDate = new DateTime(2007, 1, 23), PageLength = 30 }
            };
            _data = new Dictionary<int, Book>();

            foreach (Book b in books)
            {
                _data.Add(b.Id, b);
            }
        }

        public IEnumerable<Book> Books
        {
            get
            {
                return _data.Values;
            }
        }

        public Book Create(Book entity)
        {
            entity.Id = _data.Keys.Count + 1;
            return _data[entity.Id] = entity;
        }
    }
}
