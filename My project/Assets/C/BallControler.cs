using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    public float initialSpeed = 5f;
    public float speedIncrement = 0.5f;
    public float maxSpeed = 15f; // límite de velocidad máxima

    private Rigidbody2D rb;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();

        // Configuración recomendada para evitar tunneling
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

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
        if (!IsServer) return;

        // Rebote con paletas
        if (collision.gameObject.CompareTag("Paddle"))
        {
            float hitPoint = (transform.position.y - collision.transform.position.y)
                             / collision.collider.bounds.size.y;

            Vector2 dir = new Vector2(rb.linearVelocity.x, hitPoint).normalized;
            rb.linearVelocity = dir * rb.linearVelocity.magnitude;
        }

        // Rebote con paredes (si usas tag "Wall")
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
        }

        // Aceleración progresiva + límite de velocidad
        rb.linearVelocity = rb.linearVelocity.normalized * (rb.linearVelocity.magnitude + speedIncrement);
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }
}