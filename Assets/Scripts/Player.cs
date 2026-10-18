using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(PlayerInput))]

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    private bool canPlayerMove = true;
    private bool canCameraMove = true;
    private bool onFloor;
    //private Vector3 playerOffset = new Vector3(0, 0.5f, 0);

    private float verticalVelocity;
    private float gravity = -9.8f;
    private Vector2 moveInput;

    private LayerMask floorLayer;
    private float rayDistance = 1.01f;

    private void Awake()
    {
        floorLayer = LayerMask.GetMask("Floor");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance, floorLayer))
        {
            onFloor = true;
            // Debug.Log($"Tocando? {hit}, {onFloor}. Layer: {floorLayer}");
        }
        else
        {
            onFloor= false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (onFloor && verticalVelocity < 0f)
        {
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = Vector3.zero;
        move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        move.y = verticalVelocity;
        transform.position += move * Time.deltaTime;
    }

    private void OnMove(InputValue value)
    {
        if (canPlayerMove)
        {
            moveInput = value.Get<Vector2>();
        }
    }
    private void OnJump(InputValue value)
    {
        if (canPlayerMove && value.isPressed && onFloor)
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void OnDrawGizmos()
    {
        if (onFloor)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
    }


    public bool GetCanPlayerMove()
    {
        return canPlayerMove;
    }
    public bool GetCanCameraMove()
    {
        return canCameraMove;
    }
    public void SetCanPlayerMove(bool newCanPlayerMove) => canPlayerMove = newCanPlayerMove;
    
    public void SetCanCameraMove(bool newCanCameraMove) => canCameraMove = newCanCameraMove;

}
