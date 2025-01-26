using UnityEngine;
using Whimsical.Debug;
using Whimsical.Gameplay.Health;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HealthPoints player1Health;
    [SerializeField] private HealthPoints player2Health;

    private void Start()
    {

        player1Health.OnDeath += HandlePlayerDeath;
        player2Health.OnDeath += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        DebugExtensions.Log("Game Over");

        // trigger activado para desplegar una accion o pantalla luego de la muerte de uno de los jugadores
        Time.timeScale = 0;
    }

    private void OnDestroy()
    {

        if (player1Health != null) player1Health.OnDeath -= HandlePlayerDeath;
        if (player2Health != null) player2Health.OnDeath -= HandlePlayerDeath;
    }
}