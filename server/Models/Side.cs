namespace ChessApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Side
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int GameID { get; set; }

        public int PlayerID { get; set; }

        [Required]
        [StringLength(5)]
        public string Color { get; set; }

        public decimal Points { get; set; }

        public int Draw { get; set; }

        public int Resign { get; set; }
    }
}
