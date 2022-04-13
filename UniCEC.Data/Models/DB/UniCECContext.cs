﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace UniCEC.Data.Models.DB
{
    public partial class UniCECContext : DbContext
    {
        public UniCECContext()
        {
        }

        public UniCECContext(DbContextOptions<UniCECContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<BlogType> BlogTypes { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Club> Clubs { get; set; }
        public virtual DbSet<ClubActivity> ClubActivities { get; set; }
        public virtual DbSet<ClubPreviou> ClubPrevious { get; set; }
        public virtual DbSet<ClubRole> ClubRoles { get; set; }
        public virtual DbSet<Competition> Competitions { get; set; }
        public virtual DbSet<CompetitionEntity> CompetitionEntities { get; set; }
        public virtual DbSet<CompetitionInClub> CompetitionInClubs { get; set; }
        public virtual DbSet<CompetitionType> CompetitionTypes { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DepartmentInUniversity> DepartmentInUniversities { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<Major> Majors { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<MemberTakesActivity> MemberTakesActivities { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<ParticipantInTeam> ParticipantInTeams { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SeedsWallet> SeedsWallets { get; set; }
        public virtual DbSet<Sponsor> Sponsors { get; set; }
        public virtual DbSet<SponsorInCompetition> SponsorInCompetitions { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<University> Universities { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=UniCEC;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blog");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BlogTypeId).HasColumnName("BlogTypeID");

                entity.Property(e => e.ClubId).HasColumnName("ClubID");

                entity.Property(e => e.CompetitionId).HasColumnName("CompetitionID");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.BlogType)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BlogTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Blog__BlogTypeID__5441852A");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.ClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Blog__ClubID__5535A963");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.CompetitionId)
                    .HasConstraintName("FK__Blog__Competitio__5629CD9C");
            });

            modelBuilder.Entity<BlogType>(entity =>
            {
                entity.ToTable("BlogType");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("City");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Club>(entity =>
            {
                entity.ToTable("Club");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Founding).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UniversityId).HasColumnName("UniversityID");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.Clubs)
                    .HasForeignKey(d => d.UniversityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Club__University__571DF1D5");
            });

            modelBuilder.Entity<ClubActivity>(entity =>
            {
                entity.ToTable("ClubActivity");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Beginning).HasColumnType("datetime");

                entity.Property(e => e.ClubId).HasColumnName("ClubID");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Ending).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SeedsCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.ClubActivities)
                    .HasForeignKey(d => d.ClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ClubActiv__ClubI__5812160E");
            });

            modelBuilder.Entity<ClubPreviou>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClubId).HasColumnName("ClubID");

                entity.Property(e => e.ClubRoleId).HasColumnName("ClubRoleID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.ClubPrevious)
                    .HasForeignKey(d => d.ClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ClubPrevi__ClubI__59063A47");

                entity.HasOne(d => d.ClubRole)
                    .WithMany(p => p.ClubPrevious)
                    .HasForeignKey(d => d.ClubRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ClubPrevi__ClubR__59FA5E80");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.ClubPrevious)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ClubPrevi__Membe__5AEE82B9");
            });

            modelBuilder.Entity<ClubRole>(entity =>
            {
                entity.ToTable("ClubRole");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Competition>(entity =>
            {
                entity.ToTable("Competition");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ApprovedTime).HasColumnType("datetime");

                entity.Property(e => e.CompetitionTypeId).HasColumnName("CompetitionTypeID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.EndTimeRegister).HasColumnType("datetime");

                entity.Property(e => e.Organizer)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SeedsCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.StartTimeRegister).HasColumnType("datetime");

                entity.HasOne(d => d.CompetitionType)
                    .WithMany(p => p.Competitions)
                    .HasForeignKey(d => d.CompetitionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Competiti__Compe__5BE2A6F2");
            });

            modelBuilder.Entity<CompetitionEntity>(entity =>
            {
                entity.ToTable("CompetitionEntity");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompetitionId).HasColumnName("CompetitionID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.EntityTypeId).HasColumnName("EntityTypeID");

                entity.Property(e => e.LastModified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Size)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.CompetitionEntities)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Competiti__Compe__5CD6CB2B");

                entity.HasOne(d => d.EntityType)
                    .WithMany(p => p.CompetitionEntities)
                    .HasForeignKey(d => d.EntityTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Competiti__Entit__5DCAEF64");
            });

            modelBuilder.Entity<CompetitionInClub>(entity =>
            {
                entity.ToTable("CompetitionInClub");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClubId).HasColumnName("ClubID");

                entity.Property(e => e.CompetitionId).HasColumnName("CompetitionID");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.CompetitionInClubs)
                    .HasForeignKey(d => d.ClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Competiti__ClubI__5EBF139D");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.CompetitionInClubs)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Competiti__Compe__5FB337D6");
            });

            modelBuilder.Entity<CompetitionType>(entity =>
            {
                entity.ToTable("CompetitionType");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompetitionId).HasColumnName("CompetitionID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.CompetitionId)
                    .HasConstraintName("FK__Departmen__Compe__60A75C0F");
            });

            modelBuilder.Entity<DepartmentInUniversity>(entity =>
            {
                entity.ToTable("DepartmentInUniversity");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.UniversityId).HasColumnName("UniversityID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.DepartmentInUniversities)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Departmen__Depar__619B8048");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.DepartmentInUniversities)
                    .HasForeignKey(d => d.UniversityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Departmen__Unive__628FA481");
            });

            modelBuilder.Entity<EntityType>(entity =>
            {
                entity.ToTable("EntityType");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Major>(entity =>
            {
                entity.ToTable("Major");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.MajorCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Majors)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Major__Departmen__6383C8BA");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("Member");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Member__StudentI__6477ECF3");
            });

            modelBuilder.Entity<MemberTakesActivity>(entity =>
            {
                entity.ToTable("MemberTakesActivity");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClubActivityId).HasColumnName("ClubActivityID");

                entity.Property(e => e.Deadline).HasColumnType("datetime");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.ClubActivity)
                    .WithMany(p => p.MemberTakesActivities)
                    .HasForeignKey(d => d.ClubActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MemberTak__ClubA__656C112C");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.MemberTakesActivities)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MemberTak__Membe__66603565");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("Participant");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompetitionId).HasColumnName("CompetitionID");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.Property(e => e.RegisterTime).HasColumnType("datetime");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Participa__Compe__6754599E");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("FK__Participa__Membe__68487DD7");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Participa__Stude__693CA210");
            });

            modelBuilder.Entity<ParticipantInTeam>(entity =>
            {
                entity.ToTable("ParticipantInTeam");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ParticipantId).HasColumnName("ParticipantID");

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.ParticipantInTeams)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Participa__Parti__6A30C649");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.ParticipantInTeams)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Participa__TeamI__6B24EA82");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SeedsWallet>(entity =>
            {
                entity.ToTable("SeedsWallet");

                entity.Property(e => e.Id)
                    .HasMaxLength(20)
                    .HasColumnName("ID");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.SeedsWallets)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SeedsWall__Stude__6C190EBB");
            });

            modelBuilder.Entity<Sponsor>(entity =>
            {
                entity.ToTable("Sponsor");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Logo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Sponsors)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sponsor__UserID__6D0D32F4");
            });

            modelBuilder.Entity<SponsorInCompetition>(entity =>
            {
                entity.ToTable("SponsorInCompetition");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompetitionId).HasColumnName("CompetitionID");

                entity.Property(e => e.SponsorId).HasColumnName("SponsorID");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.SponsorInCompetitions)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SponsorIn__Compe__6E01572D");

                entity.HasOne(d => d.Sponsor)
                    .WithMany(p => p.SponsorInCompetitions)
                    .HasForeignKey(d => d.SponsorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SponsorIn__Spons__6EF57B66");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Team");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompetitionId).HasColumnName("CompetitionID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.InvitedCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Team__Competitio__6FE99F9F");
            });

            modelBuilder.Entity<University>(entity =>
            {
                entity.ToTable("University");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.Closing)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Founding).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Openning)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.UniCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Universities)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Universit__CityI__70DDC3D8");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Dob)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOB");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.MajorId).HasColumnName("MajorID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UniversityId).HasColumnName("UniversityID");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("UserID");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.MajorId)
                    .HasConstraintName("FK__User__MajorID__71D1E811");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__User__RoleID__72C60C4A");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK__User__University__73BA3083");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}