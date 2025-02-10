using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public GameObject GetGameObject();
    public float GetHealth();
    public float GetMaxHealth();
}
