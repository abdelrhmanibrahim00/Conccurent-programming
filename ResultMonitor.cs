using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1_practice1
{
    class ResultMonitor
    {
        const int maxSize = 200;
        private  Book[] result_books;
        private int count;

        public ResultMonitor()
        {
            result_books = new Book[maxSize];
            count = 0;
        }


     public void AddResultData(Book item)
     {
        lock (this)
         {        
            while (count >= maxSize)
            {
                Monitor.Wait(this); 
            }        
            result_books[count++] = item;
            BubbleSort();
            Monitor.Pulse(this);
               
        }
     }
        public int GetCount()
        {       
                return count;        
        }

        public Book[] GetData()
        {         
             Book[] results = new Book[count];
             for (int i = 0; i < count; i++)
             {
                  results[i] = result_books[i];
             }              
             return results;  
        }


        private void BubbleSort()
        {
            int n = count;
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 1; i < n; i++)
                {
                    if (result_books[i - 1].CompareTo(result_books[i]) > 0)
                    {
                        Book temp = result_books[i - 1];
                        result_books[i - 1] = result_books[i];
                        result_books[i] = temp;
                        swapped = true;
                    }
                }
                n--;
            } while (swapped);
        }


    }
}

/*
public Book[] GetData()
{
    lock (this)
    {
        while (count == 0)
        {
            Monitor.Wait(this);
        }

        Book[] results = new Book[count];
        for (int i = 0; i < count; i++)
        {
            results[i] = result_books[i];
        }
        Monitor.Pulse(this);
              */