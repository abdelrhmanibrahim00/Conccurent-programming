using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1_practice1
{
    class DataMonitor
    {
        const int MaxSize = 13; 
        private Book[] books; 
        private int count;
        private int counter=0;
      
        public DataMonitor()
        {
            count = 0; 
            books = new Book[MaxSize]; 
        }

        
        public void AddData(Book item)
        {
            lock (this) 
            {         
                while (count >= MaxSize)
                {
                    Monitor.Wait(this); 
                }
                books[count++] = item; 
                counter++;
                Monitor.Pulse(this);
            }
        }

   
        public Book RemoveData()
        {
            lock (this)
            {
                while (count == 0 && counter < 25)
                {
                    Monitor.Wait(this);
                }
                if (count == 0 && counter >= 25)
                {
                    return null;
                }
                Book item = books[--count];
                Monitor.Pulse(this);
                return item;
            }
        }
       
        public int GetCount()
        {
            return count;
        }

    }


}
