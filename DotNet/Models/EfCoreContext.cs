using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Endpoints.Models;

public partial class EfCoreContext : DbContext
{
    public EfCoreContext(DbContextOptions<EfCoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<Flat> Flats { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Addresses_pkey");

            entity.ToTable("addresses", "rental");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.City)
                .HasColumnType("character varying")
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasColumnType("character varying")
                .HasColumnName("country");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Number)
                .HasColumnType("character varying")
                .HasColumnName("number");
            entity.Property(e => e.PostCode)
                .HasColumnType("character varying")
                .HasColumnName("post_code");
            entity.Property(e => e.Street)
                .HasColumnType("character varying")
                .HasColumnName("street");
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Building_pkey");

            entity.ToTable("building", "rental");

            entity.HasIndex(e => e.AddressId, "fki_fk_addressId");

            entity.HasIndex(e => e.OwnerId, "fki_fk_ownerId");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");

            entity.HasOne(d => d.Address).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_addressId");

            entity.HasOne(d => d.Owner).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ownerId");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Facilities_pkey");

            entity.ToTable("facilities", "rental");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasMany(d => d.Flats).WithMany(p => p.Facilities)
                .UsingEntity<Dictionary<string, object>>(
                    "FlatFacility",
                    r => r.HasOne<Flat>().WithMany()
                        .HasForeignKey("FlatId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_flatId"),
                    l => l.HasOne<Facility>().WithMany()
                        .HasForeignKey("FacilityId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_facilityId"),
                    j =>
                    {
                        j.HasKey("FacilityId", "FlatId").HasName("FlatFacility_pkey");
                        j.ToTable("flat_facility", "rental");
                        j.HasIndex(new[] { "FacilityId" }, "fki_fk_facilityId");
                        j.IndexerProperty<long>("FacilityId").HasColumnName("facility_id");
                        j.IndexerProperty<long>("FlatId").HasColumnName("flat_id");
                    });
        });

        modelBuilder.Entity<Flat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Flats_pkey");

            entity.ToTable("flats", "rental");

            entity.HasIndex(e => e.BuildingId, "fki_fk_buildingId");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BuildingId).HasColumnName("building_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.DailyPricePerPerson).HasColumnName("daily_price_per_person");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.FlatNumber)
                .HasColumnType("character varying")
                .HasColumnName("flat_number");

            entity.HasOne(d => d.Building).WithMany(p => p.Flats)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_buildingId");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Reservations_pkey");

            entity.ToTable("reservations", "rental");

            entity.HasIndex(e => e.FlatId, "fki_fk_flatId");

            entity.HasIndex(e => e.ReservedById, "fki_fk_reservedById");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FlatId).HasColumnName("flat_id");
            entity.Property(e => e.GuestNumber).HasColumnName("guest_number");
            entity.Property(e => e.ReservedById).HasColumnName("reserved_by_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.TotalCost).HasColumnName("total_cost");

            entity.HasOne(d => d.Flat).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.FlatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_flatId");

            entity.HasOne(d => d.ReservedBy).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ReservedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reservedById");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.ToTable("users", "rental");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("last_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
