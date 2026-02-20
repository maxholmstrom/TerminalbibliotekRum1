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
            connection.Query<string>("SELECT Name FROM Authors");

            if (args.Length >= 3 &&
                (args[0].Equals("add", StringComparison.OrdinalIgnoreCase) ||
                 args[0].Equals("a", StringComparison.OrdinalIgnoreCase)) &&
                 args[1].Equals("author", StringComparison.OrdinalIgnoreCase) ||
                 args[1].Equals("a", StringComparison.OrdinalIgnoreCase))
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


        }
    }
}
