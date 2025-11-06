using System;
using System.Collections.Generic;

namespace Personal.UI.Models.Domain;

public partial class Concepto
{
    public Guid Id { get; set; }

    public Guid OrganizacionId { get; set; }

    public string Clave { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int TipoConceptoId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual ICollection<NotificacionDet> NotificacionDets { get; set; } = new List<NotificacionDet>();

    public virtual Organizacion Organizacion { get; set; } = null!;

    public virtual CatTipoConcepto TipoConcepto { get; set; } = null!;
}
