using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

#pragma warning disable 0649, 0414
namespace Project
{
    public class GameController : MonoBehaviour
    {
        //------------------------------------dependencies------------------------------------
        [Inject] SignalBus _signalBus;
        [Inject] PotController _potController;
        [Inject] List<Recipe> _recipes;
        [SerializeField] Transform _spawnPosition;
        [SerializeField] float _spawnDelayPerItem;

        //----------------------------------Unity Messages----------------------------------
        void Start()
        {
            setupSignalListeners();
            _recipesQueue = new Queue<Recipe>(_recipes);
            setCurrentRecipe();
        }

        void OnDestroy()
        {
            removeSignalListeners();
        }

        //----------------------------------------signals----------------------------------------
        void setupSignalListeners()
        {
            _signalBus.Subscribe<RecipeCompletedSignal>(setCurrentRecipe);
        }

        void removeSignalListeners()
        {
            _signalBus.TryUnsubscribe<RecipeCompletedSignal>(setCurrentRecipe);
        }

        //----------------------------------------details----------------------------------------
        Queue<Recipe> _recipesQueue;

        void setCurrentRecipe()
        {
            Recipe recipe = _recipesQueue.Dequeue();
            _potController.Recipe = recipe;
            StartCoroutine(nameof(spawnIngredients), recipe.Ingredients);
            _recipesQueue.Enqueue(recipe);
        }

        IEnumerator spawnIngredients(List<Ingredient> ingredients)
        {
            yield return new WaitForSeconds(2.0f);
            foreach (var i in ingredients)
            {
                var ingredientCtrl = Instantiate(i.Model, _spawnPosition).GetComponent<IngredientController>();
                ingredientCtrl.BounceUp();
                yield return new WaitForSeconds(_spawnDelayPerItem);
            }
        }
    }
}
