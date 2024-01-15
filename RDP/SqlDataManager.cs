using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getting_Rudolph_Table_From_PDF_To_PostgreSQL
{
    internal class SqlDataManager
    {
        public static void Connection(string name, List<string> collumnNames, List<double> tableValues)
        {
            string connectionString = "Host=localhost;Username=postgres;password=kk;Database=kk";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    if (TableExists(connection, name))
                    {
                        using (NpgsqlCommand command2 = new NpgsqlCommand(PostgreSQLQueryBuilder.AddValuesToQuery(name, collumnNames, tableValues), connection))
                        {
                            command2.ExecuteNonQuery();
                            Console.WriteLine("Dane zostały dodane do tabeli");
                        }
                    }
                    else
                    {
                        using (NpgsqlCommand command = new NpgsqlCommand(PostgreSQLQueryBuilder.CreateTable(name, collumnNames), connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine("Baza Utworzona");
                        }
                        using (NpgsqlCommand command2 = new NpgsqlCommand(PostgreSQLQueryBuilder.AddValuesToQuery(name, collumnNames, tableValues), connection))
                        {
                            command2.ExecuteNonQuery();
                            Console.WriteLine("Dane zostały dodane do tabeli");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static bool TableExists(NpgsqlConnection connection, string tableName)
        {
            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = @tableName)";
                command.Parameters.AddWithValue("@tableName", tableName.ToLower());
                object result = command.ExecuteScalar();
                return result != null && (bool)result;
            }
        }
        public static void DataBaseConnection(string query, string key)
        {
            string connectionString = "Host=localhost;Username=postgres;password=kk;Database=kk";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                object value1 = reader["punkty"];
                                Console.WriteLine($"Na dystansie {DataFormat.TranslateStrokeBack(key)} uzyskałeś {value1}pkt w skali Rudolpha");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Na dystansie {DataFormat.TranslateStrokeBack(key)} uzyskałeś/aś mniej niż 1 pkt");
                        }
                    }
                }
            }
        }
    }
}
