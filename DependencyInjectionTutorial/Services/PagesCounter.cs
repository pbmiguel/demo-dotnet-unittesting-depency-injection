using DependencyInjectionTutorial.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionTutorial.Models;

namespace DependencyInjectionTutorial.Services
{
    public class PagesCounter : ICounter
    {
        private IBookData _bd;
        private RequestTrackerData _tracker;

        public PagesCounter(IBookData b, RequestTrackerData rt)
        {
            _bd = b;
            _tracker = rt;
        }

        public int Count()
        {
            _tracker.Add("PageCounter.Count");
            int result = 0;
            foreach (Book b in _bd.Books)
            {
                result += b.PageLength;
            }
            return result;
        } 
    }
}
