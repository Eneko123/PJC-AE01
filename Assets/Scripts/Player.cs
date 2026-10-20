using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.2f;

    private bool canPlayerMove = true;
    private bool canCameraMove = true;

    private CharacterController controller;
    private Vector2 moveInput;
    private float verticalVelocity;
    private const float Gravity = -9.8f;
    private const float GroundedStick = -2f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        GetComponent<PlayerInput>().actions.FindActionMap("UI").Enable();
    }

    private void Update()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = GroundedStick;
        else
            verticalVelocity += Gravity * Time.deltaTime;

        // Movimiento relativo a hacia dónde mira el jugador
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);   // un único Move por frame
    }

    private void OnMove(InputValue value)
    {
        moveInput = canPlayerMove ? value.Get<Vector2>() : Vector2.zero;
    }

    private void OnJump(InputValue value)
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;   
        if (canPlayerMove && value.isPressed && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Gravity);
    }

    private void OnPause(InputValue value)
    {
        if (value.isPressed && GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    public bool GetCanPlayerMove() => canPlayerMove;
    public bool GetCanCameraMove() => canCameraMove;
    public void SetCanPlayerMove(bool newCanPlayerMove) => canPlayerMove = newCanPlayerMove;
    public void SetCanCameraMove(bool newCanCameraMove) => canCameraMove = newCanCameraMove;
}