using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private ProvincialManager provincialManager;
    private UnitManager unitManager;
    private EconomyManager economyManager;
    public TMP_Text totalGoldText;
    public TMP_Text goldPerTurnText;
    public GameObject playerUI; 

    public void Initialize(ProvincialManager provincialManager, UnitManager unitManager, EconomyManager economyManager)
    {
        this.provincialManager = provincialManager;
        this.unitManager = unitManager;
        this.economyManager = economyManager;
        playerUI.SetActive(true);
    }

    public void UpdateGameStateUI(int currentPlayerIndex)
    {
        if (currentPlayerIndex == 0)
        {
            playerUI.SetActive(true);
            UpdateGoldDisplay();
        }
        else
        {
            playerUI.SetActive(false);
        }
    }

    private void UpdateGoldDisplay()
    {
        Province playerProvince = provincialManager.GetPlayerProvince();
        if (playerProvince != null)
        {
            totalGoldText.text = $"Total Gold: {playerProvince.Treasury}";
            goldPerTurnText.text = $"Gold per Turn: {(playerProvince.GoldPerTurn >= 0 ? "+" : "")}{playerProvince.GoldPerTurn}";
        }
    }

    public void OnBuyUnitButtonPressed(int unitGrade = 0)
    {
        if (provincialManager == null || unitManager == null || economyManager == null)
        {
            Debug.LogError("Managers are not assigned in UIManager.");
            return;
        }

        Province playerProvince = provincialManager.GetPlayerProvince();
        if (playerProvince != null)
        {
            int unitCost = unitManager.GetUnitCost(unitGrade);
            if (playerProvince.Treasury >= unitCost)
            {
                unitManager.SpawnUnitInProvince(playerProvince, unitGrade);
                
                economyManager.UpdateGoldPerTurn(provincialManager, unitManager, 0);
                
                UpdateGoldDisplay();
            }
            else
            {
                Debug.LogError("Not enough gold to buy this grade of unit.");
            }
        }
        else
        {
            Debug.LogError("Player province is null.");
        }
    }
}