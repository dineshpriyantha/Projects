using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Watch.Models;

namespace Watch.Models
{
    public class MainImage
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        //public virtual ICollection<SubImages> SubImages { get; set; }

    }
}