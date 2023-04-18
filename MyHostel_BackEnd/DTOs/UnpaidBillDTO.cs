namespace MyHostel_BackEnd.DTOs
{
    public class UnpaidBillDTO
    {
        public int Id { get; set; }
        public string AtTime { get; set; }
        public decimal Rent  { get; set; }
        public decimal Electricity { get; set; }
        public decimal Water { get; set; }
        public decimal Internet { get; set; }
        public List<OtherCostDTO> OtherCost { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal NeedToPay { get; set; }
        public int Status { get; set; }
        public string CreatedAt { get; set; }
    }
}
