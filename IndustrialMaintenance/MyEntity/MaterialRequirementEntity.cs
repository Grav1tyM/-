using System;

namespace MyEntity
{
    /// <summary>
    /// 领料清单，对应表 MaterialRequirements
    /// </summary>
    public class MaterialRequirementEntity
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int PartId { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string Specification { get; set; }
        public string Unit { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal StockQuantity { get; set; }
        public int IsShortage { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
