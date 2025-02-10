using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceEvents
{
   public event Action<int> onPlayerGainGold;
    public void PlayerGainGold(int gold)
    {
        onPlayerGainGold?.Invoke(gold);
    }
    public event Action<int> onPlayerLoseGold;
    public void PlayerLoseGold(int gold)
    {
        onPlayerLoseGold?.Invoke(gold);
    }
    public event Action<int> onPlayerGainMisc;
    public void PlayerGainMisc(int misc)
    {
        onPlayerGainMisc?.Invoke(misc);
    }
    public event Action<int> onPlayerLoseMisc;
    public void PlayerLoseMisc(int misc)
    {
        onPlayerLoseMisc?.Invoke(misc);
    }
}
