using System;
using System.Collections.Generic;

namespace Personal.UI.Models.Domain;

public partial class ReporteConceptosBitacora
{
    public Guid Id { get; set; }

    public Guid OrganizacionId { get; set; }

    public DateTime Fecha { get; set; }

    public int Registros { get; set; }

    public string Quincena { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual Organizacion Organizacion { get; set; } = null!;
}
