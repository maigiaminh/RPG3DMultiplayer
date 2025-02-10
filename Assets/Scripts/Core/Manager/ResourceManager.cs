using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] private bool _loadResourceState = true;


    public int Misc { get; private set; }
    public int Gold { get; private set; }

    private const string ResourceKey = "Gold";

    protected override void Awake()
    {
        base.Awake();
        LoadResource();
    }

    private void OnEnable()
    {
        if (GameEventManager.Instance == null)
        {
            Debug.LogError("GameEventManager is null");
        }
        if (GameEventManager.Instance.ResourceEvents == null)
        {
            Debug.LogError("GameEventManager.GoldEvents is null");
        }
        GameEventManager.Instance.ResourceEvents.onPlayerGainGold += AddGold;
        GameEventManager.Instance.ResourceEvents.onPlayerLoseGold += SpendGold;

        GameEventManager.Instance.ResourceEvents.onPlayerGainMisc += AddMisc;
        GameEventManager.Instance.ResourceEvents.onPlayerLoseMisc += SpendMisc;

    }

    private void OnDisable()
    {
        if (GameEventManager.Instance == null)
        {
            Debug.LogError("GameEventManager is null");
        }
        if (GameEventManager.Instance.ResourceEvents == null)
        {
            Debug.LogError("GameEventManager.GoldEvents is null");
        }
        GameEventManager.Instance.ResourceEvents.onPlayerGainGold -= AddGold;
        GameEventManager.Instance.ResourceEvents.onPlayerLoseGold -= SpendGold;

        GameEventManager.Instance.ResourceEvents.onPlayerGainMisc -= AddMisc;
        GameEventManager.Instance.ResourceEvents.onPlayerLoseMisc -= SpendMisc;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameEventManager.Instance.ResourceEvents.PlayerGainGold(100);
        } else if(Input.GetKeyDown(KeyCode.H))
        {
            GameEventManager.Instance.PlayerEvents.PlayerGainExperience(100);
        }
    }

    private void SpendGold(int amount)
    {
        Gold -= amount;
        if (Gold < 0)
        {
            Gold = 0;
        }
        ResourceView.Instance.UpdateGoldText(Gold);
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        ResourceView.Instance.UpdateGoldText(Gold);
    }

    public bool CheckGold(int amount){
        return (Gold - amount) >= 0; 
    }
    
    private void SpendMisc(int amount)
    {
        Misc -= amount;
        if (Misc < 0)
        {
            Misc = 0;
        }
        ResourceView.Instance.UpdateMiscText(Gold);
    }

    private void AddMisc(int amount)
    {
        Misc += amount;
        ResourceView.Instance.UpdateMiscText(Gold);
    }

    private void LoadResource()
    {
        Gold = UserContainer.Instance.UserData.Gold;
        Misc = UserContainer.Instance.UserData.Misc;
        ResourceView.Instance.UpdateGoldText(Gold);
        ResourceView.Instance.UpdateMiscText(Misc);
    }
}
