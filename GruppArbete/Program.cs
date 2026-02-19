using Dapper;
using Microsoft.Data.SqlClient;

namespace TerminalBibliotek
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var connection = new SqlConnection("Server=localhost,1433;Database=Exempelnamn;User ID=sa;Password=Lösenord!;Encrypt=True;TrustServerCertificate=True;");
            connection.Open();

            connection.Execute("INSERT INTO ducks(name) VALUE('Kalle Anka')");

        }
    }
}
