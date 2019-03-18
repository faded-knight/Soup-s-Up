﻿using UnityEngine;

#pragma warning disable 0649, 0414

namespace Project
{
    [CreateAssetMenuAttribute(fileName = "DefaultIngredientSettings", menuName =  "Custom/IngredientSettings", order = 1)]
    public class IngretientSettings : ScriptableObject
    {
        [Range(1, 10)]
        public float UpwardForce;
        [Range(1, 10)]
        public float BouncingAngle;
        [Range(1, 10)]
        public float MaxFallingSpeed;

    }
}
