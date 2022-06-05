using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Server.Models
{
    public partial class TreeDBContext : DbContext
    {
        public TreeDBContext()
        {
        }

        public TreeDBContext(DbContextOptions<TreeDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Datalogger> Dataloggers { get; set; } = null!;
        public virtual DbSet<Measurement> Measurements { get; set; } = null!;
        public virtual DbSet<Plant> Plants { get; set; } = null!;
        public virtual DbSet<PlantType> PlantTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-4AN5991;Database=TreeDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Datalogger>(entity =>
            {
                entity.ToTable("datalogger");

                entity.Property(e => e.DataloggerId).HasColumnName("datalogger_id");

                entity.Property(e => e.MaxAirHumidity).HasColumnName("max__air_humidity");

                entity.Property(e => e.MaxAirTemperature).HasColumnName("max_air_temperature");

                entity.Property(e => e.MinAirHumidity).HasColumnName("min_air_humidity");

                entity.Property(e => e.MinAirTemperature).HasColumnName("min_air_temperature");
            });

            modelBuilder.Entity<Measurement>(entity =>
            {
                entity.ToTable("measurement");

                entity.Property(e => e.MeasurementId).HasColumnName("measurement_id");

                entity.Property(e => e.AirHumidity).HasColumnName("air_humidity");

                entity.Property(e => e.AirTemperature).HasColumnName("air_temperature");

                entity.Property(e => e.DataloggerId).HasColumnName("datalogger_id");

                entity.Property(e => e.MeasurementDate)
                    .HasColumnType("datetime")
                    .HasColumnName("measurement_date");

                entity.Property(e => e.PlantId).HasColumnName("plant_id");

                entity.Property(e => e.SoilIsDry).HasColumnName("soil_is_dry");

                entity.HasOne(d => d.Datalogger)
                    .WithMany(p => p.Measurements)
                    .HasForeignKey(d => d.DataloggerId)
                    .HasConstraintName("FK_measurement_datalogger");

                entity.HasOne(d => d.Plant)
                    .WithMany(p => p.Measurements)
                    .HasForeignKey(d => d.PlantId)
                    .HasConstraintName("FK_measurement_plant");
            });

            modelBuilder.Entity<Plant>(entity =>
            {
                entity.ToTable("plant");

                entity.Property(e => e.PlantId).HasColumnName("plant_id");

                entity.Property(e => e.DataloggerId).HasColumnName("datalogger_id");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.PlantTypeId).HasColumnName("plant_type_id");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.WarrantyStartDate)
                    .HasColumnType("date")
                    .HasColumnName("warranty_start_date");

                entity.HasOne(d => d.Datalogger)
                    .WithMany(p => p.Plants)
                    .HasForeignKey(d => d.DataloggerId)
                    .HasConstraintName("FK_plant_datalogger");

                entity.HasOne(d => d.PlantType)
                    .WithMany(p => p.Plants)
                    .HasForeignKey(d => d.PlantTypeId)
                    .HasConstraintName("FK_plant_plant_type");
            });

            modelBuilder.Entity<PlantType>(entity =>
            {
                entity.ToTable("plant_type");

                entity.Property(e => e.PlantTypeId).HasColumnName("plant_type_id");

                entity.Property(e => e.PlantTypeName)
                    .HasMaxLength(256)
                    .HasColumnName("plant_type_name")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
