using Items;
using UnityEngine;

namespace Board
{
    public class Cell : MonoBehaviour
    {
        public SpriteRenderer cellSpriteRenderer; 
        
        public int xIndex;
        public int yIndex;
        
        public bool IsDefencePlacementArea { get; private set; }
        
        public DefenceItem PlacedDefenceItem { get; private set; }
        
        public Color normalColor = Color.white;
        public Color placementAreaColor = Color.green;
        
        public void Initialize(int yIndex, int xIndex, bool isDefencePlacementArea = false)
        {
            this.yIndex = yIndex;
            this.xIndex = xIndex;
            IsDefencePlacementArea = isDefencePlacementArea;

            if (cellSpriteRenderer)
            {
                cellSpriteRenderer.color = IsDefencePlacementArea ? placementAreaColor : normalColor;
            }
        }
        
        public void PlaceDefenceItem(DefenceItem item)
        {
            if (IsDefencePlacementArea && !PlacedDefenceItem)
            {
                PlacedDefenceItem = item;
                Debug.Log($"Defence Item placed at Row: {yIndex}, Column: {xIndex}");
            }
            else
            {
                Debug.LogWarning($"Cannot place item here. IsDefencePlacementArea: {IsDefencePlacementArea}, PlacedItem: {PlacedDefenceItem != null}");
            }
        }

        public void RemoveDefenceItem()
        {
            PlacedDefenceItem = null;
        }
    }
}