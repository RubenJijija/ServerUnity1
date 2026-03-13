using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public GameObject barraIzquierdaPrefab;
    public GameObject barraDerechaPrefab;
    public GameObject pelotaPrefab;

    private GameObject barraIzqInstancia;
    private GameObject barraDerInstancia;
    private GameObject pelotaInstancia;

    public NetworkVariable<int> scoreJugador1 = new NetworkVariable<int>();
    public NetworkVariable<int> scoreJugador2 = new NetworkVariable<int>();

    public int puntosParaGanar = 10;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // Solo el servidor crea los objetos, pero no se asigna a sí mismo
            CrearObjetos();
        }
    }

    private void CrearObjetos()
    {
        barraIzqInstancia = Instantiate(barraIzquierdaPrefab, new Vector2(-7f, 0f), Quaternion.identity);
        barraIzqInstancia.GetComponent<NetworkObject>().Spawn();

        barraDerInstancia = Instantiate(barraDerechaPrefab, new Vector2(7f, 0f), Quaternion.identity);
        barraDerInstancia.GetComponent<NetworkObject>().Spawn();

        pelotaInstancia = Instantiate(pelotaPrefab, Vector2.zero, Quaternion.identity);
        pelotaInstancia.GetComponent<NetworkObject>().Spawn();
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnServerStarted += CrearObjetos;
        NetworkManager.Singleton.OnClientConnectedCallback += AsignarBarra;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnServerStarted -= CrearObjetos;
        NetworkManager.Singleton.OnClientConnectedCallback -= AsignarBarra;
    }

    private void AsignarBarra(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (barraIzqInstancia == null || barraDerInstancia == null)
        {
            Debug.LogWarning("Las barras aún no están instanciadas, esperando...");
            return;
        }

        var barraIzqNetObj = barraIzqInstancia.GetComponent<NetworkObject>();
        var barraDerNetObj = barraDerInstancia.GetComponent<NetworkObject>();

        if (barraIzqNetObj.OwnerClientId == NetworkManager.ServerClientId)
        {
            barraIzqNetObj.ChangeOwnership(clientId);
            Debug.Log($"Cliente {clientId} asignado a barra izquierda");
        }
        else if (barraDerNetObj.OwnerClientId == NetworkManager.ServerClientId)
        {
            barraDerNetObj.ChangeOwnership(clientId);
            Debug.Log($"Cliente {clientId} asignado a barra derecha");
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

        if (barraIzqInstancia != null) barraIzqInstancia.GetComponent<NetworkObject>().Despawn(true);
        if (barraDerInstancia != null) barraDerInstancia.GetComponent<NetworkObject>().Despawn(true);
        if (pelotaInstancia != null) pelotaInstancia.GetComponent<NetworkObject>().Despawn(true);

        CrearObjetos();
    }
}