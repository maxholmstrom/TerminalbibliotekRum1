using Dapper;
using Microsoft.Data.SqlClient;

namespace TerminalBibliotek
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Man behöver kolla så att alla ha databas som heter "Exempel" och Lösenord "Lösenord!" på dator (det ska vara MSSQL) Kolla ReadMe
            using var connection = new SqlConnection("Server=localhost,1433;Database=Exempel; User ID=sa;Password=Lösenord!;Encrypt=True;TrustServerCertificate=True;");
            connection.Open();

            connection.Execute(@"
                    IF NOT EXISTS 
                        (SELECT 1 FROM sys.tables WHERE name = 'Authors')
                           BEGIN
                               CREATE TABLE Authors (
                                      Id INT IDENTITY(1,1) PRIMARY KEY,
                                      Name NVARCHAR(100) NOT NULL
                        );
                        END");


            //SKAPA TABELLER - BOOK
            connection.Execute(@"
                    IF NOT EXISTS 
                        (SELECT 1 FROM sys.tables WHERE name = 'Books')
                           BEGIN
                               CREATE TABLE Books (
                                      Id INT IDENTITY(1,1) PRIMARY KEY,
                                      Name NVARCHAR(100) NOT NULL UNIQUE,
                                      
                        );
                        END");
            
            connection.Execute(@"
           IF NOT EXISTS 
                 (SELECT 1 FROM sys.tables WHERE name = 'Library')
    
                   CREATE TABLE Library (
                           AuthorId INT NOT NULL,
                           BookId INT NOT NULL,
                           FOREIGN KEY (AuthorId) REFERENCES Authors(Id),
                           FOREIGN KEY (BookId) REFERENCES Books(Id));");

            if (args.Length >= 3 &&
                (args[0].Equals("add", StringComparison.OrdinalIgnoreCase) ||
                args[0].Equals("a", StringComparison.OrdinalIgnoreCase)) &&
                (args[1].Equals("author", StringComparison.OrdinalIgnoreCase) ||
                args[1].Equals("authors", StringComparison.OrdinalIgnoreCase) ||
                args[1].Equals("a", StringComparison.OrdinalIgnoreCase)))
            {
                string name = string.Join(" ", args.Skip(2)).Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Du måste ange ett namn.");
                    return;
                }

                connection.Execute(
                    "INSERT INTO Authors (Name) VALUES (@Name)",
                    new { Name = name });

                Console.WriteLine("Författare tillagd.");
            }
            else if (args.Length == 2 &&
                    (args[0].Equals("list", StringComparison.OrdinalIgnoreCase) ||
                    args[0].Equals("l", StringComparison.OrdinalIgnoreCase)) &&
                    (args[1].Equals("authors", StringComparison.OrdinalIgnoreCase) ||
                    args[1].Equals("author", StringComparison.OrdinalIgnoreCase) ||
                    args[1].Equals("a", StringComparison.OrdinalIgnoreCase)))
            {
                var authors = connection.Query<Author>("SELECT Id, Name FROM Authors").ToList();

                if (authors.Count == 0)
                {
                    Console.WriteLine("Inga författare hittades.");
                    return;
                }

                Console.WriteLine("Författare:");
                foreach (var author in authors)
                {
                    Console.WriteLine($"- {author.Name} (ID: {author.Id})");
                }
            }

            else if (args.Length == 3 &&
                    (args[0].Equals("list", StringComparison.OrdinalIgnoreCase) ||
                    args[0].Equals("l", StringComparison.OrdinalIgnoreCase)) &&
                    (args[1].Equals("authors", StringComparison.OrdinalIgnoreCase) ||
                    args[1].Equals("author", StringComparison.OrdinalIgnoreCase) ||
                    args[1].Equals("a", StringComparison.OrdinalIgnoreCase)) &&
                    ((args[2].Equals("--books", StringComparison.OrdinalIgnoreCase)) ||
                    args[2].Equals("-b", StringComparison.OrdinalIgnoreCase)))
            {
                var lists = connection.Query<AuthorBook>(@"
                    SELECT 
                                Authors.Name AS AuthorName, 
                                Books.Name AS BookName
                                    FROM  Library 
                    JOIN Authors ON Authors.Id = Library.AuthorId
                    JOIN Books ON Books.Id = Library.BookId").ToList();

                if (lists.Count == 0)
                {
                    Console.WriteLine("Inga författare hittades.");
                    return;
                }

                var gruperadList = lists.GroupBy(x => x.AuthorName);
                foreach (var author in gruperadList)
                {
                    Console.WriteLine($"{author.Key}: ");
                    foreach(var books in author)
                    { Console.WriteLine($" - {books.BookName}"); }
                }
            }


            else if (args.Length == 3 &&
                    (args[0].Equals("list", StringComparison.OrdinalIgnoreCase) ||
                    args[0].Equals("l", StringComparison.OrdinalIgnoreCase)) &&
                    (args[1].Equals("books", StringComparison.OrdinalIgnoreCase) ||
                    args[1].Equals("book", StringComparison.OrdinalIgnoreCase) ||
                    args[1].Equals("b", StringComparison.OrdinalIgnoreCase)) &&
                    ((args[2].Equals("--authors", StringComparison.OrdinalIgnoreCase)) ||
                    args[2].Equals("--a", StringComparison.OrdinalIgnoreCase)))
            {
                var lists = connection.Query<AuthorBook>(@"
                    SELECT 
                                Authors.Name AS AuthorName, 
                                Books.Name AS BookName
                                    FROM  Library 
                    JOIN Authors ON Authors.Id = Library.AuthorId
                    JOIN Books ON Books.Id = Library.BookId").ToList();

                if (lists.Count == 0)
                {
                    Console.WriteLine("Inga författare hittades.");
                    return;
                }

                var gruperadList = lists.GroupBy(x => x.BookName);
                foreach (var book in gruperadList)
                {
                    Console.WriteLine($"{book.Key}: ");
                    foreach (var author in book)
                    { Console.WriteLine($" - {author.AuthorName}"); }
                }
                return;
            }

            if (args.Length >= 3 &&
                (args[0].Equals("add", StringComparison.OrdinalIgnoreCase) ||
                args[0].Equals("a", StringComparison.OrdinalIgnoreCase)) &&
                (args[1].Equals("book", StringComparison.OrdinalIgnoreCase) ||
                args[1].Equals("b", StringComparison.OrdinalIgnoreCase)))
            {
                string name = string.Join(" ", args.Skip(2)).Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Du måste ange ett namn.");
                    return;
                }

                connection.Execute(
                    "INSERT INTO Books (Name) VALUES (@Name)",
                    new { Name = name });

                Console.WriteLine("Book tillagd.");
            }
            else if (args.Length == 2 &&
                    (args[0].Equals("list", StringComparison.OrdinalIgnoreCase) ||
                    args[0].Equals("l", StringComparison.OrdinalIgnoreCase)) &&
                    (args[1].Equals("books", StringComparison.OrdinalIgnoreCase) ||
                    args[1].Equals("b", StringComparison.OrdinalIgnoreCase)))
            {
                var books = connection.Query<Book>("SELECT Id, Name FROM Authors").ToList();

                if (books.Count == 0)
                {
                    Console.WriteLine("Inga böcker hittades.");
                    return;
                }

                Console.WriteLine("Böcker:");
                foreach (var book in books)
                {
                    Console.WriteLine($"- {book.Name} (ID: {book.Id})");
                }
            }

            // Ta bort författare
            else if (args.Length == 3 &&
                (args[0].Equals("delete", StringComparison.OrdinalIgnoreCase) ||
                 args[0].Equals("d", StringComparison.OrdinalIgnoreCase)) &&
                (args[1].Equals("author", StringComparison.OrdinalIgnoreCase) ||
                 args[1].Equals("a", StringComparison.OrdinalIgnoreCase)))
            {
                var authorName = args[2];

                var rowsAffected = connection.Execute(
                    "DELETE FROM Authors WHERE Name = @Name",
                    new { Name = authorName });

                if (rowsAffected == 0)
                {
                    Console.WriteLine("Ingen författare hittades.");
                }
                else
                {
                    Console.WriteLine("Författare borttagen.");
                }
            }
            // Ta bort bok
            else if (args.Length == 3 &&
                (args[0].Equals("delete", StringComparison.OrdinalIgnoreCase) ||
                 args[0].Equals("d", StringComparison.OrdinalIgnoreCase)) &&
                (args[1].Equals("book", StringComparison.OrdinalIgnoreCase) ||
                 args[1].Equals("b", StringComparison.OrdinalIgnoreCase)))
            {
                var booksName = args[2];

                var rowsAffected = connection.Execute(
                    "DELETE FROM Books WHERE Name = @Name",
                    new { Name = booksName });

                if (rowsAffected == 0)
                {
                    Console.WriteLine("Ingen bok hittades.");
                }
                else
                {
                    Console.WriteLine("Bok borttagen.");
                }
            }
            //modifiera författare
            else if (args.Length == 6 &&
                (args[0].Equals("modify", StringComparison.OrdinalIgnoreCase) ||
                 args[0].Equals("m", StringComparison.OrdinalIgnoreCase)) &&
                (args[1].Equals("author", StringComparison.OrdinalIgnoreCase) ||
                 args[1].Equals("a", StringComparison.OrdinalIgnoreCase)) &&
                (args[3].Equals("add", StringComparison.OrdinalIgnoreCase) ||
                 args[3].Equals("a", StringComparison.OrdinalIgnoreCase)) &&
                (args[4].Equals("book", StringComparison.OrdinalIgnoreCase) ||
                 args[4].Equals("b", StringComparison.OrdinalIgnoreCase)))
            {
                string authorName = args[2];
                string bookTitle = args[5];

                var author = connection.QueryFirstOrDefault<Author>(
                    "SELECT Id FROM Authors WHERE Name = @Name",
                    new { Name = authorName });
                var book = connection.QueryFirstOrDefault<Book>(
                    "SELECT Id FROM Books WHERE Name = @Name;",
                    new { Name = bookTitle });
                if (author == null)
                    {
                    Console.WriteLine("Författaren hittades inte.");
                    return;
                }
                if (book == null)
                {
                    Console.WriteLine("Boken hittades inte.");
                    return;
                }

                connection.Execute(
                    "INSERT INTO Library (AuthorId, BookId) VALUES (@AuthorId, @BookId)",
                    new { AuthorId = author.Id, BookId = book.Id });
                
            }
            // Ta bort bok från författare
            else if (args.Length == 6 &&
                (args[0].Equals("modify", StringComparison.OrdinalIgnoreCase) ||
                 args[0].Equals("m", StringComparison.OrdinalIgnoreCase)) &&
                (args[1].Equals("author", StringComparison.OrdinalIgnoreCase) ||
                 args[1].Equals("a", StringComparison.OrdinalIgnoreCase)) &&
                (args[3].Equals("remove", StringComparison.OrdinalIgnoreCase) ||
                 args[3].Equals("r", StringComparison.OrdinalIgnoreCase)) &&
                (args[4].Equals("book", StringComparison.OrdinalIgnoreCase) ||
                 args[4].Equals("b", StringComparison.OrdinalIgnoreCase)))
            {
                string authorName = args[2];
                string bookTitle = args[5];

                var author = connection.QueryFirstOrDefault<Author>(
                    "SELECT Id FROM Authors WHERE Name = @Name",
                    new { Name = authorName });
                var book = connection.QueryFirstOrDefault<Book>(
                    "SELECT Id FROM Books WHERE Name = @Name;",
                    new { Name = bookTitle });
                if (author == null)
                {
                    Console.WriteLine("Författaren hittades inte.");
                    return;
                }
                if (book == null)
                {
                    Console.WriteLine("Boken hittades inte.");
                    return;
                }
                connection.Execute(
                    "DELETE FROM Library WHERE AuthorId = @AuthorId AND BookId = @BookId",
                    new { AuthorId = author.Id, BookId = book.Id });
            }
            else
            {
                Console.WriteLine("Ogiltigt kommando eller argument.");
            }


        }
        public class Author
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }

        public class Book
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
        }

        public class AuthorBook
        {
            public string AuthorName { get; set; } = "";
            public string BookName { get; set; } = "";
        }
    }
}