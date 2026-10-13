using System;
using System.Data;
using MyEntity;
using MySql.Data.MySqlClient;

namespace MyDAL
{
    public class MaterialRequirementDAL
    {
        public DataTable GetAll(string keyword = null)
        {
            string sql = @"SELECT Id, OrderNo, PartId, PartCode, PartName, Specification, Unit,
                                  RequiredQuantity, StockQuantity, IsShortage, Status, Remark, CreateTime
                           FROM MaterialRequirements WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += " AND (OrderNo LIKE @kw OR PartCode LIKE @kw OR PartName LIKE @kw OR Status LIKE @kw)";
                return DBHelper.ExecuteQuery(sql + " ORDER BY Id DESC",
                    new MySqlParameter("@kw", "%" + keyword.Trim() + "%"));
            }
            return DBHelper.ExecuteQuery(sql + " ORDER BY Id DESC");
        }

        public DataTable GetByOrderNo(string orderNo)
        {
            return DBHelper.ExecuteQuery(
                @"SELECT Id, OrderNo, PartId, PartCode, PartName, Specification, Unit,
                         RequiredQuantity, StockQuantity, IsShortage, Status, Remark, CreateTime
                  FROM MaterialRequirements WHERE OrderNo=@no ORDER BY Id",
                new MySqlParameter("@no", orderNo));
        }

        /// <summary>
        /// 智能算料：按 BOM 规则生成领料清单
        /// 应领数量 = max( ceil(维修数量 × 基准用量 × (1+损耗率)), 最小领用量 )
        /// </summary>
        public int CalculateMaterials(string orderNo, string repairPart, int repairQuantity)
        {
            BomRuleDAL bomDal = new BomRuleDAL();
            PartDAL partDal = new PartDAL();
            DataTable rules = bomDal.GetAll(repairPart);
            if (rules.Rows.Count == 0)
            {
                throw new Exception("未找到维修部位「" + repairPart + "」的 BOM 规则，请先在 BOM 管理中配置。");
            }

            // 先取消该工单旧的待领记录
            DBHelper.ExecuteNonQuery(
                "UPDATE MaterialRequirements SET Status='Cancelled' WHERE OrderNo=@no AND Status='Pending'",
                new MySqlParameter("@no", orderNo));

            int count = 0;
            foreach (DataRow rule in rules.Rows)
            {
                int partId = Convert.ToInt32(rule["PartId"]);
                decimal qtyPer = Convert.ToDecimal(rule["QuantityPerUnit"]);
                decimal loss = Convert.ToDecimal(rule["LossRate"]);
                decimal minQty = Convert.ToDecimal(rule["MinQuantity"]);

                PartEntity part = partDal.GetById(partId);
                if (part == null) continue;

                decimal raw = repairQuantity * qtyPer * (1 + loss);
                decimal required = Math.Ceiling(raw);
                if (required < minQty) required = minQty;

                int shortage = part.StockQuantity < required ? 1 : 0;
                string remark = string.Format("{0}维修×{1}，基准{2}，损耗{3:P0}",
                    repairPart, repairQuantity, qtyPer, loss);

                string sql = @"INSERT INTO MaterialRequirements
                               (OrderNo, PartId, PartCode, PartName, Specification, Unit,
                                RequiredQuantity, StockQuantity, IsShortage, Status, Remark)
                               VALUES
                               (@OrderNo, @PartId, @PartCode, @PartName, @Spec, @Unit,
                                @Req, @Stock, @Short, 'Pending', @Remark)";
                DBHelper.ExecuteNonQuery(sql,
                    new MySqlParameter("@OrderNo", orderNo),
                    new MySqlParameter("@PartId", part.Id),
                    new MySqlParameter("@PartCode", part.PartCode),
                    new MySqlParameter("@PartName", part.PartName),
                    new MySqlParameter("@Spec", (object)part.Specification ?? DBNull.Value),
                    new MySqlParameter("@Unit", (object)part.Unit ?? DBNull.Value),
                    new MySqlParameter("@Req", required),
                    new MySqlParameter("@Stock", part.StockQuantity),
                    new MySqlParameter("@Short", shortage),
                    new MySqlParameter("@Remark", remark));
                count++;
            }
            return count;
        }

        /// <summary>
        /// 领料出库：扣减库存并标记 Issued
        /// </summary>
        public void Issue(int requirementId)
        {
            DBHelper.ExecuteTransaction((conn, trans) =>
            {
                string orderNo = null;
                int partId = 0;
                decimal qty = 0;
                string status = null;

                using (MySqlCommand sel = new MySqlCommand(
                    @"SELECT OrderNo, PartId, RequiredQuantity, Status
                      FROM MaterialRequirements WHERE Id=@Id FOR UPDATE", conn, trans))
                {
                    sel.Parameters.AddWithValue("@Id", requirementId);
                    using (MySqlDataReader reader = sel.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception("领料记录不存在");
                        }
                        orderNo = reader.GetString(0);
                        partId = reader.GetInt32(1);
                        qty = reader.GetDecimal(2);
                        status = reader.GetString(3);
                    }
                }

                if (status != "Pending")
                {
                    throw new Exception("仅待领状态可出库，当前状态：" + status);
                }

                decimal stock;
                decimal minStock;
                using (MySqlCommand chk = new MySqlCommand(
                    "SELECT StockQuantity, MinStock FROM Parts WHERE Id=@Id FOR UPDATE", conn, trans))
                {
                    chk.Parameters.AddWithValue("@Id", partId);
                    using (MySqlDataReader reader = chk.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception("配件不存在");
                        }
                        stock = reader.GetDecimal(0);
                        minStock = reader.GetDecimal(1);
                    }
                }

                if (stock < qty)
                {
                    throw new Exception("库存不足，当前库存：" + stock + "，需领：" + qty);
                }

                decimal newStock = stock - qty;
                string newStatus = PartDAL.CalcStatus(newStock, minStock);

                using (MySqlCommand updPart = new MySqlCommand(
                    "UPDATE Parts SET StockQuantity=@s, Status=@st WHERE Id=@Id", conn, trans))
                {
                    updPart.Parameters.AddWithValue("@s", newStock);
                    updPart.Parameters.AddWithValue("@st", newStatus);
                    updPart.Parameters.AddWithValue("@Id", partId);
                    updPart.ExecuteNonQuery();
                }

                using (MySqlCommand updReq = new MySqlCommand(
                    "UPDATE MaterialRequirements SET Status='Issued', StockQuantity=@s, IsShortage=0 WHERE Id=@Id",
                    conn, trans))
                {
                    updReq.Parameters.AddWithValue("@s", newStock);
                    updReq.Parameters.AddWithValue("@Id", requirementId);
                    updReq.ExecuteNonQuery();
                }
            });
        }

        public int Cancel(int id)
        {
            return DBHelper.ExecuteNonQuery(
                "UPDATE MaterialRequirements SET Status='Cancelled' WHERE Id=@Id AND Status='Pending'",
                new MySqlParameter("@Id", id));
        }
    }
}
