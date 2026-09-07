using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChessApi.Models
{
    public class CurrentInfo
    {
        [Required]
        public int GameID { get; set; }

        [Required]
        public string FEN { get; set; }

        [Required]
        [StringLength(4)]
        public string Status { get; set; }

        [Required]
        [StringLength(255)]
        public string White { get; set; }

        [Required]
        [StringLength(255)]
        public string Black { get; set; }

        [Required]
        [StringLength(5)]
        public string LastMove { get; set; }

        [Required]
        [StringLength(5)]
        public string YourColor { get; set; }

        [Required]
        public bool IsYourMove { get; set; }

        [Required]
        [StringLength(255)]
        public string OfferDraw { get; set; }

        [Required]
        [StringLength(255)]
        public string Winner { get; set;}
    }
}