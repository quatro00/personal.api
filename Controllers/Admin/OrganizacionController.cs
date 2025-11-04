using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using personal.api.Helpers;
using personal.api.Models.Domain;
using personal.api.Models.DTO.Organizacion;
using personal.api.Repositories.Interface;

namespace personal.api.Controllers.Admin
{
    [Route("api/administrador/[controller]")]
    [ApiController]
    public class OrganizacionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOrganizacionRepository organizacionRepository;

        public OrganizacionController(
            IMapper mapper,
            IOrganizacionRepository organizacionRepository
            )
        {
            this.mapper = mapper;
            this.organizacionRepository = organizacionRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear([FromBody] CrearOrganizacionDto model)
        {
            // Validar si el modelo es válido
            if (!ModelState.IsValid)
            {
                User.GetId();
                return BadRequest("Modelo de datos invalido.");
            }

            try
            {
                var dto = mapper.Map<Organizacion>(model);
                dto.UsuarioCreacion = Guid.Parse(User.GetId());
                // Agregar el paciente al repositorio
                await this.organizacionRepository.AddAsync(dto);

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
        public async Task<IActionResult> Actualizar(Guid id, [FromBody] CrearOrganizacionDto dto)
        {
            try
            {

                var model = await this.organizacionRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound("Paciente no encontrado.");
                }

                // Mapear solo los campos permitidos del DTO a la entidad
                mapper.Map(dto, model);

                model.Id = id;
                await this.organizacionRepository.UpdateAsync(model);

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
                var result = await this.organizacionRepository.ListAsync();
                if (result == null)
                {
                    return NotFound(result);
                }

                var dto = mapper.Map<List<OrganizacionDto>>(result);

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

                var model = await this.organizacionRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound("Dato no encontrado.");
                }

                // Solo actualizamos el campo 'Activo' a false
                model.Activo = false;
                model.UsuarioModificacion = Guid.Parse(User.GetId());
                model.FechaModificacion = DateTime.Now;
                // Guardamos los cambios

                await this.organizacionRepository.UpdateAsync(model);

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

                var model = await this.organizacionRepository.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound("Dato no encontrado.");
                }

                // Solo actualizamos el campo 'Activo' a false
                model.Activo = true;
                model.UsuarioModificacion = Guid.Parse(User.GetId());
                model.FechaModificacion = DateTime.Now;
                // Guardamos los cambios

                await this.organizacionRepository.UpdateAsync(model);

                return NoContent(); // Respuesta exitosa sin contenido
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrio un error"); // O devolver un BadRequest(400) si el error es de entrada
            }

        }
    }
}
