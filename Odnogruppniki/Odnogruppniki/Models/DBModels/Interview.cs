namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Interview")]
    public class Interview
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int? IdRole { get; set; }
        public int? IdGroup { get; set; }
        public int? IdDepartment { get; set; }
        public int? IdFaculty { get; set; }
        public string Name { get; set; }
        public int? Saati { get; set; }
    }
}