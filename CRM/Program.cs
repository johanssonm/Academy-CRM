using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace CRM
{

    class Program
    {
        private static string connectionString = @"Server = (localdb)\mssqllocaldb; Database = AcademyCRM; Trusted_Connection = True";
        private static SqlConnection connection = new SqlConnection(connectionString);

        static void Main(string[] args)
        {
            MainMenu();



            PrintAllUsers();

            AddNewUser();

            PrintAllUsers();

            Console.ReadKey();
        
        }


        //  Skapa en ny kund
        //  Ändra en kund
        //  Ta bort en kund
        //  Hämta alla kunder
        static void PrintAllUsers()
        {

            {

                using (connection)
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand (@"SELECT * FROM USERS;",connection);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine("{0} {1} {2}",
                        reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                    }

                    Console.ReadKey();

                }
            
            }
        }

        static void AddNewUser()
        {
            var firstname = "Test";
            var lastname = "Test";
            var email = "Test";
            var phone = "Test";


            using (connection)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@" INSERT INTO USERS 
                                                   VALUES(@FirstName, @LastName, @Email, @Phone)", connection);
                var writer = cmd.Parameters;

                writer.AddWithValue($"@FirstName", firstname);
                writer.AddWithValue($"@LastName", lastname);
                writer.AddWithValue($"@Email", email);
                writer.AddWithValue($"@Phone", phone);

                Console.WriteLine("Användaren lades till");
                Console.ReadKey();
               
            }

        }




        static void MainMenu()
        {

            Console.WriteLine("Huvudmeny: ");
            Console.WriteLine("---");

        }
    }
}
