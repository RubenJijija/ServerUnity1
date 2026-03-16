using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem; // Importante

public class BarraOnline : NetworkBehaviour
{
    public float velocidad = 5f;

    private PlayerInput playerInput;
    private InputAction moverAction;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Cada cliente inicializa su propio PlayerInput
            playerInput = GetComponent<PlayerInput>();
            moverAction = playerInput.actions["Mover"];
        }

        if (IsServer)
        {
            // Posición inicial según el dueño
            int playerIndex = OwnerClientId == 0 ? 0 : 1;
            transform.position = playerIndex == 0 ? new Vector2(-7f, 0f) : new Vector2(7f, 0f);
        }
    }

    void Update()
    {
        if (!IsOwner || moverAction == null) return;

        float movimiento = moverAction.ReadValue<float>() * velocidad * Time.deltaTime;
        transform.Translate(Vector2.up * movimiento);

        float nuevaY = Mathf.Clamp(transform.position.y, -3.5f, 3.5f);
        transform.position = new Vector2(transform.position.x, nuevaY);
    }
}