using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

[Table("InterviewDone")]
public partial class InterviewDone
{
    public int Id { get; set; }

    public int IdInterview { get; set; }
    public int IdUser { get; set; }
}