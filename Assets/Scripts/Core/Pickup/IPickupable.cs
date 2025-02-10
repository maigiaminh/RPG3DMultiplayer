using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPickupable : MonoBehaviour
{
   protected bool isDestroy = false;
   [field: SerializeField] public ItemData data { get; set; }
   [SerializeField] private Vector3 lightVfxOffset = new Vector3(0, 1, 0);
   protected RarityVFX _rarityVFX;

   protected void Start() {
      Invoke("InitializeLightVFX", 1f);
   }
   public virtual void PickUp(int amount)
   {
      if (isDestroy) return;
      if (InventoryManager.Instance.AddItemToInventory(data, amount))
      {
         Debug.Log("Sword picked up and needs to be added to inventory");
         isDestroy = true;
         RarityLightVFXManager.Instance.ReleaseVFX(data.rarity, _rarityVFX);
         Destroy(gameObject);
      }
      else
      {
         Debug.Log("Sword picked up but inventory is full or something is adding, please wait a second");
      }

   }

   public bool CanPickUp(int amount){
      if (isDestroy) return false;
      if (InventoryManager.Instance.AddItemToInventory(data, amount))
      {
         Debug.Log("Sword picked up and needs to be added to inventory");
         isDestroy = true;
         RarityLightVFXManager.Instance.ReleaseVFX(data.rarity, _rarityVFX);
         Destroy(gameObject);
         return true;
      }
      else
      {
         Debug.Log("Sword picked up but inventory is full or something is adding, please wait a second");
         return false;
      }
   }

   public virtual Transform GetTransform() => transform;


   protected void InitializeLightVFX()
   {
      Debug.Log("InitializeLightVFX");
      if(RarityLightVFXManager.Instance){
         _rarityVFX = RarityLightVFXManager.Instance.GetVFX(data.rarity, transform.position, transform.rotation);
         _rarityVFX.transform.position = transform.position;
         _rarityVFX.transform.eulerAngles = Vector3.zero;
      } else {
         Debug.LogError("RarityLightVFXManager is not found");
      }
   }

}
