using Unity.Netcode;
using UnityEngine;

public class BarraOnline : NetworkBehaviour
{
    public float velocidad = 5f;

    public override void OnNetworkSpawn()
    {
        // Solo el servidor decide la posición inicial
        if (IsServer)
        {
            // Asignar posición según el número de jugadores conectados
            int playerIndex = OwnerClientId == 0 ? 0 : 1;

            if (playerIndex == 0)
                transform.position = new Vector2(-7f, 0f); // lado izquierdo
            else
                transform.position = new Vector2(7f, 0f);  // lado derecho
        }
    }

    void Update()
    {
        // Solo el jugador dueño de esta barra puede controlarla
        if (!IsOwner) return;

        float movimiento = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            movimiento = velocidad * Time.deltaTime;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            movimiento = -velocidad * Time.deltaTime;

        transform.Translate(Vector2.up * movimiento);

        // Limitar dentro de la pantalla
        float nuevaY = Mathf.Clamp(transform.position.y, -3.5f, 3.5f);
        transform.position = new Vector2(transform.position.x, nuevaY);
    }
}