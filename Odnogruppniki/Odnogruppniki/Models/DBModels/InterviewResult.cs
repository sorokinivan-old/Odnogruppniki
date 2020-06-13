using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Odnogruppniki.Models.DBModels
{
    [Table("InterviewResult")]
    public class InterviewResult
    {
        public int Id { get; set; }
        public int? IdInterview { get; set; }
        public int IdQuestion { get; set; }
        public int? IdAnswer { get; set; }
        public int IdUser { get; set; }
    }
}