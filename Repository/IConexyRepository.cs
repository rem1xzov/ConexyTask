using ConexyTask.Model;

namespace ConexyTask.Repository;

public interface IConexyRepository
{
    Task<List<Conexy>> GetTo_Do_Conexy_Entities();
    Task<Guid> CreateTo_Do_Conexy_Entity(Conexy conexy);
    Task<Guid> UpdateTo_Do_Conexy_Entity(Guid id, string name, string description);
    Task<Guid> DeleteTo_Do_Conexy_Entity(Guid id);
}