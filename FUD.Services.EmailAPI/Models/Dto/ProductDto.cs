﻿using System.ComponentModel.DataAnnotations;

namespace FUD.Services.EmailAPI.Models.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public int Count { get; set; } = 1;
    }
}
