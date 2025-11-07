using AutoMapper;
using Farmacia.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Concepto;
using Personal.UI.Models.DTO.Notificacion;
using Personal.UI.Repositories.Interface;

namespace Personal.UI.Controllers.Admin
{
    [Route("api/administrador/[controller]")]
    [ApiController]
    public class NotificacionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IReporteConceptoRepository reporteConceptoRepository;

        public NotificacionController(
            IMapper mapper,
            IReporteConceptoRepository reporteConceptoRepository
            )
        {
            this.mapper = mapper;
            this.reporteConceptoRepository = reporteConceptoRepository;
        }

        [HttpPost("CalcularNotificaciones")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CalcularNotificaciones([FromBody] CalcularNotificacionesRequest model)
        {
            // Validar si el modelo es válido
            if (!ModelState.IsValid)
            {
                User.GetId();
                return BadRequest("Modelo de datos invalido.");
            }

            try
            {
                var result = await this.reporteConceptoRepository.CalcularNotificaciones(model.OrganizacionId);
                //var dto = mapper.Map<Concepto>(model);
                //dto.UsuarioCreacion = User.GetId();
                //dto.FechaCreacion = DateTime.Now;
                //// Agregar el paciente al repositorio
                //await this.conceptoRepository.AddAsync(dto);

                //// Devolver la respuesta con el nuevo paciente
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException.Message); // O devolver un BadRequest(400) si el error es de entrada
            }
        }
    }
}
