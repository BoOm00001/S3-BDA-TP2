using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Vols.Data.DTO;

namespace Vols.Data;

public partial class VolsContext : DbContext
{
    public VolsContext(DbContextOptions<VolsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aeroport> Aeroports { get; set; }

    public virtual DbSet<AeroportImport> AeroportImports { get; set; }

    public virtual DbSet<Circuit> Circuits { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Inscription> Inscriptions { get; set; }

    public virtual DbSet<Pay> Pays { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Ville> Ville { get; set; }

    public virtual DbSet<Vol> Vols { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aeroport>(entity =>
        {
            entity.HasKey(e => e.IataCode).HasName("PK__aeroport__B78B655B3C7197EE");

            entity.ToTable("aeroport");

            entity.Property(e => e.IataCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IATA_CODE");
            entity.Property(e => e.IdVille).HasColumnName("id_ville");
            entity.Property(e => e.NomAeroport)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nom_aeroport");

            entity.HasOne(d => d.IdVilleNavigation).WithMany(p => p.Aeroports)
                .HasForeignKey(d => d.IdVille)
                .HasConstraintName("FK__aeroport__id_vil__3F466844");
        });

        modelBuilder.Entity<AeroportImport>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Aeroport_Import");

            entity.Property(e => e.IataCode)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("IATA_code");
            entity.Property(e => e.IdVille)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("id_ville");
            entity.Property(e => e.NomAeroport)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("nom_aeroport");
        });

        modelBuilder.Entity<Circuit>(entity =>
        {
            entity.HasKey(e => e.NoCircuit).HasName("PK__circuit__70E0A66795805205");

            entity.ToTable("circuit");

            entity.Property(e => e.NoCircuit)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("no_circuit");
            entity.Property(e => e.CodeDepart)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("code_depart");
            entity.Property(e => e.CodeDestination)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("code_destination");
            entity.Property(e => e.Duree).HasColumnName("duree");

            entity.HasOne(d => d.CodeDepartNavigation).WithMany(p => p.CircuitCodeDepartNavigations)
                .HasForeignKey(d => d.CodeDepart)
                .HasConstraintName("FK__circuit__code_de__4316F928");

            entity.HasOne(d => d.CodeDestinationNavigation).WithMany(p => p.CircuitCodeDestinationNavigations)
                .HasForeignKey(d => d.CodeDestination)
                .HasConstraintName("FK__circuit__code_de__440B1D61");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Noclient).HasName("PK__client__A4E3B1DC3F3AE562");

            entity.ToTable("Client");

            entity.Property(e => e.Noclient)
                .ValueGeneratedNever()
                .HasColumnName("noclient");
            entity.Property(e => e.AdresseClient)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("adresse_client");
            entity.Property(e => e.CodepostalClient)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("codepostal_client");
            entity.Property(e => e.IdVille).HasColumnName("id_ville");
            entity.Property(e => e.NomClient)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nom_client");
            entity.Property(e => e.TelClient)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tel_client");

            entity.HasOne(d => d.IdVilleNavigation).WithMany(p => p.Clients)
                .HasForeignKey(d => d.IdVille)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__client__id_ville__4AB81AF0");
        });

        modelBuilder.Entity<Inscription>(entity =>
        {
            entity.HasKey(e => new { e.Noclient, e.IdVol }).HasName("PK__inscript__5239729A761AA607");

            entity.ToTable("inscription", tb => tb.HasTrigger("tr_inscription"));

            entity.Property(e => e.Noclient).HasColumnName("noclient");
            entity.Property(e => e.IdVol).HasColumnName("id_vol");
            entity.Property(e => e.DateInscription)
                .HasColumnType("datetime")
                .HasColumnName("date_inscription");

            entity.HasOne(d => d.IdVolNavigation).WithMany(p => p.Inscriptions)
                .HasForeignKey(d => d.IdVol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inscripti__id_vo__4E88ABD4");

            entity.HasOne(d => d.NoclientNavigation).WithMany(p => p.Inscriptions)
                .HasForeignKey(d => d.Noclient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inscripti__nocli__4D94879B");
        });

        modelBuilder.Entity<Pay>(entity =>
        {
            entity.HasKey(e => e.IdPays).HasName("PK__pays__09412DABC7914B61");

            entity.ToTable("pays");

            entity.Property(e => e.IdPays)
                .ValueGeneratedNever()
                .HasColumnName("id_pays");
            entity.Property(e => e.CodeMonnaie)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("code_monnaie");
            entity.Property(e => e.NomPays)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nom_pays");
            entity.Property(e => e.Taux)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("taux");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.IdProvince).HasName("PK__province__E068312C7B5E54DB");

            entity.ToTable("provinces");

            entity.Property(e => e.IdProvince)
                .ValueGeneratedNever()
                .HasColumnName("id_province");
            entity.Property(e => e.IdPays).HasColumnName("id_pays");
            entity.Property(e => e.NomProvince)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nom_province");

            entity.HasOne(d => d.IdPaysNavigation).WithMany(p => p.Provinces)
                .HasForeignKey(d => d.IdPays)
                .HasConstraintName("FK__provinces__id_pa__398D8EEE");
        });

        modelBuilder.Entity<Ville>(entity =>
        {
            entity.HasKey(e => e.IdVille).HasName("PK__ville__299FE62285820E97");

            entity.Property(e => e.IdVille)
                .ValueGeneratedNever()
                .HasColumnName("id_ville");
            entity.Property(e => e.IdProvince).HasColumnName("id_province");
            entity.Property(e => e.NomVille)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nom_ville");

            entity.HasOne(d => d.IdProvinceNavigation).WithMany(p => p.Villes)
                .HasForeignKey(d => d.IdProvince)
                .HasConstraintName("FK__ville__id_provin__3C69FB99");
        });

        modelBuilder.Entity<Vol>(entity =>
        {
            entity.HasKey(e => e.IdVol).HasName("PK__Vols__6DAC346E5742460E");

            entity.Property(e => e.IdVol)
                .ValueGeneratedNever()
                .HasColumnName("id_vol");
            entity.Property(e => e.DateDepart)
                .HasColumnType("datetime")
                .HasColumnName("date_depart");
            entity.Property(e => e.Nbplacemax).HasColumnName("nbplacemax");
            entity.Property(e => e.NoCircuit)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("noCircuit");
            entity.Property(e => e.Prix).HasColumnName("prix");

            entity.HasOne(d => d.NoCircuitNavigation).WithMany(p => p.Vols)
                .HasForeignKey(d => d.NoCircuit)
                .HasConstraintName("FK__Vols__noCircuit__47DBAE45");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
