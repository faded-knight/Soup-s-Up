using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;

#pragma warning disable 0649, 0414
namespace Project
{
    public class PotController : MonoBehaviour
    {
        //------------------------------------dependencies------------------------------------
        [Inject] SignalBus _signalBus;

        //-----------------------------------------API------------------------------------------
        public Recipe Recipe
        {
            set
            {
                _recipe = value;
                _addedIngredients.Clear();
                _remainingIngredients.Clear();
                _remainingIngredients.AddRange(_recipe.Ingredients);
            }
        }

        //----------------------------------Unity Messages----------------------------------
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ingredient"))
            {
                IngredientController addedIngredient = other.GetComponent<IngredientController>();
                _ingredientAddedSignal.IngredientController = addedIngredient;
                _signalBus.Fire(_ingredientAddedSignal);
                var temp = addedIngredient.name.RemoveCloneSuffix();
                IngredientController desirableIngredient = _remainingIngredients.FirstOrDefault(i => i.name == addedIngredient.name.RemoveCloneSuffix());

                if (desirableIngredient == null)
                {
                    _signalBus.Fire<UndesirableIngredientAddedSignal>();
                    return;
                }

                _remainingIngredients.Remove(desirableIngredient);
                _addedIngredients.Add(desirableIngredient);

                if (_remainingIngredients.Count == 0)
                {
                    _recipeCompletedSignal.Recipe = _recipe;
                    _signalBus.Fire(_recipeCompletedSignal);
                }
                else
                    _signalBus.Fire<DesirableIngredientAddedSignal>();
            }
        }

        //----------------------------------------signals----------------------------------------
        IngredientAddedSignal _ingredientAddedSignal = new IngredientAddedSignal();
        RecipeCompletedSignal _recipeCompletedSignal = new RecipeCompletedSignal();

        //----------------------------------------details----------------------------------------
        Recipe _recipe;
        List<IngredientController> _remainingIngredients = new List<IngredientController>();
        List<IngredientController> _addedIngredients = new List<IngredientController>();
    }

}