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
IF NOT EXISTS (
SELECT 1 FROM sys.tables WHERE name = 'Authors'
)
BEGIN
CREATE TABLE Authors (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);
END
");
            connection.Query<string>("SELECT Name FROM Authors");

        }
    }
}
