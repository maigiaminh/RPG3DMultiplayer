using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerStateMachine : StateMachine, IDamageable
{
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    // [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Targeter Targeter { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public WeaponController WeaponController { get; private set; }
    [field: SerializeField] public PlayerAnimationController AnimationController { get; private set; }
    [field: SerializeField] public PlayerCameraController PlayerCameraController { get; private set; }
    [field: SerializeField] public Ragdoll Ragdoll { get; private set; }
    [field: SerializeField] public PlayerStatManager Stats { get; private set; }
    [field: SerializeField] public MultiAimConstraint HeadAimRig { get; private set; }
    [field: SerializeField] public Rig AimingRig { get; private set; }
    [field: SerializeField] public Transform HeadAimTarget { get; private set; }
    [field: SerializeField] public SkillManager SkillManager { get; private set; }

    [field: SerializeField] public Vector3 Momentum { get; private set; }
    [field: SerializeField] public float TargetingMovementSpeed { get; private set; }
    [field: SerializeField] public float FreeLookMovementSpeed { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public float DodgeDuration { get; private set; }
    [field: SerializeField] public float DodgeLength { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float RollLength { get; private set; }
    [field: SerializeField] public Transform MainCameraTransform { get; private set; }
    [field: SerializeField] public AudioSource LeftFoot { get; private set; }
    [field: SerializeField] public AudioSource RightFoot { get; private set; }
    [field: SerializeField] public AudioSource Chest { get; private set; }
    public Animator Animator;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Animator = GetComponent<Animator>();

        SwitchState(new PlayerFreeLookState(this));
    }

    private void OnEnable()
    {
        if (MainCameraTransform == null)
        {
            MainCameraTransform = Camera.main.transform;
        }
        GameEventManager.Instance.PlayerEvents.OnPlayerResetOnDie += ResetStateWhenDie;
        GameEventManager.Instance.PlayerEvents.OnPlayerFreeLookStateEnter += EnterFreeLookState;
        Stats.OnTakeDamage += HandleTakeDamage;
        Stats.OnDie += HandleDie;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.PlayerEvents.OnPlayerResetOnDie -= ResetStateWhenDie;
        GameEventManager.Instance.PlayerEvents.OnPlayerFreeLookStateEnter -= EnterFreeLookState;
        Stats.OnTakeDamage -= HandleTakeDamage;
        Stats.OnDie -= HandleDie;
    }

    private void HandleTakeDamage()
    {
        SwitchState(new PlayerImpactState(this));
    }
    private void HandleDie()
    {
        Debug.Log("Player Die");
        SwitchState(new PlayerDeadState(this));
    }

    public void SetMomentum(Vector3 momentum) => Momentum = momentum;

    public void ResetFreeLook()
    {
        AnimationController.animator.CrossFadeInFixedTime(AnimationController.GetFreeLookBlendTree((int)WeaponController.weaponLogic.weaponType), 0.1f);
    }

    public void TakeDamage(Transform trans, int damage)
    {
        if (currentState != null && currentState is not PlayerImpactState)
        {
            if (TryGetComponent<ForceReceiver>(out ForceReceiver forceReceiver))
            {
                Vector3 direction = trans != null ? (transform.position - trans.position).normalized : transform.forward;
                forceReceiver.AddForce(direction * 20f);
            }

            if(currentState.CanTakeDamage()){
                var dmg = damage - Mathf.CeilToInt(PlayerStatManager.Instance.Defense / 10) > 0 ? damage - Mathf.CeilToInt(PlayerStatManager.Instance.Defense / 10) : 0;
                Stats.DealDamage(dmg);
            }

            SwitchState(new PlayerImpactState(this));
        }
        
        
        
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public void ApplyCC(CCType ccType, float ccDuration)
    {

    }

    public void PlayLeftFoot()
    {
        PlayFootstepSound(LeftFoot);
    }

    public void PlayRightFoot()
    {
        PlayFootstepSound(RightFoot);
    }

    public void PlayFootstepSound(AudioSource side)
    {
        if (Controller.isGrounded && Controller.velocity.magnitude > 0.1f)
        {
            if (Animator.GetFloat("FreeLookSpeed") >= 0.1 || Animator.GetFloat("FormalFreeLook") >= 0.1)
            {
                Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1f))
                {
                    var newTag = hit.collider.tag;
                    var sound = SoundManager.GetFootstepSound(newTag);
                    SoundManager.PlaySound(sound, side);
                }
            }
        }
    }

    public void PlayAttackSound(int currentWeapon)
    {
        var sound = SoundManager.GetAttackSound(currentWeapon);
        SoundManager.PlaySound(sound, Chest);
    }

    public void PlayBowDraw()
    {
        SoundManager.PlaySound(SoundType.BOW_DRAW, Chest);
    }

    public void PlayBowHandle()
    {
        SoundManager.PlaySound(SoundType.BOW_HANDLE, Chest);
    }


    private void ResetStateWhenDie()
    {
        Debug.Log("ResetStateWhenDie");

        Ragdoll.ToggleRagdoll(false);
        WeaponController.EnableLeft();
        WeaponController.EnableRight();
        SwitchState(new PlayerFreeLookState(this));
    }

    private void EnterFreeLookState()
    {
        SwitchState(new PlayerFreeLookState(this));
    }


}

