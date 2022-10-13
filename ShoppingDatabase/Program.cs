using System;
using MySql.Data.MySqlClient;
using ShoppingDatabase;

namespace CSharpToMySQL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string server = "localhost"; // server ip
            string database = "ShoppingDatabase"; // database name
            string username = "root"; // MySQL username
            string password = Environment.GetEnvironmentVariable("SNHU_PASS"); // MySQL password
            string connstring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";"; // connection string for connection object

            int userSelection = 0; // option selection
            string userName; // application username
            string userPassword; // application password

            MySqlConnection conn = new MySqlConnection(connstring); // MySQL connection object


            while (userSelection != 3) // login prompt loop
            {
                Console.WriteLine("Choose an option...");
                Console.WriteLine("1) Sign in");
                Console.WriteLine("2) Sign up");
                Console.WriteLine("3) Exit from interface");
                int.TryParse(Console.ReadLine(), out userSelection); // user select integer validation
                switch (userSelection)
                {
                    case 1: // Attempt to login
                        Console.WriteLine("Username:");
                        userName = Console.ReadLine();
                        Console.WriteLine("Password:");
                        userPassword = Console.ReadLine();
                        if (MySQLFunctions.doesUserExist(conn, userName, userPassword))
                        {
                            userSelection = 3;
                            Console.WriteLine("Successfully logged in\n");
                            startShoppingInterface(conn, userName);
                        }
                        else
                        {
                            Console.WriteLine("This user does not exist");
                        }
                        break;

                    case 2: // Attempt to sign up
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
                            Console.WriteLine("Failed to insert user");
                        }
                        break;

                    case 3: // Break and end program
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
            int currentInvoiceId = MySQLFunctions.getLatestInvoice(conn) + 1; // create new invoice id
            int userSelection = -1; // user selection

            while (userSelection != 4) // main application loop
            {
                Console.WriteLine("Choose an option...");
                Console.WriteLine("1) View products");
                Console.WriteLine("2) Add product to order");
                Console.WriteLine("3) View current order");
                Console.WriteLine("4) Exit from program");

                if (!int.TryParse(Console.ReadLine(), out userSelection))
                {
                    Console.WriteLine("That wasn't a number! Please do a number!");
                }


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
                        if (!int.TryParse(Console.ReadLine(), out productId)) // integer validation
                        {
                            Console.WriteLine("That wasn't a number! Please do a number!");
                            break;
                        }

                        Console.WriteLine("Input amount to buy: ");

                        if (!int.TryParse(Console.ReadLine(), out quantity)) // integer validation
                        {
                            Console.WriteLine("That wasn't a number! Please do a number!");
                            break;
                        }

                        if (!MySQLFunctions.doesProductExist(conn, productId)) // product validation
                        {
                            Console.WriteLine("This product does not exist!\n");
                            break;
                        }

                        totalQuantity = MySQLFunctions.getNumberOfProductsInStock(conn, productId); // get amount of product in stock
                        int remainingQuantity = totalQuantity - quantity; // calculate what is left after purchase
                        if (quantity > totalQuantity) // make sure we have enough product in stock
                        {
                            Console.WriteLine("Sorry, we don't enough of that product!\n");
                            break;
                        }

                        int price = MySQLFunctions.getProductPrice(conn, productId); // get product base price
                        int totalPrice = price * quantity; // calculate total price of product purchase
                        MySQLFunctions.updateProductQuantity(conn, remainingQuantity, productId); // decrease product quantity to remaining quantity
                        MySQLFunctions.insertNewInvoice(conn, currentInvoiceId, userId, productId, quantity, totalPrice); // create a new invoice
                        Console.WriteLine("Added new item to order\n");
                        break;

                    case 3: // Print the current order
                        MySQLFunctions.printInvoice(conn, currentInvoiceId);
                        break;

                    case 4:
                        Console.WriteLine("Exiting the program");
                        break;

                    default:
                        Console.WriteLine("Invalid Selection. Choose again.");
                        break;
                }
            }
        }
    }
}
