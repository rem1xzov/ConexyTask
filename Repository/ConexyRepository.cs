using ConexyTask.DbContext;
using ConexyTask.Entity;
using ConexyTask.Model;
using Microsoft.EntityFrameworkCore;

namespace ConexyTask.Repository;

public class ConexyRepository : IConexyRepository
{
    private readonly DbConexy _context;
    public ConexyRepository (DbConexy context)
    {
        _context = context;
    }

    public async Task<List<Conexy>> GetTo_Do_Conexy_Entities()
    {
        var to_Do_Flip_Entities = await _context.ConexyEntities
            .AsNoTracking()
            .ToListAsync();
        
        var todo = to_Do_Flip_Entities 
            .Select(t => Conexy.Create(t.Id,t.Description,t.Name).conexy )
            .ToList();
        return todo;
    }

    public async Task<Guid> CreateTo_Do_Conexy_Entity(Conexy conexy)
    {
        var to_Do_Flip_Entity = new ConexyEntity
        {
            Name = conexy.Name,
            Description = conexy.Description,
            Id = conexy.Id
        };
        
        await _context.ConexyEntities.AddAsync(to_Do_Flip_Entity);
        await _context.SaveChangesAsync();
        
        return to_Do_Flip_Entity.Id;
    }

    public async Task<Guid> UpdateTo_Do_Conexy_Entity(Guid id, string name, string description)
    {
        await _context.ConexyEntities
            .Where(t => t.Id == id)
            
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Name, name)
                .SetProperty(t => t.Description, description));
        
        return id;
    }

    public async Task<Guid> DeleteTo_Do_Conexy_Entity(Guid id)
    {
        await _context.ConexyEntities
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync();
        return id;
    }
}