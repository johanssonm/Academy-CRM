﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using ConsoleTableExt;

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
                    SqlCommand cmd = new SqlCommand (@"SELECT * FROM Users;",connection);

                    var reader = cmd.ExecuteReader();

                    Console.WriteLine();

                    using (DataTable dt = new DataTable())
                    {
                        dt.TableName = "users";
                        dt.Columns.Add("ID", typeof(string));
                        dt.Columns.Add("Förnamn", typeof(string));
                        dt.Columns.Add("Efternamn", typeof(string));
                        dt.Columns.Add("E-post", typeof(string));
                        dt.Columns.Add("Telefon", typeof(string));
                        dt.Columns.Add("Skapad", typeof(DateTime));
                        dt.Columns.Add("Ändrad", typeof(DateTime));

                        while (reader.Read())
                        {
                        //Console.WriteLine("* {0} \t\t{1} {2} \t\t{3} \t\t{4}",
                        //                    reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        dt.Rows.Add(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetDateTime(5), reader.GetDateTime(6));

                        }

                        ConsoleTableBuilder
                            .From(dt)
                            .WithFormat(ConsoleTableBuilderFormat.Minimal)
                            .ExportAndWriteLine();

                        Console.WriteLine($"Antal rader i tabellen: " + dt.Rows.Count);
                    }


                }

            
            }
        }

        static void AddNewUser()

        {
            Console.WriteLine();
            Console.Write("1/4, Ange ett förnamn: ");
            var firstname = Console.ReadLine();
            Console.Write("2/4, Ange ett efternamn: ");
            var lastname = Console.ReadLine();
            Console.Write("3/4, Ange en e-post: ");
            var email = Console.ReadLine();
            Console.Write("4/4, Ange ett telefonnummer: ");
            var phone = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (connection)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@" INSERT INTO Users (FirstName, LastName, Email, Phone)
                                                   VALUES(@FirstName, @LastName, @Email, @Phone, @Created);", connection);
                var writer = cmd.Parameters;

                writer.AddWithValue("@FirstName", firstname);
                writer.AddWithValue("@LastName", lastname);
                writer.AddWithValue("@Email", email);
                writer.AddWithValue("@Phone", phone);
                writer.AddWithValue("@Created", DateTime.Now);



                int rows = cmd.ExecuteNonQuery();

                Console.WriteLine($"{rows} st användare lades till ");
                Console.ReadKey();
               
            }

        }

        static void ModifyUser()

        {
            PrintAllUsers();

            Console.WriteLine("Vilken användare vill du ändra? Ange ID :");
            var id = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine();

            Console.Write("1/4, Ange ett förnamn: ");
            var firstname = Console.ReadLine();
            Console.Write("2/4, Ange ett efternamn: ");
            var lastname = Console.ReadLine();
            Console.Write("3/4, Ange en e-post: ");
            var email = Console.ReadLine();
            Console.Write("4/4, Ange ett telefonnummer: ");
            var phone = Console.ReadLine();


            using (SqlConnection connection = new SqlConnection(connectionString))
            using (connection)
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@" Update Users
                                                   SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Phone=@Phone, Updated=@Updated
                                                   WHERE Id=@Id", connection);
                var writer = cmd.Parameters;

                writer.AddWithValue("@Id", id);
                writer.AddWithValue("@FirstName", firstname);
                writer.AddWithValue("@LastName", lastname);
                writer.AddWithValue("@Email", email);
                writer.AddWithValue("@Phone", phone);
                writer.AddWithValue("@Updated", DateTime.Now);



                int rows = cmd.ExecuteNonQuery();

                Console.WriteLine($"{rows} st användare uppdaterades. ");
                Console.ReadKey();

            }

        }

        static void RemoveUser()

        {
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

                Console.WriteLine($"{rows} st användare med ID {id} raderades. ");

            }

        }




        static void MainMenu()
        {

            while (true)

            {

                Console.WriteLine();
                Console.WriteLine("Huvudmeny: ");
                Console.WriteLine("---");
                Console.WriteLine("1. Ny användare");
                Console.WriteLine("2. Radera användare");
                Console.WriteLine("3. Ändra användare");
                Console.WriteLine("4. Lista alla användare");
                Console.WriteLine("---");
                Console.WriteLine("0. för att avsluta");

                try
                {

                    var menuChoice = Convert.ToInt32(Console.ReadLine());

                    if (menuChoice == 1)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("1. Ny användare");
                        Console.ResetColor();

                        AddNewUser();

                        Console.Clear();
                        continue;


                    }

                    if (menuChoice == 2)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("2. Radera användare");
                        Console.ResetColor();
                        Console.WriteLine();
                        Console.Write("Vilken användare vill du radera? Ange ID :");

                        RemoveUser();

                        Console.WriteLine();
                        Console.Clear();
                        continue;

                    }

                    if (menuChoice == 3)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("3. Ändra användare");
                        Console.ResetColor();
                        ModifyUser();
                        Console.Clear();
                        continue;


                    }

                    if (menuChoice == 4)
                    {
                        PrintAllUsers();
                        Console.Write("Tryck på en tangent för att avsluta...");
                        Console.ReadKey();
                        Console.Clear();
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
                    continue;

                }

            }
            Console.WriteLine();
            Console.WriteLine("Tack och hej!");
            Console.WriteLine();
            Console.ReadKey();


        }
    }
}
