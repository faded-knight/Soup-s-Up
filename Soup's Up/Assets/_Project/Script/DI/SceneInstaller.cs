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

        public override void InstallBindings()
        {
            Container.BindInstance(_ingretientSettings);
            Container.Bind<Recipe>().FromMethodMultiple(ctx => _recipes.Select(Instantiate).ToList()).AsCached();
        }
    }
}