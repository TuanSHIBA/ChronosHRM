using Chronos.Application.Interfaces;
using Chronos.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {    
        protected readonly ChronosDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ChronosDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
           Expression<Func<T, bool>>? filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
           string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            // 1. Xử lý Filter (ví dụ: lấy theo EmployeeId)
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // 2. Xử lý Include (Vòng lặp thần thánh)
            // Nếu bạn truyền chuỗi "Employee", nó sẽ chạy query.Include("Employee")
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // 3. Xử lý OrderBy
            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

    }
}
