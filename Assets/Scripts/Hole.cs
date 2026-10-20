using UnityEngine;

public class Hole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball ball) && !ball.IsHeld)
        {
            ball.Score();
            GameManager.Instance.AddScore();
        }
    }
}