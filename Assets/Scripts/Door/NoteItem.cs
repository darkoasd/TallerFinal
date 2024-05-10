using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoteItem", menuName = "Artefacto/Note Item")]
public class NoteItem : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public string itemDescription;
    public string noteText;
}