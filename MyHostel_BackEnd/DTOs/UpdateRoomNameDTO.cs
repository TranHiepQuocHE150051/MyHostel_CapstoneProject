﻿namespace MyHostel_BackEnd.DTOs
{
    public class UpdateRoomNameDTO
    {
        public int RoomId { get; set; }
        public string Name { get; set; } = null!;
    }
}
