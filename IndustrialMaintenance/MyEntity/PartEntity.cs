using System;

namespace MyEntity
{
    /// <summary>
    /// 配件信息，对应表 Parts
    /// </summary>
    public class PartEntity
    {
        public int Id { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string Specification { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal MinStock { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
