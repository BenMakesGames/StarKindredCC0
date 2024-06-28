using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;

namespace StarKindred.API.Utility;

public static class ResourceHelper
{
    public static List<ResourceQuantity> Add(List<ResourceQuantity> list, ResourceQuantity r)
    {
        var newList = new List<ResourceQuantity>(list);

        var existing = newList.FindIndex(l => l.Type == r.Type);

        if (existing >= 0)
            newList[existing] = newList[existing] with { Quantity = newList[existing].Quantity + r.Quantity };
        else
            newList.Add(r);
        
        return newList;
    }

    public static void PayOrThrow(List<Resource> resources, List<ResourceQuantity> cost)
    {
        foreach (var c in cost)
        {
            var resource = resources.FirstOrDefault(r => r.Type == c.Type);
            
            if(resource == null || resource.Quantity < c.Quantity)
                throw new UnprocessableEntity($"You don't have enough {c.Type} :(");

            resource.Quantity -= c.Quantity;
        }
    }

    public static async Task CollectResources(Db db, Guid userId, List<ResourceQuantity> gains, CancellationToken cToken)
    {
        var gainTypes = gains.Where(g => g.Quantity > 0).Select(g => g.Type).ToList();
        
        var resources = await db.Resources
            .Where(r => r.UserId == userId && gainTypes.Contains(r.Type))
            .ToListAsync(cToken)
        ;
        
        foreach (var g in gains.Where(g => g.Quantity > 0))
        {
            var resource = resources.FirstOrDefault(r => r.Type == g.Type);
            
            if(resource == null)
            {
                resource = new Resource
                {
                    UserId = userId,
                    Type = g.Type,
                    Quantity = g.Quantity
                };
                
                db.Resources.Add(resource);
            }
            else
            {
                resource.Quantity += g.Quantity;
            }
        }
    }
}