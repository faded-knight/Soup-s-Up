using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Project
{
    public class RecipeUIController : MonoBehaviour
    {
        //------------------------------------dependencies------------------------------------
        [Inject] SignalBus _signalBus;
        [SerializeField] List<GameObject> _ingredients;

        //-----------------------------------------API------------------------------------------
        public Recipe Recipe
        {
            set
            {
                _recipe = value;
                for (int i = 0; i < _spriteRenderers.Count; i++)
                {
                    _spriteRenderers[i][1].gameObject.SetActive(false);// hide correctIcon
                    _spriteRenderers[i][0].gameObject.SetActive(false);// hide ingredientSprite

                    if (i >= _recipe.Ingredients.Count)
                        break;

                    _spriteRenderers[i][0].sprite = _recipe.Ingredients[i].Sprite;
                    _spriteRenderers[i][0].gameObject.SetActive(true);
                }
            }
        }

        //----------------------------------Unity Messages----------------------------------
        void Awake()
        {
            setupSpriteRenderers();
        }

        void Start()
        {
            setupSignalListeners();
        }

        void OnDestroy()
        {
            removeSignalListeners();
        }

        //----------------------------------------signals----------------------------------------
        void setupSignalListeners()
        {
            _signalBus.Subscribe<DesirableIngredientAddedSignal>(onDesirableIngredientAdded);
        }

        void removeSignalListeners()
        {
            _signalBus.TryUnsubscribe<DesirableIngredientAddedSignal>(onDesirableIngredientAdded);
        }

        void onDesirableIngredientAdded(DesirableIngredientAddedSignal args)
        {
            for (int i = 0; i < _recipe.Ingredients.Count; i++)
                if (_recipe.Ingredients[i].name == args.IngredientController.name.RemoveCloneSuffix())
                    _spriteRenderers[i][1].gameObject.SetActive(true);
        }

        //----------------------------------------details----------------------------------------
        Recipe _recipe;
        List<List<SpriteRenderer>> _spriteRenderers = new List<List<SpriteRenderer>>(); //each item is [ingredientSprite, correctIconSprite]

        void setupSpriteRenderers()
        {
            for (int i = 0; i < _ingredients.Count; i++)
                _spriteRenderers.Add(new List<SpriteRenderer>(_ingredients[i].GetComponentsInChildren<SpriteRenderer>()));
        }
    }
}
