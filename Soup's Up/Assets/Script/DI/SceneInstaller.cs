using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

#pragma warning disable 0649, 0414
namespace Project
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] IngretientSettings _ingretientSettings;
        [SerializeField] List<Recipe> _recipes;
        [SerializeField] List<GameObject> _allIngredients;

        public override void InstallBindings()
        {
            Container.BindInstance(_ingretientSettings);
            Container.Bind<Recipe>().FromMethodMultiple(ctx => _recipes.Select(Instantiate).ToList()).AsCached();
            Container.BindInstance(_allIngredients).WithId("allIngredients");

            foreach (var ingredient in _allIngredients)
                Container.BindMemoryPool<IngredientController, IngredientController.Pool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(ingredient)
                .UnderTransformGroup("IngredientPools");
        }
    }
}