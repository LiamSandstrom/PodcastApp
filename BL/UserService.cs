using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DAL.MongoDB.Interfaces;
using Models;
using BL.Interfaces;

namespace BL
{
   public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService (IUserRepository userRepo, IUnitOfWork unitOfWork)
        {
            userRepository = userRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CreateUserAsync(string email)
        {
            try
            {
                if (!IsValidEmail(email))
                    return false;

                await _unitOfWork.StartTransactionAsync();
                var session = _unitOfWork.Session;

                var user = new User
                {
                    Email = email,
                    Name = email.Split('@')[0],
                    CreatedAt = DateTime.Now
                };

                await userRepository.AddAsync(user, session);

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
}
