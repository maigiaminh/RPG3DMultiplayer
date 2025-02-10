using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertBoardManager : Singleton<AlertBoardManager>
{
    // Item added Alert Board
    [SerializeField] private GameObject itemAddedAlertBoard;
    [SerializeField] private ItemAddedAlert itemAddedAlertPrefab;
    // Item added Notice Panel
    [SerializeField] private GameObject itemAddedNoticePanel;
    [SerializeField] private ItemAddedAlert itemAddedNoticePrefab;




    private void OnEnable() {
        GameEventManager.Instance.InventoryEvents.OnItemAdded += OnItemAdded;
    }
    private void OnDisable(){
        GameEventManager.Instance.InventoryEvents.OnItemAdded -= OnItemAdded;
    }



    private void OnItemAdded(ItemData data, int amount, bool isDictionaryChanged = true)
    {
        if(itemAddedAlertBoard.transform.childCount < 3){
            ItemAddedAlert itemAddedAlert = Instantiate(itemAddedAlertPrefab, itemAddedAlertBoard.transform);
            itemAddedAlert.Initialize(data, amount);
            itemAddedAlert.transform.SetParent(itemAddedAlertBoard.transform);
        }
    }
}
