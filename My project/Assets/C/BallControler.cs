using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    public float initialSpeed = 5f;
    public float speedIncrement = 0.5f;
    private Rigidbody2D rb;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();

        // Solo el servidor controla la f�sica
        if (IsServer)
        {
            LaunchBall();
        }
    }

    private void LaunchBall()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(0, 2) == 0 ? -1 : 1;
        Vector2 direction = new Vector2(x, y).normalized;

        rb.linearVelocity = direction * initialSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return; // Solo el servidor procesa rebotes

        // Rebote con paletas
        if (collision.gameObject.CompareTag("Paddle"))
        {
            float hitPoint = (transform.position.y - collision.transform.position.y)
                             / collision.collider.bounds.size.y;

            Vector2 dir = new Vector2(rb.linearVelocity.x, hitPoint).normalized;
            rb.linearVelocity = dir * rb.linearVelocity.magnitude;
        }

        // Aceleraci�n progresiva
        rb.linearVelocity = rb.linearVelocity.normalized * (rb.linearVelocity.magnitude + speedIncrement);
    }
}