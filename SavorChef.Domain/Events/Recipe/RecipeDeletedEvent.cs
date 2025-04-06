namespace SavorChef.Domain.Events.Recipe;

public class RecipeDeletedEvent(Entities.Recipe recipe) : BaseEvent
{
    public Entities.Recipe Item { get; } = recipe;
}