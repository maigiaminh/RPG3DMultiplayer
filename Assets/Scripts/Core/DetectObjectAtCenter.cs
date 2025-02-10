using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectObjectAtCenter : MonoBehaviour
{
    public float rangeDetectItem = 5f;
    public List<IPickupable> pickupables = new List<IPickupable>();

    public IPickupable currentItem = null;
    public InputReader inputReader;
    private bool canPickUp = false;

    private void OnEnable()
    {
        inputReader.PickUpEvent += PickUp;
    }

    private void OnDisable()
    {
        inputReader.PickUpEvent -= PickUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Pickupable")) return;
        other.TryGetComponent<IPickupable>(out IPickupable pickupable);
        if (pickupable == null) return;

        pickupables.Add(pickupable);
        Debug.Log("Found pickupable object: " + pickupable.data.itemName);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Pickupable")) return;
        other.TryGetComponent<IPickupable>(out IPickupable pickupable);
        if (pickupable == null) return;
        if (!pickupables.Contains(pickupable)) return;

        pickupables.Remove(pickupable);
    }

    private void Update()
    {
        if (pickupables.Count == 0)
        {
            ActivateNoticePanel(false);
            return;
        }

        int min = Int32.MaxValue;
        Optional<IPickupable> tempItem = null;

        foreach (IPickupable pickupable in pickupables)
        {
            if (Vector3.Distance(pickupable.GetTransform().position, transform.position) > min) continue;
            tempItem = pickupable;
        }

        tempItem.Match(
            (item) => ActivateNoticePanel(false),
            () => { Debug.LogError("IPickupable is null in DetectObjectAtcenter"); return; }
        );


        currentItem = tempItem;

        if (ItemNoticePanelControl.Instance == null)
        {
            Debug.LogError("ItemNoticePanelControl is null");
            return;
        }

        ItemNoticePanelControl.Instance.Initialize(currentItem.data);
        ActivateNoticePanel(true);
        
    }

    private void ActivateNoticePanel(bool isActive)
    {
        canPickUp = isActive;
        if (ItemNoticePanelControl.Instance == null) return;
        ItemNoticePanelControl.Instance.panel.SetActive(isActive);
        ItemNoticePanelControl.Instance.pickupKey.text = $"Press {InputManager.Instance.GetKeyName("PickUp")} To Add";
    }

    private void PickUp()
    {
        if (!canPickUp) return;
        if (currentItem == null) return;

        Debug.Log("Pickup item: " + currentItem.data.itemName);
        if(!currentItem.CanPickUp(1)) return;
        
        SoundManager.PlaySound(SoundType.PICKUP);
        pickupables.Remove(currentItem);
        ActivateNoticePanel(false);
        currentItem = null;
    }
}
