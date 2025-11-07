using Personal.UI.Models.Domain;
using Personal.UI.Models.Interfaces;
using System.Linq.Expressions;

namespace Personal.UI.Models.Specifications
{
    public class ConceptoSpecification : ISpecification<Concepto>
    {
        public Expression<Func<Concepto, bool>> Criteria { get; }
        public List<string> IncludeStrings { get; set; }

        public ConceptoSpecification(FiltroGlobal filtro)
        {
            Criteria = p =>
                (filtro.IncluirInactivos || (p.Activo));
        }
    }
}
