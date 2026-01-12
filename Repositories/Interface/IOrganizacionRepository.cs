using Farmacia.UI.Models;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Notificacion;

namespace Personal.UI.Repositories.Interface
{
    public interface IOrganizacionRepository : IGenericRepository<Organizacion>
    {
        Task<ResponseModel> GetQuincenas(Guid organizacionId);
    }
}
