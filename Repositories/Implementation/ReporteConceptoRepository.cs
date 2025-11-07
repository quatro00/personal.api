using Microsoft.EntityFrameworkCore;
using Personal.UI.Data;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Notificacion;
using Personal.UI.Repositories.Interface;

namespace Personal.UI.Repositories.Implementation
{
    public class ReporteConceptoRepository : GenericRepository<ReporteConcepto>, IReporteConceptoRepository
    {
        private readonly DbContext _context;
        private readonly DbSet<ReporteConcepto> _dbSet;

        public ReporteConceptoRepository(PersonalContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<ReporteConcepto>();
        }

        public async Task<List<NotificacionDto>> CalcularNotificaciones(Guid organizacionId)
        {
            //buscamos los datos del ultimo reporte cargado
            var reporteConceptos = _context.Set<ReporteConcepto>();
            var bitacora = _context.Set<ReporteConceptosBitacora>();
            var conceptos = _context.Set<Concepto>();

            var bitacoraList = await bitacora.Where(x=>x.OrganizacionId == organizacionId).OrderByDescending(x=>x.Fecha).FirstOrDefaultAsync();
            var reporteConceptosList = await reporteConceptos.Where(x=>x.OrganizacionId == bitacoraList.OrganizacionId && x.Quincena == bitacoraList.Quincena).ToListAsync();
            var conceptosList = await conceptos.Where(x=>x.OrganizacionId == organizacionId && x.Activo == true).ToListAsync();

            var conceptosGenerales = conceptosList.Where(x => x.TipoConceptoId == 1).Select(x => x.Clave).ToArray();
            var conceptosEntrada = conceptosList.Where(x => x.TipoConceptoId == 2).Select(x => x.Clave).ToArray();
            var conceptosSalida = conceptosList.Where(x => x.TipoConceptoId == 3).Select(x => x.Clave).ToArray();

            reporteConceptosList = 
                reporteConceptosList.Where(x => 
                    conceptosGenerales.Contains(x.Concepto) || 
                    conceptosEntrada.Contains(x.IncEntrada) || 
                    conceptosSalida.Contains(x.IncSalida))
                .ToList();

            var resultado = reporteConceptosList
            .GroupBy(x => new { x.Quincena, x.Matricula, x.Nombre })
            .Select(g => new NotificacionDto
            {
                Quincena = g.Key.Quincena,
                Matricula = g.Key.Matricula,
                Nombre = g.Key.Nombre,
                Detalle = g.Select(x => new NotificacionDetDto
                {
                    Fecha = x.Fecha,
                    Concepto = x.Concepto,
                    Descripcion = x.Descripcion,
                    IncEnt = x.IncEntrada,
                    IncSal = x.IncSalida
                }).OrderByDescending(x=>x.Fecha).ToList()
            })
            .ToList();

            return resultado;
        }

        public async Task GuardarReporteConBitacoraAsync(IEnumerable<ReporteConcepto> conceptos, ReporteConceptosBitacora bitacora)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //obtenemos los datos de quincena y organizacion
                var quincena = conceptos.First().Quincena;
                var organizacionId = conceptos.First().OrganizacionId;

                //borramos los datos anteriores
                var existentes = _dbSet.Where(x => x.Quincena == quincena && x.OrganizacionId == organizacionId);
                _dbSet.RemoveRange(existentes);


                //insertamos los datos nuevos
                await _dbSet.AddRangeAsync(conceptos);
                await _context.Set<ReporteConceptosBitacora>().AddAsync(bitacora);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
