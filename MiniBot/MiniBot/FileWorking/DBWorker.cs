using MiniBot.Exceptions;
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
    class DBWorker : IDBProduct
    {
        private string _connectionString;
        private static int _nextId = 0;
        private char _separator = '|';

        public DBWorker(string path)
        {
            _connectionString = path;
        }

        public void AddToDB(Product product)
        {
            string table = "", fields = "@Id, @Cost, @Name, @Description, @Score, @Discount, ";
            switch (product)
            {
                case Pizza:
                    table = PizzaTable;
                    fields += "@Ingredients, @Weight, @Size";
                    break;
                case Sushi:
                    table = SushiTable;
                    fields += "@Ingredients, @Weight, @IsRaw";
                    break;
                case Drink:
                    table = DrinkTable;
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
                    SqlParameter ParamCost = new SqlParameter(fieldsArray[1], product.Cost);
                    command.Parameters.Add(ParamCost);
                    SqlParameter ParamName = new SqlParameter(fieldsArray[2], product.Name);
                    command.Parameters.Add(ParamName);
                    SqlParameter ParamDescription = new SqlParameter(fieldsArray[3], product.Description);
                    command.Parameters.Add(ParamDescription);
                    SqlParameter ParamScore = new SqlParameter(fieldsArray[4], product.Score);
                    command.Parameters.Add(ParamScore);
                    SqlParameter ParamDiscount = new SqlParameter(fieldsArray[5], product.Discount);
                    command.Parameters.Add(ParamDiscount);

                    SqlParameter ParamFirst = null, ParamSecond = null, ParamThird = null;
                    StringBuilder ingredients = new StringBuilder();
                    
                    switch (product)
                    {
                        case Pizza:
                            foreach (var item in (product as Pizza).Ingredients)
                            {
                                ingredients.Append(item);
                                ingredients.Append(_separator);
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
                                ingredients.Append(_separator);
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
                        throw new DBWorkerException("Null additional values");
                    }

                    command.Parameters.Add(ParamFirst);
                    command.Parameters.Add(ParamSecond);
                    command.Parameters.Add(ParamThird);

                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Product> GetFromDB(Func<Product, bool> predicate)
        {
            List<Product> answerList = new List<Product>();
            Product product = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sqlExpression;

                foreach (var table in ProductTables)
                {
                    sqlExpression = $"SELECT * FROM {table}";

                    using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                switch (table)
                                {
                                    case PizzaTable:
                                        product = new Pizza((int)reader["Id"], (string)reader["Name"], (float)reader["Cost"],
                                            (byte)reader["Score"], (string)reader["Description"],
                                            ((string)reader["Ingredients"]).Split(_separator), (short)reader["Weight"], (byte)reader["Size"]);
                                        break;
                                    case SushiTable:
                                        product = new Sushi((int)reader["Id"], (string)reader["Name"], (float)reader["Cost"],
                                            (byte)reader["Score"], (string)reader["Description"],
                                            ((string)reader["Ingredients"]).Split(_separator), (short)reader["Weight"], (bool)reader["IsRaw"]);
                                        break;
                                    case DrinkTable:
                                        product = new Drink((int)reader["Id"], (string)reader["Name"], (float)reader["Cost"],
                                            (byte)reader["Score"], (string)reader["Description"],
                                            (float)reader["Volume"], (bool)reader["HasGase"], (bool)reader["IsAlcohol"]);
                                        break;
                                }

                                if (product == null)
                                {
                                    throw new DBWorkerException("Null product");
                                }

                                product.Discount = (byte)reader["Discount"];

                                if (predicate(product))
                                {
                                    answerList.Add(product);
                                }
                            }
                        }

                        reader.Close();
                    }
                }
            }

            return answerList;
        }

        public Product GetById(int id)
        {
            Product result = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (var table in ProductTables)
                {
                    string sqlExpression = $"SELECT * FROM {table} WHERE Id={id}";
                    using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows && reader.Read())
                        {
                            switch (table)
                            {
                                case PizzaTable:
                                    result = new Pizza((int)reader["Id"], (string)reader["Name"], (float)reader["Cost"],
                                        (byte)reader["Score"], (string)reader["Description"],
                                        ((string)reader["Ingredients"]).Split(_separator), (short)reader["Weight"], (byte)reader["Size"]);
                                    break;
                                case SushiTable:
                                    result = new Sushi((int)reader["Id"], (string)reader["Name"], (float)reader["Cost"],
                                        (byte)reader["Score"], (string)reader["Description"],
                                        ((string)reader["Ingredients"]).Split(_separator), (short)reader["Weight"], (bool)reader["IsRaw"]);
                                    break;
                                case DrinkTable:
                                    result = new Drink((int)reader["Id"], (string)reader["Name"], (float)reader["Cost"],
                                        (byte)reader["Score"], (string)reader["Description"],
                                        (float)reader["Volume"], (bool)reader["HasGase"], (bool)reader["IsAlcohol"]);
                                    break;
                            }

                            if (result == null)
                            {
                                throw new DBWorkerException("Not found object");
                            }

                            result.Discount = (byte)reader["Discount"];
                        }

                        reader.Close();
                    }
                }
            }

            return result;
        }

        public void RemoveFromDB(Func<Product, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
