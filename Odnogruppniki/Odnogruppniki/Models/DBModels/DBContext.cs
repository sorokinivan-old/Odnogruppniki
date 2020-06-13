namespace Odnogruppniki.Models.DBModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext5")
        {
        }

        public static DBContext Create()
        {
            return new DBContext();
        }


        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupDegree> GroupDegrees { get; set; }
        public virtual DbSet<GroupMessage> GroupMessages { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PersonalInfo> PersonalInfoes { get; set; }
        public virtual DbSet<PersonalMessage> PersonalMessages { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Interview> Interviews { get; set; }
        public virtual DbSet<InterviewQuestion> InterviewQuestions { get; set; }
        public virtual DbSet<InterviewAnswer> InterviewAnswers { get; set; }
        public virtual DbSet<InterviewResult> InterviewResults { get; set; }
        public virtual DbSet<InterviewDone> InterviewDones { get; set; }
        public virtual DbSet<InterviewCategory> InterviewCategories { get; set; }
        public virtual DbSet<InterviewCategoryResult> InterviewCategoryResults { get; set; }
        public virtual DbSet<InterviewCategoriesOfInterview> InterviewCategoriesOfInterviews { get; set; }
        public virtual DbSet<InterviewQuestionsOfInterview> InterviewQuestionsOfInterviews { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
