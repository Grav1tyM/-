using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace MyDAL
{
    /// <summary>
    /// MySQL 数据库访问帮助类
    /// </summary>
    public static class DBHelper
    {
        private static readonly string ConnStr =
            ConfigurationManager.ConnectionStrings["MySqlConn"]?.ConnectionString
            ?? "server=localhost;port=3306;database=IndustrialMaintenance;user id=root;password=114514;charset=utf8mb4;";

        public static MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConnStr);
        }

        public static DataTable ExecuteQuery(string sql, params MySqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            try
            {
                using (MySqlConnection conn = CreateConnection())
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("查询失败：" + ex.Message, ex);
            }
            return table;
        }

        public static int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            try
            {
                using (MySqlConnection conn = CreateConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("执行失败：" + ex.Message, ex);
            }
        }

        public static object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            try
            {
                using (MySqlConnection conn = CreateConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("标量查询失败：" + ex.Message, ex);
            }
        }

        public static void ExecuteTransaction(Action<MySqlConnection, MySqlTransaction> work)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        work(conn, trans);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            trans.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            throw new Exception(
                                "事务回滚失败：" + rollbackEx.Message + "；原始错误：" + ex.Message,
                                ex);
                        }
                        throw new Exception("事务执行失败：" + ex.Message, ex);
                    }
                }
            }
        }
    }
}
