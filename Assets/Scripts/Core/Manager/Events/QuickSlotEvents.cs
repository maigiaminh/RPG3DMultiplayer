
using System;

public class WeaponQuickSlotEvents 
{

    public event Action<ItemData> OnWeaponChanged;
    public void WeaponChanged(ItemData item)
    {
        OnWeaponChanged?.Invoke(item);
    }
}
