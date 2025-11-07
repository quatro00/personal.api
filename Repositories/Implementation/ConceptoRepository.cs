using Microsoft.EntityFrameworkCore;
using Personal.UI.Data;
using Personal.UI.Models.Domain;
using Personal.UI.Repositories.Interface;

namespace Personal.UI.Repositories.Implementation
{
    public class ConceptoRepository : GenericRepository<Concepto>, IConceptoRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<Concepto> _dbSet;

        public ConceptoRepository(PersonalContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<Concepto>();
        }

    }
}
