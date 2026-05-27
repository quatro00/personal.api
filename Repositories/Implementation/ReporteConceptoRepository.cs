using Farmacia.UI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Personal.UI.Data;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Notificacion;
using Personal.UI.Repositories.Interface;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var bitacora = await _context.Set<ReporteConceptosBitacora>().Where(x => x.OrganizacionId == organizacionId).OrderByDescending(x => x.Fecha).FirstOrDefaultAsync();
            var reporteConceptosList = await _context.Set<ReporteConcepto>().Where(x => x.OrganizacionId == organizacionId && x.Quincena == bitacora.Quincena).ToListAsync();
            var conceptosList = await _context.Set<Concepto>().Where(x => x.OrganizacionId == organizacionId && x.Activo == true).ToListAsync();

            var dicGenerales = conceptosList
                .Where(x => x.TipoConceptoId == 1)
                .ToDictionary(x => x.Clave, x => x.Descripcion);

            var dicEntrada = conceptosList
                .Where(x => x.TipoConceptoId == 2)
                .ToDictionary(x => x.Clave, x => x.Descripcion);

            var dicSalida = conceptosList
                .Where(x => x.TipoConceptoId == 3)
                .ToDictionary(x => x.Clave, x => x.Descripcion);

            //var conceptosGenerales = conceptosList.Where(x => x.TipoConceptoId == 1).Select(x => x.Clave).ToArray();
            //var conceptosEntrada = conceptosList.Where(x => x.TipoConceptoId == 2).Select(x => x.Clave).ToArray();
            //var conceptosSalida = conceptosList.Where(x => x.TipoConceptoId == 3).Select(x => x.Clave).ToArray();

            reporteConceptosList = reporteConceptosList
            .Where(x =>
                dicGenerales.ContainsKey(x.Concepto) ||
                dicEntrada.ContainsKey(x.IncEntrada) ||
                dicSalida.ContainsKey(x.IncSalida))
            .ToList();

            var resultado = reporteConceptosList
                .GroupBy(x => new { x.Quincena, x.Matricula, x.Nombre })
                .Select(g => new NotificacionDto
                {
                    Quincena = g.Key.Quincena,
                    Matricula = g.Key.Matricula,
                    Nombre = g.Key.Nombre,
                    OrganizacionId = organizacionId,
                    Detalle = g.Select(x =>
                    {
                        string descripcionCatalogo = null;

                        if (!string.IsNullOrEmpty(x.Concepto) && dicGenerales.TryGetValue(x.Concepto, out var descGen))
                            descripcionCatalogo = descGen;
                        else if (!string.IsNullOrEmpty(x.IncEntrada) && dicEntrada.TryGetValue(x.IncEntrada, out var descEnt))
                            descripcionCatalogo = descEnt;
                        else if (!string.IsNullOrEmpty(x.IncSalida) && dicSalida.TryGetValue(x.IncSalida, out var descSal))
                            descripcionCatalogo = descSal;

                        return new NotificacionDetDto
                        {
                            Fecha = x.Fecha,
                            Concepto = x.Concepto,
                            Descripcion = descripcionCatalogo ?? "", // 👈 ahora viene del catálogo
                            IncEnt = x.IncEntrada,
                            IncSal = x.IncSalida
                        };
                    })
                    .OrderByDescending(x => x.Fecha)
                    .ToList()
                })
                .ToList();

            var matriculas = resultado
            .Select(x => int.Parse(x.Matricula.Trim()))
            .Distinct()
            .ToList();

            var table = new DataTable();
            table.Columns.Add("Matricula", typeof(int));

            foreach (var m in matriculas)
            {
                table.Rows.Add(m);
            }

            var param = new SqlParameter("@Matriculas", table)
            {
                TypeName = "dbo.MatriculaList"
            };

            /*
              var correoTrabajador = this._context.Set<CorreoTrabajadorDto>()
                .FromSqlRaw("EXEC SPQ_GetCorreoTrabajador @Matricula",
                    new SqlParameter("@Matricula", matricula))
                .AsEnumerable()
                .FirstOrDefault();
             */
            _context.Database.SetCommandTimeout(0);
            var correos = await _context.Set<CorreoDto>()
            .FromSqlRaw("EXEC SPQ_GetCorreosPorMatriculas @Matriculas", param)
            .ToListAsync();

            foreach (var item in correos)
            {
                foreach (var itm in resultado.Where(x => x.Matricula == item.Matricula.ToString()))
                {
                    itm.Correo = item.Correo;
                }

            }

            return resultado;
        }
        public async Task<List<NotificacionDto>> ConsultarNotificaciones(Guid organizacionId, string quincena)
        {
            //buscamos los datos del ultimo reporte cargado
            var reporteConceptosList = await _context.Set<ReporteConcepto>().Where(x => x.OrganizacionId == organizacionId && x.Quincena == quincena).ToListAsync();
            var bitacora =await _context.Set<ReporteConceptosBitacora>().ToListAsync();
            var conceptosList = await _context.Set<Concepto>().Where(x => x.OrganizacionId == organizacionId && x.Activo == true).ToListAsync();

            //var reporteConceptosList = await reporteConceptos.Where(x => x.OrganizacionId == organizacionId && x.Quincena == quincena).ToListAsync();
            //var conceptosList = await conceptos.Where(x => x.OrganizacionId == organizacionId && x.Activo == true).ToListAsync();

            var dicGenerales = conceptosList
                .Where(x => x.TipoConceptoId == 1)
                .ToDictionary(x => x.Clave, x => x.Descripcion);

            var dicEntrada = conceptosList
                .Where(x => x.TipoConceptoId == 2)
                .ToDictionary(x => x.Clave, x => x.Descripcion);

            var dicSalida = conceptosList
                .Where(x => x.TipoConceptoId == 3)
                .ToDictionary(x => x.Clave, x => x.Descripcion);

            //var conceptosGenerales = conceptosList.Where(x => x.TipoConceptoId == 1).Select(x => x.Clave).ToArray();
            //var conceptosEntrada = conceptosList.Where(x => x.TipoConceptoId == 2).Select(x => x.Clave).ToArray();
            //var conceptosSalida = conceptosList.Where(x => x.TipoConceptoId == 3).Select(x => x.Clave).ToArray();

            reporteConceptosList = reporteConceptosList
            .Where(x =>
                dicGenerales.ContainsKey(x.Concepto) ||
                dicEntrada.ContainsKey(x.IncEntrada) ||
                dicSalida.ContainsKey(x.IncSalida))
            .ToList();

            var resultado = reporteConceptosList
                .GroupBy(x => new { x.Quincena, x.Matricula, x.Nombre })
                .Select(g => new NotificacionDto
                {
                    Quincena = g.Key.Quincena,
                    Matricula = g.Key.Matricula,
                    Nombre = g.Key.Nombre,
                    Detalle = g.Select(x =>
                    {
                        string descripcionCatalogo = null;

                        if (!string.IsNullOrEmpty(x.Concepto) && dicGenerales.TryGetValue(x.Concepto, out var descGen))
                            descripcionCatalogo = descGen;
                        else if (!string.IsNullOrEmpty(x.IncEntrada) && dicEntrada.TryGetValue(x.IncEntrada, out var descEnt))
                            descripcionCatalogo = descEnt;
                        else if (!string.IsNullOrEmpty(x.IncSalida) && dicSalida.TryGetValue(x.IncSalida, out var descSal))
                            descripcionCatalogo = descSal;

                        return new NotificacionDetDto
                        {
                            Fecha = x.Fecha,
                            Concepto = x.Concepto,
                            Descripcion = descripcionCatalogo ?? "", // 👈 ahora viene del catálogo
                            IncEnt = x.IncEntrada,
                            IncSal = x.IncSalida
                        };
                    })
                    .OrderByDescending(x => x.Fecha)
                    .ToList()
                })
                .ToList();

            var matriculas = resultado
            .Select(x => int.Parse(x.Matricula.Trim()))
            .Distinct()
            .ToList();

            var table = new DataTable();
            table.Columns.Add("Matricula", typeof(int));

            foreach (var m in matriculas)
            {
                table.Rows.Add(m);
            }

            var param = new SqlParameter("@Matriculas", table)
            {
                TypeName = "dbo.MatriculaList"
            };

            /*
              var correoTrabajador = this._context.Set<CorreoTrabajadorDto>()
                .FromSqlRaw("EXEC SPQ_GetCorreoTrabajador @Matricula",
                    new SqlParameter("@Matricula", matricula))
                .AsEnumerable()
                .FirstOrDefault();
             */
            _context.Database.SetCommandTimeout(0);
            var correos = await _context.Set<CorreoDto>()
            .FromSqlRaw("EXEC SPQ_GetCorreosPorMatriculas @Matriculas", param)
            .ToListAsync();

            foreach (var item in correos)
            {
                foreach (var itm in resultado.Where(x => x.Matricula == item.Matricula.ToString()))
                {
                    itm.Correo = item.Correo;
                }

            }

            return resultado;
        }
        public string GetCorreo(string matricula)
        {
            try
            {
                var correoTrabajador = this._context.Set<CorreoTrabajadorDto>()
                .FromSqlRaw("EXEC SPQ_GetCorreoTrabajador @Matricula",
                    new SqlParameter("@Matricula", matricula))
                .AsEnumerable()
                .FirstOrDefault();
                if (correoTrabajador == null)
                {
                    correoTrabajador = new CorreoTrabajadorDto() { Correo = "" };
                }
                return correoTrabajador.Correo;
            }
            catch (Exception ex) {
                return "";
            }
            
        }
        public async Task<ResponseModel> EnviarNotificaciones(List<NotificacionDto> model, string usuarioId)
        {
            ResponseModel resultado = new ResponseModel();

            var notificaciones = new List<Notificacion>();

            var apiUrl = "https://apinotificacion.portalito.mx/api/integraciones/envios";
            var apiKey = "ec_live_dII3WV-7NJKNcuMrBY6E7O2X4Degp3LsxnPlV-78JfQ";

            var loteId = Guid.NewGuid().ToString("N");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            foreach (var item in model)
            {
                if (string.IsNullOrWhiteSpace(item.Correo))
                {
                    continue;
                }

                var idempotencyKey = $"notificacion-incidencia-{item.Matricula}-{item.Quincena}-{loteId}"
                    .Replace("/", "-")
                    .Replace(" ", "-")
                    .ToLower();

                var referenciaExterna = $"NOTIFICACION-{item.Matricula}-{item.Quincena}-{loteId}"
                    .Replace("/", "-")
                    .Replace(" ", "-")
                    .ToUpper();

                var request = new
                {
                    aplicacionClave = "IMSS_PERSONAL",
                    claveTemplate = "DOCUMENTACION_PENDIENTE",
                    referenciaExterna,
                    payload = new
                    {
                        Nombre = item.Nombre,
                        Quincena = item.Quincena,
                        Matricula = item.Matricula,
                        Tabla = item.Detalle.Select(det => new
                        {
                            Fecha = det.Fecha,
                            Concepto = det.Concepto,
                            Descripcion = det.Descripcion,
                            Entrada = det.IncEnt,
                            Salida = det.IncSal
                        }).ToList()
                    },
                    destinatarios = new[]
                    {
                new
                {
                    tipo = "TO",
                    email = item.Correo,
                    nombre = item.Nombre
                }
            },
                    prioridad = 5,
                    maxIntentos = 3,
                    procesarAhora = false
                };

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                httpRequest.Headers.Add("Idempotency-Key", idempotencyKey);
                httpRequest.Content = JsonContent.Create(request);

                var response = await httpClient.SendAsync(httpRequest);
                var responseBody = await response.Content.ReadAsStringAsync();

                // aquí manejas response igual que ya lo venías haciendo
            }

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
