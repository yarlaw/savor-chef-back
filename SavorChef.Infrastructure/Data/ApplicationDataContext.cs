using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Domain.Entities;
using SavorChef.Infrastructure.Identity;

namespace SavorChef.Infrastructure.Data;

public class ApplicationDataContext(DbContextOptions<ApplicationDataContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDataContext
{
    public DbSet<Recipe> Recipes => Set<Recipe>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}