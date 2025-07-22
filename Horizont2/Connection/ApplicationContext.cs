using System;
using System.Collections.Generic;
using System.Linq;
using Horizont.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Horizont.Connection
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Assortment> Assortments { get; set; }
        public virtual DbSet<Contrpartner> Contrpartners { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleDocument> SaleDocuments { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public List<Contrpartner> GetContrpartners()
        {
            return Contrpartners.ToList();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres ;Username=postgres ;Password=Ujhbpjyns_02042020");
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "ru-RU");

            modelBuilder.Entity<Assortment>(entity =>
            {
                entity.ToTable("assortment", "dipipe");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("assortment_id");

                entity.Property(e => e.AssortmentCategory)
                    .HasMaxLength(100)
                    .HasColumnName("assortment_category");

                entity.Property(e => e.AssortmentName)
                    .HasMaxLength(500)
                    .HasColumnName("assortment_name");

                entity.Property(e => e.Diameter).HasColumnName("diameter");

                entity.Property(e => e.Diameter2).HasColumnName("diameter2");

                entity.Property(e => e.Standard)
                    .HasMaxLength(100)
                    .HasColumnName("standard");

                entity.Property(e => e.SteelGrade)
                    .HasMaxLength(50)
                    .HasColumnName("steel_grade");

                entity.Property(e => e.Wall).HasColumnName("wall");

                entity.Property(e => e.Wall2).HasColumnName("wall2");
            });

            modelBuilder.Entity<Contrpartner>(entity =>
            {
                entity.ToTable("contrpartner", "dipipe");

                entity.HasComment("Контрагент");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("contrpartner_id");

                entity.Property(e => e.ContrpartnerInn).HasColumnName("contrpartner_inn");

                entity.Property(e => e.ContrpartnerName)
                    .HasMaxLength(200)
                    .HasColumnName("contrpartner_name");

                entity.Property(e => e.ContrpartnerStatus)
                    .HasMaxLength(50)
                    .HasColumnName("contrpartner_status");

                entity.Property(e => e.ContrpartnerType)
                    .HasMaxLength(200)
                    .HasColumnName("contrpartner_type");

                entity.Property(e => e.LevelSale)
                    .HasMaxLength(100)
                    .HasColumnName("level_sale");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role", "dipipe");

                entity.HasComment("Роль пользователя");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("role_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("sale", "dipipe");

                entity.HasComment("продажи");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("sale_id");

                entity.Property(e => e.AssortmentId).HasColumnName("assortment_id");

                entity.Property(e => e.SaleDocumentId).HasColumnName("sale_document_id");

                entity.Property(e => e.Tns).HasColumnName("tns");

                entity.HasOne(d => d.Assortment)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.AssortmentId)
                    .HasConstraintName("sale_assortment_fk");

                entity.HasOne(d => d.SaleDocument)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.SaleDocumentId)
                    .HasConstraintName("sale_sale_document_fk");
            });

            modelBuilder.Entity<SaleDocument>(entity =>
            {
                entity.ToTable("sale_document", "dipipe");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("sale_document_id");

                entity.Property(e => e.ContrpartnerId).HasColumnName("contrpartner_id");

                entity.Property(e => e.Division)
                    .HasMaxLength(100)
                    .HasColumnName("division");

                entity.Property(e => e.DocumentDate).HasColumnName("document_date");

                entity.Property(e => e.DocumentName)
                    .HasMaxLength(200)
                    .HasColumnName("document_name");

                entity.Property(e => e.ManagerName)
                    .HasMaxLength(200)
                    .HasColumnName("manager_name");

                entity.Property(e => e.Supplier)
                    .HasMaxLength(100)
                    .HasColumnName("supplier");

                entity.Property(e => e.Warehouse)
                    .HasMaxLength(200)
                    .HasColumnName("warehouse");

                entity.Property(e => e.WarehouseType)
                    .HasMaxLength(100)
                    .HasColumnName("warehouse_type");

                entity.HasOne(d => d.Contrpartner)
                    .WithMany(p => p.SaleDocuments)
                    .HasForeignKey(d => d.ContrpartnerId)
                    .HasConstraintName("sale_document_contrpartner_fk");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "dipipe");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("user_id");

                entity.Property(e => e.Hash)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("hash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("user_role_fk");
            });

            modelBuilder.HasSequence("contrpartner_sq", "dipipe");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
