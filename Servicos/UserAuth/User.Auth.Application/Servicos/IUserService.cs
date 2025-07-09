using System.Threading.Tasks;
using UserAuth.Aplicacao.DTOs;

namespace UserAuth.Aplicacao.Servicos
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(string username);
        Task<bool> UpdateProfileAsync(string username, UpdateUserProfileDto updateUserProfileDto);
    }
}
