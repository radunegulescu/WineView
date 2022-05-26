﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineView.Models
{
    public class Style
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Style")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
