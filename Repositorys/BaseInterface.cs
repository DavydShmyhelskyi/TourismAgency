namespace Repositorys
{
    public interface BaseInterface<T>
    {
        void Create(T entity);
        IEnumerable<T> Get();
        T Get(int id);
        void Update(T entity);
        void Delete(int id);
        void SaveChanges();

    }
}
