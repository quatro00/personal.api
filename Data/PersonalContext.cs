using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using personal.api.Models.Domain;

namespace personal.api.Data;

public partial class PersonalContext : DbContext
{
    public PersonalContext()
    {
    }

    public PersonalContext(DbContextOptions<PersonalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CatTipoConcepto> CatTipoConceptos { get; set; }

    public virtual DbSet<Concepto> Conceptos { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<NotificacionDet> NotificacionDets { get; set; }

    public virtual DbSet<Organizacion> Organizacions { get; set; }

    public virtual DbSet<ReporteConcepto> ReporteConceptos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-JM00DK5;Initial Catalog=Personal;Persist Security Info=True;User ID=sa;Password=sql2;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatTipoConcepto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CatTipoConcepto_1");

            entity.ToTable("CatTipoConcepto");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion).HasMaxLength(50);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(50);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(50);
        });

        modelBuilder.Entity<Concepto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CatTipoConcepto");

            entity.ToTable("Concepto");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Clave).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(50);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(50);

            entity.HasOne(d => d.Organizacion).WithMany(p => p.Conceptos)
                .HasForeignKey(d => d.OrganizacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Concepto_Organizacion");

            entity.HasOne(d => d.TipoConcepto).WithMany(p => p.Conceptos)
                .HasForeignKey(d => d.TipoConceptoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Concepto_CatTipoConcepto");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.ToTable("Notificacion");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Cc)
                .HasMaxLength(250)
                .HasColumnName("CC");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Matricula).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(250);
            entity.Property(e => e.Para).HasMaxLength(250);
            entity.Property(e => e.Quincena).HasMaxLength(50);
            entity.Property(e => e.Titulo).HasMaxLength(250);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(50);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(50);

            entity.HasOne(d => d.Organizacion).WithMany(p => p.Notificacions)
                .HasForeignKey(d => d.OrganizacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notificacion_Organizacion");
        });

        modelBuilder.Entity<NotificacionDet>(entity =>
        {
            entity.ToTable("NotificacionDet");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.Concepto).WithMany(p => p.NotificacionDets)
                .HasForeignKey(d => d.ConceptoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotificacionDet_Concepto");

            entity.HasOne(d => d.Notificacion).WithMany(p => p.NotificacionDets)
                .HasForeignKey(d => d.NotificacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotificacionDet_Notificacion");
        });

        modelBuilder.Entity<Organizacion>(entity =>
        {
            entity.ToTable("Organizacion");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Clave).HasMaxLength(500);
            entity.Property(e => e.Direccion).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(500);
            entity.Property(e => e.Responsable).HasMaxLength(500);
            entity.Property(e => e.Telefono).HasMaxLength(500);
        });

        modelBuilder.Entity<ReporteConcepto>(entity =>
        {
            entity.ToTable("ReporteConcepto");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Autorizo).HasMaxLength(50);
            entity.Property(e => e.Cat).HasMaxLength(50);
            entity.Property(e => e.Concepto).HasMaxLength(50);
            entity.Property(e => e.Depto).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Duracion).HasMaxLength(50);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.IncEntrada).HasMaxLength(50);
            entity.Property(e => e.IncSalida).HasMaxLength(50);
            entity.Property(e => e.Matricula).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(250);
            entity.Property(e => e.Quincena).HasMaxLength(50);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(50);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(50);

            entity.HasOne(d => d.Organizacion).WithMany(p => p.ReporteConceptos)
                .HasForeignKey(d => d.OrganizacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReporteConcepto_Organizacion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
