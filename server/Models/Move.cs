namespace ChessApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Move
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int GameID { get; set; }

        public int PlayerID { get; set; }

        public int PLY { get; set; }

        [Required]
        [StringLength(255)]
        public string FEN { get; set; }

        [Required]
        [StringLength(10)]
        public string MoveNext { get; set; }
    }
}
