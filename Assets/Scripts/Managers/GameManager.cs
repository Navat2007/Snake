using System;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public event Action OnGameFinish;
        public event Action<GameState> OnGameStateChange;

        private GameState _currentState = GameState.PAUSE;

        public GameState GetState => _currentState;
        
        private void Awake()
        {
            ServiceLocator.GameManager = this;
        }
    
        private async void Start()
        {
            ServiceLocator.CurrencyManager.Init();
            ServiceLocator.UIManager.Init();
            ServiceLocator.Timer.Init();
            await ServiceLocator.SaveLoadManager.Init();

            ServiceLocator.CurrencyManager.Subscribe();
            ServiceLocator.UIManager.Subscribe();
            ServiceLocator.Timer.Subscribe();
            
            ServiceLocator.SaveLoadManager.LoadCurrency();
        }

        private void OnEnable()
        {
            EventBus.StartLevelEvent += StartLevel;
            EventBus.GameOverEvent += GameOver;
            EventBus.ExitLevelEvent += ExitLevel;
            EventBus.PauseEvent += PauseLevel;
            EventBus.UnPauseEvent += UnPauseLevel;
        }

        private void OnDisable()
        {
            EventBus.StartLevelEvent -= StartLevel;
            EventBus.GameOverEvent -= GameOver;
            EventBus.ExitLevelEvent -= ExitLevel;
            EventBus.PauseEvent -= PauseLevel;
            EventBus.UnPauseEvent -= UnPauseLevel;
        }

        private void Update()
        {

            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
                {
                    QuitGame();
                }
            }
        
            if (Input.GetKey(KeyCode.Escape))
            {
                ServiceLocator.UIManager.ShowQuestionPopup(
                    "Are you sure you want to exit?", 
                    "Yes", 
                    "I'm stay", 
                    QuitGame, 
                    null
                    );
            }

        }

        private void StartLevel()
        {
            ServiceLocator.CurrencyManager.SetCurrency(Currency.APPLE, 0);
            UnPauseLevel();
        }
        
        private void UnPauseLevel()
        {
            SetState(GameState.PLAY);
        }

        private void PauseLevel()
        {
            SetState(GameState.PAUSE);
        }

        private void GameOver()
        {
            PauseLevel();
            ServiceLocator.UIManager.SetPanel(Panels.RESULT);
        }

        private void ExitLevel(bool doubleScore)
        {
            var gold = ServiceLocator.CurrencyManager.GetCurrency(Currency.APPLE);

            if (doubleScore)
                gold *= 2;
            
            ServiceLocator.CurrencyManager.AddCurrency(Currency.GOLD, gold);
            OnGameFinish?.Invoke();
        }
        
        private void SetState(GameState state)
        {
            _currentState = state;
            OnGameStateChange?.Invoke(_currentState);
        }
        
        private void OnApplicationQuit()
        {
            QuitGame();
        }

        private async void QuitGame()
        {
            await ServiceLocator.SaveLoadManager.AsyncSave();
            
            Application.Quit();
        }
    }
}

public enum GameState
{
    MENU,
    PLAY,
    PAUSE
}