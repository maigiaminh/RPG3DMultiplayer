using System;
using UnityEngine;

public class NpcStateMachine : StateMachine
{
    [Header("References")]
    [field: SerializeField] public NpcAnimationController AnimationController { get; private set; }
    [field: SerializeField] public GameObject HeadAimTarget { get; private set; }
    [field: SerializeField] public PlayerDetector PlayerDetector { get; private set; }

    [Header("Attributes")]
    [field: SerializeField] public float WalkSpeed { get; private set; }
    [field: SerializeField] public float RangeDetectPlayer { get; private set; }
    [field: SerializeField] public float TimeToRotate { get; private set; }
    [HideInInspector] public Animator animator;
    public Vector3 initialHeadPos { get; private set; }

    private void Start()
    {
        initialHeadPos = HeadAimTarget.transform.position;
        animator = GetComponent<Animator>();
        SwitchState(new NpcIdleState(this));
        if (AnimationController == null) AnimationController = GetComponent<NpcAnimationController>();
        if(PlayerDetector == null) PlayerDetector = GetComponent<PlayerDetector>();
    }
}
