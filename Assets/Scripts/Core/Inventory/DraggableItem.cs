using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemStack itemStack;
    public InventorySlot parentSlot;
    public Image icon;
    public CanvasGroup canvasGroup;

    public static bool isDragging = false;

    private float _size;
    private void Start()
    {
        parentSlot = GetComponentInParent<InventorySlot>();
        _size = GetComponent<RectTransform>().rect.width;
        canvasGroup = GetComponent<CanvasGroup>();

        icon = transform.Find("Icon").GetComponent<Image>();
        icon.sprite = itemStack.item.icon;

    }

    #region Drag and Drop Implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(parentSlot.transform.parent);
        Debug.Log("---------Begin Drag---------");
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag, Pointer Current Raycast: " + eventData.pointerCurrentRaycast.gameObject);
        isDragging = false;

        canvasGroup.blocksRaycasts = true; // make sure that the item is interactable again

        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            ReturnDefaultState();
            return;
        }
        InventorySlot nextSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>();

        if (nextSlot == null)
        {
            nextSlot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<InventorySlot>();
        }
        if (nextSlot is QuickSlot && itemStack.item.itemType != ItemType.Equipment)
        {
            ReturnDefaultState();
            return;
        }
        if (nextSlot == null)
        {
            parentSlot.UpdateSlot(this);
            return;
        }

        if(nextSlot is QuickSlot){
            WeaponController weaponController = FindAnyObjectByType<WeaponController>();
            if(!weaponController.CanEquip((int)itemStack.item.weaponType)) {
                ReturnDefaultState();
                return;
            }
        }
        DraggableItem nextItem = nextSlot.GetComponentInChildren<DraggableItem>();
        // if (nextItem != null)
        // {
        //     parentSlot.UpdateSlot(nextItem);
        // }
        // Update the draggable of the next slot
        InventorySlot nextSlotScript = nextSlot.GetComponent<InventorySlot>();


        parentSlot.UpdateSlot(nextItem);

        nextSlotScript.UpdateSlot(this);

        var temp = parentSlot;
        parentSlot = nextSlotScript;
        if(nextItem != null) nextItem.parentSlot = temp;

        // make sure that the border is highlighted in case player drag the item to null slot
        // or other which is not a slot

        GameEventManager.Instance.InventoryEvents.InventoryItemEndDragged(this);


        Debug.Log("---------End Drag---------");
    }



    public void OnDrop(PointerEventData eventData)
    {

        GameEventManager.Instance.InventoryEvents.InventoryItemDropped(this);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter");
        if (isDragging) return;
        Debug.Log("Pointer Enter 2");
        if (parentSlot is QuickSlot) return;
        Debug.Log("Pointer Enter 3");
        Debug.Log("Item: " + itemStack.item + " type");
        GameEventManager.Instance.InventoryEvents.ItemDescriptionBoardOpened(itemStack.item);
        parentSlot = GetComponentInParent<InventorySlot>();

    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (parentSlot is QuickSlot) return;
        GameEventManager.Instance.InventoryEvents.ItemDescriptBoardClosed(itemStack.item);
    }

    #endregion


    private void ReturnDefaultState()
    {
        parentSlot.UpdateSlot(this);
        if (parentSlot.hightlightBorder) parentSlot.hightlightBorder.SetActive(true);
        GameEventManager.Instance.InventoryEvents.InventoryItemEndDragged(this);
    }




}
