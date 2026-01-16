using System;
using System.Collections.Generic;

namespace Personal.UI.Models.Domain;

public partial class Notificacion
{
    public Guid Id { get; set; }

    public Guid OrganizacionId { get; set; }

    public string Quincena { get; set; } = null!;

    public string Matricula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Cc { get; set; } = null!;

    public bool Enviado { get; set; }

    public string Mensaje { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; } = null!;

    public virtual ICollection<NotificacionDet> NotificacionDets { get; set; } = new List<NotificacionDet>();

    public virtual Organizacion Organizacion { get; set; } = null!;
}
