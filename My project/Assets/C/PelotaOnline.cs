using UnityEngine;
using Unity.Netcode;

public class PelotaOnline : NetworkBehaviour
{
    public float velocidad = 5f;
    private Vector2 direccion;

    private void Start()
    {
        if (IsServer)
        {
            direccion = new Vector2(Random.value > 0.5f ? 1 : -1,
                                    Random.Range(-1f, 1f)).normalized;
        }
    }

    private void Update()
    {
        if (!IsServer) return; // Solo el servidor mueve la pelota

        transform.Translate(direccion * velocidad * Time.deltaTime);

        // Rebote contra límites verticales
        if (transform.position.y > 3.5f || transform.position.y < -3.5f)
        {
            direccion.y = -direccion.y;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsServer) return;

        if (col.gameObject.CompareTag("Barra"))
        {
            direccion.x = -direccion.x;

            float diferencia = transform.position.y - col.transform.position.y;
            direccion.y = diferencia * 0.5f;
            direccion = direccion.normalized;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer) return;

        if (col.CompareTag("GolIzquierda"))
        {
            FindObjectOfType<GameManager>().SumarPuntoJugador2();
            ReiniciarPelota();
        }
        else if (col.CompareTag("GolDerecha"))
        {
            FindObjectOfType<GameManager>().SumarPuntoJugador1();
            ReiniciarPelota();
        }
    }

    private void ReiniciarPelota()
    {
        transform.position = Vector2.zero;
        direccion = new Vector2(Random.value > 0.5f ? 1 : -1,
                                Random.Range(-1f, 1f)).normalized;
    }
}