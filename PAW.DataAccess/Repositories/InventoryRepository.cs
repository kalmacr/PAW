
using PAW.DataAccess.MSSQL;
using PAW.Models;
using PAW.Repositories;

namespace PAW.DataAccess.Repositories;

public interface IInventoryRepository : IRepositoryBase<Inventory>
{
    Task<bool> UpsertAsync(Inventory entity, bool isUpdating);
    Task<bool> CreateAsync(Inventory entity);
    Task<bool> DeleteAsync(Inventory entity);
    Task<IEnumerable<Inventory>> ReadAsync();
    Task<Inventory> FindAsync(int id);

    Task<bool> UpdateAsync(Inventory entity);
    Task<bool> UpdateManyAsync(IEnumerable<Inventory> entities);
    Task<bool>ExistsAsync(Inventory entity);
}

public class InventoryRepository : RepositoryBase<Inventory>, IInventoryRepository
{
    
}
