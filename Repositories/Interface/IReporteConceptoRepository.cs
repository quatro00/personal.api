using Farmacia.UI.Models;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Notificacion;

namespace Personal.UI.Repositories.Interface
{
    public interface IReporteConceptoRepository : IGenericRepository<ReporteConcepto>
    {
        Task GuardarReporteConBitacoraAsync(IEnumerable<ReporteConcepto> conceptos,ReporteConceptosBitacora bitacora);
        Task<List<NotificacionDto>> CalcularNotificaciones(Guid organizacionId);
        Task<List<NotificacionDto>> ConsultarNotificaciones(Guid organizacionId, string quincena);
        Task<ResponseModel> EnviarNotificaciones(List<NotificacionDto> model);
    }
}
