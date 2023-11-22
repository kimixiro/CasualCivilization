using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentPlayerIndex = 0;
    public int totalPlayers = 2;

    public MapManager mapManager;
    public ProvincialManager provincialManager;
    public UIManager uiManager;
    public UnitManager unitManager;
    public EconomyManager economyManager;
    public BattleManager battleManager;
    public BuildingManager buildingManager;
    public AIManager aiManager;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        mapManager.Initialize();
        provincialManager.Initialize(totalPlayers,mapManager,buildingManager,unitManager);
        uiManager.Initialize(provincialManager,unitManager,economyManager);
        unitManager.Initialize(uiManager,mapManager,battleManager,provincialManager);
        economyManager.Initialize();
        battleManager.Initialize(mapManager,unitManager);
        buildingManager.Initialize();
        aiManager.Initialize(this,mapManager,unitManager,economyManager,provincialManager,battleManager,totalPlayers-1);
        
        currentPlayerIndex = 0;
        StartTurn();
    }

    void StartTurn()
    {
        Debug.Log($"Starting turn for Player {currentPlayerIndex}");
        UpdateUI();

        if (IsHumanPlayer(currentPlayerIndex))
        {
          
        }
        else
        {
            ProcessAITurn();
        }
    }

    void UpdateUI()
    {
        uiManager.UpdateGameStateUI(currentPlayerIndex);
    }

    public void EndTurn()
    {
        economyManager.UpdateEconomy(provincialManager, unitManager, currentPlayerIndex);
        unitManager.ResetUnitMovements();

        AdvanceToNextPlayer();
    }
    
    void AdvanceToNextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        StartTurn();
    }

    public bool IsHumanPlayer(int playerIndex)
    {
        return playerIndex == 0;
    }

    void ProcessAITurn()
    {
        StartCoroutine(ProcessAITurnCoroutine());
    }
    
    private IEnumerator ProcessAITurnCoroutine()
    {
        yield return StartCoroutine(aiManager.MakeDecisions());
        EndTurn();
    }
    
}
