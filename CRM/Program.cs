using System;
using System.Data.SqlClient;

namespace CRM
{
    class Program
    {
        static void Main(string[] args)
        {
            MainMenu();


            //  Skapa en ny kund
            //  Ändra en kund
            //  Ta bort en kund
            //  Hämta alla kunder

            using (Sql)

                Console.ReadKey();



        }

        static void MainMenu()
        {

            Console.WriteLine("Huvudmeny: ");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

        }
    }
}
