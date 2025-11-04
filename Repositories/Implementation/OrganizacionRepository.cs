using Microsoft.EntityFrameworkCore;
using personal.api.Data;
using personal.api.Models.Domain;
using personal.api.Repositories.Interface;

namespace personal.api.Repositories.Implementation
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
