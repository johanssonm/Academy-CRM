using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        }


        //  Skapa en ny kund
        //  Ändra en kund
        //  Ta bort en kund
        //  Hämta alla kunder
        static void PrintAllUsers()
        {
            Console.WriteLine();

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

        static void ModifyUser()

        {
            Console.WriteLine();
            Console.WriteLine("Vilken användare vill du ändra? Ange ID :");
            var id = Console.ReadLine();
            Console.WriteLine();

            PrintAllUsers();

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
                SqlCommand cmd = new SqlCommand(@" Update Users
                                                   SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Phone=@Phone
                                                   WHERE Id=@Id", connection);
                var writer = cmd.Parameters;

                writer.AddWithValue("@Id", id);
                writer.AddWithValue("@FirstName", firstname);
                writer.AddWithValue("@LastName", lastname);
                writer.AddWithValue("@Email", email);
                writer.AddWithValue("@Phone", phone);



                int rows = cmd.ExecuteNonQuery();

                Console.WriteLine($"{rows} st användare uppdaterades. ");
                Console.ReadKey();

            }

        }

        static void RemoveUser()

        {
            Console.WriteLine();
            Console.WriteLine("Vilken användare vill du radera? Ange ID :");
            Console.WriteLine();

            PrintAllUsers();

            Console.WriteLine();

            var id = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (connection)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@" DELETE FROM Users
                                                   WHERE Id=@Id", connection);
                var writer = cmd.Parameters;

                writer.AddWithValue("@Id", id);

                int rows = cmd.ExecuteNonQuery();

                Console.WriteLine($"{rows} st användare uppdaterades. ");

            }

        }




        static void MainMenu()
        {

            while (true)

            {

                Console.WriteLine();
                Console.WriteLine("Huvudmeny: ");
                Console.WriteLine("---");
                Console.WriteLine("1. Lägg till en ny användare");
                Console.WriteLine("2. Ändra befintlig användare");
                Console.WriteLine("3. Radera befintlig användare");
                Console.WriteLine("4. Lista alla användare");
                Console.WriteLine("---");
                Console.WriteLine("0. för att avsluta");

                try
                {

                    var menuChoice = Convert.ToInt32(Console.ReadLine());

                    if (menuChoice == 1)
                    {
                        AddNewUser();

                        Console.WriteLine();
                        continue;
                    }

                    if (menuChoice == 2)
                    {
                        RemoveUser();

                        Console.WriteLine();
                        continue;

                    }

                    if (menuChoice == 3)
                    {
                        ModifyUser();

                        Console.WriteLine();
                        continue;
                    }

                    if (menuChoice == 4)
                    {
                        PrintAllUsers();

                        Console.WriteLine();
                        continue;


                    }

                    if (menuChoice == 0)
                    {
                        break;
                    }

                    else
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Var vänlig välj något i menyn... ");
                        Console.ResetColor();
                        Console.WriteLine();
                    }

                }

                catch (SystemException)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Var vänlig välj något i menyn... ");
                    Console.ResetColor();
                    Console.WriteLine();

                }

            }
            Console.WriteLine();
            Console.WriteLine("Tack och hej!");
            Console.WriteLine();
            Console.ReadKey();


        }
    }
}
