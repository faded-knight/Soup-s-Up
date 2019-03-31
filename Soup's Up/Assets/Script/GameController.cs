using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        [Inject] RecipeUIController _recipeUIController;
        [Inject] List<Recipe> _recipes;
        [Inject(Id = "allIngredients")] List<GameObject> _allIngredients;
        [Inject] List<IngredientController.Pool> ingredientPools_;
        [SerializeField] Transform _spawnPosition;
        [SerializeField] float _spawnDelayPerItem;
        [SerializeField] TextMeshProUGUI _currentScoreUI;
        [SerializeField] List<GameObject> _livesUI;

        //----------------------------------Unity Messages----------------------------------
        void Start()
        {
            setupSignalListeners();
            _recipesQueue = new Queue<Recipe>(_recipes.OrderBy(r => System.Guid.NewGuid()));
            _score = 0;
            updateScore(0);
            setLives();
            setIngredientPools();
            setCurrentRecipe();
            StartCoroutine(nameof(spawnIngredients));
        }

        void OnDestroy()
        {
            removeSignalListeners();
        }

        //----------------------------------------signals----------------------------------------
        void setupSignalListeners()
        {
            _signalBus.Subscribe<IngredientAddedSignal>(onIngredientAdded);
            _signalBus.Subscribe<IngredientTouchedSignal>(onIngredientTouched);
            _signalBus.Subscribe<IngredientTrashedSignal>(onIngredientTrashed);
            _signalBus.Subscribe<RecipeCompletedSignal>(onRecipeCompleted);
            _signalBus.Subscribe<UndesirableIngredientAddedSignal>(onUndesirableIngredientAdded);
            Lean.Touch.LeanTouch.OnFingerSwipe += onFingerSwiped;
        }

        void removeSignalListeners()
        {
            _signalBus.TryUnsubscribe<IngredientAddedSignal>(onIngredientAdded);
            _signalBus.TryUnsubscribe<IngredientTouchedSignal>(onIngredientTouched);
            _signalBus.TryUnsubscribe<IngredientTrashedSignal>(onIngredientTrashed);
            _signalBus.TryUnsubscribe<RecipeCompletedSignal>(onRecipeCompleted);
            _signalBus.TryUnsubscribe<UndesirableIngredientAddedSignal>(onUndesirableIngredientAdded);
            Lean.Touch.LeanTouch.OnFingerSwipe -= onFingerSwiped;
        }

        void onIngredientAdded(IngredientAddedSignal args)
        {
            StartCoroutine(nameof(despawnIngredientCtrlWithDelay), args.IngredientController);
        }

        void onIngredientTouched(IngredientTouchedSignal args)
        {
            _selectedIngredientController = args.IngredientController;
        }

        void onIngredientTrashed(IngredientTrashedSignal args)
        {
            StartCoroutine(nameof(despawnIngredientCtrlWithDelay), args.IngredientController);
        }

        void onRecipeCompleted(RecipeCompletedSignal args)
        {
            updateScore(args.Recipe.Points);
            setCurrentRecipe();
        }

        void onUndesirableIngredientAdded(UndesirableIngredientAddedSignal args)
        {
            if (_lives == 0){
                _signalBus.Fire<GameIsOver>();
                StopCoroutine(nameof(spawnIngredients));
                return;
            }

            _lives--;
            _livesUI[_lives].SetActive(false);
        }

        void onFingerSwiped(Lean.Touch.LeanFinger finger)
        {
            _selectedIngredientController?.SwipeBounce(finger.SwipeScaledDelta);
        }

        //----------------------------------------details----------------------------------------
        Queue<Recipe> _recipesQueue;
        Dictionary<string, IngredientController.Pool> _ingredientPools = new Dictionary<string, IngredientController.Pool>();
        IngredientController _selectedIngredientController;
        int _score;
        int _lives;

        void setIngredientPools()
        {
            for (int i = 0; i < _allIngredients.Count; i++)
                _ingredientPools.Add(_allIngredients[i].GetComponent<IngredientController>().name, ingredientPools_[i]);
        }

        void setCurrentRecipe()
        {
            Recipe recipe = _recipesQueue.Dequeue();
            _potController.Recipe = recipe;
            _recipeUIController.Recipe = recipe;
            _recipesQueue.Enqueue(recipe);
        }

        void updateScore(int points)
        {
            _score += points;
            _currentScoreUI.text = _score.ToString();
        }

        void setLives()
        {
            foreach (var ui in _livesUI)
                ui.SetActive(true);
            
            _lives = _livesUI.Count;
        }

        IEnumerator spawnIngredients()
        {
            yield return new WaitForSeconds(2.0f);
            while (true)
            {
                _shuffledAllIngredients.Clear();
                _shuffledAllIngredients.AddRange(_allIngredients.OrderBy(i => System.Guid.NewGuid()).Select(i => i.name)) ;

                for (int i = 0; i < _shuffledAllIngredients.Count; i++)
                {
                    var ingredientCtrl = _ingredientPools[_shuffledAllIngredients[i]].Spawn(_spawnPosition.position);
                    ingredientCtrl.InitialBounce();

                    yield return new WaitForSeconds(_spawnDelayPerItem);
                }
            }
        }
        List<string> _shuffledAllIngredients = new List<string>();

        IEnumerator despawnIngredientCtrlWithDelay(IngredientController ingredientController)
        {
            yield return new WaitForSeconds(4.0f);
            _ingredientPools[ingredientController.name.RemoveCloneSuffix()].Despawn(ingredientController);
        }
    }
}
