using System;
using System.Collections.Generic;
using System.Threading;

namespace booksforall
{
    public class Customer
    {
        private Book? _currentBook; // Book currently held by the customer
        private readonly int _id;

        public Customer(int customerId)
        {
            _currentBook = null;
            _id = customerId;
        }

        public Book? GetCurrentBook()
        {
            return _currentBook;
        }

        public void DoWork()
        {
            Program.counterSemaphore.Wait();
            try
            {
                if (Program.counter.Count > 0)
                {
                    _currentBook = Program.counter.First.Value;
                    Program.counter.RemoveFirst();
                }
            }
            finally
            {
                Program.counterSemaphore.Release();
            }

            if (_currentBook != null)
            {
                Console.WriteLine($"Customer {_id} is about to read the book {_currentBook.BookId}");

                Thread.Sleep(new Random().Next(100, 500));

                Console.WriteLine($"Customer {_id} is dropping off the book {_currentBook.BookId}");

                Program.dropoffSemaphore.Wait();
                try
                {
                    Program.dropoff.AddFirst(_currentBook);
                }
                finally
                {
                    Program.dropoffSemaphore.Release();
                }

                _currentBook = null;
            }

            Console.WriteLine($"Customer {_id} is leaving the library");
        }
    }
}
