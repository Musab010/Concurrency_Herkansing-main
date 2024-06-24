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
            // Klant wacht tot er een boek beschikbaar is op de balie
            Program.balieSemaphore.Wait();
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
                Program.balieSemaphore.Release();
            }

            if (_currentBook != null)
            {
                Console.WriteLine($"Customer {_id} is about to read the book {_currentBook.BookId}");

                // Klant leest het boek
                Thread.Sleep(new Random().Next(100, 500));

                // Klant brengt het boek terug naar de inleverplaats
                Console.WriteLine($"Customer {_id} is dropping off the book {_currentBook.BookId}");

                Program.inleverplaatsSemaphore.Wait();
                try
                {
                    Program.dropoff.AddFirst(_currentBook);
                }
                finally
                {
                    Program.inleverplaatsSemaphore.Release();
                }

                _currentBook = null;
            }

            Console.WriteLine($"Customer {_id} is leaving the library");
        }
    }
}

/*
Musab Sivrikaya (0988932)
Ozeir Moradi (0954800)
*/