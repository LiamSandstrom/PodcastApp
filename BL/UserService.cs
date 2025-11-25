using BL.Interfaces;
using DAL.MongoDB.Interfaces;
using Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BL
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMongoClient _client;

        public UserService(IUserRepository userRepo, IMongoClient mongoClient)
        {
            userRepository = userRepo;
            _client = mongoClient;
        }
        public async Task<bool> CreateUserAsync(string email)
        {
            try
            {

                if (!IsValidEmail(email))
                    return false;


                var user = new User
                {
                    Email = email,
                    Name = email.Split('@')[0],
                    CreatedAt = DateTime.Now
                };


                var result = await userRepository.AddAsync(user);

                return true;
            }
            catch (Exception ex)
            {
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
