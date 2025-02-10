
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
public class InventoryManager : Singleton<InventoryManager>
{

    public bool IsInventoryOpen { get; set; }
    [Header("Inventory Container")]
    public GameObject inventoryBoard;
    public GameObject inventoryContainer;
    [Header("Slot Properties")]
    public int numberOfSlots;
    public InventorySlot inventorySlotPrefab;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    [Header("Item Assets")]
    [SerializeField] private List<AssetReference> itemAssetReferences = new List<AssetReference>();
    [SerializeField] private List<GameObject> items = new List<GameObject>();

    [Header("Description Board Base On Item Type")]
    public GameObject questDescriptBoard;

    // [SerializeField] private EquipmentDescriptBoard _equipmentDescriptBoard;
    // [SerializeField] private ConsumableDescriptBoard _consumableDescriptBoard;

    [Header("Quick Slot Choice Panel")]
    [SerializeField] private GameObject quickSlotChoicePanel;

    [Header("Options choice button")]
    [SerializeField] private Button allButton;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button otherButton;

    // 
    private Dictionary<ItemData, int> _inventoryItems = new Dictionary<ItemData, int>();

    public Dictionary<ItemData, int> GetAllInventoryItems => _inventoryItems;
    private Dictionary<ItemData, List<DraggableItem>> _itemPool = new Dictionary<ItemData, List<DraggableItem>>();
    private object _inventoryLock = new object();
    // [SerializeField] private List<> quickSlotChoiceContainer;

    public bool IsAddingItem { get; private set; }



    private GameEventManager _gameEventManager;



    protected override void Awake()
    {
        base.Awake();
        InitializeInventorySlot();
        InitializeButton();

        LoadInventoryItems();
        FilterInventory(ItemType.All);
    }


    private void OnEnable()
    {
        _gameEventManager = GameEventManager.Instance;
        _gameEventManager.InventoryEvents.OnItemAdded += OnItemAdded;
        _gameEventManager.InventoryEvents.OnItemRemoved += OnItemRemoved;

        _gameEventManager.PlayerContactUIEvents.OnPlayerOpenInventory += OnPlayerOpenInventory;
        _gameEventManager.PlayerContactUIEvents.OnPlayerCloseInventory += OnPlayerCloseInventory;

        _gameEventManager.InventoryEvents.OnInventorySlotRightClicked += OnInventorySlotRightClicked;
    }


    private void OnDisable()
    {
        _gameEventManager.InventoryEvents.OnItemAdded -= OnItemAdded;
        _gameEventManager.InventoryEvents.OnItemRemoved -= OnItemRemoved;

        _gameEventManager.PlayerContactUIEvents.OnPlayerOpenInventory -= OnPlayerOpenInventory;
        _gameEventManager.PlayerContactUIEvents.OnPlayerCloseInventory -= OnPlayerCloseInventory;

        _gameEventManager.InventoryEvents.OnInventorySlotRightClicked -= OnInventorySlotRightClicked;

    }

