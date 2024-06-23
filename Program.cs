﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace booksforall
{
    internal class Program
    {
        public static int n_threads = 50; // Pas dit aan naar behoefte

        private static readonly string studentname1 = "Musab Sivrikaya"; // name and surname of the student1
        private static readonly string studentnum1 = "0988932"; // student number
        private static readonly string studentname2 = "Ozeir Moradi"; // name and surname of the student2
        private static readonly string studentnum2 = "0954800"; // student number2

        public static int n_books = n_threads;
        public static int n_customers = n_threads;
        public static readonly Clerk[] clerks = new Clerk[n_threads];
        public static readonly Customer[] customers = new Customer[n_threads];
        public static LinkedList<Book> counter = new();
        public static LinkedList<Book> dropoff = new();
        public static SemaphoreSlim counterSemaphore = new(1, 1);
        public static SemaphoreSlim dropoffSemaphore = new(1, 1);
        public static SemaphoreSlim bookAvailable = new(0, int.MaxValue); // To signal when a book is available

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, we are starting our new pickup LIBRARY!");
            InitLibrary(); // Do not alter this method

            // init the customers
            InitCustomers(); // Do not alter this call

            // init the clerks
            InitClerks(); // Do not alter this call

            // init records
            Clerk.initRecords(dropoff); // Do not alter this line
            // clean the dropoff
            dropoff.Clear(); // Do not alter this line

            // start the clerks
            StartClerks(); // Do not alter this call

            // start the customers
            StartCustomers(); // Do not alter this call
            // DO NOT CHANGE THE CODE ABOVE
            // Use the space below to add your code if needed

            // Ensure clerks process all remaining books in dropoff before closing
            foreach (var clerk in clerks)
            {
                clerk.ProcessRemainingBooks();
            }

            // DO NOT CHANGE THE CODE BELOW
            // The library is closing, DO NOT ALTER the following lines
            Console.WriteLine("Books left in the library " + Clerk.checkBookInInventory());

            if (counter.Count != 0)
            {
                Console.WriteLine("Books left and not picked up: " + counter.Count);
            }
            else
            {
                Console.WriteLine("Books left and not picked up: NOTHING LEFT!");
            }

            Console.WriteLine("Books left on the dropoff and not processed: " + dropoff.Count);
            // The lists should be empty
            Console.WriteLine("Name: " + studentname1 + " Student number: " + studentnum1);
            Console.WriteLine("Name: " + studentname2 + " Student number: " + studentnum2);
        }

        public static void InitLibrary()
        {
            for (int i = 0; i < n_books; i++)
            {
                Book book = new Book(i);
                dropoff.AddLast(book);
            }
        }

        public static void InitCustomers()
        {
            for (int i = 0; i < n_customers; i++)
            {
                customers[i] = new Customer(i);
            }
        }

        public static void InitClerks()
        {
            for (int i = 0; i < n_threads; i++)
            {
                clerks[i] = new Clerk(i);
            }
        }

        public static void StartClerks()
        {
            Thread[] clerkThreads = new Thread[n_threads];
            for (int i = 0; i < n_threads; i++)
            {
                clerkThreads[i] = new Thread(clerks[i].DoWork);
                clerkThreads[i].Start();
            }
            foreach (Thread t in clerkThreads)
            {
                t.Join();
            }
        }

        public static void StartCustomers()
        {
            Thread[] customerThreads = new Thread[n_customers];
            for (int i = 0; i < n_customers; i++)
            {
                customerThreads[i] = new Thread(customers[i].DoWork);
                customerThreads[i].Start();
            }
            foreach (Thread t in customerThreads)
            {
                t.Join();
            }
        }
    }

    public class Book // Do not alter this class
    {
        public int BookId { get; set; }
        public Book(int bookId)
        {
            BookId = bookId;
        }
    }

    public class BookRecord // Do not alter this class
    {
        public Book Book { get; set; }
        public bool IsBorrowed { get; set; }

        public BookRecord(Book book, bool isBorrowed)
        {
            Book = book;
            IsBorrowed = isBorrowed;
        }
    }
}