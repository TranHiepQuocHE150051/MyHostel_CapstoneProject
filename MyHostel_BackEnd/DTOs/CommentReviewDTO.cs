﻿namespace MyHostel_BackEnd.DTOs
{
    public class CommentReviewDTO
    {
        public int MemberId { get; set; }
        public string AvatarUrl { get; set; } = null!;
        public string Text { get; set; } = null!;
        public string CreatedDate { get; set; } 
    }
}