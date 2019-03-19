using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649, 0414
namespace Project
{
    [CreateAssetMenu(fileName = "RecipeName", menuName = "Custom/Recipe")]
    public class Recipe : ScriptableObject
    {
        public List<IngredientController> Ingredients;
    }
}
