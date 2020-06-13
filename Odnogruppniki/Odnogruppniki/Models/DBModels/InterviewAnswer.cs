namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InterviewAnswer")]
    public class InterviewAnswer
    {
        public int Id { get; set; }
        public int IdQuestion { get; set; }
        public string Answer { get; set; }
        public double Weight { get; set; }
    }
}