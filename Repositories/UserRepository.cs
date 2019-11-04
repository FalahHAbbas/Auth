using System;
using Auth.DB;
using Auth.Models;
using AutoMapper;

namespace Auth.Repositories
{
    public interface IUserRepository : IBaseRepository<User, Guid>
    {
    }

    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        private readonly AuthContext _db;

        public UserRepository(AuthContext context, IMapper mapper) : base(context, mapper)
        {
            _db = context;
        }
    }
}