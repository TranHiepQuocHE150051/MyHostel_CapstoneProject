namespace MyHostel_BackEnd.DTOs
{
    public class AddTransactionRequest
    {
        public int[] roomId { get; set; }
        public decimal electricity { get; set; }

        public decimal water { get; set; }

        public decimal internet { get; set; }

        public decimal rent { get; set; }
        public decimal security { get; set; }
        public string sendAt { get; set; }

        public string[] other { get; set; }




    }
}
