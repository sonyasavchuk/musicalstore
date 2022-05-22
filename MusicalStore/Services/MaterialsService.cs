using Microsoft.EntityFrameworkCore;
using MusicalStore.Models.Entities;

namespace MusicalStore.Services;

public class MaterialsService
{
    private readonly MusicalStoreContext _context;
    
    public MaterialsService(MusicalStoreContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Material>> GetOrAddMaterialsByNames(IEnumerable<string> materialNames)
    {
        materialNames = materialNames.ToArray();
        var existingMaterials = await _context.Materials
            .Where(x => materialNames.Contains(x.MaterialName))
            .ToListAsync();

        var newMaterials = materialNames.Except(existingMaterials.Select(x => x.MaterialName))
            .Select(labelName => new Material
            {
                MaterialName = labelName
            })
            .ToArray();

        _context.Materials.AddRange(newMaterials);
        await _context.SaveChangesAsync();

        existingMaterials.AddRange(newMaterials);
        return existingMaterials;
    }
}
