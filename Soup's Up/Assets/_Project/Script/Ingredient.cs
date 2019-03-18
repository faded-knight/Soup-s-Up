using UnityEngine;

#pragma warning disable 0649, 0414
namespace Project
{
    [CreateAssetMenu(fileName = "IngredientName", menuName = "Custom/Ingredient")]
    public class Ingredient : ScriptableObject
    {
        public GameObject Model;
        public Sprite Sprite;
    }
}
