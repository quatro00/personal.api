using AutoMapper;
using Farmacia.UI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Personal.UI.Models;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Concepto;
using Personal.UI.Models.DTO.Organizacion;
using Personal.UI.Models.Specifications;
using Personal.UI.Repositories.Interface;

namespace Personal.UI.Controllers.Admin
{
    [Route("api/administrador/[controller]")]
    [ApiController]
    public class ConceptoController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IConceptoRepository conceptoRepository;

        public ConceptoController(
            IMapper mapper,
            IConceptoRepository conceptoRepository
            )
        {
            this.mapper = mapper;
            this.conceptoRepository = conceptoRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear([FromBody] CrearConceptoDto model)
        {
            // Validar si el modelo es válido
            if (!ModelState.IsValid)
            {
                User.GetId();
                return BadRequest("Modelo de datos invalido.");
            }

            try
            {
                var dto = mapper.Map<Concepto>(model);
                dto.UsuarioCreacion = User.GetId();
                dto.FechaCreacion = DateTime.Now;
                // Agregar el paciente al repositorio
                await this.conceptoRepository.AddAsync(dto);

                // Devolver la respuesta con el nuevo paciente
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException.Message); // O devolver un BadRequest(400) si el error es de entrada
            }
        }
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(Guid id, [FromBody] CrearConceptoDto dto)
        {
            try
            {

                var model = await this.conceptoRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound("Concepto no encontrado.");
                }

                // Mapear solo los campos permitidos del DTO a la entidad
                mapper.Map(dto, model);

                model.Id = id;
                await this.conceptoRepository.UpdateAsync(model);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrio un error al actualizar.");
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                FiltroGlobal filtro = new FiltroGlobal() { IncluirInactivos = true };
                var spec = new ConceptoSpecification(filtro);
                spec.IncludeStrings = new List<string> { "TipoConcepto" };

                var result = await this.conceptoRepository.ListAsync(spec);
                if (result == null)
                {
                    return NotFound(result);
                }

                var dto = mapper.Map<List<ConceptoDto>>(result);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException.Message); // O devolver un BadRequest(400) si el error es de entrada
            }

        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}/desactivar")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            try
            {
                // Obtener el paciente actual desde la base de datos
                //UpdateContactoDto dto;

                var model = await this.conceptoRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound("Dato no encontrado.");
                }

                // Solo actualizamos el campo 'Activo' a false
                model.Activo = false;
                model.UsuarioModificacion = User.GetId();
                model.FechaModificacion = DateTime.Now;
                // Guardamos los cambios

                await this.conceptoRepository.UpdateAsync(model);

                return NoContent(); // Respuesta exitosa sin contenido
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrio un error"); // O devolver un BadRequest(400) si el error es de entrada
            }

        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}/activar")]
        public async Task<IActionResult> Activar(Guid id)
        {
            try
            {
                // Obtener el paciente actual desde la base de datos
                //UpdateContactoDto dto;

                var model = await this.conceptoRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound("Dato no encontrado.");
                }

                // Solo actualizamos el campo 'Activo' a false
                model.Activo = true;
                model.UsuarioModificacion = User.GetId();
                model.FechaModificacion = DateTime.Now;
                // Guardamos los cambios

                await this.conceptoRepository.UpdateAsync(model);

                return NoContent(); // Respuesta exitosa sin contenido
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrio un error"); // O devolver un BadRequest(400) si el error es de entrada
            }

        }
    }
}
