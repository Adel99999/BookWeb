using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWeb.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Display(Name ="List Price")]
        [Range(0,10000)]
        public double ListPrice { get; set; }
        [Required]
        [Display(Name ="Price1-50")]
        [Range(0,10000)]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Price +50")]
        [Range(0, 10000)]
        public double Price50 { get; set; }
        [Required]
        [Display(Name ="Price +100")]
        [Range(0,10000)]
        public double Price100 { get; set; }

        
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category category { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}
