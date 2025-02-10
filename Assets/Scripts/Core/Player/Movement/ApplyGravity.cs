using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyGravity : MonoBehaviour
{
    private CharacterController characterController;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void FixedUpdate()
    {
        if (characterController.isGrounded) return;

        characterController.Move(Vector3.down * Time.deltaTime * 9.81f);
    }
}
