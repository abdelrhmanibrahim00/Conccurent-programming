using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_practice1
{
    class Book
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public double Price { get; set; }
      

        public Book(string title, int year, double price)
        {
            Title = title;
            Year = year;
            Price = price;
        }

        public int CompareTo(Book book1)
        {
            int poz = String.Compare(this.Title, book1.Title, StringComparison.CurrentCulture);
            return poz;
        }      
    }
}
