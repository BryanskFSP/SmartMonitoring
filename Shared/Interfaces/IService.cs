namespace SmartMonitoring.Shared.Interfaces;

public interface IService<TEntity, TEdit, TIDType>
{
    public List<TEntity> GetAll();
    public string GetAllHash();
    public List<TEntity> GetAllFull();
    public string GetAllFullHash();
    public Task<TEntity?> GetByID(TIDType id);
    public Task<string> GetByIDHash(TIDType id);
    public Task<TEntity?> GetByIDFull(TIDType id);
    public Task<string> GetByIDFullHash(TIDType id);

    public Task<TEntity?> Update(TIDType id, TEdit editModel);

    public Task<TEntity?> Create(TEdit editModel);

    public Task<bool> Delete(TIDType id);
}