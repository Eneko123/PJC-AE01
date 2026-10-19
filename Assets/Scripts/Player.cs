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

    private void Update()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = GroundedStick;
        else
            verticalVelocity += Gravity * Time.deltaTime;

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void OnMove(InputValue value)
    {
        moveInput = canPlayerMove ? value.Get<Vector2>() : Vector2.zero;
    }

    private void OnJump(InputValue value)
    {
        if (canPlayerMove && value.isPressed && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Gravity);
    }

    public bool GetCanPlayerMove() => canPlayerMove;
    public bool GetCanCameraMove() => canCameraMove;
    public void SetCanPlayerMove(bool newCanPlayerMove) => canPlayerMove = newCanPlayerMove;
    public void SetCanCameraMove(bool newCanCameraMove) => canCameraMove = newCanCameraMove;
}