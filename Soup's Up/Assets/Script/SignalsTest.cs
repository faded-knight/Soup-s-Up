using UnityEngine;
using Zenject;

#pragma warning disable 0649, 0414
namespace Project
{
    public class SignalsTest : MonoBehaviour
    {
        //------------------------------------dependencies------------------------------------
        [Inject] SignalBus _signalBus;

        void Start()
        {
            setupSignalListeners();
        }

        //----------------------------------------signals----------------------------------------
        void setupSignalListeners()
        {
            _signalBus.Subscribe<DesirableIngredientAddedSignal>((DesirableIngredientAddedSignal signal) => print(signal.ToString()));
            _signalBus.Subscribe<RecipeCompletedSignal>((RecipeCompletedSignal signal) => print(signal.ToString()));
            _signalBus.Subscribe<UndesirableIngredientAddedSignal>((UndesirableIngredientAddedSignal signal) => print(signal.ToString()));
        }
    }
}
