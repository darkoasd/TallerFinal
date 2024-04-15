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
            slot.SetActive(true); // Aseg�rate de activar el slot aqu�
            slots.Add(slot);
        }
    }

    public void AddItem(Item itemToAdd)
    {
        if (items.Count < slotsCount)
        {
            items.Add(itemToAdd); // A�ade el item a la lista
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
        // Aseg�rate de que todos los slots est�n desactivados inicialmente
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        // Activa y actualiza solo los slots que tienen items
        for (int i = 0; i < slotsCount; i++)
        {
            if (i < items.Count)
            {
                // Hay un item para este slot, as� que actualiza la informaci�n del slot
                slots[i].SetActive(true); // Asegura que el slot est� activo
                slots[i].GetComponent<Image>().sprite = items[i].icon; // Actualiza el �cono
                slots[i].GetComponent<Image>().enabled = true; // Asegura que el �cono est� visible
                                                               // Si tambi�n muestras cantidades o tienes m�s informaci�n para actualizar, hazlo aqu�
            }
            else
            {
                // No hay un item para este slot, as� que configura el slot como vac�o
                slots[i].SetActive(true); // A�n as�, mant�n el slot activo
                slots[i].GetComponent<Image>().sprite = null; // No hay �cono para mostrar
                slots[i].GetComponent<Image>().enabled = false; // Desactiva el �cono ya que el slot est� vac�o
                                                                // Aseg�rate de limpiar cualquier otra informaci�n del slot aqu�
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
        // Aqu� puedes implementar la l�gica espec�fica para usar el item
    }
}
