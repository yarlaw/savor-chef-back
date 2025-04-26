using SavorChef.Domain.Entities;

namespace SavorChef.Application.Common.Interfaces;
public interface IApplicationDataContext
{
    DbSet<Recipe> Recipes { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}