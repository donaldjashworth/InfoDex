using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InfoDex.Models
{
    public class AccessDatabase
    {

        private OleDbConnection connection;
        private OleDbCommand command;
        private OleDbTransaction transaction;
        private string connectionString = string.Empty;

        //  Hide the default constructor.
        private AccessDatabase()
        {
        }

        public AccessDatabase(string connectionString)
        {
            if (connectionString == string.Empty)
                throw new Exception("Missing connection string.");

            this.connectionString = connectionString;
        }

        public OleDbDataReader QueryDataReader(string query)
        {
            this.connection = new OleDbConnection(this.connectionString);
            this.connection.Open();

            this.command = new OleDbCommand(query, this.connection);
            return command.ExecuteReader();
        }

        public DataSet QueryDataSet(string query, Dictionary<string, object> parameters)
        {
            DataSet dataSet = new DataSet();

            this.connection = new OleDbConnection(this.connectionString);
            this.connection.Open();

            this.command = new OleDbCommand(query, this.connection);
            this.command.CommandType = CommandType.Text;

            OleDbDataAdapter adapter = new OleDbDataAdapter();


            adapter.SelectCommand = this.command;

            adapter.Fill(dataSet);

            return dataSet;
        }


        public DataTable QueryDataTable(string query, Dictionary<string, object> parameters)
        {
            DataTable dataTable = new DataTable();

            this.connection = new OleDbConnection(this.connectionString);
            this.connection.Open();

            this.command = new OleDbCommand(query, this.connection);
            this.command.CommandType = CommandType.Text;

            OleDbDataAdapter adapter = new OleDbDataAdapter();

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                    this.command.Parameters.Add(parameter.Key, parameter.Value);
            }

            adapter.SelectCommand = this.command;

            adapter.Fill(dataTable);

            return dataTable;
        }


        public int QueryExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            int result = 0;


            try
            {
                this.connection = new OleDbConnection(this.connectionString);
                this.connection.Open();

                this.command = new OleDbCommand(query, this.connection);
                this.command.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> parameter in parameters)
                        this.command.Parameters.Add(parameter.Key, parameter.Value);
                }

                result = this.command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
            }

            return result;
        }

        public object QueryExecuteScalar(string query, Dictionary<string, object> parameters)
        {
            object result = null;


            try
            {
                this.connection = new OleDbConnection(this.connectionString);
                this.connection.Open();

                this.command = new OleDbCommand(query, this.connection);
                this.command.CommandType = CommandType.Text;

                OleDbDataAdapter adapter = new OleDbDataAdapter();

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> parameter in parameters)
                        this.command.Parameters.Add(parameter.Key, parameter.Value);
                }

                result = this.command.ExecuteScalar();
            }
            catch (Exception exception)
            {
                result = null;
            }
            finally
            {
            }

            return result;
        }

        public bool TransStart()
        {
            bool result = false;


            try
            {
                this.connection = new OleDbConnection(this.connectionString);
                this.connection.Open();

                this.transaction = this.connection.BeginTransaction(IsolationLevel.ReadCommitted);

                result = true;
            }
            catch (Exception exception)
            {
                result = false;
            }
            finally
            {
            }

            return result;
        }

        public int TransExecute(string query, Dictionary<string, object> parameters)
        {
            int result = 0;

            try
            {
                this.command = new OleDbCommand(query, this.connection);
                this.command.Transaction = this.transaction;
                this.command.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> parameter in parameters)
                        this.command.Parameters.Add(parameter.Key, parameter.Value);
                }

                result = this.command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
            }

            return result;
        }

        public void TransRollBack()
        {
            this.transaction.Rollback();
        }

        public void TransCommit()
        {
            this.transaction.Commit();
        }

        public void Close()
        {
            this.connection.Close();
            this.connection.Dispose();
            this.connection = null;
        }
    }
}
