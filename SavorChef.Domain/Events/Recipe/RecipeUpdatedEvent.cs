namespace SavorChef.Domain.Events.Recipe;

public class RecipeUpdatedEvent(Entities.Recipe recipe) : BaseEvent
{
    public Entities.Recipe Item { get; } = recipe;
}