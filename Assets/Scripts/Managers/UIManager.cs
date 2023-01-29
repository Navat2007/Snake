using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Timer _timer;

        [Header("Main")] 
        [SerializeField] private Transform _mainMenuPanel;
        [SerializeField] private TMP_Text _mainGoldText;
        [SerializeField] private Button _mainMenuStartButton;

        [Header("Level")] 
        [SerializeField] private Transform _levelGamePanel;
        [SerializeField] private TMP_Text _levelAppleText;
        [SerializeField] private TMP_Text _levelTimeText;
        [SerializeField] private Button _levelPauseButton;

        [Header("Level Pause")] 
        [SerializeField] private Transform _levelPausePanel;

        [SerializeField] private Button _levelPauseSettingButton;
        [SerializeField] private Button _levelPauseResumeButton;
        [SerializeField] private Button _levelPauseExitButton;

        [Header("Level Settings")] 
        [SerializeField] private Transform _levelSettingPanel;
        [SerializeField] private Button _levelPauseSettingCloseButton;

        [Header("Level Result")] 
        [SerializeField] private Transform _levelResultPanel;
        [SerializeField] private TMP_Text _levelResultAppleText;
        [SerializeField] private TMP_Text _levelResultMaxAppleText;
        [SerializeField] private Button _levelResultClaimButton;
        [SerializeField] private Button _levelResultClaimX2Button;
        
        [Header("Всплывающие окна:")] 
        [SerializeField] private GameObject _popupQuestion;

        void Awake()
        {
            ServiceLocator.UIManager = this;

            if (_mainMenuPanel is not null && _levelGamePanel is not null)
            {
                _mainMenuPanel.gameObject.SetActive(true);
                _levelGamePanel.gameObject.SetActive(false);
            }
        }

        public void Init()
        {
            #region [MAIN SCREEN]

            if (_mainMenuStartButton is not null)
                _mainMenuStartButton.onClick.AddListener(() =>
                {
                    _mainMenuPanel.gameObject.SetActive(false);
                    _levelGamePanel.gameObject.SetActive(true);
                    EventBus.StartLevelEvent?.Invoke();
                });

            #endregion

            #region [GAME SCREEN]

            #region LEVEL

            if (_levelPauseButton is not null)
                _levelPauseButton.onClick.AddListener(() =>
                {
                    EventBus.PauseEvent?.Invoke();
                    _levelPausePanel.gameObject.SetActive(true);
                });

            #endregion

            #region LEVEL PAUSE

            if (_levelPauseSettingButton is not null)
                _levelPauseSettingButton.onClick.AddListener(() => { _levelSettingPanel.gameObject.SetActive(true); });
            
            if (_levelPauseResumeButton is not null)
                _levelPauseResumeButton.onClick.AddListener(() =>
                {
                    _levelPausePanel.gameObject.SetActive(false);
                    EventBus.UnPauseEvent?.Invoke();
                });
            
            if (_levelPauseExitButton is not null)
                _levelPauseExitButton.onClick.AddListener(ShowLevelResult);

            #endregion

            #region LEVEL SETTINGS

            if (_levelPauseSettingCloseButton is not null)
                _levelPauseSettingCloseButton.onClick.AddListener(() =>
                {
                    _levelSettingPanel.gameObject.SetActive(false);
                });

            #endregion

            #region LEVEL RESULT

            if (_levelResultClaimButton is not null)
                _levelResultClaimButton.onClick.AddListener(() =>
                {
                    EventBus.ExitLevelEvent?.Invoke(false);
                });
            
            if (_levelResultClaimX2Button is not null)
                _levelResultClaimX2Button.onClick.AddListener(() =>
                {
                    EventBus.ExitLevelEvent?.Invoke(true);
                });


            #endregion

            #endregion
        }

        public void Subscribe()
        {
            ServiceLocator.CurrencyManager.OnGoldChange += (value) => UpdateCurrency(value, Currency.GOLD);
            ServiceLocator.CurrencyManager.OnAppleChange += (value) => UpdateCurrency(value, Currency.APPLE);
            ServiceLocator.CurrencyManager.OnMaxAppleChange += (value) => UpdateCurrency(value, Currency.MAX_APPLE);

            if (_timer != null && _levelTimeText != null)
            {
                _timer.OnTimerChange += (time) =>
                {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(time);
                    _levelTimeText.text = $"Время: {timeSpan:m\\:ss}";
                };
            }
        }

        public void SetPanel(Panels panel)
        {
            _levelGamePanel.gameObject.SetActive(false);
            _levelResultPanel.gameObject.SetActive(false);

            switch (panel)
            {
                case Panels.GAME:
                    _levelGamePanel.gameObject.SetActive(true);
                    break;
                case Panels.RESULT:
                    _levelResultPanel.gameObject.SetActive(true);
                    break;
            }
        }

        void UpdateCurrency(int value, Currency currency)
        {
            switch (currency)
            {
                case Currency.GOLD:
                    if (_mainGoldText != null)
                        _mainGoldText.text = $"{value}";
                    break;
                case Currency.APPLE:
                    if (_levelAppleText != null)
                        _levelAppleText.text = $"Яблок: {value}";
                    
                    if (_levelResultAppleText != null)
                        _levelResultAppleText.text = $"Ваш результат - {value}";
                    break;
                case Currency.MAX_APPLE:
                    if (_levelResultMaxAppleText != null)
                        _levelResultMaxAppleText.text = $"Ваш рекорд - {value}";
                    break;
            }
        }

        public void ShowLevelResult()
        {
            SetPanel(Panels.RESULT);
        }

        public void ShowMainScreen()
        {
            _levelPausePanel.gameObject.SetActive(false);
            _levelResultPanel.gameObject.SetActive(false);
            _levelSettingPanel.gameObject.SetActive(false);
            _levelGamePanel.gameObject.SetActive(true);
            
            _mainMenuPanel.gameObject.SetActive(true);
        }

        public void ShowQuestionPopup(string title, string okText, string cancelText, Action okCallback,
            Action cancelCallback)
        {
            var popupQuestionScript = _popupQuestion.GetComponent<PopupQuestion>();
            popupQuestionScript.Init(title, okText, cancelText, okCallback, cancelCallback);
            _popupQuestion.SetActive(true);
        }
    }

    public enum Panels
    {
        GAME,
        RESULT
    }
}
