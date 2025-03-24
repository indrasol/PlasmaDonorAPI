using NewPlasmaDonorsAPI.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NewPlasmaDonorsAPI.Repositories
{
    public class GenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetQuery(Func<IQueryable<T>, IQueryable<T>>? spec = null, Func<IQueryable<T>, IOrderedQueryable<T>>? sort = null)
        {
            IQueryable<T> query = _context.Set<T>();  // Start with the base query for the entity

            // Apply specification (filter) if provided
            if (spec != null)
            {
                query = spec(query); // Equivalent to applying Specification (filter)
            }

            // Apply sorting if provided
            if (sort != null)
            {
                query = sort(query); // Equivalent to applying Sort (ordering)
            }

            return query;
        }

        // Find all entities with optional sorting
        public List<T> FindAll()
        {
            return GetQuery(null, null).ToList();
        }
    }
} 
