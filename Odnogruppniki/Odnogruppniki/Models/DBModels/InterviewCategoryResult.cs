namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InterviewCategoryResult")]
    public class InterviewCategoryResult
    {
        [Key]
        public int Id { get; set; }
        public int IdInterview { get; set; }
        public int IdCategory { get; set; }
        public int Result { get; set; }
    }
}