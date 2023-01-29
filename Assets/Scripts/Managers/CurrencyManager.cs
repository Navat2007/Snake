using System;
using UnityEngine;

namespace Managers
{
    public class CurrencyManager : MonoBehaviour
    {
        
        public event Action<int> OnGoldChange;
        public event Action<int> OnAppleChange;
        public event Action<int> OnMaxAppleChange;
        
        [SerializeField] private int _gold = 0;
        [SerializeField] private int _apple = 0;
        [SerializeField] private int _maxApple = 0;

        void Awake ()
        {
            ServiceLocator.CurrencyManager = this;
        }
        
        public void Init()
        {
            
        }
        
        public void Subscribe()
        {
            EventBus.AppleCollectEvent += () => AddCurrency(Currency.APPLE, 1);
        }

        public int GetCurrency(Currency currency)
        {
            switch (currency)
            {
                case Currency.GOLD:
                    return _gold;
                case Currency.APPLE:
                    return _apple;
                case Currency.MAX_APPLE:
                    return _maxApple;
                default:
                    return 0;
            }
        }
        
        public void AddCurrency(Currency currency, int value)
        {
            switch (currency)
            {
                case Currency.GOLD:
                    _gold += value;
                    OnGoldChange?.Invoke(_gold);
                    break;
                case Currency.APPLE:
                    _apple += value;
                    if (_apple > _maxApple)
                    {
                        SetCurrency(Currency.MAX_APPLE, _apple);
                    }
                    OnAppleChange?.Invoke(_apple);
                    break;
                case Currency.MAX_APPLE:
                    _maxApple += value;
                    OnMaxAppleChange?.Invoke(_maxApple);
                    break;
            }
        }
    
        public void SetCurrency(Currency currency, int value)
        {
            switch (currency)
            {
                case Currency.GOLD:
                    _gold = value;
                    OnGoldChange?.Invoke(_gold);
                    break;
                case Currency.APPLE:
                    _apple = value;
                    if (_apple > _maxApple)
                    {
                        SetCurrency(Currency.MAX_APPLE, _apple);
                    }
                    OnAppleChange?.Invoke(_apple);
                    break;
                case Currency.MAX_APPLE:
                    _maxApple = value;
                    OnMaxAppleChange?.Invoke(_maxApple);
                    break;
            }
        }
        
        public void FullReset()
        {
            SetCurrency(Currency.GOLD, 0);
            SetCurrency(Currency.APPLE, 0);
            SetCurrency(Currency.MAX_APPLE, 0);
        }

    }

}

public enum Currency
{
    GOLD,
    APPLE,
    MAX_APPLE
}