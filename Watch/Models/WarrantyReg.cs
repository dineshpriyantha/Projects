using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Watch.Models
{
    public class WarrantyReg
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please Enter ModelNo")]
        public string ModelNo { get; set; }

        [Required(ErrorMessage = "Please Enter Dealer")]
        public string Dealer { get; set; }

        [Required(ErrorMessage = "Please Enter Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please Select Date")]
        public string Date { get; set; }
    }
}