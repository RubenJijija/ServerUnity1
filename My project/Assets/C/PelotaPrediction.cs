using UnityEngine;
using Unity.Netcode;

public class PelotaPrediction : NetworkBehaviour
{
    public float velocidad = 5f;
    private Rigidbody2D rb;
    private Vector2 direccion = Vector2.right;

    // Posición oficial sincronizada
    public NetworkVariable<Vector2> posicionOficial = new NetworkVariable<Vector2>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsServer)
        {
            // Movimiento real en el servidor
            rb.MovePosition(rb.position + direccion * velocidad * Time.deltaTime);

            // Rebotes contra paredes (ejemplo con límites Y)
            if (rb.position.y > 3.5f || rb.position.y < -3.5f)
            {
                direccion.y = -direccion.y;
            }

            // Rebotes contra barras (ejemplo simple con límites X)
            if (rb.position.x > 7f || rb.position.x < -7f)
            {
                direccion.x = -direccion.x;
            }

            // Actualizar posición oficial
            posicionOficial.Value = rb.position;
        }
        else if (IsClient)
        {
            // Predicción local (misma lógica que el servidor)
            rb.MovePosition(rb.position + direccion * velocidad * Time.deltaTime);

            if (rb.position.y > 3.5f || rb.position.y < -3.5f)
            {
                direccion.y = -direccion.y;
            }

            if (rb.position.x > 7f || rb.position.x < -7f)
            {
                direccion.x = -direccion.x;
            }

            // Corrección suave hacia la posición oficial
            rb.position = Vector2.Lerp(rb.position, posicionOficial.Value, 0.2f);
        }
    }
}