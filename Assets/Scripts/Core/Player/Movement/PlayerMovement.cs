using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private NetworkVariable<short> _animState = new NetworkVariable<short>();


    private enum AnimState
    {
        Idle = 0,
        Walk = 1,
        Talk = 2
    }


    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CameraController cameraController;
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float rotateSpeed = 5;
    private Rigidbody rb;
    public Vector2 previousInput;

    private Animator _animator;
    private float ping;
    private float lastPingTime;
    #region Behaviour Callbacks



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        Time.timeScale = 1;
    }

    private void Update(){
        if (IsClient && IsOwner) {
        if (!NetworkManager.Singleton.IsConnectedClient) {
            Debug.Log("⚠ [Client] Chưa kết nối vào Server!");
            return;
        }

        if (Time.time - lastPingTime > 1f) {
            lastPingTime = Time.time;
            Debug.Log("⏳ [Client] Gửi yêu cầu Ping đến Server...");
            RequestPingServerRpc(NetworkManager.Singleton.LocalTime.Time);
        }
    }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn");
        _animState.OnValueChanged += HandleAnimChange;
        HandleAnimStateServerRpc(Vector2.zero);
    }

    public override void OnNetworkDespawn()
    {
        _animState.OnValueChanged -= HandleAnimChange;


    }



    private void FixedUpdate()
    {
        if (!IsOwner) return;
        HandleMove(inputReader.MovementValue);
        HandleThirdPersonViewPlayerMove(previousInput);
        HandleAnimStateServerRpc(previousInput);
    }






    #endregion

    #region Movement Script
    private void HandleThirdPersonViewPlayerMove(Vector2 input)
    {
        // Di chuyển theo trục X và Z chỉ khi input.y (tiến lùi) có giá trị khác 0
        if (input.y != 0)
        {
            // Di chuyển nhân vật theo hướng input.y (tiến/lùi)
            Vector3 moveDirection = new Vector3(0, 0, input.y).normalized;
            Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + transform.forward * movement.z);
        }

        // Quay nhân vật khi nhấn trái phải (input.x != 0)
        if (input.x != 0)
        {
            // Quay nhân vật theo hướng input.x (trái/phải)
            Quaternion currentRotation = transform.rotation;
            Quaternion rotation = Quaternion.Euler(0, currentRotation.eulerAngles.y + input.x * rotateSpeed, 0);
            transform.rotation = rotation;
        }
    }


    private void HandleMove(Vector2 vector)
    {
        previousInput = vector;
    }

    #endregion

    #region Other Methods

    #endregion

    private void HandleAnimChange(short previousValue, short newValue)
    {
        _animator.SetInteger("AnimState", newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleAnimStateServerRpc(Vector2 direct)
    {
        if (direct != Vector2.zero)
        {
            if (_animState.Value != (int)AnimState.Walk)
            {
                _animState.Value = (int)AnimState.Walk;
            }
        }
        else
        {
            if (_animState.Value != (int)AnimState.Idle)
            {
                _animState.Value = (int)AnimState.Idle;
            }
        }

    }

    [ServerRpc]
    private void RequestPingServerRpc(double clientTime, ServerRpcParams rpcParams = default) {
        Debug.Log("📡 [Server] Nhận ping request từ client: " + rpcParams.Receive.SenderClientId);
        Debug.Log("📡 Client Time: " + clientTime);
        RespondPingClientRpc(clientTime, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void RespondPingClientRpc(double clientTime, ulong clientId) {
        Debug.Log("📡 [Client] Nhận phản hồi ping từ server, tính toán ping...");
        Debug.Log("📡 Server Time: " + NetworkManager.Singleton.LocalTime.Time);

        if (clientId == NetworkManager.Singleton.LocalClientId) {
            double roundTripTime = NetworkManager.Singleton.LocalTime.Time - clientTime;
            ping = (float)(roundTripTime * 1000f); // Chuyển thành mili-giây
            Debug.Log("Ping: " + ping + "ms");
        }
    }
}
