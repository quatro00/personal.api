using Farmacia.UI.Models;
using Microsoft.EntityFrameworkCore;
using Personal.UI.Data;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Notificacion;
using Personal.UI.Repositories.Interface;
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

        public async Task<ResponseModel> EnviarNotificaciones(List<NotificacionDto> model)
        {
            ResponseModel resultado = new ResponseModel();
            try
            {
                var fromAddress = new MailAddress("f19668@365i.team", "UMAE 25 Adquisiciones");
                string fromPassword = "Suikoden2";

                var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Temp",
                "NotificacionIncidencias.html"
            );
                foreach(var item in model)
                {
                    string html = File.ReadAllText(path);
                    string tabla = "";
                    foreach(var det in item.Detalle)
                    {
                        tabla = tabla + "<tr>" +
                            "<td style=\"border:1px solid #dcdcdc;\">" + det.Fecha + "</td>" +
                            "<td style=\"border:1px solid #dcdcdc;\">" + det.Concepto + "</td>" +
                            "<td style=\"border:1px solid #dcdcdc;\">" + det.Descripcion + "</td>" +
                            "<td style=\"border:1px solid #dcdcdc; text-align:center;\">" + det.IncEnt + "</td>" +
                            "<td style=\"border:1px solid #dcdcdc; text-align:center;\">" + det.IncSal + "</td>" +
                            "</tr>";
                    }
                    html = html
                    .Replace("{{#Nombre}}", item.Nombre)
                    .Replace("{{#Quincena}}", item.Quincena)
                    .Replace("{{#Matricula}}", item.Matricula)
                    .Replace("{{#Tabla}}", tabla);

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.office365.com",
                        Port = 587,
                        UseDefaultCredentials = false,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                        TargetName = "STARTTLS/smtp.office365.com", // Set to avoid MustIssueStartTlsFirst exception
                        EnableSsl = true,
                    };
                    
                    using (var message = new MailMessage("f19668@365i.team", "josecarlosgarciadiaz@gmail.com")
                    {
                        IsBodyHtml = true,
                        Subject = "documentacion pendiente",
                        Body = "<h1>Prueba SMTP</h1><p>Si llega, SMTP funciona</p>"
                    })
                    {

                        //Attachment at = new Attachment(ruta, MediaTypeNames.Application.Octet);
                        //message.Attachments.Add(at);

                        //message.CC.Add(new MailAddress("alejandro.jimenezga@imss.gob.mx"));
                        try
                        {
                            smtp.Send(message);

                        }
                        catch (Exception ioe)
                        {
                            //marcamos el estatus como error en envio de notificacion
                            //(new PenalizacionBL()).ActuallizarEstatusPenalizacion(model.Folio, 5, SessionHelper.GetFullUser().Matricula);
                            int x = 0;
                        }

                    }

                }
            }
            catch
            {
                throw;
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
