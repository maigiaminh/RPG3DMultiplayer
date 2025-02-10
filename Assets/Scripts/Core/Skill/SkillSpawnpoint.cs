using UnityEngine;

public class SkillSpawnpoint : MonoBehaviour
{
    public enum SpawnpointType {
        LeftHand = 0,
        RightHand = 1,
        Head = 2,
        Chest = 3,
        Legs = 4,
        Feet = 5,
        UnderCenter = 6,
        ForwardNear = 7,
        ForwardFar = 8,
        Forward8cm = 9,
        Forward4cm = 10,
    }

    public SpawnpointType type;

    public Vector3 GetPosition() => transform.position;

}
