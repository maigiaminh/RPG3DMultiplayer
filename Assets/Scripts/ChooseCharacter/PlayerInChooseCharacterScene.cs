using UnityEngine;

public class PlayerInChooseCharacterScene : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; 
    [SerializeField] private float movementSpeed;
    [SerializeField] private float RotationDamping;
    private InputReader inputReader;
    private Animator animator;
    private CharacterController characterController;
    private ForceReceiver forceReceiver;

    private readonly int MoveHash = Animator.StringToHash("Movement");

    private const float AnimatorDampTime = 0.1f;
    void Start()
    {
        animator = GetComponent<Animator>();
        inputReader = GetComponent<InputReader>();
        characterController = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();
        CursorManager.Instance.ChangeCursorMode(CursorLockMode.Locked, false);
    }

    void Update()
    {
        Vector3 movement = CalculateMovement();
        
        Move(movement * movementSpeed, Time.deltaTime);
        
        if (inputReader.MovementValue == Vector2.zero)
        {
            animator.SetFloat(MoveHash, 0, AnimatorDampTime, Time.deltaTime);
            return;
        }

        animator.SetFloat(MoveHash, 1, AnimatorDampTime, Time.deltaTime);

        FaceMovementDirection(movement, Time.deltaTime);
    }

    private Vector3 CalculateMovement()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * inputReader.MovementValue.y +
            right * inputReader.MovementValue.x;
    }

    private void Move(Vector3 motion, float deltaTime)
    {
        if (characterController == null) { return; }
        characterController.Move((motion + forceReceiver.Movement) * deltaTime);
        // characterController.Move(motion);

    }

    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * RotationDamping);
    }

}
