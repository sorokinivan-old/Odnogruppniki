namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GroupDegree")]
    public partial class GroupDegree
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
