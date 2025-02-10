using System;
using UnityEngine;

public class PlayerEvents
{
    public event Action<int> OnPlayerLevelChange;
    public void PlayerLevelChange(int level)
    {
        OnPlayerLevelChange?.Invoke(level);
    }

    public event Action<int> OnPlayerGainExperience;
    public void PlayerGainExperience(int experience)
    {
        OnPlayerGainExperience?.Invoke(experience);
    }

    public event Action<IEnemy, int> OnPlayerDefeatEnemy;
    public void PlayerDefeatEnemy(IEnemy enemy, int amount)
    {
        OnPlayerDefeatEnemy?.Invoke(enemy, amount);
    }

    public event Action<Transform> OnPlayerInteractStateEnter;
    public void PlayerInteractStateEnter(Transform trans = null)
    {
        OnPlayerInteractStateEnter?.Invoke(trans);
    }

    public event Action OnPlayerInteractStateExit;
    public void PlayerInteractStateExit()
    {
        OnPlayerInteractStateExit?.Invoke();
    }

    public event Action OnPlayerFreeLookStateEnter;
    public void PlayerFreeLookStateEnter()
    {
        OnPlayerFreeLookStateEnter?.Invoke();
    }

    public event Action OnPlayerResetOnDie;
    public void PlayerResetOnDie()
    {
        OnPlayerResetOnDie?.Invoke();
    }

    public event Action OnUnEquipWeapon;
    public void UnEquipWeapon()
    {
        OnUnEquipWeapon?.Invoke();
    }

    public event Action OnPlayerTired;
    public void PlayerTired()
    {
        OnPlayerTired?.Invoke();
    }

    public event Action OnPlayerNotTired;
    public void PlayerNotTired()
    {
        OnPlayerNotTired?.Invoke();
    }
}
