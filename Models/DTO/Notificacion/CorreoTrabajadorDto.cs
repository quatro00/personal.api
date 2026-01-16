using Microsoft.EntityFrameworkCore;

namespace Personal.UI.Models.DTO.Notificacion
{
    [Keyless]
    public class CorreoTrabajadorDto
    {
        public string Correo { get; set; }
    }

    [Keyless]
    public class CorreoDto
    {
        public int Matricula { get; set; }
        public string Correo { get; set; }
    }
}
