
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe", order = 1)]
public class Recipe : ScriptableObject
{
    public Item result;
    public List<ItemAmount> ingredients;

    [System.Serializable]
    public struct ItemAmount
    {
        public Item item;
        public int amount;
    }
}

