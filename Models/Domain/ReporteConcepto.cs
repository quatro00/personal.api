using System;
using System.Collections.Generic;

namespace personal.api.Models.Domain;

public partial class ReporteConcepto
{
    public Guid Id { get; set; }

    public Guid OrganizacionId { get; set; }

    public string Quincena { get; set; } = null!;

    public string Matricula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Duracion { get; set; } = null!;

    public int Turno { get; set; }

    public string Autorizo { get; set; } = null!;

    public string Depto { get; set; } = null!;

    public string Concepto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Cat { get; set; } = null!;

    public string IncEntrada { get; set; } = null!;

    public string IncSalida { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual Organizacion Organizacion { get; set; } = null!;
}
