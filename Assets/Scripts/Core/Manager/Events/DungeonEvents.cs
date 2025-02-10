using System;
using UnityEngine;

public class DungeonEvents 
{
    public event Action<DungeonRoom> OnRoomEntered;
    public void RoomEntered(DungeonRoom room)
    {
        OnRoomEntered?.Invoke(room);
    }

    public event Action OnDungeonFinished;
    public void DungeonFinished()
    {
        OnDungeonFinished?.Invoke();
    }
}
