using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SavorChef.Domain.Entities;
using SavorChef.Domain.Enums;
using SavorChef.Infrastructure.Identity;

namespace SavorChef.Infrastructure.Data.Configurations;

public class RecipeEntityConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        // Configure the TimeSpan conversion (can't be done with annotations)
        builder.Property(x => x.PreparationTime)
            .HasConversion(x => x.ToString(), x => TimeSpan.Parse(x));

        // Configure the Enum to string conversion
        builder.Property(x => x.Difficulty)
            .HasConversion(new EnumToStringConverter<RecipeDifficulty>());

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.ClientCascade);

    }
}