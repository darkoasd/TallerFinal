using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform dragRectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    public Image itemImage;

    public Item item;
    public int currentSlotIndex;

    void Awake()
    {
        dragRectTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    public void SetupItem(Item newItem, int slotIndex, Transform parentTransform)
    {
        item = newItem;
        currentSlotIndex = slotIndex;
        originalParent = parentTransform;
        if (itemImage != null)
            itemImage.sprite = item.icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Inventario inventoryScript = FindObjectOfType<Inventario>();
        inventoryScript.ItemSelected(item);
        originalPosition = dragRectTransform.anchoredPosition;
        dragRectTransform.SetParent(canvas.transform);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        Vector2 newPos;
        int newSlotIndex;
        Inventario inventoryScript = FindObjectOfType<Inventario>();

        if (inventoryScript.TryGetPositionForItem(item, dragRectTransform, Input.mousePosition, out newPos, out newSlotIndex))
        {
            inventoryScript.ClearSlots(item, currentSlotIndex);
            inventoryScript.MarkSlotsAsOccupied(item, newSlotIndex);

            dragRectTransform.anchoredPosition = newPos;
            currentSlotIndex = newSlotIndex;

            inventoryScript.ItemSelected(item);
        }
        else
        {
            dragRectTransform.anchoredPosition = originalPosition;
        }
        dragRectTransform.SetParent(originalParent);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Inventario inventoryScript = FindObjectOfType<Inventario>();
        if (inventoryScript != null)
        {
            inventoryScript.ItemSelected(item);
        }
    }
}