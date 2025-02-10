using UnityEngine;

public class ForceReceiver : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float drag = 5f;
    private Vector3 dampingVelocity;

    private Vector3 impact;

    private float verticalVelocity;
    public float VerticalVelocity => verticalVelocity;

    public Vector3 Movement => impact + Vector3.up * verticalVelocity;

    private void Update()
    {
        if (verticalVelocity < 0f && controller.isGrounded)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, drag);

        // impact = Vector3.Lerp(impact, Vector3.zero, drag * Time.deltaTime);
        
        // if(impact.sqrMagnitude < 0.2f * 0.2f)
        // {
        //     impact = Vector3.zero;
        // }
    }

    public void AddForce(Vector3 force)
    {
        impact += force;
    }
    
    public void Jump(float jumpForce)
    {
        verticalVelocity += jumpForce;
    }

    public void ResetVerticalVelocity()
    {
        verticalVelocity = 0f;
    }
}
