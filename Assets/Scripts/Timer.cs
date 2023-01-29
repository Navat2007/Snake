using System;
using System.Threading.Tasks;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private int _scoreOnTime = 5;
    [SerializeField] private float _currentTime;

    public event Action<float> OnTimerChange;

    private float _scoreTimerMax = 1f;
    private float _scoreTimer = 0;

    private void Awake()
    {
        ServiceLocator.Timer = this;
    }

    public void Init()
    {
        
    }
    
    public void Subscribe()
    {
        ServiceLocator.GameManager.OnGameFinish += Reset;
    }

    private void Update()
    {
        if (ServiceLocator.GameManager.GetState == GameState.PLAY)
        {
            _currentTime += Time.deltaTime;
            OnTimerChange?.Invoke(_currentTime);

            _scoreTimer += Time.deltaTime;

            if (_scoreTimer >= _scoreTimerMax)
            {
                _scoreTimer = 0;
                //CurrencyManager.Instance.AddCurrency(Currency.SCORE, _scoreOnTime);
            }
        }
    }

    public void Reset()
    {
        _currentTime = 0;
    }
}