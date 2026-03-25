using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BarraOnline : NetworkBehaviour
{
    public float velocidad = 5f;

    private PlayerInput playerInput;
    private InputAction moverAction;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerInput = GetComponent<PlayerInput>();
            moverAction = playerInput.actions["Mover"];
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetInitialPositionServerRpc(Vector2 pos)
    {
        transform.position = pos;
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