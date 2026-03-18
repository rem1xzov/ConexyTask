using ConexyTask.Model;
using ConexyTask.Repository;

namespace ConexyTask.Service;

public class ConexyService : IConexyService
{
    private readonly IConexyRepository _iconexyRepository;
    public ConexyService (IConexyRepository iconexyRepository)
    {
        _iconexyRepository = iconexyRepository;
    }

    public async Task<List<Conexy>> GetAllTask()
    {
        return await _iconexyRepository.GetTo_Do_Conexy_Entities();
    }

    public async Task<Guid> CreateTask(Conexy conexy)
    {
        return await _iconexyRepository.CreateTo_Do_Conexy_Entity(conexy);
    }

    public async Task<Guid> UpdateTask(Guid id, string name, string description)
    {
        return await _iconexyRepository.UpdateTo_Do_Conexy_Entity(id, name, description);
    }

    public async Task<Guid> DeleteTask(Guid id)
    {
        return await _iconexyRepository.DeleteTo_Do_Conexy_Entity(id);
    }
}