namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InterviewQuestionsOfInterview")]
    public class InterviewQuestionsOfInterview
    {
        public int Id { get; set; }
        public int IdInterview { get; set; }
        public int IdQuestion { get; set; }
    }
}