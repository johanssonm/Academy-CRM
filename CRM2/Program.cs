using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CRM
{
	class Customer
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public int ID { get; set; }


        public Customer()
		{
			
		}

		public Customer(string firstname, string lastName, string email)
		{
			FirstName = firstname;
			LastName = lastName;
			Email = email;
		    ID = Math.Abs(email.GetHashCode() * 2 + 15);
		}

	}

	class Program
	{
		private static string connectionString = @"Server = (localdb)\mssqllocaldb; Database = AcademyCRM2; Trusted_Connection = True";
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

					SqlCommand cmd = new SqlCommand(@" SELECT Users.ID,FirstName,LastName,Email,

														STUFF((
														SELECT cast(', ' as VARCHAR(MAX)) + UserPhones.Number
														FROM UserPhones
														WHERE Users.ID = UserPhones.UserID
														FOR xml path('')), 1, 1, '') AS Phone,

														Created,Updated 
														FROM Users;", connection);

					var reader = cmd.ExecuteReader();

					Console.WriteLine();

					using (DataTable dt = new DataTable())
					{
						dt.TableName = "Users";
						dt.Columns.Add("ID", typeof(string));
						dt.Columns.Add("Förnamn", typeof(string));
						dt.Columns.Add("Efternamn", typeof(string));
						dt.Columns.Add("E-post", typeof(string));
						dt.Columns.Add("Telefon", typeof(string));
						dt.Columns.Add("Skapad", typeof(DateTime));
						dt.Columns.Add("Ändrad", typeof(DateTime));

						while (reader.Read())
						{
							dt.Rows.Add(reader.GetString(0),
										reader.GetString(1),
										reader.GetString(2),
										reader.GetString(3),
										reader.GetString(4), 
										reader.GetDateTime(5),
										reader.GetDateTime(6));

						}


						Console.WriteLine($"Antal rader i tabellen: " + dt.Rows.Count);
					}


				}


			}
		}

		static void AddNewUser()

		{

			Console.WriteLine();
			Console.Write("1/4, Ange ett förnamn: ");
			var firstName = Console.ReadLine();

			Console.Write("2/4, Ange ett efternamn: ");
			var lastName = Console.ReadLine();

			Console.Write("3/4, Ange en e-post: ");
			var email = Console.ReadLine();

		    var customer = new Customer(firstName, lastName, email);

            using (SqlConnection connection = new SqlConnection(connectionString))
		    using (connection)
		    {
		        connection.Open();
		        SqlCommand userQuery = new SqlCommand(@" INSERT INTO Users (ID, FirstName, LastName, Email, Created, Updated)
													     VALUES(@ID, @FirstName, @LastName, @Email, @Created, @Updated);", connection);

           

		        var writer = userQuery.Parameters;
		        writer.AddWithValue("@ID", customer.ID);
		        writer.AddWithValue("@FirstName", customer.FirstName);
		        writer.AddWithValue("@LastName", customer.LastName);
		        writer.AddWithValue("@Email", customer.Email);
		        writer.AddWithValue("@Created", DateTime.Now);
		        writer.AddWithValue("@Updated", DateTime.Now);

                int rows = userQuery.ExecuteNonQuery();
		        Console.WriteLine();
		        Console.ForegroundColor = ConsoleColor.Green;
		        Console.WriteLine($"{rows} st användare lades till med ID {customer.ID.ToString()}.");
                Console.ResetColor();
		    }


            while (true)
		    {

                Console.Write("4/4, Vill du ange ett telefonnummer?  Ja : Nej ");

		        var hasmoorenumbers = Console.ReadLine();

				if (hasmoorenumbers.ToLower() == "ja")
				{
					Console.Write("Ange ett telefonnummer: ");

					customer.Phone = Console.ReadLine();

				    using (SqlConnection connection = new SqlConnection(connectionString))
				    using (connection)
				    {
				        connection.Open();
				        SqlCommand userQuery = new SqlCommand(@" INSERT INTO UserPhones (UserID, Number)
															     VALUES(@UserID, @Number);", connection);

				        var writer = userQuery.Parameters;
				        var id = customer.ID;

                        writer.AddWithValue("@Number", Convert.ToInt32(customer.Phone));
				        writer.AddWithValue("@UserID", id.GetHashCode());

				        int rows = userQuery.ExecuteNonQuery();

				        Console.WriteLine($"{rows} st telefonnummer lades till ");

                        connection.Close();
				    }
                    continue;



                }

				if (hasmoorenumbers == "nej")
				{
					break;

				}

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
