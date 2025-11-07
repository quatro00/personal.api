namespace Personal.UI.Models.DTO.Concepto
{
    public class CrearConceptoDto
    {
        public Guid OrganizacionId { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public int TipoConceptoId {  get; set; }
    }
}
