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

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        mapManager.Initialize();
        provincialManager.Initialize(totalPlayers,mapManager,buildingManager);
        uiManager.Initialize(provincialManager,unitManager,economyManager);
        unitManager.Initialize(uiManager,mapManager,battleManager);
        economyManager.Initialize();
        battleManager.Initialize(mapManager);
        buildingManager.Initialize();
        
        
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

    bool IsHumanPlayer(int playerIndex)
    {
        return playerIndex == 0;
    }

    void ProcessAITurn()
    {
        EndTurn();
    }
    
}
