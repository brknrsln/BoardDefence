using UnityEngine;

namespace Board
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform cellParent;
        
        [SerializeField] private float cellSpacing = 1.1f; 
        
        private Cell[,] _cells;
        
        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            CreateBoard();
        }

        private void CreateBoard()
        {
            const int rows = Constants.BoardRows;
            const int columns = Constants.BoardColumns;
            
            var startX = -(columns / 2.0f) * cellSpacing + (cellSpacing / 2.0f);
            var startY = (rows / 2.0f) * cellSpacing - (cellSpacing / 2.0f);
            
            _cells = new Cell[rows, columns];

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < columns; x++)
                {
                    var cellObject = Instantiate(cellPrefab, cellParent);
                    var localPosition = new Vector3(startX + x * cellSpacing, startY - y * cellSpacing, 0);
                    cellObject.transform.localPosition = localPosition;
                    
                    var cell = cellObject.GetComponent<Cell>();
                    
                    if (!cell)
                    {
                        Debug.LogError("Cell prefab does not have a Cell component.");
                        return;
                    }
                    
                    var isDefenceArea = y >= rows / 2;
                    cell.Initialize(y, x, isDefenceArea);
                    _cells[y, x] = cell;
                    cellObject.name = $"Cell ({y}, {x})";
                }
            }
            IsInitialized = true;
        }
        
        public Cell GetCell(int row, int column)
        {
            if (row is >= 0 and < Constants.BoardRows && column is >= 0 and < Constants.BoardColumns)
            {
                return _cells[row, column];
            }
            
            Debug.LogError($"Invalid cell indices: ({row}, {column})");
            
            return null;
        }
    }
}