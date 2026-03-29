using Lab_03.Models;

namespace Lab_03.Repositories
{
    public interface IDestinationRepository
    {
        Task<IEnumerable<Destination>> GetAllAsync();
        Task<IEnumerable<Destination>> GetAllWithToursAsync();
        Task<Destination?> GetByIdAsync(int id);
        Task<Destination?> GetByIdWithToursAsync(int id);
        Task AddAsync(Destination destination);
        Task UpdateAsync(Destination destination);
        Task DeleteAsync(int id);
    }
}
