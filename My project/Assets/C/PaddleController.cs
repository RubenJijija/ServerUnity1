using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;



[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    // Buffer de inputs para prediction + reconciliation
    private Queue<PlayerInput> inputBuffer = new Queue<PlayerInput>();

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Capturar input local
        float moveY = Input.GetAxisRaw("Vertical");
        var input = new PlayerInput
        {
            tick = TickManager.CurrentTick,
            moveY = moveY
        };

        // Prediction inmediata en cliente
        ApplyInputLocally(input);

        // Guardar en buffer para reconciliación
        inputBuffer.Enqueue(input);

        // Enviar al servidor
        SendInputServerRpc(input);
    }

    private void ApplyInputLocally(PlayerInput input)
    {
        // Movimiento suave usando velocity
        rb.linearVelocity = new Vector2(0, input.moveY * speed);
    }

    [ServerRpc]
    private void SendInputServerRpc(PlayerInput input, ServerRpcParams rpcParams = default)
    {
        // Validar input en servidor y mandar snapshot oficial
        ApplyInputLocally(input);
        SendStateClientRpc(input);
    }

    [ClientRpc]
    private void SendStateClientRpc(PlayerInput state)
    {
        if (IsOwner) return; // el dueño ya predijo

        // Corrección en clientes no dueños
        rb.linearVelocity = new Vector2(0, state.moveY * speed);
    }
}
