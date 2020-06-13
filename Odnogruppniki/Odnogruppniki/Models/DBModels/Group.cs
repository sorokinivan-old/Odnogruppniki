namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Group")]
    public partial class Group
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int IdDepartment { get; set; }
        public int IdDegree { get; set; }
    }
}