    private void FilterInventory(ItemType filterType)
    {
        lock (_inventoryLock)
        {
            // Disable all draggable items first
            foreach (var item in inventorySlots)
            {
                if (item.currentItem != null)
                {
                    item.currentItem.gameObject.SetActive(false);
                    item.ClearSlot(); // Make sure the slot is considered empty
                }
            }

            ItemData currentQuickslotItem = QuickSlotManager.Instance.quickSlot.currentQuickSlotItem ? QuickSlotManager.Instance.quickSlot.currentQuickSlotItem.itemStack.item : null;

            //Then loop through each itemData, amount in the dictionary
            foreach (KeyValuePair<ItemData, int> item in _inventoryItems)
            {
                if (item.Key == currentQuickslotItem) continue;
                if (filterType == ItemType.Equipment)
                {
                    if (item.Key.itemType != ItemType.Equipment) continue;
                }
                if (filterType == ItemType.Other)
                {
                    if (item.Key.itemType == ItemType.Equipment) continue;
                }
                int amount = item.Value;
                int numStacks = Mathf.CeilToInt((float)amount / item.Key.maxStack);

                for (int i = 0; i < numStacks; i++)
                {
                    int stackAmount = Mathf.Min(amount, item.Key.maxStack);
                    InventorySlot slot = FindNextSlot();

                    if (slot != null)
                    {
                        StartCoroutine(GetOrCreateDraggableItem(item.Key, draggableItem =>
                        {
                            if (draggableItem != null)
                            {
                                draggableItem.itemStack = new ItemStack(item.Key, stackAmount);
                                draggableItem.gameObject.SetActive(true);
                                slot.UpdateSlot(draggableItem);
                            }
                            else
                            {
                                Debug.LogError("Failed to create or retrieve a draggable item.");
                            }
                        }));

                    }
                    else
                    {
                        Debug.LogError("Not enough slots to display all items.");
                        break;  // Stop adding items if we run out of slots
                    }
                    amount -= stackAmount;
                }
            }
        }
    }




    private IEnumerator GetOrCreateDraggableItem(ItemData data, System.Action<DraggableItem> callback)
    {
        lock (_inventoryLock)
        {
            if (!_itemPool.ContainsKey(data))
            {
                _itemPool[data] = new List<DraggableItem>();
            }

            List<DraggableItem> pool = _itemPool[data];
            foreach (var item in pool)
            {
                if (!item.gameObject.activeSelf)
                {
                    callback(item);
                    yield break;
                }
            }

            foreach (GameObject item in items)
            {
                if (item.name == data.itemName)
                {
                    DraggableItem newItem = Instantiate(item).GetComponent<DraggableItem>();

                    newItem.GetComponent<RectTransform>().localScale = Vector3.one;
                    pool.Add(newItem);
                    newItem.gameObject.SetActive(false); // Initially deactivate the new item
                    callback(newItem);
                    yield break;

                }
            }
            // If no inactive item is found, instantiate a new one
//             foreach (AssetReference asset in itemAssetReferences)
//             {
// #if UNITY_EDITOR
//                 string assetName = asset.editorAsset.name;
//                 Debug.Log("Comparing: " + assetName + " with " + data.itemName);
//                 if (assetName == data.itemName) // Compare with RuntimeKey or other identifier
//                 {
//                     AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(asset);
//                     yield return handle;

//                     if (handle.Status == AsyncOperationStatus.Succeeded)
//                     {
//                         DraggableItem newItem = Instantiate(handle.Result).GetComponent<DraggableItem>();
//                         newItem.GetComponent<RectTransform>().localScale = Vector3.one;
//                         pool.Add(newItem);
//                         newItem.gameObject.SetActive(false); // Initially deactivate the new item
//                         Addressables.Release(handle);
//                         callback(newItem);
//                         yield break;
//                     }
//                     else
//                     {
//                         Debug.LogError("Failed to load asset for item: " + data.itemName);
//                     }
//                 }
// #endif
//             }

            Debug.LogError("Item not found: " + data.itemName);
            callback(null);
        }
    }


    #region Inventory Events



    private void OnItemAdded(ItemData data, int amount, bool isDictionaryChanged = true)
    {
        lock (_inventoryLock)
        {
            if (isDictionaryChanged)
            {
                if (_inventoryItems.ContainsKey(data))
                {
                    _inventoryItems[data] += amount;
                }
                else
                {
                    _inventoryItems.Add(data, amount);
                }
            }
        }

        // Refresh the inventory display based on the current filter
        // You'll need a way to track the current filter type (e.g., a member variable)
        FilterInventory(ItemType.All); // Replace _currentFilterType with your actual filter variable
    }

