using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class WinnerUI : MonoBehaviour
{
    public Text winnerText; // Asigna aquí un Text de la UI
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        winnerText.text = ""; // Inicialmente vacío
    }

    void Update()
    {
        if (gameManager != null)
        {
            // Mostrar mensaje cuando alguien gana
            if (gameManager.scoreJugador1.Value >= gameManager.puntosParaGanar)
            {
                winnerText.text = "¡Jugador 1 gana!";
            }
            else if (gameManager.scoreJugador2.Value >= gameManager.puntosParaGanar)
            {
                winnerText.text = "¡Jugador 2 gana!";
            }
            else
            {
                winnerText.text = ""; // Ocultar si no hay ganador
            }
        }
    }
}