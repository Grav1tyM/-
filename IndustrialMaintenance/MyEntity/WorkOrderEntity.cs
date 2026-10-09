using System;

namespace MyEntity
{
    /// <summary>
    /// 维修工单，对应表 WorkOrders
    /// </summary>
    public class WorkOrderEntity
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string DeviceNo { get; set; }
        public string DeviceName { get; set; }
        public string RepairPart { get; set; }
        public string FaultDesc { get; set; }
        public string PhotoPath { get; set; }
        public string RecognitionJson { get; set; }
        public int RepairQuantity { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Reporter { get; set; }
        public string Assignee { get; set; }
        public DateTime? AssignTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public string Remark { get; set; }
    }
}
