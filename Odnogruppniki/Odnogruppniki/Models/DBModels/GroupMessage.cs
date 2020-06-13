namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GroupMessage")]
    public partial class GroupMessage
    {
        public int Id { get; set; }

        public int IdOut { get; set; }

        public int IdIn { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
