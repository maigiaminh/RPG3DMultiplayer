using System;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("References")]
    public DraggableItem currentItem;
    public WeaponController WeaponController;
    public QuickSlot QuickSlot;
    public GameObject quantityPanel;
    public TextMeshProUGUI quantityText;
    public GameObject hightlightBorder;
    public int Index { get; set; }

    private object _lock = new object();
    private bool _isReserved = false;



    protected virtual void Awake()
    {
        quantityPanel = transform.GetChild(1).gameObject;
        quantityText = quantityPanel.GetComponentInChildren<TextMeshProUGUI>();
        hightlightBorder.SetActive(false);
    }

    private void Start()
    {
        WeaponController = FindAnyObjectByType<WeaponController>();
        QuickSlot = FindAnyObjectByType<QuickSlot>();
    }
    
    protected virtual void OnEnable()
    {
        GameEventManager.Instance.InventoryEvents.OnInventorySlotLeftClicked += OnInventorySlotLeftClicked;
        GameEventManager.Instance.InventoryEvents.OnInventoryItemEndDragged += OnInventoryItemEndDragged;
    }

    protected virtual void OnDisable()
    {
        GameEventManager.Instance.InventoryEvents.OnInventorySlotLeftClicked -= OnInventorySlotLeftClicked;
        GameEventManager.Instance.InventoryEvents.OnInventoryItemEndDragged -= OnInventoryItemEndDragged;
    }


    protected virtual void Update()
    {
        quantityPanel.SetActive(currentItem != null);
    }

    public void UpdateQuantityText(int quantity)
    {
        quantityText.text = quantity.ToString();
    }

    public void UpdateSlot(DraggableItem item)
    {
        lock (_lock)
        {
            if (item)
            {

                currentItem = item;
                currentItem.transform.SetParent(transform);
                currentItem.transform.localPosition = Vector3.zero;

                if (quantityText) UpdateQuantityText(item.itemStack.quantity);

                item.GetComponent<RectTransform>().localScale = Vector3.one;
            }
            else
            {
                currentItem = null;
                if (quantityPanel) quantityPanel.SetActive(false);
            }
            _isReserved = false;
        }
    }

    public bool IsHavingItem()
    {
        return currentItem != null || _isReserved; // Kiểm tra trạng thái đặt trước
    }

    public void ReserveSlot()
    {
        _isReserved = true;
    }

    protected virtual void OnInventorySlotLeftClicked(InventorySlot slot)
    {
        if (slot != this)
        {
            if (hightlightBorder) hightlightBorder.SetActive(false);
            return;
        }
        if (currentItem == null) return;

        if (hightlightBorder) hightlightBorder.SetActive(!hightlightBorder.activeSelf);

    }



    protected virtual void OnInventoryItemEndDragged(DraggableItem item)
    {
        if (hightlightBorder) hightlightBorder.SetActive(item == currentItem);
    }


    public void ClearSlot()
    {
        this.currentItem = null;
        this._isReserved = false;
        //Other clear operations for UI elements in the slot. For example if you have a stack size text
    }
}
