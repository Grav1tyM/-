using System;
using System.Data;
using System.Text;
using MyEntity;
using MySql.Data.MySqlClient;

namespace MyDAL
{
    public class RepairRecordDAL
    {
        public DataTable GetAll(string keyword = null)
        {
            string sql = @"SELECT Id, WorkOrderId, DeviceNo, DeviceName, RepairPart, FaultDesc,
                                  PartsSummary, Engineer, RepairTime, Remark
                           FROM RepairRecords WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += " AND (DeviceNo LIKE @kw OR DeviceName LIKE @kw OR RepairPart LIKE @kw OR Engineer LIKE @kw)";
                return DBHelper.ExecuteQuery(sql + " ORDER BY Id DESC",
                    new MySqlParameter("@kw", "%" + keyword.Trim() + "%"));
            }
            return DBHelper.ExecuteQuery(sql + " ORDER BY Id DESC");
        }

        /// <summary>
        /// 工单完工：更新工单状态并写入维修历史
        /// </summary>
        public void CompleteWorkOrder(WorkOrderEntity order, string engineer)
        {
            DBHelper.ExecuteTransaction((conn, trans) =>
            {
                using (MySqlCommand upd = new MySqlCommand(
                    @"UPDATE WorkOrders SET Status='Completed', FinishTime=NOW(),
                                     Assignee=IFNULL(@eng, Assignee)
                      WHERE Id=@Id", conn, trans))
                {
                    upd.Parameters.AddWithValue("@eng", (object)engineer ?? DBNull.Value);
                    upd.Parameters.AddWithValue("@Id", order.Id);
                    upd.ExecuteNonQuery();
                }

                StringBuilder summary = new StringBuilder();
                using (MySqlCommand mat = new MySqlCommand(
                    @"SELECT PartName, Specification, RequiredQuantity, Unit
                      FROM MaterialRequirements
                      WHERE OrderNo=@no AND Status='Issued'", conn, trans))
                {
                    mat.Parameters.AddWithValue("@no", order.OrderNo);
                    using (MySqlDataReader reader = mat.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (summary.Length > 0) summary.Append("；");
                            string name = reader.GetString(0);
                            string spec = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            decimal qty = reader.GetDecimal(2);
                            string unit = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            summary.AppendFormat("{0}{1} ×{2}{3}", name,
                                string.IsNullOrEmpty(spec) ? "" : spec, qty, unit);
                        }
                    }
                }

                using (MySqlCommand ins = new MySqlCommand(
                    @"INSERT INTO RepairRecords
                      (WorkOrderId, DeviceNo, DeviceName, RepairPart, FaultDesc, PartsSummary, Engineer, Remark)
                      VALUES
                      (@Wid, @Dno, @Dname, @Rp, @Fault, @Sum, @Eng, @Remark)", conn, trans))
                {
                    ins.Parameters.AddWithValue("@Wid", order.Id);
                    ins.Parameters.AddWithValue("@Dno", order.DeviceNo);
                    ins.Parameters.AddWithValue("@Dname", (object)order.DeviceName ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@Rp", (object)order.RepairPart ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@Fault", (object)order.FaultDesc ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@Sum", summary.Length == 0 ? (object)DBNull.Value : summary.ToString());
                    ins.Parameters.AddWithValue("@Eng", (object)engineer ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@Remark", (object)order.Remark ?? DBNull.Value);
                    ins.ExecuteNonQuery();
                }
            });
        }

        public int Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM RepairRecords WHERE Id=@Id",
                new MySqlParameter("@Id", id));
        }
    }
}
