namespace booksforall
{
    public class Clerk
    {
        private static LinkedList<BookRecord> _records; // Do not alter this line
        private int _id;
        private static readonly SemaphoreSlim recordSlotSemaphore = new(1, 1);

        public Clerk(int clerkId)
        {
            _id = clerkId;
            if (_records == null)
            {
                _records = new LinkedList<BookRecord>();
            }
        }

        public static void initRecords(LinkedList<Book> books) // Do not alter this method
        {
            if (_records == null)
            {
                _records = new LinkedList<BookRecord>();
            }
            foreach (Book book in books)
            {
                _records.AddFirst(new BookRecord(book, false));
            }
        }

        internal static int checkBookInInventory() // Do not alter this method
        // This method is called when the library is closing
        // This method should return the number of books left in the library
        {
            int counter = 0;
            foreach (var record in _records)
            {
                if (record.IsBorrowed == false) // We are counting the books that are in the library (not borrowed)
                {
                    counter++;
                }
            }

            if (counter != _records.Count)
            {
                Console.WriteLine("Error: the number of books left in the library does not match the number of records." + counter + _records.Count);
            }
            return counter;
        }

        public void DoWork()
        {
            // The clerk will put the book on the counter
            Console.WriteLine($"Clerk [{_id}] is going to check in the records for a book to put on the counter");

            Book? t_book = null;

            lock (_records)
            {
                foreach (var record in _records)
                {
                    if (record.IsBorrowed == false)
                    {
                        t_book = record.Book;
                        record.IsBorrowed = true;
                        break;
                    }
                }
            }

            if (t_book == null)
            {
                Console.WriteLine($"Clerk [{_id}] found no available books to put on the counter.");
                return;
            }

            Console.WriteLine($"Clerk [{_id}] putting book [{t_book.BookId}] on the counter");

            Program.balieSemaphore.Wait();
            try
            {
                Program.counter.AddFirst(t_book);
            }
            finally
            {
                Program.balieSemaphore.Release();
            }
            Program.boekBeschikbaar.Release();

            // The clerk will take a nap for overworking
            Thread.Sleep(new Random().Next(100, 500));

            // The clerk will wait for a book in the dropoff
            Program.boekBeschikbaar.Wait();
            Program.inleverplaatsSemaphore.Wait();

            try
            {
                if (Program.dropoff.Count > 0)
                {
                    t_book = Program.dropoff.First();
                    Program.dropoff.RemoveFirst();
                }
                else
                {
                    t_book = null;
                }
            }

            finally
            {
                Program.inleverplaatsSemaphore.Release();
            }

            if (t_book == null)
            {
                Console.WriteLine($"Clerk [{_id}] found no books in the dropoff.");
                return;
            }

            // The clerk will check the book in the records
            Console.WriteLine($"Clerk [{_id}] is checking in the book [{t_book.BookId}] in the records");
            lock (_records)
            {
                foreach (BookRecord record in _records)
                {
                    if (record.Book.BookId == t_book.BookId)
                    {
                        record.IsBorrowed = false;
                        break;
                    }
                }
            }
        }

        // Modified method to process only one remaining book from the dropoff
        public void ProcessRemainingBooks()
        {
            Book? t_book = null;

            Program.inleverplaatsSemaphore.Wait();
            try
            {
                if (Program.dropoff.Count > 0)
                {
                    t_book = Program.dropoff.First();
                    Program.dropoff.RemoveFirst();
                }
            }
            finally
            {
                Program.inleverplaatsSemaphore.Release();
            }

            if (t_book != null)
            {
                bool isReturned = false;
                lock (_records)
                {
                    foreach (BookRecord record in _records)
                    {
                        if (record.Book.BookId == t_book.BookId)
                        {
                            record.IsBorrowed = false;
                            isReturned = true;
                            break;
                        }
                    }
                }
                if (isReturned)
                {
                    Console.WriteLine($"Clerk [{_id}] is checking in the book [{t_book.BookId}] in the records");
                    Console.WriteLine($"Clerk [{_id}] has marked book [{t_book.BookId}] as returned in the records.");
                }
            }
        }
    }
}

/*
Musab Sivrikaya (0988932)
Ozeir Moradi (0954800)
*/