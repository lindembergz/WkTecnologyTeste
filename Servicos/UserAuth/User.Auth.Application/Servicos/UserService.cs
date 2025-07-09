using System.Threading.Tasks;
using UserAuth.Aplicacao.DTOs;
using UserAuth.Dominio.Repositories;
using UserAuth.Dominio.UoW;

namespace UserAuth.Aplicacao.Servicos
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUserUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileDto> GetProfileAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }

            return new UserProfileDto
            {
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<bool> UpdateProfileAsync(string username, UpdateUserProfileDto updateUserProfileDto)
        {
            if (updateUserProfileDto.Username == null || updateUserProfileDto.Email == null)
            {
                return false;
            }

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            user.UpdateProfile(updateUserProfileDto.Username, updateUserProfileDto.Email);
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
