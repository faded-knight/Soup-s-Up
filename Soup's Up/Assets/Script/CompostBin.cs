using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

#pragma warning disable 0649, 0414
namespace Project
{
    public class CompostBin : MonoBehaviour
    {
        //------------------------------------dependencies------------------------------------
        [Inject] SignalBus _signalBus;

        void Start()
        {
            _ingredientTrashedSignal = new IngredientTrashedSignal();
        }

        IngredientTrashedSignal _ingredientTrashedSignal;

        //Destroys Ingredient on Entry
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ingredient") && other.GetType() == typeof(MeshCollider))
            {
                _ingredientTrashedSignal.IngredientController = other.GetComponent<IngredientController>();
                _signalBus.Fire(_ingredientTrashedSignal);
            }
        }
    }
}

