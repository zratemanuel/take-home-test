using Fundo.Applications.Repository.Entity;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.Repository
{
    public class ContextDB : DbContext
    {
        public ContextDB(DbContextOptions<ContextDB> options)
            : base(options)
        {

        }
		

        public DbSet<Loan>? Loans { get; set; }

        public DbSet<Applicant>? Applicants { get; set; }

        public DbSet<HistoryDeduce>? HistoryDeduces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntities(modelBuilder);
            
            base.OnModelCreating(modelBuilder);

            ExecuteSeedData(modelBuilder);
        }

        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Applicant>().ToTable("Applicants");

            modelBuilder.Entity<Loan>()
              .ToTable("Loans")
              .HasOne(l => l.Applicant)
              .WithMany(a => a.Loans)
              .HasForeignKey(l => l.ApplicantId);

            modelBuilder.Entity<Loan>()
               .Property(l => l.CurrentBalance)
               .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Loan>()
              .Property(l => l.Amount)
              .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<HistoryDeduce>();

            modelBuilder.Entity<Loan>()
               .Property(l => l.CurrentBalance)
               .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Loan>()
              .Property(l => l.Amount)
              .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }

        private void ExecuteSeedData(ModelBuilder modelBuilder)
        {
            var applicantId = BaseEntity.GenerateId();
            modelBuilder.Entity<Applicant>()
                .HasData(new Applicant
                {
                    ApplicantId = applicantId,
                    ApplicantName = "John Doe",
                    Document = "123456789",
                    User = "john@test.com",
                    Password = "1234"
                });

            modelBuilder.Entity<Loan>()
                .HasData(new Loan
                {
                    LoanId = BaseEntity.GenerateId(),
                    Amount = 1000,
                    CurrentBalance = 500,
                    Status = 1,
                    ApplicantId = applicantId
                });

            modelBuilder.Entity<Loan>()
                .HasData(new Loan
                {
                    LoanId = BaseEntity.GenerateId(),
                    Amount = 2000,
                    CurrentBalance = 1500,
                    Status = 1,
                    ApplicantId = applicantId
                });

            modelBuilder.Entity<Loan>()
                .HasData(new Loan
                {
                    LoanId = BaseEntity.GenerateId(),
                    Amount = 3000,
                    CurrentBalance = 2500,
                    Status = 1,
                    ApplicantId = applicantId
                });
        }
    }
}
