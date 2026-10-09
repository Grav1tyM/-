using System;
using System.Data;
using MyEntity;
using MySql.Data.MySqlClient;

namespace MyDAL
{
    public class PartDAL
    {
        public DataTable GetAll(string keyword = null)
        {
            string sql = @"SELECT p.Id, p.PartCode, p.PartName, p.Specification, p.CategoryId,
                                  c.CategoryName, p.Unit, p.StockQuantity, p.MinStock, p.Price,
                                  p.Status, p.Remark, p.CreateTime
                           FROM Parts p
                           LEFT JOIN PartCategories c ON p.CategoryId = c.Id
                           WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += " AND (p.PartCode LIKE @kw OR p.PartName LIKE @kw OR p.Specification LIKE @kw)";
                return DBHelper.ExecuteQuery(sql + " ORDER BY p.Id",
                    new MySqlParameter("@kw", "%" + keyword.Trim() + "%"));
            }
            return DBHelper.ExecuteQuery(sql + " ORDER BY p.Id");
        }

        public PartEntity GetByCode(string partCode)
        {
            string sql = @"SELECT p.Id, p.PartCode, p.PartName, p.Specification, p.CategoryId,
                                  c.CategoryName, p.Unit, p.StockQuantity, p.MinStock, p.Price,
                                  p.Status, p.Remark, p.CreateTime
                           FROM Parts p
                           LEFT JOIN PartCategories c ON p.CategoryId = c.Id
                           WHERE p.PartCode=@Code LIMIT 1";
            DataTable dt = DBHelper.ExecuteQuery(sql, new MySqlParameter("@Code", partCode));
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }

        public PartEntity GetById(int id)
        {
            string sql = @"SELECT p.Id, p.PartCode, p.PartName, p.Specification, p.CategoryId,
                                  c.CategoryName, p.Unit, p.StockQuantity, p.MinStock, p.Price,
                                  p.Status, p.Remark, p.CreateTime
                           FROM Parts p
                           LEFT JOIN PartCategories c ON p.CategoryId = c.Id
                           WHERE p.Id=@Id LIMIT 1";
            DataTable dt = DBHelper.ExecuteQuery(sql, new MySqlParameter("@Id", id));
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }

        public static string CalcStatus(decimal stock, decimal minStock)
        {
            if (stock <= 0) return "OutOfStock";
            if (stock <= minStock) return "LowStock";
            return "InStock";
        }

        public int Insert(PartEntity entity)
        {
            entity.Status = CalcStatus(entity.StockQuantity, entity.MinStock);
            string sql = @"INSERT INTO Parts(PartCode, PartName, Specification, CategoryId, Unit,
                                             StockQuantity, MinStock, Price, Status, Remark)
                           VALUES(@Code, @Name, @Spec, @Cat, @Unit, @Stock, @Min, @Price, @Status, @Remark)";
            return DBHelper.ExecuteNonQuery(sql,
                new MySqlParameter("@Code", entity.PartCode),
                new MySqlParameter("@Name", entity.PartName),
                new MySqlParameter("@Spec", (object)entity.Specification ?? DBNull.Value),
                new MySqlParameter("@Cat", entity.CategoryId),
                new MySqlParameter("@Unit", entity.Unit ?? "个"),
                new MySqlParameter("@Stock", entity.StockQuantity),
                new MySqlParameter("@Min", entity.MinStock),
                new MySqlParameter("@Price", (object)entity.Price ?? 0m),
                new MySqlParameter("@Status", entity.Status),
                new MySqlParameter("@Remark", (object)entity.Remark ?? DBNull.Value));
        }

        public int Update(PartEntity entity)
        {
            entity.Status = CalcStatus(entity.StockQuantity, entity.MinStock);
            string sql = @"UPDATE Parts SET PartCode=@Code, PartName=@Name, Specification=@Spec,
                                  CategoryId=@Cat, Unit=@Unit, StockQuantity=@Stock, MinStock=@Min,
                                  Price=@Price, Status=@Status, Remark=@Remark
                           WHERE Id=@Id";
            return DBHelper.ExecuteNonQuery(sql,
                new MySqlParameter("@Id", entity.Id),
                new MySqlParameter("@Code", entity.PartCode),
                new MySqlParameter("@Name", entity.PartName),
                new MySqlParameter("@Spec", (object)entity.Specification ?? DBNull.Value),
                new MySqlParameter("@Cat", entity.CategoryId),
                new MySqlParameter("@Unit", entity.Unit ?? "个"),
                new MySqlParameter("@Stock", entity.StockQuantity),
                new MySqlParameter("@Min", entity.MinStock),
                new MySqlParameter("@Price", (object)entity.Price ?? 0m),
                new MySqlParameter("@Status", entity.Status),
                new MySqlParameter("@Remark", (object)entity.Remark ?? DBNull.Value));
        }

        public int Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM Parts WHERE Id=@Id",
                new MySqlParameter("@Id", id));
        }

        private static PartEntity Map(DataRow row)
        {
            return new PartEntity
            {
                Id = Convert.ToInt32(row["Id"]),
                PartCode = row["PartCode"].ToString(),
                PartName = row["PartName"].ToString(),
                Specification = row["Specification"] == DBNull.Value ? null : row["Specification"].ToString(),
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                CategoryName = row.Table.Columns.Contains("CategoryName") && row["CategoryName"] != DBNull.Value
                    ? row["CategoryName"].ToString() : null,
                Unit = row["Unit"].ToString(),
                StockQuantity = Convert.ToDecimal(row["StockQuantity"]),
                MinStock = Convert.ToDecimal(row["MinStock"]),
                Price = row["Price"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["Price"]),
                Status = row["Status"].ToString(),
                Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString(),
                CreateTime = Convert.ToDateTime(row["CreateTime"])
            };
        }
    }
}
