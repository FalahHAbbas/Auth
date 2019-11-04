using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Auth.DB;
using Auth.Forms;
using Auth.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repositories {
    public interface IBaseRepository<T, in TId> {
        Task<T> Add(T t);
        T AddNow(T t);
        Task<T> Remove(TId id);
        Task<T> Update(T t, TId id);
        Task<T> GetById(TId id);
        Task<DataResult<T>> GetAll(int pageNumber = 1);
        IQueryable<T> GetRaw(int pageNumber = 1);
        IQueryable<T> GetRaw(Expression<Func<T, bool>> predicate, int pageNumber = 1);
        Task<DataResult<T>> GetAll(Expression<Func<T, bool>> predicate, int pageNumber = 1);
        Task<T> Get(Expression<Func<T, bool>> predicate);
    }

    public abstract class BaseRepository<T, TId> : IBaseRepository<T, TId> where T : Model<TId> {
        private readonly AuthContext _context;
        private readonly IMapper _mapper;

        protected BaseRepository(AuthContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<T> Add(T t) {
            await _context.Set<T>().AddAsync(t);
            await _context.SaveChangesAsync();
            return t;
        }

        public T AddNow(T t) {
            _context.Set<T>().Add(t);
            _context.SaveChanges();
            return t;
        }

        public async Task<T> Remove(TId id) {
            var result = await GetById(id);
            if (result == null) return null;
            _context.Remove(result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<T> Update(T t, TId id) {
            var result = await GetById(id);
            _mapper.Map(t, result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<T> GetById(TId id) =>
            await _context.Set<T>().FirstOrDefaultAsync(x => x.Id.Equals(id));

        public async Task<DataResult<T>> GetAll(int pageNumber = 1) => new DataResult<T> {
            Data = await _context.Set<T>().Skip(Startup.PageSize * (pageNumber - 1))
                .Take(Startup.PageSize).ToListAsync(),
            TotalCount = (await _context.Set<T>().CountAsync() / Startup.PageSize - 1) /
                         Startup.PageSize + 1
        };

        public IQueryable<T> GetRaw(int pageNumber = 1) => _context.Set<T>()
            .Skip(Startup.PageSize * (pageNumber - 1)).Take(Startup.PageSize).AsQueryable();

        public IQueryable<T> GetRaw(Expression<Func<T, bool>> predicate, int pageNumber = 1) =>
            _context.Set<T>().Where(predicate).Skip(Startup.PageSize * (pageNumber - 1))
                .Take(Startup.PageSize).AsQueryable();

        public async Task<DataResult<T>>
            GetAll(Expression<Func<T, bool>> predicate, int pageNumber = 1) => new DataResult<T> {
            Data = await _context.Set<T>().Where(predicate)
                .Skip(Startup.PageSize * (pageNumber - 1)).Take(Startup.PageSize).ToListAsync(),
            TotalCount = (await _context.Set<T>().Where(predicate).CountAsync() - 1) /
                         Startup.PageSize + 1
        };

        public async Task<T> Get(Expression<Func<T, bool>> predicate) =>
            await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }
}