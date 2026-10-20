using UnityEngine;
using UnityEngine.InputSystem;

// El cuerpo gira en horizontal (yaw) y la cámara en vertical (pitch).
[RequireComponent(typeof(Player), typeof(PlayerInput))]
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;

    [Header("Mando (grados por segundo)")]
    [SerializeField] private float stickSpeed = 220f;

    [Header("Ratón (grados por píxel)")]
    [SerializeField] private float mouseSensitivity = 0.07f;

    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool invertY = false;

    private Player player;
    private InputAction lookAction;
    private Vector2 lookInput;
    private bool fromMouse;
    private float pitch;

    private void Awake()
    {
        player = GetComponent<Player>();
        lookAction = GetComponent<PlayerInput>().actions["Look"];
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        // El ratón entrega píxeles por frame; el stick entrega -1..1. Se tratan distinto.
        fromMouse = lookAction.activeControl != null && lookAction.activeControl.device is Mouse;
    }

    private void Update()
    {
        if (!player.GetCanCameraMove()) return;
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;  

        Vector2 delta;
        if (fromMouse)
        {
            delta = lookInput * mouseSensitivity;              
            lookInput = Vector2.zero;                          
        }
        else
        {
            delta = lookInput * stickSpeed * Time.deltaTime;  
        }

        pitch = Mathf.Clamp(pitch + (invertY ? delta.y : -delta.y), -maxPitch, maxPitch);
        transform.Rotate(0f, delta.x, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}