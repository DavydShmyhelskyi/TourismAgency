using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Countries
    {
        [Key]
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        //допоміжні властивості
        public ICollection<Cities> Cities { get; set; }
    }
}

/*public async Task<IEnumerable<T>> GetAsync()
{
    return await _dbSet.ToListAsync();
}

public async Task SaveChangesAsync()
{
    await _context.SaveChangesAsync();
}
*/