namespace Personal.UI.Models.DTO.Notificacion
{
    public class NotificacionDto
    {
        public string Quincena { get; set; }
        public string Matricula { get; set; }
        public string Nombre { get; set; }
        public List<NotificacionDetDto> Detalle { get; set; }
    }

    public class NotificacionDetDto
    {
        public string Fecha { get; set; }
        public string Concepto { get; set; }
        public string Descripcion { get; set; }
        public string IncEnt { get; set; }
        public string IncSal { get; set; }
    }
}
