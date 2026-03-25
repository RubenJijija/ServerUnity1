using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public GameObject barraIzquierdaPrefab;
    public GameObject barraDerechaPrefab;
    public GameObject pelotaPrefab;

    private GameObject pelotaInstancia;

    public NetworkVariable<int> scoreJugador1 = new NetworkVariable<int>();
    public NetworkVariable<int> scoreJugador2 = new NetworkVariable<int>();

    public int puntosParaGanar = 10;

    // Lista ordenada de jugadores conectados
    private List<ulong> jugadores = new List<ulong>();

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

        if (!jugadores.Contains(clientId))
            jugadores.Add(clientId);

        GameObject barraPrefab = null;
        Vector2 posicion = Vector2.zero;

        int indice = jugadores.IndexOf(clientId);

        if (indice == 0)
        {
            barraPrefab = barraIzquierdaPrefab;
            posicion = new Vector2(-7f, 0f);
        }
        else if (indice == 1)
        {
            barraPrefab = barraDerechaPrefab;
            posicion = new Vector2(7f, 0f);
        }

        if (barraPrefab != null)
        {
            var barraInstancia = Instantiate(barraPrefab, posicion, Quaternion.identity);
            var netObj = barraInstancia.GetComponent<NetworkObject>();
            netObj.SpawnWithOwnership(clientId);

            // Forzar posición inicial desde el servidor
            barraInstancia.GetComponent<BarraOnline>().SetInitialPositionServerRpc(posicion);

            Debug.Log($"Cliente {clientId} asignado a {(indice == 0 ? "izquierda" : "derecha")} en {posicion}");
        }

        if (jugadores.Count == 2 && pelotaInstancia == null)
        {
            CrearPelota();
            Debug.Log(">>> Pelota creada al conectarse ambos jugadores <<<");
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

        if (pelotaInstancia != null)
            pelotaInstancia.GetComponent<NetworkObject>().Despawn(true);

        CrearPelota();
    }
}


















