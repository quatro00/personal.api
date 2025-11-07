using AutoMapper;
using Farmacia.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Configuracion;
using Personal.UI.Repositories.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Personal.UI.Controllers.Admin
{
    [Route("api/administrador/[controller]")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IReporteConceptoRepository reporteConceptoRepository;

        public ConfiguracionController(
            IMapper mapper,
            IReporteConceptoRepository reporteConceptoRepository
            )
        {
            this.mapper = mapper;
            this.reporteConceptoRepository = reporteConceptoRepository;
        }

        [HttpPost("ReporteDeConceptos")]
        [Authorize(Roles = "Administrador")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> Crear([FromForm] ActualizarReporteConceptosDto model)
        {
            // Validar si el modelo es válido
            if (!ModelState.IsValid)
            {
                User.GetId();
                return BadRequest("Modelo de datos invalido.");
            }

            try
            {
                IFormFile excel = model.Archivo;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var lista = new List<ReporteConcepto>();

                using (var stream = new MemoryStream())
                {
                    excel.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var ws = package.Workbook.Worksheets.First();
                        int startRow = 2; // DATOS INICIAN EN LA FILA 2 (ajústalo si es necesario)
                        for (int row = startRow; row <= ws.Dimension.End.Row; row++)
                        {
                            var matricula = ws.Cells[row, 1].Text?.Trim();
                            if (string.IsNullOrWhiteSpace(matricula))
                                continue;

                            if (!int.TryParse(matricula, out int matriculaInt))
                                continue;

                            var item = new ReporteConcepto
                            {
                                Matricula = matricula,
                                Nombre = ws.Cells[row, 7].Text?.Trim(),
                                Fecha = ws.Cells[row, 12].Text?.Trim(),
                                Duracion = ws.Cells[row, 16].Text?.Trim(),
                                Turno = ws.Cells[row, 18].Text?.Trim(),
                                Autorizo = ws.Cells[row, 20].Text?.Trim(),
                                Depto = ws.Cells[row, 23].Text?.Trim(),
                                Concepto = ws.Cells[row, 26].Text?.Trim(),
                                Descripcion = ws.Cells[row, 28].Text?.Trim(),
                                Cat = ws.Cells[row, 33].Text?.Trim(),
                                IncEntrada = ws.Cells[row, 38].Text?.Trim(),
                                IncSalida = ws.Cells[row, 40].Text?.Trim(),
                                Activo = true,
                                OrganizacionId = model.OrganizacionId,
                                Quincena = model.Quincena,
                                FechaCreacion = DateTime.Now,
                                UsuarioCreacion = User.GetId()
                            };

                            lista.Add(item);
                        }
                    }
                }

                //guardamos los datos
                ReporteConceptosBitacora bitacora = new ReporteConceptosBitacora()
                {
                    OrganizacionId = model.OrganizacionId,
                    Quincena = model.Quincena,
                    Registros = lista.Count,
                    UsuarioCreacion = User.GetId(),
                    FechaCreacion = DateTime.Now,
                    Activo = true,
                    Fecha = DateTime.Now,
                };

                await reporteConceptoRepository.GuardarReporteConBitacoraAsync(lista, bitacora);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException.Message); // O devolver un BadRequest(400) si el error es de entrada
            }
        }
    }
}
