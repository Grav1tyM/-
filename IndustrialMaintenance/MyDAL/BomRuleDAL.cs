using System;
using System.Data;
using MyEntity;
using MySql.Data.MySqlClient;

namespace MyDAL
{
    public class BomRuleDAL
    {
        public DataTable GetAll(string repairPart = null)
        {
            string sql = @"SELECT b.Id, b.RepairPart, b.PartId, p.PartCode, p.PartName,
                                  b.QuantityPerUnit, b.LossRate, b.MinQuantity, b.Unit, b.Remark
                           FROM BomRules b
                           INNER JOIN Parts p ON b.PartId = p.Id
                           WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(repairPart))
            {
                sql += " AND b.RepairPart=@rp";
                return DBHelper.ExecuteQuery(sql + " ORDER BY b.Id",
                    new MySqlParameter("@rp", repairPart.Trim()));
            }
            return DBHelper.ExecuteQuery(sql + " ORDER BY b.Id");
        }

        public DataTable GetRepairParts()
        {
            return DBHelper.ExecuteQuery(
                "SELECT DISTINCT RepairPart FROM BomRules ORDER BY RepairPart");
        }

        public int Insert(BomRuleEntity entity)
        {
            string sql = @"INSERT INTO BomRules(RepairPart, PartId, QuantityPerUnit, LossRate, MinQuantity, Unit, Remark)
                           VALUES(@rp, @pid, @qty, @loss, @min, @unit, @remark)";
            return DBHelper.ExecuteNonQuery(sql,
                new MySqlParameter("@rp", entity.RepairPart),
                new MySqlParameter("@pid", entity.PartId),
                new MySqlParameter("@qty", entity.QuantityPerUnit),
                new MySqlParameter("@loss", entity.LossRate),
                new MySqlParameter("@min", entity.MinQuantity),
                new MySqlParameter("@unit", (object)entity.Unit ?? DBNull.Value),
                new MySqlParameter("@remark", (object)entity.Remark ?? DBNull.Value));
        }

        public int Update(BomRuleEntity entity)
        {
            string sql = @"UPDATE BomRules SET RepairPart=@rp, PartId=@pid, QuantityPerUnit=@qty,
                                     LossRate=@loss, MinQuantity=@min, Unit=@unit, Remark=@remark
                           WHERE Id=@Id";
            return DBHelper.ExecuteNonQuery(sql,
                new MySqlParameter("@Id", entity.Id),
                new MySqlParameter("@rp", entity.RepairPart),
                new MySqlParameter("@pid", entity.PartId),
                new MySqlParameter("@qty", entity.QuantityPerUnit),
                new MySqlParameter("@loss", entity.LossRate),
                new MySqlParameter("@min", entity.MinQuantity),
                new MySqlParameter("@unit", (object)entity.Unit ?? DBNull.Value),
                new MySqlParameter("@remark", (object)entity.Remark ?? DBNull.Value));
        }

        public int Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM BomRules WHERE Id=@Id",
                new MySqlParameter("@Id", id));
        }
    }
}
