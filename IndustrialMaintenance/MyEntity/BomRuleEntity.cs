namespace MyEntity
{
    /// <summary>
    /// BOM 算料规则，对应表 BomRules
    /// </summary>
    public class BomRuleEntity
    {
        public int Id { get; set; }
        public string RepairPart { get; set; }
        public int PartId { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public decimal QuantityPerUnit { get; set; }
        public decimal LossRate { get; set; }
        public decimal MinQuantity { get; set; }
        public string Unit { get; set; }
        public string Remark { get; set; }
    }
}
