using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInventory : MonoBehaviour
{
    public List<KeyItem> keys = new List<KeyItem>();
    public List<NoteItem> notes = new List<NoteItem>();
    public static KeyInventory instance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // Destruye el nuevo objeto si ya existe una instancia
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void AddKey(KeyItem key)
    {
        keys.Add(key);
        ArtefactosUIManager.Instance.UpdateInventoryDisplay();
        Debug.Log("Llave añadida: " + key.itemName);
    }

    public bool HasKey(string keyId)
    {
        foreach (var key in keys)
        {
            if (key.keyId == keyId)
                return true;
        }
        return false;
    }

    public void AddNote(NoteItem note)
    {
        notes.Add(note);
        ArtefactosUIManager.Instance.UpdateInventoryDisplay();
        Debug.Log("Nota añadida: " + note.itemName);
    }
}
