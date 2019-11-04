using System;
using Auth.DB;
using Auth.Models;
using AutoMapper;

namespace Auth.Repositories
{
    public interface IRoleRepository : IBaseRepository<Role, Guid>
    {
    }

    public class RoleRepository : BaseRepository<Role, Guid>, IRoleRepository
    {
        private readonly AuthContext _db;

        public RoleRepository(AuthContext context, IMapper mapper) : base(context, mapper)
        {
            _db = context;
        }
    }
}