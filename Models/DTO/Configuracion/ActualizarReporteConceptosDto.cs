namespace Personal.UI.Models.DTO.Configuracion
{
    public class ActualizarReporteConceptosDto
    {
        public Guid OrganizacionId { get; set; }
        public string Quincena { get; set; }
        public IFormFile Archivo { get; set; }
    }
}
