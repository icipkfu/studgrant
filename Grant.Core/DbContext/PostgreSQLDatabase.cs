using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Grant.Core.DbContext
{
    /// <summary>
    /// Class that encapsulates PostgreSQL database connections and CRUD operations.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class PostgreSQLDatabase : System.Data.Entity.DbContext
    {
        private NpgsqlConnection _connection;

        
        /// <summary>
        /// Default constructor which uses the "DefaultConnection" connectionString, often located in web.config.
        /// </summary>
        public PostgreSQLDatabase() : base("DefaultConnection")
        {
            ConfigureContext();
        }

        /// <summary>
        /// Constructor which takes the connection string name.
        /// </summary>
        /// <param name="connectionStringName">The name of the connection string.</param>
        public PostgreSQLDatabase(string connectionStringName)
        {
            Init(connectionStringName);
        }

        /// <summary>
        /// Executes a non-query PostgreSQL statement.
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Optional parameters to pass to the query.</param>
        /// <returns>The count of records affected by the PostgreSQL statement.</returns>
        public int Execute(string commandText, Dictionary<string, object> parameters)
        {
            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            int result;
            try
            {
                OpenConnection();
                var command = CreateCommand(commandText, parameters);
                result = command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Executes a PostgreSQL query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Optional parameters to pass to the query.</param>
        /// <returns></returns>
        public object QueryValue(string commandText, Dictionary<string, object> parameters)
        {
            object result;

            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                OpenConnection();
                var command = CreateCommand(commandText, parameters);
                result = command.ExecuteScalar();
            }
            finally
            {
                CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Parameters to pass to the PostgreSQL query.</param>
        /// <returns>A list of a Dictionary of Key, values pairs representing the ColumnName and corresponding value.</returns>
        public List<Dictionary<string, string>> Query(string commandText, Dictionary<string, object> parameters)
        {
            List<Dictionary<string, string>> rows = null;
            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                OpenConnection();
                var command = CreateCommand(commandText, parameters);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    rows = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                            row.Add(columnName, columnValue);
                        }
                        rows.Add(row);
                    }
                }
            }
            catch (Exception)
            {
                //TODO: Обработка исключения
                //var msg = ex.Message;
                throw;
            }
            finally
            {
                CloseConnection();
            }

            return rows;
        }

        /// <summary>
        /// Creates a NpgsqlCommand object with the given parameters.
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Parameters to pass to the PostgreSQL query.</param>
        /// <returns></returns>
        private NpgsqlCommand CreateCommand(string commandText, Dictionary<string, object> parameters)
        {
            NpgsqlCommand command = _connection.CreateCommand();
            command.CommandText = commandText;
            AddParameters(command, parameters);

            return command;
        }

        /// <summary>
        /// Adds the parameters to a PostgreSQL command.
        /// </summary>
        /// <param name="command">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Parameters to pass to the PostgreSQL query.</param>
        private static void AddParameters(NpgsqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var param in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = param.Key;
                parameter.Value = param.Value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Helper method to return query a string value. 
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Parameters to pass to the PostgreSQL query.</param>
        /// <returns>The string value resulting from the query.</returns>
        public string GetStrValue(string commandText, Dictionary<string, object> parameters)
        {
            string value = QueryValue(commandText, parameters) as string;
            return value;
        }

        private void Init(string connectionStringName = "DefaultConnection")
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _connection = new NpgsqlConnection(connectionString);
            ConfigureContext();
        }

        /// <summary>
        /// Opens a connection if not open.
        /// </summary>
        private void OpenConnection()
        {
            if (_connection == null)
            {
                Init();
            }

            if (_connection == null)
            {
                throw new Exception("Cannot init connection");
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        /// <summary>
        /// Closes the connection if it is open.
        /// </summary>
        public void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        private void ConfigureContext()
        {
            Database.CommandTimeout = 1200;

            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;

            Configuration.ValidateOnSaveEnabled = false;
            
        }

        //TODO Нужно что-то с этим делать
        //public void Dispose()
        //{
        //    if (_connection != null)
        //    {
        //        _connection.Dispose();
        //        _connection = null;
        //    }
        //}
    }
}
