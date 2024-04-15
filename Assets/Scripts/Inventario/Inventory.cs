using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int slotsCount = 20;
    public GameObject slotPrefab;
    public Transform slotsParent;
    private List<GameObject> slots = new List<GameObject>();
    private List<Item> items = new List<Item>(); // Lista para manejar los items

    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }
    void Start()
    {
        InitializeSlots();
    }

    void InitializeSlots()
    {
        for (int i = 0; i < slotsCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            slot.SetActive(true); // Asegúrate de activar el slot aquí
            slots.Add(slot);
        }
    }

    public void AddItem(Item itemToAdd)
    {
        if (items.Count < slotsCount)
        {
            items.Add(itemToAdd); // Añade el item a la lista
            UpdateSlotUI(); // Actualiza la UI de los slots
        }
        else
        {
            Debug.Log("Inventory is full.");
        }
    }

    public void RemoveItem(Item itemToRemove)
    {
        if (items.Remove(itemToRemove))
        {
            UpdateSlotUI(); // Actualiza la UI si se remueve un item
        }
    }

    void UpdateSlotUI()
    {
        // Asegúrate de que todos los slots estén desactivados inicialmente
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        // Activa y actualiza solo los slots que tienen items
        for (int i = 0; i < slotsCount; i++)
        {
            if (i < items.Count)
            {
                // Hay un item para este slot, así que actualiza la información del slot
                slots[i].SetActive(true); // Asegura que el slot esté activo
                slots[i].GetComponent<Image>().sprite = items[i].icon; // Actualiza el ícono
                slots[i].GetComponent<Image>().enabled = true; // Asegura que el ícono esté visible
                                                               // Si también muestras cantidades o tienes más información para actualizar, hazlo aquí
            }
            else
            {
                // No hay un item para este slot, así que configura el slot como vacío
                slots[i].SetActive(true); // Aún así, mantén el slot activo
                slots[i].GetComponent<Image>().sprite = null; // No hay ícono para mostrar
                slots[i].GetComponent<Image>().enabled = false; // Desactiva el ícono ya que el slot está vacío
                                                                // Asegúrate de limpiar cualquier otra información del slot aquí
            }
        }
    }
    void Update()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // Cambiar el item seleccionado a items[i] si es que existe en la barra
                if (i < items.Count)
                {
                    UseItem(items[i]);
                }
            }
        }
    }

    void UseItem(Item item)
    {
        item.Use();
        // Aquí puedes implementar la lógica específica para usar el item
    }
}
