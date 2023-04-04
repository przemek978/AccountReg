using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AccountReg.Models;

public partial class UserContext : DbContext
{
    public UserContext()
    {
    }

    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=UserDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Pesel).HasName("PK__User__4F16EE7ED245505D");

            entity.ToTable("User");

            entity.Property(e => e.Pesel)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("PESEL");
            entity.Property(e => e.AvgCon)
                .HasColumnType("decimal(18, 3)")
                .HasColumnName("AVG_Con");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
