namespace Personal.UI.Models.DTO.Concepto
{
    public class ConceptoDto
    {
        public Guid Id { get; set; }
        public Guid OrganizacionId { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public int TipoConceptoId { get; set; }
        public string TipoConcepto { get; set; }
        public bool Activo { get; set; }
    }
}
