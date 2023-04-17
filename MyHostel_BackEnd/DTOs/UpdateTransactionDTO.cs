namespace MyHostel_BackEnd.DTOs
{
    public class UpdateTransactionDTO
    {
        public int Id { get; set; }
        public decimal Electricity { get; set; }
        public decimal Water { get; set; }
        public decimal Internet { get; set; }
        public decimal Rent { get; set; }
        public string[] Other { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
