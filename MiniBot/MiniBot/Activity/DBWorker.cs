using MiniBot.Interfaces;
using MiniBot.Products;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniBot.Activity.Sources;

namespace MiniBot.Activity
{
    class DBWorker : IDBWork
    {
        private string _connectionString;
        private static int _nextId = 0;

        public DBWorker(string path)
        {
            _connectionString = path;
        }

        public void AddToDB(object product)
        {
            string table = "", fields = "@Id, @Cost, @Name, @Description, @Score, @Discount, ";
            switch (product)
            {
                case Pizza:
                    table = "PizzaTable";
                    fields += "@Ingredients, @Weight, @Size";
                    break;
                case Sushi:
                    table = "SushiTable";
                    fields += "@Ingredients, @Weight, @IsRaw";
                    break;
                case Drink:
                    table = "DrinkTable";
                    fields += "@Volume, @HasGase, @IsAlcohol";
                    break;
                default:
                    break;
            }
            
            string sqlExpression = $"INSERT INTO {table} ({fields.Replace("@", "")}) VALUES ({fields})";
            fields = fields.Replace(",", "");
            string[] fieldsArray = fields.Split(' ');

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    SqlParameter ParamId = new SqlParameter(fieldsArray[0], _nextId++);
                    command.Parameters.Add(ParamId);
                    SqlParameter ParamCost = new SqlParameter(fieldsArray[1], (product as Product).Cost);
                    command.Parameters.Add(ParamCost);
                    SqlParameter ParamName = new SqlParameter(fieldsArray[2], (product as Product).Name);
                    command.Parameters.Add(ParamName);
                    SqlParameter ParamDescription = new SqlParameter(fieldsArray[3], (product as Product).Description);
                    command.Parameters.Add(ParamDescription);
                    SqlParameter ParamScore = new SqlParameter(fieldsArray[4], (product as Product).Score);
                    command.Parameters.Add(ParamScore);
                    SqlParameter ParamDiscount = new SqlParameter(fieldsArray[5], (product as Product).Discount);
                    command.Parameters.Add(ParamDiscount);

                    SqlParameter ParamFirst = null, ParamSecond = null, ParamThird = null;
                    StringBuilder ingredients = new StringBuilder();
                    
                    switch (product)
                    {
                        case Pizza:
                            foreach (var item in (product as Pizza).Ingredients)
                            {
                                ingredients.Append(item);
                                ingredients.Append("|");
                            }
                            ingredients = ingredients.Remove(ingredients.Length - 1, 1);
                            ParamFirst = new SqlParameter(fieldsArray[6], ingredients.ToString());
                            ParamSecond = new SqlParameter(fieldsArray[7], (product as Pizza).Weight);
                            ParamThird = new SqlParameter(fieldsArray[8], (product as Pizza).Size);
                            break;
                        case Sushi:
                            foreach (var item in (product as Sushi).Ingredients)
                            {
                                ingredients.Append(item);
                                ingredients.Append("|");
                            }
                            ingredients = ingredients.Remove(ingredients.Length - 1, 1);
                            ParamFirst = new SqlParameter(fieldsArray[6], ingredients.ToString());
                            ParamSecond = new SqlParameter(fieldsArray[7], (product as Sushi).Weight);
                            ParamThird = new SqlParameter(fieldsArray[8], (product as Sushi).IsRaw);
                            break;
                        case Drink:
                            ParamFirst = new SqlParameter(fieldsArray[6], (product as Drink).Volume);
                            ParamSecond = new SqlParameter(fieldsArray[7], (product as Drink).HasGase);
                            ParamThird = new SqlParameter(fieldsArray[8], (product as Drink).IsAlcohol);
                            break;
                        default:
                            break;
                    }

                    if (ParamFirst == null || ParamSecond == null || ParamThird == null) 
                    {
                        throw new NullReferenceException();
                    }

                    command.Parameters.Add(ParamFirst);
                    command.Parameters.Add(ParamSecond);
                    command.Parameters.Add(ParamThird);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void GetFromDB(Action<Product, short> action, ProductType producttype)
        {
            string table = "";
            switch (producttype)
            {
                case ProductType.Pizza:
                    table = "PizzaTable";
                    break;
                case ProductType.Sushi:
                    table = "SushiTable";
                    break;
                case ProductType.Drink:
                    table = "DrinkTable";
                    break;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlExpression = $"SELECT * FROM {table}";
                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            switch (producttype)
                            {
                                case ProductType.Pizza:
                                    Pizza pizza = new Pizza((string)reader["Name"], (float)reader["Cost"], 
                                        (byte)reader["Score"], (string)reader["Description"],
                                        ((string)reader["Ingredients"]).Split('|'), (short)reader["Weight"], (byte)reader["Size"]);
                                    pizza.Discount = (byte)reader["Discount"];
                                    action(pizza, (short)(int)reader["Id"]);
                                    break;
                                case ProductType.Sushi:
                                    Sushi sushi = new Sushi((string)reader["Name"], (float)reader["Cost"],
                                        (byte)reader["Score"], (string)reader["Description"],
                                        ((string)reader["Ingredients"]).Split('|'), (short)reader["Weight"], (bool)reader["IsRaw"]);
                                    sushi.Discount = (byte)reader["Discount"];
                                    action(sushi, (short)(int)reader["Id"]);
                                    break;
                                case ProductType.Drink:
                                    Drink drink = new Drink((string)reader["Name"], (float)reader["Cost"], 
                                        (byte)reader["Score"], (string)reader["Description"],
                                        (float)reader["Volume"], (bool)reader["HasGase"], (bool)reader["IsAlcohol"]);
                                    drink.Discount = (byte)reader["Discount"];
                                    action(drink, (short)(int)reader["Id"]);
                                    break;
                            }
                        }
                    }

                    reader.Close();
                }
            }

            GC.Collect();
        }

        public object GetById(ProductType producttype, short id)
        {
            Product result = null;

            string table = "";
            switch (producttype)
            {
                case ProductType.Pizza:
                    table = "PizzaTable";
                    break;
                case ProductType.Sushi:
                    table = "SushiTable";
                    break;
                case ProductType.Drink:
                    table = "DrinkTable";
                    break;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sqlExpression = $"SELECT * FROM {table} WHERE Id={id}";
                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows && reader.Read())
                    {
                        switch (producttype)
                        {
                            case ProductType.Pizza:
                                result = new Pizza((string)reader["Name"], (float)reader["Cost"],
                                    (byte)reader["Score"], (string)reader["Description"],
                                    ((string)reader["Ingredients"]).Split('|'), (short)reader["Weight"], (byte)reader["Size"]);
                                break;
                            case ProductType.Sushi:
                                result = new Sushi((string)reader["Name"], (float)reader["Cost"],
                                    (byte)reader["Score"], (string)reader["Description"],
                                    ((string)reader["Ingredients"]).Split('|'), (short)reader["Weight"], (bool)reader["IsRaw"]);
                                break;
                            case ProductType.Drink:
                                result = new Drink((string)reader["Name"], (float)reader["Cost"],
                                    (byte)reader["Score"], (string)reader["Description"],
                                    (float)reader["Volume"], (bool)reader["HasGase"], (bool)reader["IsAlcohol"]);
                                break;
                        }
                        result.Discount = (byte)reader["Discount"];
                    }
                }
            }

            return result;
        }

        public void RemoveFromDB(object product)
        {
            
        }
    }
}
