using ConexyTask.Model;

namespace ConexyTask.Service;

public interface IConexyService
{
    Task<List<Conexy>> GetAllTask();
    Task<Guid> CreateTask(Conexy conexy);
    Task<Guid> UpdateTask(Guid id, string name, string description);
    Task<Guid> DeleteTask(Guid id);
}