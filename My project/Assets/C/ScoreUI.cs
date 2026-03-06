using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ScoreUI : MonoBehaviour
{
    public Text scoreText; // Asigna aquí un Text de la UI
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gameManager != null)
        {
            scoreText.text = $"Jugador 1: {gameManager.scoreJugador1.Value} | Jugador 2: {gameManager.scoreJugador2.Value}";
        }
    }
}