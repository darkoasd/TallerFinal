using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ArtefactosUIManager : MonoBehaviour
{
    public static ArtefactosUIManager Instance { get; private set; }

    public GameObject itemPanel;  // Panel donde se listar�n los �tems
    public GameObject itemButtonPrefab;  // A�ade esta l�nea para referenciar el prefab del bot�n

    public TextMeshProUGUI itemNameText;  // Texto para el nombre del �tem
    public TextMeshProUGUI itemDescriptionText;  // Texto para la descripci�n del �tem
    public Image itemImageDisplay;  // Imagen del �tem

    public KeyInventory keyInventory;

    void Start()
    {
        keyInventory = FindObjectOfType<KeyInventory>();
        if (keyInventory == null)
            Debug.LogError("KeyInventory component not found in the scene!");

        UpdateInventoryDisplay();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void UpdateInventoryDisplay()
    {
        if (keyInventory == null)
        {
            Debug.LogError("KeyInventory is null");
            return;
        }
        // Aseg�rate de limpiar el panel antes de a�adir nuevos botones
        foreach (Transform child in itemPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (KeyItem key in keyInventory.keys)
        {
            CreateItemButton(key);
        }
        foreach (NoteItem note in keyInventory.notes)
        {
            CreateItemButton(note);
        }
    }

    private void CreateItemButton(ScriptableObject item)
    {
        GameObject button = Instantiate(itemButtonPrefab, itemPanel.transform);
        button.GetComponent<Button>().onClick.AddListener(() => DisplayItemDetails(item));
    }

    public void DisplayItemDetails(ScriptableObject item)
    {
        if (item is KeyItem key)
        {
            itemNameText.text = key.itemName;
            itemDescriptionText.text = key.itemDescription;
            itemImageDisplay.sprite = key.itemImage;
        }
        else if (item is NoteItem note)
        {
            itemNameText.text = note.itemName;
            itemDescriptionText.text = note.itemDescription + "\nNota: " + note.noteText;
            itemImageDisplay.sprite = note.itemImage;
        }
    }
}