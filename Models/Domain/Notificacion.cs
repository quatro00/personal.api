using System;
using System.Collections.Generic;

namespace personal.api.Models.Domain;

public partial class Notificacion
{
    public Guid Id { get; set; }

    public string Quincena { get; set; } = null!;

    public Guid OrganizacionId { get; set; }

    public string Matricula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Para { get; set; } = null!;

    public string Cc { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public int EstatusNotificacionId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; } = null!;

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual ICollection<NotificacionDet> NotificacionDets { get; set; } = new List<NotificacionDet>();

    public virtual Organizacion Organizacion { get; set; } = null!;
}
