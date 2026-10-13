using System;
using System.Data;
using MyEntity;
using MySql.Data.MySqlClient;

namespace MyDAL
{
    public class PartCategoryDAL
    {
        public DataTable GetAll()
        {
            return DBHelper.ExecuteQuery(
                "SELECT Id, CategoryName, Remark FROM PartCategories ORDER BY Id");
        }

        public int Insert(PartCategoryEntity entity)
        {
            return DBHelper.ExecuteNonQuery(
                "INSERT INTO PartCategories(CategoryName, Remark) VALUES(@Name, @Remark)",
                new MySqlParameter("@Name", entity.CategoryName),
                new MySqlParameter("@Remark", (object)entity.Remark ?? DBNull.Value));
        }

        public int Update(PartCategoryEntity entity)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE PartCategories SET CategoryName=@Name, Remark=@Remark WHERE Id=@Id",
                new MySqlParameter("@Id", entity.Id),
                new MySqlParameter("@Name", entity.CategoryName),
                new MySqlParameter("@Remark", (object)entity.Remark ?? DBNull.Value));
        }

        public int Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM PartCategories WHERE Id=@Id",
                new MySqlParameter("@Id", id));
        }
    }
}
