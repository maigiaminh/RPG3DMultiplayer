using System;
using UnityEngine;

public class DungeonTokenEvents 
{
    public event Action<int> OnTokenAdded;
    public event Action<int> OnTokenRemoved;
    public void AddToken(int amount)
    {
        OnTokenAdded?.Invoke(amount);
    }
    public void RemoveToken(int amount)
    {
        OnTokenRemoved?.Invoke(amount);
    }
}
