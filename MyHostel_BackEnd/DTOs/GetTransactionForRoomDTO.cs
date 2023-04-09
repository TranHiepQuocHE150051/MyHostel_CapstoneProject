namespace MyHostel_BackEnd.DTOs
{
    public class GetTransactionForRoomDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public decimal? Rent { get; set; }
        public decimal? Electricity { get; set; }
        public decimal? Water { get; set; }
        public decimal? Internet { get; set; }
        public OtherCostDTO[] OtherCost { get; set; }
        public Boolean? IsPaid { get; set; }
        public decimal? Total { get; set; }
    }
}
