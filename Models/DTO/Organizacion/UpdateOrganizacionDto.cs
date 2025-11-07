namespace Personal.UI.Models.DTO.Organizacion
{
    public class UpdateOrganizacionDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Responsable { get; set; }
    }
}
