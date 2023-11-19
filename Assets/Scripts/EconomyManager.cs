using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    private Dictionary<Province, int> provinceEconomies;

    public void Initialize()
    {
        provinceEconomies = new Dictionary<Province, int>();
    }

    public void UpdateEconomy(ProvincialManager provincialManager, UnitManager unitManager, int currentPlayerId)
    {
        UpdateGoldPerTurn(provincialManager,unitManager,currentPlayerId);
        UpdateTreasury(provincialManager,currentPlayerId);
    }
    
    public void UpdateGoldPerTurn(ProvincialManager provincialManager, UnitManager unitManager, int currentPlayerId)
    {
        foreach (var provinceEntry in provincialManager.provinces)
        {
            Province province = provinceEntry.Value;
            
            if (province.OwnerId == currentPlayerId)
            {
                int goldIncome = province.Territory.Count * 1;
                int goldExpenses = unitManager.GetUnitsInProvince(provinceEntry.Key)
                    .Sum(unit => unit.UpkeepCost);
                province.GoldPerTurn = goldIncome - goldExpenses;

                Debug.Log($"Province {provinceEntry.Key}: Income {goldIncome}, Expenses {goldExpenses}, Gold Per Turn {province.GoldPerTurn}");
            }
        }
    }
    
    public void UpdateTreasury(ProvincialManager provincialManager, int currentPlayerId)
    {
        Province playerProvince = provincialManager.provinces
            .Values
            .FirstOrDefault(province => province.OwnerId == currentPlayerId);

        if (playerProvince != null)
        {
            playerProvince.Treasury += playerProvince.GoldPerTurn;
            Debug.Log($"Province {playerProvince.Center} Treasury after update: {playerProvince.Treasury}");
        }
    }
}