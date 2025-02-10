using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : Singleton<ShopUIManager>
{
    [Header("Shop Panel")]
    public GameObject shopPanel;
    [Header("Category Data")]
    public List<Category> categories;

    [Header("UI References")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Image itemDetailImg;
    [SerializeField] private TextMeshProUGUI itemDetailTitle;
    [SerializeField] private TextMeshProUGUI itemDetailPrice;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI pageText;
    [Header("Buttons")]
    [SerializeField] private CategoryButton[] categoryButtons;

    private Dictionary<string, List<GameObject>> categoryItems = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();
    private Dictionary<string, CategoryButton> categoryButtonDictionary = new Dictionary<string, CategoryButton>();

    private int maxPage = 3;
    private int currentPage = 1;
    private int currentQuantity = 1;
    private int itemsPerPage = 6;
    private int currentPrice;
    private string currentCategory;
    private ItemButton currentItem;
    private ItemData currentData;
    private CategoryButton currentCategoryButton;
    private Animator quantityAnimator;

    private void Start()
    {
        if (quantityText != null)
        {
            quantityText.TryGetComponent<Animator>(out quantityAnimator);
        }

        foreach (var button in categoryButtons)
        {
            string key = button.gameObject.name;
            if (!categoryButtonDictionary.ContainsKey(key))
            {
                categoryButtonDictionary.Add(key, button);
                Debug.Log($"Added {key} to dictionary.");
            }
            else
            {
                Debug.LogWarning($"Duplicate key detected: {key}. Skipping...");
            }
        }

        currentCategoryButton = categoryButtonDictionary.First().Value;
        currentCategory = categoryButtonDictionary.First().Value.name;

        GenerateAllItems();
        ShowCategory(categories[0].categoryName);

        ResetSelectedItem();
    }



    public void RegisterUI(string key, GameObject element)
    {
        if (!uiElements.ContainsKey(key))
        {
            uiElements[key] = element;
        }
    }

    public GameObject GetUIElement(string key)
    {
        return uiElements.ContainsKey(key) ? uiElements[key] : null;
    }

    public CategoryButton GetCategoryButton(string key)
    {
        return categoryButtonDictionary.ContainsKey(key) ? categoryButtonDictionary[key] : null;
    }

    public void SendEventToUI(string key, string eventName)
    {
        if (uiElements.ContainsKey(key))
        {
            var interactable = uiElements[key].GetComponent<InteractableObject>();
            if (interactable != null)
            {
                switch (eventName)
                {
                    case "Hover":
                        interactable.OnHoverEnter();
                        break;
                    case "Click":
                        interactable.OnClick();
                        break;
                }
            }
        }
    }

    public void OnIncreaseQuantity()
    {
        currentQuantity = Mathf.Clamp(currentQuantity + 1, 1, 99);
        currentPrice = currentItem.shopItem.itemPrice * currentQuantity;
        UpdateQuantityUI();
    }

    public void OnDecreaseQuantity()
    {
        currentQuantity = Mathf.Clamp(currentQuantity - 1, 1, 99);
        currentPrice = currentItem.shopItem.itemPrice * currentQuantity;
        UpdateQuantityUI();
    }

    private void UpdateQuantityUI()
    {
        quantityText.text = currentQuantity.ToString("00");
        itemDetailPrice.text = currentPrice.ToString();
        quantityAnimator.Play("QuantitiyChangeValue");
    }

    public void OnChangeCategory(string name)
    {
        if (currentCategoryButton == null) return;
        if (currentCategoryButton.name == name) return;

        currentCategoryButton.GetComponent<Animator>()?.Play("CategoryUnselected");

        currentCategoryButton = GetCategoryButton(name);
        ShowCategory(GetCategoryButton(name).name);
        currentCategoryButton.GetComponent<Animator>()?.Play("CategorySelected");

        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
    }

    public void OnIncreasePage()
    {
        if (currentPage < GetTotalPages() - 1)
        {
            foreach (var itemGO in categoryItems[currentCategory])
            {
                itemGO.SetActive(false);
            }

            currentPage++;
            UpdatePagination();

            SoundManager.PlaySound(SoundType.PAGE);
        }

        //UpdatePageUI();
    }

    public void OnDecreasePage()
    {
        if (currentPage > 0)
        {
            foreach (var itemGO in categoryItems[currentCategory])
            {
                itemGO.SetActive(false);
            }

            currentPage--;
            UpdatePagination();

            SoundManager.PlaySound(SoundType.PAGE);
        }

        //UpdatePageUI();
    }

    private void UpdatePageUI()
    {
        pageText.text = $"PAGE {currentPage}/{maxPage}";
    }

    public void OnChangeItem()
    {
        //if(currentCategory.name == name) return;
        currentCategoryButton.GetComponent<Animator>()?.Play("ItemUnselected");

        currentCategoryButton = GetCategoryButton(name);
        currentCategoryButton.GetComponent<Animator>()?.Play("ItemSelected");
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
    }

    private void GenerateAllItems()
    {
        foreach (var category in categories)
        {
            var itemList = new List<GameObject>();

            foreach (var item in category.items)
            {
                GameObject itemGO = Instantiate(itemPrefab, itemContainer);
                itemGO.GetComponent<ItemButton>().SetupItem(item);
                itemGO.SetActive(false);
                itemList.Add(itemGO);
            }

            categoryItems[category.categoryName] = itemList;
        }
    }

    private void ShowCategory(string categoryName)
    {
        foreach (var itemGO in categoryItems[currentCategory])
        {
            itemGO.SetActive(false);
        }

        currentCategory = categoryName;
        currentPage = 0;
        UpdatePagination();
        ResetSelectedItem();
    }

    private int GetTotalPages()
    {
        int totalItems = categoryItems[currentCategory].Count;
        return Mathf.CeilToInt((float)totalItems / itemsPerPage);
    }

    private void UpdatePagination()
    {
        ResetSelectedItem();
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, categoryItems[currentCategory].Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            categoryItems[currentCategory][i].SetActive(true);
        }

        pageText.text = $"Page {currentPage + 1}/{GetTotalPages()}";
    }

    public void OnItemChange(ItemButton item)
    {
        if (currentItem != null)
        {
            currentItem.GetComponent<Animator>()?.SetBool("Selected", false);
        }

        currentItem = item;
        currentData = item.shopItem.data;
        currentPrice = item.shopItem.itemPrice;
        currentQuantity = 1;
        UpdateQuantityUI();

        itemDetailImg.sprite = item.shopItem.data.icon;
        itemDetailTitle.text = item.shopItem.data.itemName;
        itemDetailPrice.text = item.shopItem.itemPrice.ToString();

        currentItem.GetComponent<Animator>()?.SetBool("Selected", true);
    }

    private void ResetSelectedItem()
    {
        currentItem = null;
        itemDetailImg.sprite = Resources.Load<Sprite>("logo-removebg-preview");
        itemDetailTitle.text = null;
        itemDetailPrice.text = null;
    }

    public void OnCloseShopPanel()
    {
        shopPanel.GetComponent<Animator>()?.Play("ShopPanelClose");
        Invoke(nameof(CloseStore), 1.5f);
    }
    public void OpenStore()
    {
        shopPanel.gameObject.SetActive(true);
        GameEventManager.Instance.PlayerContactUIEvents.PlayerOpenStore();
    }
    public void CloseStore()
    {
        shopPanel.gameObject.SetActive(false);
        GameEventManager.Instance.PlayerContactUIEvents.PlayerCloseStore();
    }

    public void Purchase(){
        if(currentItem = null){
            Debug.Log("Current Purchase Item Empty");
        }
        else{
            if(ResourceManager.Instance.CheckGold(currentPrice)){
                if(InventoryManager.Instance.AddItemToInventory(currentData, currentQuantity)){
                    GameEventManager.Instance.ResourceEvents.PlayerLoseGold(currentPrice);
                    SoundManager.PlaySound(SoundType.COIN);
                }
                else{
                    Debug.Log("Inventory Is Full");
                }
            }
            else{
                Debug.Log("Not Enought Gold");
            }
        }
    }
}
