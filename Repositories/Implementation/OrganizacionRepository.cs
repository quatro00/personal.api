using Farmacia.UI.Models;
using Microsoft.EntityFrameworkCore;
using Personal.UI.Data;
using Personal.UI.Models.Domain;
using Personal.UI.Repositories.Interface;
using System.Net.Mail;
using System.Net;
using Personal.UI.Models.DTO.Organizacion;

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

        public async Task<ResponseModel> GetQuincenas(Guid organizacionId)
        {
            ResponseModel resultado = new ResponseModel();
            try
            {
                List<GetQuincenaDto> res = new List<GetQuincenaDto>();
                var quicenas = await _context.Set<ReporteConceptosBitacora>().Where(x => x.OrganizacionId == organizacionId).Select(x => x.Quincena).Distinct().ToListAsync();

                foreach (var item in quicenas) 
                {
                    res.Add(new GetQuincenaDto()
                    {
                        id = item,
                        nombre = item,
                    });
                }
                resultado.result = res;
                resultado.SetResponse(true);
            }
            catch (Exception ioe)
            {
                resultado.SetResponse(false, ioe.InnerException.ToString());
            }

            return resultado;
        }
    }
}
