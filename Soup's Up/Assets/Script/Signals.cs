namespace Project
{
    public class DesirableIngredientAddedSignal
    {
        public IngredientController IngredientController;
    }
    public class GameIsOver { }
    public class IngredientAddedSignal
    {
        public IngredientController IngredientController;
    }
    public class IngredientTouchedSignal
    {
        public IngredientController IngredientController;
    }
    public class IngredientTrashedSignal
    {
        public IngredientController IngredientController;
    }
    public class RecipeCompletedSignal
    {
        public Recipe Recipe;
    }
    public class UndesirableIngredientAddedSignal { }
}
