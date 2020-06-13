namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InterviewCategoriesOfInterview")]
    public class InterviewCategoriesOfInterview
    {
        public int Id { get; set; }
        public int IdInterview { get; set; }
        public int IdCategory{ get; set; }
    }
}