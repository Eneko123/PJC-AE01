using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private float pickupRadius = 0.3f;
    [SerializeField] private float throwSpeed = 12f;
    [SerializeField] private float throwUpward = 2f;
    [SerializeField] private LayerMask ballLayer;

    private Ball held;

    private bool CanAct => GameManager.Instance == null || GameManager.Instance.IsPlaying;

    private void OnTake(InputValue value)
    {
        if (!value.isPressed || held != null || !CanAct) return;

        // Coge la bola a la que miras
        if (!Physics.SphereCast(cameraPivot.position, pickupRadius, cameraPivot.forward, out RaycastHit hit, pickupDistance, ballLayer)) return;

        if (!hit.collider.TryGetComponent(out Ball ball) || ball.IsHeld) return;

        held = ball;
        held.PickUp(holdPoint);
        DevLog.Log("Bola recogida");
    }

    private void OnThrow(InputValue value)
    {
        if (!value.isPressed || held == null || !CanAct) return;

        held.Throw(cameraPivot.forward * throwSpeed + Vector3.up * throwUpward);
        held = null;
        DevLog.Log("Bola lanzada");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (cameraPivot == null) return;

        Vector3 origin = cameraPivot.position;
        Vector3 dir = cameraPivot.forward;

        // Verde: no hay bola a la vista. Rojo: se coge la bola a la que apuntas.
        bool hitBall = Physics.SphereCast(origin, pickupRadius, dir, out RaycastHit hit, pickupDistance, ballLayer);
        Gizmos.color = hitBall ? Color.red : Color.green;

        float length = hitBall ? hit.distance : pickupDistance;
        Vector3 end = origin + dir * length;

        Gizmos.DrawWireSphere(origin, pickupRadius);   // inicio del cast
        Gizmos.DrawWireSphere(end, pickupRadius);      // final
        Gizmos.DrawLine(origin, end);                  // trayectoria

        if (hitBall) Gizmos.DrawSphere(hit.point, 0.05f);  // punto exacto de contacto
    }
#endif
}