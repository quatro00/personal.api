using System;
using System.Collections.Generic;

namespace Personal.UI.Models.Domain;

public partial class NotificacionDet
{
    public Guid Id { get; set; }

    public Guid NotificacionId { get; set; }

    public Guid ConceptoId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public Guid UsuarioCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public Guid? UsuarioModificacion { get; set; }

    public virtual Concepto Concepto { get; set; } = null!;

    public virtual Notificacion Notificacion { get; set; } = null!;
}
