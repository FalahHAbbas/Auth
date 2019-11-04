using Auth.DB;
using AutoMapper;

namespace Auth.Repositories {
    public interface IRepositoryWrapper {
        IUserRepository UserRepository { get; }
        IPrivilegeRepository PrivilegeRepository { get; }
        IRoleRepository RoleRepository { get; }
    }

    public class RepositoryWrapper : IRepositoryWrapper {
        private readonly AuthContext _repoContext;
        private IUserRepository _userRepository;
        private IPrivilegeRepository _privilegeRepository;
        private IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RepositoryWrapper(AuthContext authContext, IMapper mapper) {
            _repoContext = authContext;
            _mapper = mapper;
        }

        public IRoleRepository RoleRepository =>
            _roleRepository ?? (_roleRepository = new RoleRepository(_repoContext, _mapper));

        public IPrivilegeRepository PrivilegeRepository =>
            _privilegeRepository ??
            (_privilegeRepository = new PrivilegeRepository(_repoContext, _mapper));

        public IUserRepository UserRepository =>
            _userRepository ?? (_userRepository = new UserRepository(_repoContext, _mapper));
    }
}