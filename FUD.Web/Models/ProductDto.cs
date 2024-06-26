﻿using System.ComponentModel.DataAnnotations;

namespace FUD.Web.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }
        public string Description { get; set; }

        public string CategoryName { get; set; }
        
        public string ImageUrl { get; set; }

        [Range(1, 100)]
        public int Count { get; set; } = 1;
    }
}