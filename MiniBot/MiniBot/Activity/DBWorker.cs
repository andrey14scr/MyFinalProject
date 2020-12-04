using MiniBot.Interfaces;
using MiniBot.Products;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public object GetFromDB()
        {

            return null;
        }

        public void RemoveFromDB(object product)
        {
            
        }
    }
}
