using System;
using System.Data;
using MyEntity;
using MySql.Data.MySqlClient;

namespace MyDAL
{
    public class WorkOrderDAL
    {
        public DataTable GetAll(string keyword = null)
        {
            string sql = @"SELECT Id, OrderNo, DeviceNo, DeviceName, RepairPart, FaultDesc,
                                  PhotoPath, RecognitionJson, RepairQuantity, Status, Priority,
                                  Reporter, Assignee, AssignTime, CreateTime, FinishTime, Remark
                           FROM WorkOrders WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += @" AND (OrderNo LIKE @kw OR DeviceNo LIKE @kw OR DeviceName LIKE @kw
                               OR RepairPart LIKE @kw OR Status LIKE @kw)";
                return DBHelper.ExecuteQuery(sql + " ORDER BY Id DESC",
                    new MySqlParameter("@kw", "%" + keyword.Trim() + "%"));
            }
            return DBHelper.ExecuteQuery(sql + " ORDER BY Id DESC");
        }

        public WorkOrderEntity GetByOrderNo(string orderNo)
        {
            string sql = @"SELECT Id, OrderNo, DeviceNo, DeviceName, RepairPart, FaultDesc,
                                  PhotoPath, RecognitionJson, RepairQuantity, Status, Priority,
                                  Reporter, Assignee, AssignTime, CreateTime, FinishTime, Remark
                           FROM WorkOrders WHERE OrderNo=@no LIMIT 1";
            DataTable dt = DBHelper.ExecuteQuery(sql, new MySqlParameter("@no", orderNo));
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }

        public string GenerateOrderNo()
        {
            string prefix = "WO" + DateTime.Now.ToString("yyyyMMdd");
            object obj = DBHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM WorkOrders WHERE OrderNo LIKE @p",
                new MySqlParameter("@p", prefix + "%"));
            int seq = Convert.ToInt32(obj) + 1;
            return prefix + seq.ToString("D3");
        }

        public int Insert(WorkOrderEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.OrderNo))
            {
                entity.OrderNo = GenerateOrderNo();
            }
            string sql = @"INSERT INTO WorkOrders
                           (OrderNo, DeviceNo, DeviceName, RepairPart, FaultDesc, PhotoPath,
                            RecognitionJson, RepairQuantity, Status, Priority, Reporter, Assignee,
                            AssignTime, FinishTime, Remark)
                           VALUES
                           (@OrderNo, @DeviceNo, @DeviceName, @RepairPart, @FaultDesc, @PhotoPath,
                            @RecognitionJson, @RepairQuantity, @Status, @Priority, @Reporter, @Assignee,
                            @AssignTime, @FinishTime, @Remark)";
            return DBHelper.ExecuteNonQuery(sql, BuildParams(entity, false));
        }

        public int Update(WorkOrderEntity entity)
        {
            string sql = @"UPDATE WorkOrders SET
                           OrderNo=@OrderNo, DeviceNo=@DeviceNo, DeviceName=@DeviceName,
                           RepairPart=@RepairPart, FaultDesc=@FaultDesc, PhotoPath=@PhotoPath,
                           RecognitionJson=@RecognitionJson, RepairQuantity=@RepairQuantity,
                           Status=@Status, Priority=@Priority, Reporter=@Reporter, Assignee=@Assignee,
                           AssignTime=@AssignTime, FinishTime=@FinishTime, Remark=@Remark
                           WHERE Id=@Id";
            return DBHelper.ExecuteNonQuery(sql, BuildParams(entity, true));
        }

        public int Delete(int id)
        {
            return DBHelper.ExecuteNonQuery("DELETE FROM WorkOrders WHERE Id=@Id",
                new MySqlParameter("@Id", id));
        }

        private static MySqlParameter[] BuildParams(WorkOrderEntity e, bool includeId)
        {
            var list = new System.Collections.Generic.List<MySqlParameter>
            {
                new MySqlParameter("@OrderNo", e.OrderNo),
                new MySqlParameter("@DeviceNo", e.DeviceNo),
                new MySqlParameter("@DeviceName", (object)e.DeviceName ?? DBNull.Value),
                new MySqlParameter("@RepairPart", (object)e.RepairPart ?? DBNull.Value),
                new MySqlParameter("@FaultDesc", (object)e.FaultDesc ?? DBNull.Value),
                new MySqlParameter("@PhotoPath", (object)e.PhotoPath ?? DBNull.Value),
                new MySqlParameter("@RecognitionJson", (object)e.RecognitionJson ?? DBNull.Value),
                new MySqlParameter("@RepairQuantity", e.RepairQuantity),
                new MySqlParameter("@Status", e.Status ?? "Pending"),
                new MySqlParameter("@Priority", e.Priority ?? "Normal"),
                new MySqlParameter("@Reporter", (object)e.Reporter ?? DBNull.Value),
                new MySqlParameter("@Assignee", (object)e.Assignee ?? DBNull.Value),
                new MySqlParameter("@AssignTime", (object)e.AssignTime ?? DBNull.Value),
                new MySqlParameter("@FinishTime", (object)e.FinishTime ?? DBNull.Value),
                new MySqlParameter("@Remark", (object)e.Remark ?? DBNull.Value)
            };
            if (includeId)
            {
                list.Add(new MySqlParameter("@Id", e.Id));
            }
            return list.ToArray();
        }

        private static WorkOrderEntity Map(DataRow row)
        {
            return new WorkOrderEntity
            {
                Id = Convert.ToInt32(row["Id"]),
                OrderNo = row["OrderNo"].ToString(),
                DeviceNo = row["DeviceNo"].ToString(),
                DeviceName = row["DeviceName"] == DBNull.Value ? null : row["DeviceName"].ToString(),
                RepairPart = row["RepairPart"] == DBNull.Value ? null : row["RepairPart"].ToString(),
                FaultDesc = row["FaultDesc"] == DBNull.Value ? null : row["FaultDesc"].ToString(),
                PhotoPath = row["PhotoPath"] == DBNull.Value ? null : row["PhotoPath"].ToString(),
                RecognitionJson = row["RecognitionJson"] == DBNull.Value ? null : row["RecognitionJson"].ToString(),
                RepairQuantity = Convert.ToInt32(row["RepairQuantity"]),
                Status = row["Status"].ToString(),
                Priority = row["Priority"].ToString(),
                Reporter = row["Reporter"] == DBNull.Value ? null : row["Reporter"].ToString(),
                Assignee = row["Assignee"] == DBNull.Value ? null : row["Assignee"].ToString(),
                AssignTime = row["AssignTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["AssignTime"]),
                CreateTime = Convert.ToDateTime(row["CreateTime"]),
                FinishTime = row["FinishTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["FinishTime"]),
                Remark = row["Remark"] == DBNull.Value ? null : row["Remark"].ToString()
            };
        }
    }
}
