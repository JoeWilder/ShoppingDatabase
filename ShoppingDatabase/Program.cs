using System;
using MySql.Data.MySqlClient;
using ShoppingDatabase;

namespace CSharpToMySQL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //test
            string server = "localhost"; // If not using a cloud database, use localhost. If using cloud database, use the IP to the server
            string database = "ShoppingDatabase"; // Whatever database you want to use for the application
            string username = "root"; // The username of your MySQL client
            string password = Environment.GetEnvironmentVariable("SNHU_PASS"); // The password of your MySQL client

            // Throw all the variables into this connection string
            // MUST USE THIS FORMAT! (server, database, username, then password)
            string connstring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

            string userSelectionString;
            int userSelection = 0;

            string userName;
            string userPassword;

            // Initializes the connection object called "conn", using the connection string
            MySqlConnection conn = new MySqlConnection(connstring);


            while (userSelection != 3)
            {
                Console.WriteLine("Choose an option...");
                Console.WriteLine("1) Sign in");
                Console.WriteLine("2) Sign up");
                Console.WriteLine("3) Exit from interface");

                userSelectionString = Console.ReadLine();
                userSelection = Convert.ToInt32(userSelectionString);

                switch (userSelection)
                {
                    case 1:
                        Console.WriteLine("Username:");
                        userName = Console.ReadLine();
                        Console.WriteLine("Password:");
                        userPassword = Console.ReadLine();

                        if (MySQLFunctions.doesUserExist(conn, userName, userPassword))
                        {
                            userSelection = 3;
                            Console.WriteLine("User does exist, start application here");
                            startShoppingInterface(conn, userName);
                        }
                        else
                        {
                            Console.WriteLine("This user does not exist");
                        }
                        break;
                    case 2:
                        Console.WriteLine("Username:");
                        userName = Console.ReadLine();
                        Console.WriteLine("Password:");
                        userPassword = Console.ReadLine();

                        if (MySQLFunctions.insertNewUser(conn, userName, userPassword))
                        {
                            Console.WriteLine("Created a new account");
                        }
                        else
                        {
                            Console.WriteLine("This user already exists");
                        }
                        break;
                    case 3:

                        break;
                    default:
                        Console.WriteLine("Invalid Selection. Choose again.");
                        break;
                }
            }
        }


        


        


        


        // Start the main application
        static void startShoppingInterface(MySqlConnection conn, string userName)
        {


            int currentInvoiceId = MySQLFunctions.getLatestInvoice(conn) + 1;
            string userSelectionString;
            int userSelection = -1;

            


            while (userSelection != 3)
            {


                Console.WriteLine("Choose an option...");
                Console.WriteLine("1) View products");
                Console.WriteLine("2) Add product to order");
                Console.WriteLine("3) View current order");

                userSelectionString = Console.ReadLine();
                userSelection = Convert.ToInt32(userSelectionString);

                

                switch (userSelection)
                {
                    case 1: // Print the product table
                        MySQLFunctions.printProductTable(conn);
                        break;

                    case 2: // Add a new invoice, subtract products from the product table

                        int productId; // Product id to buy
                        int quantity; // Amount of product to buy
                        int totalQuantity;
                        int userId = MySQLFunctions.getCurrentUserId(conn, userName);
                        Console.WriteLine("Input product ID to buy: ");
                        productId = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Input amount to buy: ");
                        quantity = Convert.ToInt32(Console.ReadLine());

                        totalQuantity = MySQLFunctions.getNumberOfProductsInStock(conn, productId);


                        int remainingQuantity = totalQuantity - quantity;
                        int price = MySQLFunctions.getProductPrice(conn, productId);
                        int totalPrice = price * quantity;


                        MySQLFunctions.updateProductQuantity(conn, remainingQuantity, productId);
                        MySQLFunctions.insertNewInvoice(conn, currentInvoiceId, userId, productId, quantity, totalPrice);


                        break;

                    case 3: // Print the current order



                        MySQLFunctions.printInvoice(conn, currentInvoiceId);


                        break;

                    default:
                        Console.WriteLine("Invalid Selection. Choose again.");
                        break;
                }






                

            }
        }
    }
}