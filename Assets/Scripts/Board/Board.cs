using UnityEngine;

namespace Board
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private GameObject defaultCellPrefab;
        [SerializeField] private GameObject defenceCellPrefab;
        [SerializeField] private Transform cellParent;
        
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
            const float cellSpacing = Constants.CellSpacing;
            
            const float startX = -(columns / 2.0f) * cellSpacing + (cellSpacing / 2.0f);
            const float startY = (rows / 2.0f) * cellSpacing - (cellSpacing / 2.0f);
            
            _cells = new Cell[rows, columns];
            
            var xPositions = new float[columns];
            
            for (var i = 0; i < columns; i++)
            {
                xPositions[i] = startX + i * cellSpacing;
            }

            for (var y = 0; y < rows; y++)
            {
                var isDefenceArea = y >= rows / 2;
                
                for (var x = 0; x < columns; x++)
                {
                    var cellObject = Instantiate(isDefenceArea ? defenceCellPrefab : defaultCellPrefab, cellParent);

                    var localPosition = new Vector3(xPositions[x], startY - y * cellSpacing, 0);
                    cellObject.transform.localPosition = localPosition;
                    
                    var cell = cellObject.GetComponent<Cell>();
                    
                    if (!cell)
                    {
                        Debug.LogError("Cell prefab does not have a Cell component.");
                        return;
                    }
                    
                    cell.Initialize(y, x);
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