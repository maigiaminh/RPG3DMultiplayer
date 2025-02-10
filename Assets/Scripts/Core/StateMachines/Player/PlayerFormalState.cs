using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class PlayerFormalState : PlayerBaseState
{
    private readonly int FreeLookSpeedHash = Animator.StringToHash("FormalFreeLook");
    
    private const float AnimatorDampTime = 0.1f;
    private float runSpeed = 0.1f;
    // private float speed = 0.5f;
    private int sheathHash;
    private int blendTreeHash;
    public PlayerFormalState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.InputReader.TestEvent += OnReturn;
        sheathHash = stateMachine.AnimationController.GetFormalStand((int)stateMachine.WeaponController.weaponLogic.weaponType);
        blendTreeHash = stateMachine.AnimationController.GetFreeLookBlendTree(-1);
        stateMachine.WeaponController.ChangeWeapon(0, "Unarmed");

        stateMachine.Animator.CrossFadeInFixedTime(sheathHash, AnimatorDampTime);
    }

    public override void Tick(float deltaTime)
    {   
        if(!stateMachine.AnimationController.animator.GetBool("Done-Anim")) { return; }

        Vector3 movement = CalculateMovement();
        
        Move(movement * stateMachine.FreeLookMovementSpeed * runSpeed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 1, AnimatorDampTime, deltaTime);

        FaceMovementDirection(movement, deltaTime);

    }

    public override void Exit()
    {
        stateMachine.InputReader.TestEvent -= OnReturn;
    }

    private void OnReturn()
    {        
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }

}
