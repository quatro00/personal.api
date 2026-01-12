using Microsoft.EntityFrameworkCore;

namespace Personal.UI.Models.DTO.Notificacion
{
    [Keyless]
    public class CorreoTrabajadorDto
    {
        public string Correo { get; set; }
    }
}
