using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1_practice1
{
    class Program
    {
        private static List<Book> ReadFromFile(string filename)
        {
            List<Book> books = new List<Book>();

            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while((line = reader.ReadLine()) != null)
                    {
                        string[] part  = line.Split(';');
                        string book_name = part[0];
                        int year = int.Parse(part[1]);
                        double price = double.Parse(part[2]);
                        Book book = new Book(book_name,year,price);
                        books.Add(book); 
                    }

                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception :"+e.Message);
            }
            return books;
        }

        private static void WorkerThreadFunctionWriter(DataMonitor dm,ResultMonitor rm)
        {
            try
            {
                while (true)
                {
                    Book book = dm.RemoveData();

                    if (book == null)
                    {
                        break;
                    }

                  double computed = ResultComputation(book);
                    if (computed > 300)
                    {
                        rm.AddResultData(book);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }
        }

        private static double ResultComputation(Book book)
        {
            double ComputedPrice;

            if(book.Year > 2015)
            {
                ComputedPrice = 2* book.Price; 
            }
            else
            {
                ComputedPrice = 1.5 * book.Price;
            }
            return ComputedPrice;
        }

        private static void WriteResultsToFile(ResultMonitor rmonitor, string fileName)
        {
            int c = 0;
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.WriteLine("--------------------------------------------------------------------");
                    writer.WriteLine("|    Title        |   Year       |   Price      | Computed Price  |");
                    writer.WriteLine("--------------------------------------------------------------------");
                    foreach (var result in rmonitor.GetData())
                    {
                        double computed = ResultComputation(result);
                        writer.WriteLine("| {0,-15} | {1,-12} | {2,-12:F2} |  {3,-10:F2}     |", result.Title, result.Year, result.Price,computed);
                        c++;               
                    }
                    writer.WriteLine("---------------------------------------------------------------------");
                    writer.WriteLine("Number of data : "+c);
                }
            }
            catch (Exception e)
            {
              
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        static void Main(string[] args)
        {
          
            string resultfile = "IFU-1_MahadeerT_L1_rez.txt";
            DataMonitor dm = new DataMonitor();
            ResultMonitor rm = new ResultMonitor();

            string datafile1 = "IFU-1_MahadeerT_L1_dat_1.txt";
            string datafile2 = "IFU-1_MahadeerT_L1_dat_2.txt";
            string datafile3 = "IFU-1_MahadeerT_L1_dat_3.txt";


            List<Book> book_list = ReadFromFile(datafile3);
            int workerCount = Math.Max(2,5);
          
            Thread[] workerThreads = new Thread[workerCount];

            for (int i = 0; i < workerCount; i++)
            {            
                workerThreads[i] = new Thread(() => WorkerThreadFunctionWriter(dm, rm));
                workerThreads[i].Start();          
            }
          
            foreach (var b in book_list)
            {
                dm.AddData(b);
            }
           
            foreach (var thread in workerThreads)
            {             
                Console.WriteLine("join Working");
                thread.Join();
                Console.WriteLine("join finished");
            }

            if (rm.GetCount() > 0)
            {
                WriteResultsToFile(rm, resultfile);
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(resultfile))
                {
                    writer.WriteLine("No data is filtered!");
                }
            }
            Console.ReadKey();
        }
    }
}
