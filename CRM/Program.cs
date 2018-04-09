using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
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
                using (SqlConnection connection = new SqlConnection(connectionString))
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
            Console.WriteLine();
            Console.WriteLine("1/4, Ange ett förnamn: ");
            var firstname = Console.ReadLine();
            Console.WriteLine("2/4, Ange ett efternamn: ");
            var lastname = Console.ReadLine();
            Console.WriteLine("3/4, Ange en e-post: ");
            var email = Console.ReadLine();
            Console.WriteLine("4/4, Ange ett telefonnummer: ");
            var phone = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (connection)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@" INSERT INTO Users (FirstName, LastName, Email, Phone)
                                                   VALUES(@FirstName, @LastName, @Email, @Phone);", connection);
                var writer = cmd.Parameters;

                writer.AddWithValue("@FirstName", firstname);
                writer.AddWithValue("@LastName", lastname);
                writer.AddWithValue("@Email", email);
                writer.AddWithValue("@Phone", phone);



                int rows = cmd.ExecuteNonQuery();

                Console.WriteLine($"{rows} st användare lades till ");
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
