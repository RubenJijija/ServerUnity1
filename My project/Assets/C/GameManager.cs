using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject barraIzquierdaPrefab;
    public GameObject barraDerechaPrefab;
    public GameObject pelotaPrefab;

    private GameObject pelotaInstancia;

    public NetworkVariable<int> scoreJugador1 = new NetworkVariable<int>();
    public NetworkVariable<int> scoreJugador2 = new NetworkVariable<int>();

    public int puntosParaGanar = 10;

    private void Start()
    {
        Debug.Log("GameManager Start ejecutado. IsServer=" + NetworkManager.Singleton.IsServer);

        if (NetworkManager.Singleton.IsServer)
        {
            CrearPelota();
        }
    }
    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AsignarBarra;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= AsignarBarra;
    }

    private void CrearPelota()
    {
        pelotaInstancia = Instantiate(pelotaPrefab, Vector2.zero, Quaternion.identity);
        pelotaInstancia.GetComponent<NetworkObject>().Spawn();
        Debug.Log(">>> Pelota creada y spawneada en el servidor <<<");
    }

    private void AsignarBarra(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        GameObject barraPrefab = null;
        Vector2 posicion = Vector2.zero;

        // Decide qué barra asignar según el orden de conexión
        if (NetworkManager.Singleton.ConnectedClients.Count == 1)
        {
            barraPrefab = barraIzquierdaPrefab;
            posicion = new Vector2(-7f, 0f);
        }
        else if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            barraPrefab = barraDerechaPrefab;
            posicion = new Vector2(7f, 0f);
        }
        if (NetworkManager.Singleton.ConnectedClients.Count == 2 && pelotaInstancia == null)
        {
            CrearPelota();
            Debug.Log(">>> Pelota creada al conectarse ambos jugadores <<<");
        }



        if (barraPrefab != null)
        {
            var barraInstancia = Instantiate(barraPrefab, posicion, Quaternion.identity);
            barraInstancia.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            Debug.Log($"Cliente {clientId} recibió su barra");
        }
    }

    public void SumarPuntoJugador1()
    {
        scoreJugador1.Value++;
        VerificarVictoria();
    }

    public void SumarPuntoJugador2()
    {
        scoreJugador2.Value++;
        VerificarVictoria();
    }

    private void VerificarVictoria()
    {
        if (scoreJugador1.Value >= puntosParaGanar)
        {
            Debug.Log("¡Jugador 1 gana!");
            Invoke(nameof(ReiniciarRonda), 3f);
        }
        else if (scoreJugador2.Value >= puntosParaGanar)
        {
            Debug.Log("¡Jugador 2 gana!");
            Invoke(nameof(ReiniciarRonda), 3f);
        }
    }

    private void ReiniciarRonda()
    {
        scoreJugador1.Value = 0;
        scoreJugador2.Value = 0;

        if (pelotaInstancia != null) pelotaInstancia.GetComponent<NetworkObject>().Despawn(true);

        CrearPelota();
    }
}