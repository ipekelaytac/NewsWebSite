using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Caching;

namespace Dal_Model
{
    public class Dal
    {
        public static string ConnectionString;

        public DateTime lastQueryTime;
        public String lastQuerySQL;

        public SqlConnection myConnection = new SqlConnection(Dal.ConnectionString);
        readonly Cache cache = new Cache();


        public List<T> TableToList<T>(DataTable dataTable) where T : class, new()
        {
            List<T> targetList = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                var target = new T();

                var targetType = target.GetType();

                foreach (var column in targetType.GetProperties())
                {
                    var value = row[column.Name];

                    if (value.GetType() == typeof(DBNull))
                    {
                        value = null;
                    }

                    targetType.GetProperty(column.Name).SetValue(target, value);

                }

                targetList.Add(target);
            }

            return targetList;
        }


        public DataSet CommandExecuteDataSet(String sql, SqlConnection conn)
        {

            lastQueryTime = DateTime.Now;
            lastQuerySQL = sql;
            DataSet ds = new DataSet();
            try
            {
                SqlCommand myCommand = new SqlCommand(sql, conn)
                {
                    CommandTimeout = 600
                };

                SqlDataAdapter dataAdapter = new SqlDataAdapter(myCommand);

                dataAdapter.Fill(ds);
            }
            catch (Exception)
            {

            }
            finally
            {
            }

            return ds;
        }

        public DataSet CommandCacheExecuteDataSet(String sql, SqlConnection conn, String key, int minute)
        {

            lastQueryTime = DateTime.Now;
            lastQuerySQL = sql;

            DataSet ds = new DataSet();
            try
            {

                if (cache[key] != null)
                {
                    ds = cache[key] as DataSet;
                }
                else
                {
                    SqlCommand myCommand = new SqlCommand(sql, conn)
                    {
                        CommandTimeout = 600
                    };

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(myCommand);

                    dataAdapter.Fill(ds);

                    cache.Insert(key, ds, null, DateTime.Now.AddMinutes(minute), System.Web.Caching.Cache.NoSlidingExpiration);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
            }

            return ds;
        }

        public DataSet CommandCacheExecuteDataSetSecond(String sql, SqlConnection conn, String key, int second)
        {

            lastQueryTime = DateTime.Now;
            lastQuerySQL = sql;

            DataSet ds = new DataSet();
            try
            {

                if (cache[key] != null)
                {
                    ds = cache[key] as DataSet;
                }
                else
                {
                    SqlCommand myCommand = new SqlCommand(sql, conn)
                    {
                        CommandTimeout = 600
                    };

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(myCommand);

                    dataAdapter.Fill(ds);

                    cache.Insert(key, ds, null, DateTime.Now.AddSeconds(second), Cache.NoSlidingExpiration);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
            }

            return ds;
        }

        public SqlDataReader CommandExecuteSQLReader(String sql, SqlConnection conn)
        {

            lastQueryTime = DateTime.Now;
            lastQuerySQL = sql;

            SqlDataReader r;
            try
            {
                SqlCommand myCommand = new SqlCommand(sql, conn)
                {
                    CommandTimeout = 600
                };

                r = myCommand.ExecuteReader();
                return r;

            }
            catch (Exception)
            {

            }
            finally
            {
            }


            return null;
        }


        public string CommandExecuteSQLScalar(String sql, SqlConnection conn)
        {

            lastQueryTime = DateTime.Now;
            lastQuerySQL = sql;

            string r;
            try
            {
                SqlCommand myCommand = new SqlCommand(sql, conn)
                {
                    CommandTimeout = 600
                };

                r = myCommand.ExecuteScalar().ToString();
                return r;

            }
            catch (Exception)
            {

            }
            finally
            {
            }


            return null;
        }

        public void CommandExecuteNonQuery(String sql, SqlConnection conn)
        {
            lastQueryTime = DateTime.Now;
            lastQuerySQL = sql;
            try
            {
                SqlCommand myCommand = new SqlCommand(sql, conn)
                {
                    CommandTimeout = 600
                };
                myCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {

            }
            finally
            {
            }
        }

        public bool BoolCommandExecuteNonQuery(String sql, SqlConnection conn)
        {
            lastQueryTime = DateTime.Now;
            lastQuerySQL = sql;
            try
            {
                SqlCommand myCommand = new SqlCommand(sql, conn)
                {
                    CommandTimeout = 600
                };
                myCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
            }
        }

        public void InstanceWithConnectionString(string ConnectionString, SqlConnection conn)
        {
            if (conn != null)
            {
                myConnection = new SqlConnection(ConnectionString);

                ConOpen(myConnection);
            }
        }

        public void ConOpen(SqlConnection c)
        {
            if (c.State != ConnectionState.Open)
            {
                c.Open();
            }
        }

        public void ConClose(SqlConnection c)
        {
            if (c.State != ConnectionState.Closed)
            {
                c.Close();
            }
        }
    }
}