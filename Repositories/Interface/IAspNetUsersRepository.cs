using personal.api.Models;
using personal.api.Models.DTO.Admin.Auth;

namespace personal.api.Repositories.Interface
{
    public interface IAspNetUsersRepository
    {
        Task<ResponseModel> GetUserById(Guid id);
        Task<ResponseModel> ForgotPassword(ForgotPasswordRequestDto model, string token);
    }
}
