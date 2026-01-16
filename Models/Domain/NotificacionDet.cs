using System;
using System.Collections.Generic;

namespace Personal.UI.Models.Domain;

public partial class NotificacionDet
{
    public Guid Id { get; set; }

    public Guid NotificacionId { get; set; }

    public string Fecha { get; set; } = null!;

    public string Concepto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string IncEnt { get; set; } = null!;

    public string IncSal { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; } = null!;

    public virtual Notificacion Notificacion { get; set; } = null!;
}
