using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BallController : NetworkBehaviour
{
    [Header("Configuración de velocidad")]
    public float initialSpeed = 5f;
    public float speedIncrement = 0.5f;
    public float maxSpeed = 15f;
    public float minBounceY = 0.2f;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;

        if (IsServer)
            LaunchBall();
    }

    private void LaunchBall()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(0, 2) == 0 ? -1 : 1;
        Vector2 dir = new Vector2(x, y).normalized;

        rb.linearVelocity = dir * initialSpeed;
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            rb.MovePosition(rb.position + rb.linearVelocity * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + rb.linearVelocity * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Paddle"))
        {
            float hitPoint = (transform.position.y - collision.transform.position.y)
                             / collision.collider.bounds.size.y;

            Vector2 dir = new Vector2(rb.linearVelocity.x, hitPoint).normalized;
            if (Mathf.Abs(dir.y) < minBounceY)
                dir.y = Mathf.Sign(dir.y) * minBounceY;

            rb.linearVelocity = dir * rb.linearVelocity.magnitude;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);

            // Corrección de penetración: empujar fuera del collider
            if (collision.contacts.Length > 0)
            {
                Vector2 correction = collision.contacts[0].normal * (circleCollider.radius + 0.01f);
                rb.position += correction;
            }

            // Rebote mínimo en Y
            if (Mathf.Abs(rb.linearVelocity.y) < minBounceY)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Sign(rb.linearVelocity.y) * minBounceY);
        }

        // Aceleración progresiva + límite
        rb.linearVelocity = rb.linearVelocity.normalized * (rb.linearVelocity.magnitude + speedIncrement);
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }
}