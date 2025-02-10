using System;
using UnityEngine;

public class NpcAnimationController : MonoBehaviour
{
    [field: SerializeField] Animator animator { get; set; }

    public readonly int IdleHashKey = Animator.StringToHash("Idle");
    public readonly int WalkHashKey = Animator.StringToHash("Walk");
    public readonly int TalkHashKey = Animator.StringToHash("Talk");
    public readonly int Dance1HashKey = Animator.StringToHash("Dance-1");
    public readonly int Dance2HashKey = Animator.StringToHash("Dance-2");
    public readonly int Dance3HashKey = Animator.StringToHash("Dance-3");
    public readonly int WaveHandsHashKey = Animator.StringToHash("WaveHands");
    public readonly int LolHashKey = Animator.StringToHash("LOL");
    public readonly int BlowKissKey = Animator.StringToHash("BlowKiss");
    public readonly int SmileKey = Animator.StringToHash("Smile");


    public int GetSpecialType(NpcSpecialState state, int specialType)
    {
        if (state is NpcDanceState)
        {
            switch (specialType)
            {
                case 1:
                    return Dance1HashKey;
                case 2:
                    return Dance2HashKey;
                case 3:
                    return Dance3HashKey;
                default:
                    return Dance1HashKey;
            }
        }
        else return 0;
    }

}
