namespace SavorChef.Domain.Events.Recipe;

public class RecipeCreatedEvent(Entities.Recipe recipe) : BaseEvent
{
    public Entities.Recipe Item { get; } = recipe;
}