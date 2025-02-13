using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorys
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    namespace Repositorys
    {
        public class BaseRepository<T> : BaseInterface<T> where T : class
        {
            protected readonly DbContext _context;
            protected readonly DbSet<T> _dbSet;

            public BaseRepository(DbContext context)
            {
                _context = context;
                _dbSet = context.Set<T>();
            }

            public void Create(T entity)
            {
                _dbSet.Add(entity);
            }

            public IEnumerable<T> Get()
            {
                return _dbSet.ToList();
            }

            public T Get(int id)
            {
                return _dbSet.Find(id);
            }

            public void Update(T entity)
            {
                _dbSet.Update(entity);
            }

            public void Delete(int id)
            {
                var entity = _dbSet.Find(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                }
            }

            public void SaveChanges()
            {
                _context.SaveChanges();
            }
        }
    }

}
