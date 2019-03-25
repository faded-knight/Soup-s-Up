using UnityEngine;
using Zenject;

namespace Project
{
    public class SignalsInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.DeclareSignal<DesirableIngredientAddedSignal>();
            Container.DeclareSignal<IngredientAddedSignal>(); 
            Container.DeclareSignal<IngredientTouchedSignal>();
            Container.DeclareSignal<IngredientTrashedSignal>();
            Container.DeclareSignal<RecipeCompletedSignal>();
            Container.DeclareSignal<UndesirableIngredientAddedSignal>();
        }
    }
}
