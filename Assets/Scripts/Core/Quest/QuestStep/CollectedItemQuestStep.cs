using System;
using UnityEngine;

public class CollectedItemQuestStep : CollectedQuestStep
{

    [SerializeField] private ItemData _itemData;

    protected override void OnEnable()
    {
        GameEventManager.Instance.InventoryEvents.OnItemAdded += OnItemAdded;
    }
    protected override void OnDisable()
    {
        GameEventManager.Instance.InventoryEvents.OnItemAdded -= OnItemAdded;
    }


    private void OnItemAdded(ItemData data, int amount, bool isDictionaryUpdate)
    {
        if (data != _itemData) return;
        base.OnPlayerCollect(amount);
    }

    public override string GetQuestProgressContent()
    {
        return _currentItemAmount + "/" + TargetItemAmount + " " + _itemData.itemName;
    }
}
