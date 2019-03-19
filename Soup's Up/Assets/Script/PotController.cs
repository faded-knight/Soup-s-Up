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
                _remainingIngredients = _recipe.Ingredients;
                _addedIngredients.Clear();
            }
        }

        //----------------------------------Unity Messages----------------------------------
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ingredient"))
            {
                Ingredient addedIngredient = other.GetComponent<IngredientController>().Ingredient;
                Ingredient desirableIngredient = _remainingIngredients.FirstOrDefault(i => i.name == addedIngredient.name);

                if (desirableIngredient == null)
                {
                    _signalBus.Fire<UndesirableIngredientAddedSignal>();
                    return;
                }

                _remainingIngredients.Remove(desirableIngredient);
                _addedIngredients.Add(desirableIngredient);

                if (_remainingIngredients.Count == 0)
                    _signalBus.Fire<RecipeCompletedSignal>();
                else
                    _signalBus.Fire<DesirableIngredientAddedSignal>();
            }
            Destroy(other.gameObject);
        }

        //----------------------------------------details----------------------------------------
        Recipe _recipe;
        List<Ingredient> _remainingIngredients = new List<Ingredient>();
        List<Ingredient> _addedIngredients = new List<Ingredient>();
    }
}
