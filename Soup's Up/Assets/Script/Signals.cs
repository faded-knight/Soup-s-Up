namespace Project
{
    public class DesirableIngredientAddedSignal { }
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
    public class RecipeCompletedSignal { }
    public class UndesirableIngredientAddedSignal { }
}
