using System;
using Auth.DB;
using Auth.Models;
using AutoMapper;

namespace Auth.Repositories
{
    public interface IPrivilegeRepository : IBaseRepository<Privilege, Guid>
    {
    }

    public class PrivilegeRepository : BaseRepository<Privilege, Guid>, IPrivilegeRepository
    {
        private readonly AuthContext _db;

        public PrivilegeRepository(AuthContext context, IMapper mapper) : base(context, mapper)
        {
            _db = context;
        }
    }
}