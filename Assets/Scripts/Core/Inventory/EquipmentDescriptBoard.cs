using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDescriptBoard : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemRarityText;
    public Image itemRerityBackground;
    // ... other UI elements (itemSet2Icon, itemSet4Icon, etc.)

    public GameObject mainStatsPanel;
    public GameObject mainStatPrefab;
    private RectTransform rectTransform;


    public GameObject EquippedGO;
    public GameObject UnEquippedGO;

    private const int HEIGHT = 475;
    private const int DELTA_Y = 50;



    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetItemDescript(ItemData itemData)
    {
        ResetInitState();
        UpdateEquipButton(itemData);

        Color itemRarityColor = RarityColor.GetRarityColor(itemData.rarity); // Assuming you have a RarityColor helper class
        itemNameText.text = itemData.itemName;
        itemRarityText.text = itemData.rarity.ToString();
        itemRarityText.color = itemRarityColor;
        itemRerityBackground.color = itemRarityColor;
        // ... set other UI elements (set icons, set stats, etc.)

        icon.sprite = itemData.icon;


        List<KeyValuePair<string, string>> keyValuePairs = itemData.GetNonZeroEquipmentStatsAsString();
        int count = 0;
        foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
        {
            GameObject mainStat = Instantiate(mainStatPrefab, mainStatsPanel.transform);
            mainStat.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = keyValuePair.Value + " " + keyValuePair.Key;
            // ... set icon (if needed)
            count++;
        }

        ModifyPanelHeight(count);
    }


    private void ModifyPanelHeight(int count)
    {
        int height = (count <= 1) ? HEIGHT : HEIGHT + (count - 1) * DELTA_Y;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    public void ResetInitState()
    {
        foreach (Transform child in mainStatsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }


    private void UpdateEquipButton(ItemData itemData)
    {
        if (!QuickSlotManager.Instance.quickSlot.currentItem)
        {
            EquippedGO.SetActive(false);
            UnEquippedGO.SetActive(true);
            return;
        }
        bool isEquipped = QuickSlotManager.Instance.quickSlot.currentItem.itemStack.item == itemData;
        EquippedGO.SetActive(isEquipped);
        UnEquippedGO.SetActive(!isEquipped);
    }
}