using System;
using System.Data;
using MyEntity;
using MySql.Data.MySqlClient;

namespace MyDAL
{
    public class UserDAL
    {
        /// <summary>
        /// 登录校验：UserName + PasswordHash + Status=1
        /// </summary>
        public UsersEntity Login(string userName, string password)
        {
            string sql = @"SELECT Id, UserName, PasswordHash, RealName, Role, Phone, Status, CreateTime
                           FROM Users
                           WHERE UserName=@UserName AND PasswordHash=@PasswordHash AND Status=1
                           LIMIT 1";
            DataTable dt = DBHelper.ExecuteQuery(sql,
                new MySqlParameter("@UserName", userName),
                new MySqlParameter("@PasswordHash", password));

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return Map(dt.Rows[0]);
        }

        public DataTable GetAll()
        {
            string sql = @"SELECT Id, UserName, RealName, Role, Phone, Status, CreateTime
                           FROM Users ORDER BY Id";
            return DBHelper.ExecuteQuery(sql);
        }

        public int Insert(UsersEntity entity)
        {
            string sql = @"INSERT INTO Users(UserName, PasswordHash, RealName, Role, Phone, Status)
                           VALUES(@UserName, @PasswordHash, @RealName, @Role, @Phone, @Status)";
            return DBHelper.ExecuteNonQuery(sql,
                new MySqlParameter("@UserName", entity.UserName),
                new MySqlParameter("@PasswordHash", entity.PasswordHash),
                new MySqlParameter("@RealName", (object)entity.RealName ?? DBNull.Value),
                new MySqlParameter("@Role", entity.Role ?? "User"),
                new MySqlParameter("@Phone", (object)entity.Phone ?? DBNull.Value),
                new MySqlParameter("@Status", entity.Status));
        }

        public int Update(UsersEntity entity)
        {
            string sql = @"UPDATE Users
                           SET RealName=@RealName, Role=@Role, Phone=@Phone, Status=@Status,
                               PasswordHash=CASE WHEN @PasswordHash IS NULL OR @PasswordHash='' THEN PasswordHash ELSE @PasswordHash END
                           WHERE Id=@Id";
            return DBHelper.ExecuteNonQuery(sql,
                new MySqlParameter("@Id", entity.Id),
                new MySqlParameter("@RealName", (object)entity.RealName ?? DBNull.Value),
                new MySqlParameter("@Role", entity.Role ?? "User"),
                new MySqlParameter("@Phone", (object)entity.Phone ?? DBNull.Value),
                new MySqlParameter("@Status", entity.Status),
                new MySqlParameter("@PasswordHash", (object)entity.PasswordHash ?? string.Empty));
        }

        public int Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM Users WHERE Id=@Id",
                new MySqlParameter("@Id", id));
        }

        private static UsersEntity Map(DataRow row)
        {
            return new UsersEntity
            {
                Id = Convert.ToInt32(row["Id"]),
                UserName = row["UserName"].ToString(),
                PasswordHash = row.Table.Columns.Contains("PasswordHash") ? row["PasswordHash"].ToString() : null,
                RealName = row["RealName"] == DBNull.Value ? null : row["RealName"].ToString(),
                Role = row["Role"].ToString(),
                Phone = row["Phone"] == DBNull.Value ? null : row["Phone"].ToString(),
                Status = Convert.ToInt32(row["Status"]),
                CreateTime = Convert.ToDateTime(row["CreateTime"])
            };
        }
    }
}
