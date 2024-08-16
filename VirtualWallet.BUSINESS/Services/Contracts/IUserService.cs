﻿using ForumProject.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IUserService
    {
        public Task<User> GetUserByIdAsync(int userId);

        public Task<UserProfile> GetUserProfileAsync(int userId);

        public Task UpdateUserAsync(User user);

        public Task UpdateUserProfileAsync(UserProfile userProfile);

        public Task DeleteUserAsync(int userId);
    }
}
