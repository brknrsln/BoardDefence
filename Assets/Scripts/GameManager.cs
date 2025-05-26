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
    [SerializeField] private GameObject gameOverUIPanel;
    [SerializeField] private GameObject levelWinUIPanel;
    
    [SerializeField] private Camera levelCamera;
    
    private const float SelectionRaycastDistance = 100f;
    
    private readonly List<Enemy> _activeEnemies = new();
    private readonly List<DefenceItem> _defenceItems = new();

    private Cell _selectedCell;
    
    private bool _isEnemySpawnFinished;
    private bool _isGameOver;
    private bool _levelWin;
    private bool _isDefenceSelectionPanelActive;

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

        if (defenceSelectionUIPanel)
        {
            defenceSelectionUIPanel.SetActive(false);
        }
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

    private void FixedUpdate()
    {
        if (_isGameOver) return;
        
        if (_levelWin) return;

        if (_selectedCell && _isDefenceSelectionPanelActive) return;
        
        HandlePlayerInput();
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
        
    private void HandlePlayerInput()
    {
        if (!Input.GetMouseButton(0) && Input.touchCount == 0)
        {
            return;
        }

        _isDefenceSelectionPanelActive = true;
        
        Vector3 pointer = Application.isEditor ? Input.mousePosition : Input.GetTouch(0).position;
        
        Vector2 pointerPos = levelCamera.ScreenToWorldPoint(pointer);
        
        var pointerRaycastHit2D = Physics2D.Raycast(pointerPos, 
            Vector2.zero, SelectionRaycastDistance, LayerMask.GetMask("Default"));

        if (!pointerRaycastHit2D.collider) return;
        var clickedCell = pointerRaycastHit2D.collider.GetComponent<Cell>();
        if (!clickedCell) return;
        Debug.Log($"Clicked Cell: Row {clickedCell.yIndex}, Column {clickedCell.xIndex}");

        if (clickedCell.IsDefencePlacementArea && !clickedCell.PlacedDefenceItem)
        {
            _selectedCell = clickedCell;
            
            if (!defenceSelectionUIPanel) return;
            
            defenceSelectionUIPanel.SetActive(true);
        }
        else
        {
            _selectedCell = null;
            _isDefenceSelectionPanelActive = false;
            
            if (defenceSelectionUIPanel)
            {
                defenceSelectionUIPanel.SetActive(false);
            }
            
            Debug.LogWarning("Cannot place defence item here.");
        }
    }

    public void PlaceDefenceItem(int itemTypeIndex)
    {
        if (!_selectedCell) return;
        
        var defenceItemType = Constants.DefenceItemDataDictionary[itemTypeIndex].Type;

        var defenceItemGO = PoolManager.Instance.SpawnFromPool(
            defenceItemType, _selectedCell.transform.position, Quaternion.identity);
        
        if (defenceItemGO)
        {
            var newDefenceItem = defenceItemGO.GetComponent<DefenceItem>();
            
            if (!newDefenceItem)
            {
                _selectedCell = null;
                PoolManager.Instance.ReturnToPool(defenceItemType, defenceItemGO);
                Debug.LogError("Spawned defence item GameObject does not have a DefenceItem component!");
                return;
            }
            
            newDefenceItem.Initialize(_selectedCell);
            
            _defenceItems.Add(newDefenceItem);

            _selectedCell.PlaceDefenceItem(newDefenceItem);

            Debug.Log($"Placed Defence Item {itemTypeIndex} at Row: {_selectedCell.yIndex}, Column: {_selectedCell.xIndex}");
        }
        else
        {
            Debug.LogError($"Invalid Defence Item Type Index: {itemTypeIndex}");
        }

        if (defenceSelectionUIPanel)
        {
            defenceSelectionUIPanel.SetActive(false);
        }
        
        _selectedCell = null;
        _isDefenceSelectionPanelActive = false;
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
        
        _levelWin = true;
            
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