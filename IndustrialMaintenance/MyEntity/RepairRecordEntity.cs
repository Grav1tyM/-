using System;

namespace MyEntity
{
    /// <summary>
    /// 维修历史，对应表 RepairRecords
    /// </summary>
    public class RepairRecordEntity
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public string DeviceNo { get; set; }
        public string DeviceName { get; set; }
        public string RepairPart { get; set; }
        public string FaultDesc { get; set; }
        public string PartsSummary { get; set; }
        public string Engineer { get; set; }
        public DateTime RepairTime { get; set; }
        public string Remark { get; set; }
    }
}
