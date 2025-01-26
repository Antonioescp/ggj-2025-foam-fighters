using UnityEngine;
using Whimsical.Debug;
using Whimsical.Gameplay.Player;

namespace Whimsical.Events
{
    using Gameplay;

    public class GameDirector : MonoBehaviour
    {
        [SerializeField] private Player player1;
        [SerializeField] private Player player2;

        private Timer _successfulParryFreezeTimer;
    
        private void Start()
        {
            player1.OnDeath += HandlePlayerDeath;
            player2.OnDeath += HandlePlayerDeath;

            player1.OnParry += HandleParry;
            player2.OnParry += HandleParry;
            
            // Setting up timers
            _successfulParryFreezeTimer = gameObject.AddComponent<Timer>();
            _successfulParryFreezeTimer.TargetTime = 0.5f;
            _successfulParryFreezeTimer.OnTimeElapsed += () => Time.timeScale = 1;
        }

        private void HandlePlayerDeath()
        {
            DebugExtensions.Log("Game Over");

            // trigger activado para desplegar una accion o pantalla luego de la muerte de uno de los jugadores
            Time.timeScale = 0;
        }

        private void HandleParry()
        {
            Time.timeScale = 0;
            _successfulParryFreezeTimer.RestartTimer();
        }

        private void OnDestroy()
        {
            if (player1 != null) player1.OnDeath -= HandlePlayerDeath;
            if (player2 != null) player2.OnDeath -= HandlePlayerDeath;
        }
    }
}