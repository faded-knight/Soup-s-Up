using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [Inject(Id = "allIngredients")] List<GameObject> _allIngredients;
        [Inject] List<IngredientController.Pool> ingredientPools_;
        [SerializeField] Transform _spawnPosition;
        [SerializeField] float _spawnDelayPerItem;

        //----------------------------------Unity Messages----------------------------------
        void Start()
        {
            setupSignalListeners();
            _recipesQueue = new Queue<Recipe>(_recipes);
            setIngredientPools();
            setCurrentRecipe();
        }

        void OnDestroy()
        {
            removeSignalListeners();
        }

        //----------------------------------------signals----------------------------------------
        void setupSignalListeners()
        {
            _signalBus.Subscribe<IngredientAddedSignal>(onIngredientAddedSignal);
            _signalBus.Subscribe<RecipeCompletedSignal>(setCurrentRecipe);
        }

        void removeSignalListeners()
        {
            _signalBus.TryUnsubscribe<IngredientAddedSignal>(onIngredientAddedSignal);
            _signalBus.TryUnsubscribe<RecipeCompletedSignal>(setCurrentRecipe);
        }

        void onIngredientAddedSignal(IngredientAddedSignal args)
        {
            StartCoroutine(nameof(despawnIngredientCtrlWithDelay), args.IngredientController);
        }

        IEnumerator despawnIngredientCtrlWithDelay(IngredientController ingredientController)
        {
            yield return new WaitForSeconds(4.0f);
            _ingredientPools[ingredientController.name.RemoveCloneSuffix()].Despawn(ingredientController);
        }

        //----------------------------------------details----------------------------------------
        Queue<Recipe> _recipesQueue;
        Dictionary<string, IngredientController.Pool> _ingredientPools = new Dictionary<string, IngredientController.Pool>();

        void setIngredientPools()
        {
            for (int i = 0; i < _allIngredients.Count; i++)
                _ingredientPools.Add(_allIngredients[i].GetComponent<IngredientController>().name, ingredientPools_[i]);
        }

        void setCurrentRecipe()
        {
            Recipe recipe = _recipesQueue.Dequeue();
            _potController.Recipe = recipe;
            StartCoroutine(nameof(spawnIngredients), recipe.Ingredients);
            _recipesQueue.Enqueue(recipe);
        }

        IEnumerator spawnIngredients(List<IngredientController> ingredients)
        {
            yield return new WaitForSeconds(2.0f);
            foreach (var ingredientCrtl in ingredients)
            {
                var ingredientCtrl = _ingredientPools[ingredientCrtl.name].Spawn(_spawnPosition.position);
                ingredientCtrl.BounceUp();
                yield return new WaitForSeconds(_spawnDelayPerItem);
            }
        }
    }
}
