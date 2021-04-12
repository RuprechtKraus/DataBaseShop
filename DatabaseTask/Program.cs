using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseTaskLib;

namespace DatabaseTask
{
    class Program
    {
        private static string _connectionString = @"Server = RUPRECHTMACHINE\SQLEXPRESS;
                Database = Shop;
                Trusted_Connection = True;";

        static void Main(string[] args)
        {
            string command = "";
            if (args.Length != 0)
              command = args[0];

            if (command == "readcustomer")
            {
                List<Customer> customers = ReadCustomers();
                customers.ForEach(x => Console.WriteLine($"Name: {x.Name}\nCity: {x.City}\n"));
            }
            else if (command == "insert")
            {
                int createdCustomerId = InsertCustomer("Калинин Максим", "Йошкар-Ола");
                Console.WriteLine("Created customer: " + createdCustomerId);
            }
            else if (command == "update")
            {
                UpdateCutomer(4, "Екатерина Владимировна", "Екатеренбург");
            }
            else if (command == "stats")
            {
                List<CustomerStatistics> statistics = GetStatistics();
                statistics.ForEach(x => Console.WriteLine($"Name: {x.Name}" +
                    $"\nNumber of orders: {x.OrdersCount}" +
                    $"\nTotal price: {Math.Round(x.TotalPrice, 2)}\n"));
            }
            else
            {
                Console.WriteLine("No such command");
            }
        }

        private static List<Customer> ReadCustomers()
        {
            List<Customer> customers = new List<Customer>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText =
                        @"SELECT
                            [Name],
                            [City]
                        FROM Customer";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customer = new Customer
                            {
                                Name = Convert.ToString(reader["Name"]),
                                City = Convert.ToString(reader["City"]),
                            };
                            customers.Add(customer);
                        }
                    }
                }
            }

            return customers;
        }

        private static int InsertCustomer(string name, string city)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO [Customer]
                       ([Name],
                        [City]) 
                    VALUES 
                       (@name,
                        @city)
                    SELECT SCOPE_IDENTITY()";

                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@city", SqlDbType.NVarChar).Value = city;

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private static void UpdateCutomer(int customerId, string name, string city)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE [Customer]
                        SET [Name] = @name, [City] = @city
                        WHERE CustomerId = @customerId";

                    command.Parameters.Add("@customerId", SqlDbType.BigInt).Value = customerId;
                    command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                    command.Parameters.Add("@city", SqlDbType.NVarChar).Value = city;

                    command.ExecuteNonQuery();
                }
            }
        }

        private static List<CustomerStatistics> GetStatistics()
        {
            List<CustomerStatistics> statistics = new List<CustomerStatistics>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT c.Name, 
                               COUNT(o.CustomerId) AS 'OrdersCount', 
                               SUM(o.Price) AS 'TotalPrice'
                        FROM Customer c
                        INNER JOIN [Order] o ON c.CustomerId = o.CustomerId
                        GROUP BY c.Name";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customerStat = new CustomerStatistics
                            {
                                Name = Convert.ToString(reader["Name"]),
                                OrdersCount = Convert.ToInt32(reader["OrdersCount"]),
                                TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                            };
                            statistics.Add(customerStat);
                        }
                    }
                }

                return statistics;
            }
        }
    }
}
