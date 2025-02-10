using UnityEngine;

public class PlayerInteractingState : PlayerBaseState
{
    private Transform lookAtTransform;
    private const float AnimatorDampTime = 0.1f;
    private int blendTreeHash;

    public PlayerInteractingState(PlayerStateMachine stateMachine, Transform trans) : base(stateMachine) 
    {
        lookAtTransform = trans;
    }

    public override void Enter()
    {
        stateMachine.InputReader.OpenMouseModeEvent += ExitInteractState;
        GameEventManager.Instance.PlayerEvents.OnPlayerInteractStateExit += ExitInteractState;
        GameEventManager.Instance.PlayerEvents.OnUnEquipWeapon += UnEquipWeapon;
        CursorManager.Instance.ChangeCursorMode(CursorLockMode.None, true);
        stateMachine.PlayerCameraController.SwitchToInteract();
        stateMachine.Animator.SetFloat("FreeLookSpeed", 0f);
    }

    public override void Tick(float deltaTime)
    {
        RotateToLookAtTransform(deltaTime);
        Move(deltaTime);
    }

    public override void Exit()
    {
        stateMachine.InputReader.OpenMouseModeEvent -= ExitInteractState;
        GameEventManager.Instance.PlayerEvents.OnPlayerInteractStateExit -= ExitInteractState;
        GameEventManager.Instance.PlayerEvents.OnUnEquipWeapon -= UnEquipWeapon;

        CursorManager.Instance.ChangeCursorMode(CursorLockMode.Locked, false);
    }

    private void ExitInteractState(){
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }

    private void RotateToLookAtTransform(float deltaTime)
    {
        if (lookAtTransform == null) return;

        Vector3 direction = (lookAtTransform.position - stateMachine.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, lookRotation, deltaTime * 2.5f);
    }

    private void UnEquipWeapon(){
        if(stateMachine.WeaponController.weaponLogic != null){
            blendTreeHash = stateMachine.AnimationController.GetFreeLookBlendTree((int) stateMachine.WeaponController.weaponLogic.weaponType);
        }
        else{
            blendTreeHash = stateMachine.AnimationController.GetFreeLookBlendTree(0);
        }
        
        stateMachine.Animator.CrossFadeInFixedTime(blendTreeHash, AnimatorDampTime);
        stateMachine.Animator.SetFloat("FreeLookSpeed", 0f);
    }
}
