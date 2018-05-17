using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Watch.Models
{
    public class SubImages
    {
        [Key]
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Extension { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}