    private void OnItemRemoved(ItemData data, int amount)
    {
        lock (_inventoryLock)
        {
            if (_inventoryItems.ContainsKey(data))
            {
                _inventoryItems[data] -= amount;
                if (_inventoryItems[data] <= 0)
                {
                    _inventoryItems.Remove(data);
                }
            }
        }

        // Refresh the inventory display based on the current filter
        // You'll need a way to track the current filter type (e.g., a member variable)
        FilterInventory(ItemType.All); // Replace _currentFilterType with your actual filter variable
    }

    public bool AddItemToInventory(ItemData item, int amount, bool isDictionaryChanged = true)
    {
        if (IsAddingItem) return false;

        IsAddingItem = true;
        if (CheckInventoryFull(amount))
        {
            IsAddingItem = false;
            return false;
        }
        _gameEventManager.InventoryEvents.ItemAdded(item, amount, isDictionaryChanged);
        IsAddingItem = false;
        return true;
    }

    private void OnPlayerOpenInventory()
    {
        SoundManager.PlaySound(SoundType.INVENTORY);
        FilterInventory(ItemType.All);
        inventoryBoard.SetActive(true);
        IsInventoryOpen = true;
    }

    private void OnPlayerCloseInventory()
    {
        SoundManager.PlaySound(SoundType.INVENTORY);
        inventoryBoard.SetActive(false);
        IsInventoryOpen = false;
    }





    private void OnInventorySlotRightClicked(InventorySlot slot)
    {

    }


    #endregion

    #region Utility
    public void InitializeInventorySlot()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            InventorySlot slot = Instantiate(inventorySlotPrefab, inventoryContainer.transform);
            slot.transform.SetParent(inventoryContainer.transform);
            slot.Index = i;
            inventorySlots.Add(slot);
        }
    }
    private void InitializeButton()
    {
        allButton.onClick.AddListener(() => OnAllButtonClicked());
        equipmentButton.onClick.AddListener(() => OnEquipmentButtonClicked());
        otherButton.onClick.AddListener(() => OnOtherButtonClicked());
    }

    private void OnAllButtonClicked()
    {
        FilterInventory(ItemType.All);
    }

    private void OnEquipmentButtonClicked()
    {
        FilterInventory(ItemType.Equipment);
    }

    private void OnOtherButtonClicked()
    {
        FilterInventory(ItemType.Other); // Assuming 'Other' represents other item types. You might need to adjust the logic if you have multiple other categories.
    }

    public InventorySlot CheckIfItemIsInInventory(ItemData item, int amount)
    {
        InventorySlot outputSlot = null;
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.IsHavingItem())
            {
                DraggableItem itemInSlot = slot.transform.GetComponentInChildren<DraggableItem>();
                if (itemInSlot == null || itemInSlot.itemStack == null || itemInSlot.itemStack.item == null)
                {
                    Debug.Log("Item in slot is null in CheckIfItemIsInInventory");
                    continue;
                }

                if (itemInSlot.itemStack.item == item && (itemInSlot.itemStack.quantity + amount) < item.maxStack)
                {
                    outputSlot = slot;
                    return outputSlot;
                }
            }
        }
        return outputSlot;
    }

    public bool CheckInventoryFull(int amount)
    {
        int count = 0;

        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.IsHavingItem() == false)
            {
                count++;
            }
            if (count >= amount) return false;
        }
        return true;
    }

    public InventorySlot FindNextSlot()
    {
        lock (_inventoryLock)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (!slot.IsHavingItem())
                {
                    slot.ReserveSlot();
                    return slot;
                }
            }
            return null;

        }
    }

    public void DeactiveAllHightlight()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.hightlightBorder.SetActive(false);
        }
    }
    #endregion

    #region Save and Load Inventory

    private void LoadInventoryItems()
    {
        if (InventoryContainer.Instance == null)
        {
            Debug.LogError("InventoryContainer is not found in InventoryManager");
            return;
        }

        _inventoryItems = InventoryContainer.Instance.PlayerItemMap;
    }

    #endregion

}