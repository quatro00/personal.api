using System;
using System.Collections.Generic;

namespace Personal.UI.Models.Domain;

public partial class VwEmpleado
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;
}
