using SavorChef.Domain.Entities;

namespace SavorChef.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Recipe, LookupDto>();
        }
    }
}
