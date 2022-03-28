using System;
using System.Data;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace ServiceSiteForTheElderly.Models
{
    public class DBAccess
    {

        // http://sairoutine.hatenablog.com/entry/2014/02/17/121504

        private IDbConnection _connection;

        public DBAccess()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;
            Connection = new SqlConnection(connectionString);
        }

        public IDbConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public IDbTransaction Transaction { get; set; }

        protected void OpenConnection()
        {
            if ((_connection != null) && (_connection.State != ConnectionState.Open))
                _connection.Open();
        }

        protected void CloseConnection()
        {
            if ((_connection != null))
                _connection.Close();
        }



        public int Execute(string sql)
        {
            int rtn = 0;
            // Dim transaction As IDbTransaction = Nothing


            OpenConnection();
            try
            {
                var cmd = _connection.CreateCommand();

                cmd.CommandText = sql;

                rtn = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                rtn = -1;
            }
            finally
            {
            }

            return rtn;
        }

        public void Query(string sql, ref DataTable dt)
        {
            DataSet ds = new DataSet();

            System.Data.Common.DbProviderFactory dbFactory = DbProviderFactories.GetFactory((DbConnection)_connection);
            IDbDataAdapter dbAdapter = dbFactory.CreateDataAdapter();

            IDbCommand dbCommand = _connection.CreateCommand();
            dbCommand.CommandText = sql;
            dbCommand.CommandType = CommandType.Text;
            dbAdapter.SelectCommand = dbCommand;
            dbAdapter.Fill(ds);

            dt = ds.Tables[0];
        }
    }




}