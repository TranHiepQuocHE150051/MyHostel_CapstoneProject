﻿namespace MyHostel_BackEnd.DTOs
{
    public class AddTransactionRequest
    {
        public decimal Electricity { get; set; }
        public decimal Water { get; set; }
        public decimal Internet { get; set; }
        public decimal Rent { get; set; }
        public string AtTime { get; set; }
        public string[] Other { get; set; }
    }
}
