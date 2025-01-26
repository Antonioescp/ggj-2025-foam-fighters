using UnityEngine;
using Whimsical.Debug;
using Whimsical.Gameplay.Player;

namespace Whimsical.Events
{
    using System.Collections.Generic;
    using System.Linq;
    using Gameplay;
    using TMPro;
    using UnityEngine.SceneManagement;

    public class GameDirector : MonoBehaviour
    {
        [SerializeField] private Player player1;
        [SerializeField] private Player player2;

        private bool _alreadyFinished;

        private Timer _randomSoundsTimer;
        [SerializeField] private List<AudioClip> _environmentSounds;
        private AudioSource _audio;

        [SerializeField] private TextMeshProUGUI _gameOverMessage;

        private Timer _restartGameTimer;
        private Timer _parryTimer;
    
        private void Start()
        {
            player1.OnDeath += HandlePlayerDeath;
            player2.OnDeath += HandlePlayerDeath;

            player1.OnParry += HandleParry;
            player2.OnParry += HandleParry;

            _audio = this.GetComponent<AudioSource>();
            
            // Setting up timers
            _randomSoundsTimer = gameObject.AddComponent<Timer>();
            _randomSoundsTimer.TargetTime = 5f;
            _randomSoundsTimer.OnTimeElapsed += () =>
            {
                if (!_environmentSounds.Any()) return;
                
                var randomIndex = Random.Range(0, _environmentSounds.Count - 1);
                var randomEnvironmentSound = _environmentSounds[randomIndex];
                _audio.clip = randomEnvironmentSound;
                _audio.Play();

                _randomSoundsTimer.TargetTime = _audio.clip.length + Random.Range(1f, 4f);
                _randomSoundsTimer.RestartTimer();
            };
            
            _randomSoundsTimer.StartTimer();

            _restartGameTimer = this.gameObject.AddComponent<Timer>();
            _restartGameTimer.TargetTime = 3f;
            _restartGameTimer.OnTimeElapsed += () =>
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("StartMenu");
            };

            _parryTimer = this.gameObject.AddComponent<Timer>();
            _parryTimer.TargetTime = 0.5f;
            _parryTimer.OnTimeElapsed += () => Time.timeScale = 1;
        }

        private void HandleParry()
        {
            Time.timeScale = 0;
            _parryTimer.RestartTimer();
        }

        private void HandlePlayerDeath()
        {
            if (_alreadyFinished) return;
            
            _alreadyFinished = true;
            DebugExtensions.Log("Game Over");

            const string template = "{0} WINS";
            var winner = player1.IsDead ? "P2" : "P1";
            _gameOverMessage.text = string.Format(template, winner);
            _gameOverMessage.gameObject.SetActive(true);
            
            _restartGameTimer.StartTimer();
            
            // trigger activado para desplegar una accion o pantalla luego de la muerte de uno de los jugadores
            Time.timeScale = 0;
        }

        private void OnDestroy()
        {
            if (player1 != null) player1.OnDeath -= HandlePlayerDeath;
            if (player2 != null) player2.OnDeath -= HandlePlayerDeath;
        }
    }
}