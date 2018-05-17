using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Watch.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter Watch Id")]
        [Display(Name = "Watch Id")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "Please Enter Watch Name")]
        [Display(Name = "Watch Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Watch Price")]
        [Display(Name = "Watch Price")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Please Enter Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please Enter Watch Category")]
        [Display(Name = "Watch Category")]
        public string Category { get; set; }

        public virtual ICollection<MainImage> MainImages { get; set; }
        public virtual ICollection<SubImages> SubImages { get; set; }




    }
}