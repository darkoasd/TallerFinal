using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{

    public Inventario inventory;

    public bool CanCraft(Recipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (!inventory.HasItem(ingredient.item.itemName, ingredient.amount))
            {
                Debug.Log($"No se puede craftear el ítem: {recipe.result.itemName}. Falta {ingredient.item.itemName}.");
                return false;
            }
        }
        return true;
    }

    public void CraftItem(Recipe recipe)
    {
        if (CanCraft(recipe))
        {
            foreach (var ingredient in recipe.ingredients)
            {
                inventory.RemoveItemFromInventory(ingredient.item, ingredient.amount);
            }
            inventory.AddItem(recipe.result);
            Debug.Log("Item crafteado: " + recipe.result.itemName);
        }
        else
        {
            Debug.Log("No se puede craftear el ítem: " + recipe.result.itemName);
        }
    }
}