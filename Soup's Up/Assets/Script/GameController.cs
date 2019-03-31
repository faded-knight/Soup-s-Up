﻿using System.Collections;
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
        [Inject] List<Recipe> _recipes;
        [Inject(Id = "allIngredients")] List<GameObject> _allIngredients;
        [Inject] List<IngredientController.Pool> ingredientPools_;
        [SerializeField] Transform _spawnPosition;
        [SerializeField] float _spawnDelayPerItem;
        [SerializeField] List<SpriteRenderer> _spriteRenderers;
        [SerializeField] TextMeshProUGUI _currentScoreUI;

        //----------------------------------Unity Messages----------------------------------
        void Start()
        {
            setupSignalListeners();
            _recipesQueue = new Queue<Recipe>(_recipes);
            _score = 0;
            updateScore(0);
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
            _signalBus.Subscribe<IngredientTrashedSignal>(onIngredientTrashedSignal);
            _signalBus.Subscribe<RecipeCompletedSignal>(onRecipeCompletedSignal);
            Lean.Touch.LeanTouch.OnFingerSwipe += onFingerSwiped;
        }

        void removeSignalListeners()
        {
            _signalBus.TryUnsubscribe<IngredientAddedSignal>(onIngredientAdded);
            _signalBus.TryUnsubscribe<IngredientTouchedSignal>(onIngredientTouched);
            _signalBus.TryUnsubscribe<IngredientTrashedSignal>(onIngredientTrashedSignal);
            _signalBus.TryUnsubscribe<RecipeCompletedSignal>(onRecipeCompletedSignal);
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

        void onIngredientTrashedSignal(IngredientTrashedSignal args)
        {
            StartCoroutine(nameof(despawnIngredientCtrlWithDelay), args.IngredientController);
        }

        void onRecipeCompletedSignal(RecipeCompletedSignal args)
        {
            updateScore(args.Recipe.Points);
            setCurrentRecipe();
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

        void setIngredientPools()
        {
            for (int i = 0; i < _allIngredients.Count; i++)
                _ingredientPools.Add(_allIngredients[i].GetComponent<IngredientController>().name, ingredientPools_[i]);
        }

        void setCurrentRecipe()
        {
            Recipe recipe = _recipesQueue.Dequeue();
            _potController.Recipe = recipe;

            for (int i = 0; i < _spriteRenderers.Count; i++)
            {
                _spriteRenderers[i].sprite = null;
                if (i >= recipe.Ingredients.Count)
                    break;
                _spriteRenderers[i].sprite = recipe.Ingredients[i].Sprite;
            }

            _recipesQueue.Enqueue(recipe);
        }

        void updateScore(int points)
        {
            _score += points;
            _currentScoreUI.text = _score.ToString();
        }

        IEnumerator spawnIngredients()
        {
            yield return new WaitForSeconds(2.0f);
            while (true)
            {
                for (int i = 0; i < _allIngredients.Count; i++)
                {
                    var ingredientCtrl = _ingredientPools.ElementAt(i).Value.Spawn(_spawnPosition.position);
                    ingredientCtrl.InitialBounce();

                    yield return new WaitForSeconds(_spawnDelayPerItem);
                }
            }
        }

        IEnumerator despawnIngredientCtrlWithDelay(IngredientController ingredientController)
        {
            yield return new WaitForSeconds(4.0f);
            _ingredientPools[ingredientController.name.RemoveCloneSuffix()].Despawn(ingredientController);
        }
    }
}
