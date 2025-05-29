using System.Collections.Generic;
using Board;
using DG.Tweening;
using Items;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Board Settings")] 
    [SerializeField] internal Board.Board board;

    [Header("Enemy Spawn Settings")] 
    [SerializeField] private float initialEnemySpawnDelay = 2f;
    [SerializeField] private float minEnemySpawnInterval = 1f;
    [SerializeField] private float maxEnemySpawnInterval = 3f;

    [SerializeField] private GameObject defenceSelectionUIPanel;
    [SerializeField] private GameObject closeDefenceSelectionButton;
    [SerializeField] private GameObject gameOverUIPanel;
    [SerializeField] private GameObject levelWinUIPanel;
    
    [SerializeField] private Camera levelCamera;
    
    
    private readonly List<Enemy> _activeEnemies = new();
    private readonly List<DefenceItem> _defenceItems = new();

    private Cell _selectedCell;
    
    private bool _isEnemySpawnFinished;
    private bool _isGameOver;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CloseDefenceSelectionPanel();
    }
        
    private void Start()
    {
        if (!board)
        {
            Debug.LogError("Board is not assigned in GameManager.");
            return;
        }

        if (!board.IsInitialized)
        {
            Debug.LogError("Board is not initialized. Please check the Board component.");
            return;
        }

        InvokeRepeating(nameof(StartSpawningEnemies), initialEnemySpawnDelay, 
            Random.Range(minEnemySpawnInterval, maxEnemySpawnInterval));
    }

    private void StartSpawningEnemies()
    {
        InvokeRepeating(nameof(SpawnRandomEnemy), 0f, 
            Random.Range(minEnemySpawnInterval, maxEnemySpawnInterval));
    }
        
    private void SpawnRandomEnemy()
    {
        if (_isGameOver) return;
        
        var randomEnemyType = Random.Range(0, Constants.EnemySize);
        var enemyType = Constants.EnemyTypes[randomEnemyType];
        var randomColumn = Random.Range(0, Constants.BoardColumns);
        
        const int startRow = 0;

        var startCell = board.GetCell(startRow, randomColumn);
        
        if (startCell)
        {
            var enemyGO =
                PoolManager.Instance.SpawnFromPool(enemyType, startCell.transform.position, Quaternion.identity);
            if (enemyGO)
            {
                var newEnemy = enemyGO.GetComponent<Enemy>();
                if (newEnemy)
                {
                    newEnemy.Initialize(startRow, randomColumn);
                    _activeEnemies.Add(newEnemy);
                }
                else
                {
                    Debug.LogError("Spawned enemy GameObject does not have an Enemy component!");
                }
            }
        }
        else
        {
            Debug.LogError($"Could not find start cell for enemy at ({startRow}, {randomColumn})");
        }

        CancelInvoke(nameof(SpawnRandomEnemy));
        InvokeRepeating(nameof(SpawnRandomEnemy), 
            Random.Range(minEnemySpawnInterval, maxEnemySpawnInterval), 0f);
    }
        
    public void OnClickCellButton(Cell cell)
    {
        _selectedCell = cell;
        
        if (!defenceSelectionUIPanel) return;
        
        defenceSelectionUIPanel.SetActive(true);
        
        if (closeDefenceSelectionButton)
        {
            closeDefenceSelectionButton.SetActive(true);
        }
    }

    public void PlaceDefenceItem(Constants.ObjectItemType itemObjectItemType)
    {
        if (!_selectedCell) return;
        

        var defenceItemGO = PoolManager.Instance.SpawnFromPool(
            itemObjectItemType, _selectedCell.transform.position, Quaternion.identity);
        
        if (defenceItemGO)
        {
            var newDefenceItem = defenceItemGO.GetComponent<DefenceItem>();
            
            if (!newDefenceItem)
            {
                _selectedCell = null;
                PoolManager.Instance.ReturnToPool(itemObjectItemType, defenceItemGO);
                Debug.LogError("Spawned defence item GameObject does not have a DefenceItem component!");
                return;
            }
            
            newDefenceItem.Initialize(_selectedCell);
            
            _defenceItems.Add(newDefenceItem);

            _selectedCell.PlaceDefenceItem(newDefenceItem);
        }
        else
        {
            Debug.LogError($"Invalid Defence Item Type Index: {itemObjectItemType}");
        }

        CloseDefenceSelectionPanel();
        
        _selectedCell = null;
    }
    
    public void CloseDefenceSelectionPanel()
    {
        if (defenceSelectionUIPanel)
        {
            defenceSelectionUIPanel.SetActive(false);
        }

        if (closeDefenceSelectionButton)
        {
            closeDefenceSelectionButton.SetActive(false);
        }
        
        _selectedCell = null;
    }

    public void EnemiesReachedBase()
    {
        if (_isGameOver) return;
        
        _isGameOver = true;
        
        foreach (var activeEnemy in _activeEnemies)
        {
            activeEnemy.transform.DOKill();
        }

        foreach (var defenceItem in _defenceItems)
        {
            defenceItem.StopAttack();
        }
        
        _activeEnemies.Clear();
        _defenceItems.Clear();
        
        gameOverUIPanel.SetActive(true);
    }

    public void EnemyDied(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy)) return;
        
        _activeEnemies.Remove(enemy);
        
        Destroy(enemy.gameObject);

        if (_activeEnemies.Count != 0) return;
        
        _isEnemySpawnFinished = PoolManager.Instance.IsTotalEnemiesSpawned();

        if (!_isEnemySpawnFinished) return;
        
        foreach (var defenceItem in _defenceItems)
        {
            defenceItem.StopAttack();
        }
            
        levelWinUIPanel.SetActive(true);
        PlayerData.Instance.NextLevel();
    }
    
    public void ReloadScene()
    {
        GameSceneManager.ReloadActiveScene();
    }

    public void ReturnToMainMenu()
    {
        GameSceneManager.LoadMainMenu();
    }
}