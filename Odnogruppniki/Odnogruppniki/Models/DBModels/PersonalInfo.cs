namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonalInfo")]
    public partial class PersonalInfo
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string AboutInfo { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        public int? IdFaculty { get; set; }

        public int? IdDepartment { get; set; }

        public int? IdRole { get; set; }

        public int? IdGroup { get; set; }

        public int IdUser { get; set; }

        public string Photo { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }
    }
}
