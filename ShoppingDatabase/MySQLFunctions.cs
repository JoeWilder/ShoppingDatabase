﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingDatabase
{
    class MySQLFunctions
    {

        static MySqlDataReader dataReader;
        static MySqlCommand cmd;
        static string query;

        /* Check if the given username is in the database */
        public static bool doesUsernameExist(MySqlConnection connection, string username)
        {
            connection.Open();

            cmd = new MySqlCommand("select userId from users where users.userName = @user;", connection);
            cmd.Parameters.AddWithValue("@user", username);
            var nId = cmd.ExecuteScalar();

            if (nId != null)
            {
                connection.Close();
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }


        /* Put a new user into the database if they do not exist already */
        public static bool insertNewUser(MySqlConnection connection, string username, string password)
        {
            


            if (username.Length > 20)
            {
                Console.WriteLine("The username is too long!");
                return false;
            }

            if (password.Length > 40)
            {
                Console.WriteLine("The password is too long!");
                return false;
            }

            if (doesUsernameExist(connection, username))
            {
                return false;
            }

            connection.Open();

            cmd = new MySqlCommand("INSERT INTO users(userName, password) VALUES (@userName, @password)", connection);
            cmd.Parameters.AddWithValue("@userName", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.ExecuteNonQuery();

            connection.Close();
            return true;
        }


        /* Check if a user exists in the database given their username and password */
        public static bool doesUserExist(MySqlConnection connection, string username, string userPassword)
        {
            connection.Open();
            cmd = new MySqlCommand("select userId from users where users.userName = @user and users.password = @password;", connection);
            cmd.Parameters.AddWithValue("@user", username);
            cmd.Parameters.AddWithValue("@password", userPassword);
            var nId = cmd.ExecuteScalar();
            if (nId != null)
            {
                connection.Close();
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }


        /* Return the newest invoice id */
        public static int getLatestInvoice(MySqlConnection connection)
        {
            int currentInvoiceId;
            connection.Open();
            query = "select MAX(invoiceId) from invoice;";
            cmd = new MySqlCommand(query, connection);
            dataReader = cmd.ExecuteReader();
            dataReader.Read();// Get first record.
            currentInvoiceId = dataReader.GetInt32(0);// Get value of first column as an integer.
            dataReader.Close();
            connection.Close();
            return currentInvoiceId;

        }


        /* Print product table */
        public static void printProductTable(MySqlConnection connection)
        {
            connection.Open();
            query = "SELECT * FROM products";
            cmd = new MySqlCommand(query, connection);
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                Console.WriteLine("Product ID: " + dataReader["productId"]);
                Console.WriteLine("Product name: " + dataReader["productName"]);
                Console.WriteLine("Product description: " + dataReader["description"]);
                Console.WriteLine("Units in stock: " + dataReader["unitsInStock"]);
                Console.WriteLine("Price: " + dataReader["price"] + "\n");
            }
            connection.Close();
        }


        /* Get total amount of products in stock */
        public static int getNumberOfProductsInStock(MySqlConnection conn, int productIdToBuy)
        {
            conn.Open();
            query = "SELECT unitsInStock FROM products where products.productID = @productId";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@productId", productIdToBuy);
            dataReader = cmd.ExecuteReader();
            dataReader.Read();// Get first record
            int totalQuantity = dataReader.GetInt32(0);// Get value of first column as integer
            dataReader.Close();
            conn.Close();
            return totalQuantity;
        }


        /* Change how many products are left */
        public static void updateProductQuantity(MySqlConnection conn, int newQuantity, int productIdToBuy)
        {
            conn.Open();
            query = "UPDATE products SET unitsInStock = @remainingStock WHERE productId = @productId;";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@remainingStock", newQuantity);
            cmd.Parameters.AddWithValue("@productId", productIdToBuy);
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        /* Get current users id */
        public static int getCurrentUserId(MySqlConnection conn, string userName)
        {
            conn.Open();
            query = "select userId from users where userName = @userName";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userName", userName);
            dataReader = cmd.ExecuteReader();
            dataReader.Read();// Get first record.
            int userId = dataReader.GetInt32(0);// Get value of first column as string.
            dataReader.Close();
            conn.Close();
            return userId;
        }


        /* Get price of a product */
        public static int getProductPrice (MySqlConnection conn, int productId)
        {
            conn.Open();
            query = "select price from products where productId = @productId;";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@productId", productId);
            dataReader = cmd.ExecuteReader();
            dataReader.Read();// Get first record.
            int userId = dataReader.GetInt32(0);// Get value of first column as string.
            dataReader.Close();
            conn.Close();
            return userId;
        }


        /* Insert new invoice into the the invoice table */
        public static void insertNewInvoice(MySqlConnection conn, int invoiceId, int userId, int productId, int quantity, int totalPrice)
        {
            conn.Open();
            query = "insert into invoice (invoiceId, userId, productId, quantity, price) values(@invoiceId, @userId, @productId, @quantity, @price)";
            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@quantity", quantity);
            cmd.Parameters.AddWithValue("@price", totalPrice);
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        /* Print product table */
        public static void printInvoice(MySqlConnection connection, int invoiceId)
        {
            connection.Open();
            query = "select * from invoice where invoiceId = @invoiceId;";
            cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            // We set the reader variable to cmd.ExecuteReader() so it can read the entries of the table it receives from first to last
            dataReader = cmd.ExecuteReader();

            // While there is data to read it will keep looping
            while (dataReader.Read())
            {
                Console.WriteLine("\nUser ID: " + dataReader["userId"]);
                Console.WriteLine("Product ID: " + dataReader["productId"]);
                Console.WriteLine("Quantity: " + dataReader["quantity"]);
                Console.WriteLine("Price: " + dataReader["price"]);
                
            }
            dataReader.Close();
            connection.Close();
            Console.WriteLine("Total invoice price: " + getTotalInvoicePrice(connection, invoiceId) + "\n");
        }


        /* Print product table */
        public static int getTotalInvoicePrice(MySqlConnection connection, int invoiceId)
        {

            connection.Open();
            query = "SELECT sum(i.price) as 'total_invoice_cost' FROM shoppingdatabase.invoice as i where i.invoiceId = @invoiceId group by i.invoiceId;";
            cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            dataReader = cmd.ExecuteReader();
            dataReader.Read();// Get first record.
            int totalInvoicePrice = dataReader.GetInt32(0);
            dataReader.Close();
            connection.Close();
            return totalInvoicePrice;
        }


        public static bool doesProductExist(MySqlConnection connection, int productId)
        {
            connection.Open();
            cmd = new MySqlCommand("select productId from products where products.productId = @productId;", connection);
            cmd.Parameters.AddWithValue("@productId", productId);
            var nId = cmd.ExecuteScalar();
            if (nId != null)
            {
                connection.Close();
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }

    }
}
