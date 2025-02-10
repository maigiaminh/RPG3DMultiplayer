using System;
using UnityEngine;

public class CollectedCoinQuestStep : CollectedQuestStep
{

    protected override void OnEnable()
    {
        GameEventManager.Instance.ResourceEvents.onPlayerGainGold += OnPlayerCollectCoins;

    }
    protected override void OnDisable()
    {
        GameEventManager.Instance.ResourceEvents.onPlayerGainGold -= OnPlayerCollectCoins;

    }

    private void OnPlayerCollectCoins(int amount)
    {
        base.OnPlayerCollect(amount);
        // ResourceManager.Instance.AddGold(amount);
    }

    public override string GetQuestProgressContent()
    {
        return _currentItemAmount + "/" + TargetItemAmount + " " + "Coins";
    }
}
