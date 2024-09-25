using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Services
{
    public class PointsResetService
    {
        private readonly IUserRepository _userRepository; 

        public PointsResetService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ResetPoints()
        {
            var today = DateTime.Now;

            if (today.Day == 1 && today.Month == 6)
            {
                var users = await _userRepository.GetAsync();
                foreach (var user in users)
                {
                    user.Point = 400;
                    await _userRepository.UpdateAsync(user.Id, user);
                }
            }

            if (today.Day == 16 && today.Month == 12)
            {
                var users = await _userRepository.GetAsync();
                foreach (var user in users)
                {
                    user.Point += 500;
                    await _userRepository.UpdateAsync(user.Id, user);
                }
            }
        }
    }
}