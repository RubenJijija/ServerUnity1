using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BarraOnline : NetworkBehaviour
{
    public float velocidad = 5f;

    void Update()
    {
        // Solo el jugador dueño de esta barra puede controlarla
        if (!IsOwner) return;

        float movimiento = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            movimiento = velocidad * Time.deltaTime;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            movimiento = -velocidad * Time.deltaTime;

        // Actualizar posición localmente y enviar al servidor
        transform.Translate(Vector2.up * movimiento);

        // Limitar dentro de la pantalla
        float nuevaY = Mathf.Clamp(transform.position.y, -3.5f, 3.5f);
        transform.position = new Vector2(transform.position.x, nuevaY);
    }
}