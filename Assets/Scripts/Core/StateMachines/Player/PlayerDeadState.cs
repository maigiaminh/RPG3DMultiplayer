using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Ragdoll.ToggleRagdoll(true);
        stateMachine.WeaponController.DisableLeft();
        stateMachine.WeaponController.DisableRight();
        stateMachine.PlayerCameraController.SwitchToInteract();
        
        SoundManager.PlaySound(SoundType.LOSE_MUSIC);
        CursorManager.Instance.ChangeCursorMode(CursorLockMode.None, true);
    }

    public override void Tick(float deltaTime) { }

    public override void Exit() { }
}
