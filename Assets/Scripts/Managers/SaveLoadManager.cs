using System.Threading.Tasks;
using Bayat.SaveSystem;
using Managers;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    
    [SerializeField] private Save _save;
    
    private void Awake()
    {
        ServiceLocator.SaveLoadManager = this;
    }
    
    public async Task Init()
    {
        _save = await Load();
    }
    
    private async Task<Save> Load()
    {
        if (!await SaveSystemAPI.ExistsAsync("save.bin")) return new Save();
        
        return await SaveSystemAPI.LoadAsync<Save>("save.bin");
    }
    
    public void LoadCurrency()
    {
        ServiceLocator.CurrencyManager.SetCurrency(Currency.GOLD, _save.GetGold());
        ServiceLocator.CurrencyManager.SetCurrency(Currency.MAX_APPLE, _save.GetMaxApple());
    }
    
    public async Task AsyncSave()
    {
        _save.SetAllData();
        await SaveSystemAPI.SaveAsync("save.bin", _save);
    }

    public void Save(bool autoSave = false)
    {
        _save.SetAllData();
        SaveSystemAPI.SaveAsync("save.bin", _save);
    }
    
    public async void Reset()
    {
        await SaveSystemAPI.DeleteAsync("save.bin");
    }
}

public class Save
{
    [SerializeField] private int _gold;
    [SerializeField] private int _maxApple;

    public void SetAllData()
    {
        _gold = ServiceLocator.CurrencyManager.GetCurrency(Currency.GOLD);
        _maxApple = ServiceLocator.CurrencyManager.GetCurrency(Currency.MAX_APPLE);
    }

    public int GetGold() => _gold;
    public int GetMaxApple() => _maxApple;

}