using Microsoft.EntityFrameworkCore;
using Personal.UI.Data;
using Personal.UI.Models.Domain;
using Personal.UI.Repositories.Interface;

namespace Personal.UI.Repositories.Implementation
{
    public class OrganizacionRepository : GenericRepository<Organizacion>, IOrganizacionRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<Organizacion> _dbSet;

        public OrganizacionRepository(PersonalContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<Organizacion>();
        }

    }
}
