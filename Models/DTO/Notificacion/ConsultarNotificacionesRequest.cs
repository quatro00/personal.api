namespace Personal.UI.Models.DTO.Notificacion
{
    public class ConsultarNotificacionesRequest
    {
        public Guid OrganizacionId { get; set; }
        public string Quincena { get; set; }
    }
}
