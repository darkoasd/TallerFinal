using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New KeyItem", menuName = "Artefacto/Key Item")]
public class KeyItem : ScriptableObject
{
    public string keyId;
    public string itemName;
    public Sprite itemImage;
    public string itemDescription;
}
