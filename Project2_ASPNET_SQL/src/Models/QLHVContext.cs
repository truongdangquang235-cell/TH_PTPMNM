using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication1.Models
{
    public partial class QLHVContext : DbContext
    {
        public QLHVContext()
        {
        }

        public QLHVContext(DbContextOptions<QLHVContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DiemThi> DiemThis { get; set; } = null!;
        public virtual DbSet<Lop> Lops { get; set; } = null!;
        public virtual DbSet<LyLich> LyLiches { get; set; } = null!;
        public virtual DbSet<MonHoc> MonHocs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=QLHV;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiemThi>(entity =>
            {
                entity.HasKey(e => new { e.Mshv, e.Msmh });

                entity.ToTable("DiemThi");

                entity.Property(e => e.Mshv)
                    .HasMaxLength(50)
                    .HasColumnName("mshv");

                entity.Property(e => e.Msmh)
                    .HasMaxLength(50)
                    .HasColumnName("msmh");

                entity.Property(e => e.Diem)
                    .HasMaxLength(50)
                    .HasColumnName("diem");

                entity.HasOne(d => d.MshvNavigation)
                    .WithMany(p => p.DiemThis)
                    .HasForeignKey(d => d.Mshv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiemThi_LyLich");

                entity.HasOne(d => d.MsmhNavigation)
                    .WithMany(p => p.DiemThis)
                    .HasForeignKey(d => d.Msmh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiemThi_MonHoc");
            });

            modelBuilder.Entity<Lop>(entity =>
            {
                entity.HasKey(e => e.Malop);

                entity.ToTable("Lop");

                entity.Property(e => e.Malop)
                    .HasMaxLength(50)
                    .HasColumnName("malop");

                entity.Property(e => e.Tenlop)
                    .HasMaxLength(50)
                    .HasColumnName("tenlop");
            });

            modelBuilder.Entity<LyLich>(entity =>
            {
                entity.HasKey(e => e.Mshv);

                entity.ToTable("LyLich");

                entity.Property(e => e.Mshv)
                    .HasMaxLength(50)
                    .HasColumnName("mshv");

                entity.Property(e => e.Malop)
                    .HasMaxLength(50)
                    .HasColumnName("malop");

                entity.Property(e => e.Ngaysinh)
                    .HasColumnType("datetime")
                    .HasColumnName("ngaysinh");

                entity.Property(e => e.Phai).HasColumnName("phai");

                entity.Property(e => e.Tenhv)
                    .HasMaxLength(50)
                    .HasColumnName("tenhv");

                entity.HasOne(d => d.MalopNavigation)
                    .WithMany(p => p.LyLiches)
                    .HasForeignKey(d => d.Malop)
                    .HasConstraintName("FK_LyLich_Lop");
            });

            modelBuilder.Entity<MonHoc>(entity =>
            {
                entity.HasKey(e => e.Msmh);

                entity.ToTable("MonHoc");

                entity.Property(e => e.Msmh)
                    .HasMaxLength(50)
                    .HasColumnName("msmh");

                entity.Property(e => e.Sotiet).HasColumnName("sotiet");

                entity.Property(e => e.Tenmh)
                    .HasMaxLength(50)
                    .HasColumnName("tenmh");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
