using Items;
using UnityEngine;
using UnityEngine.UI;

namespace Board
{
    public class DefenceCell : Cell
    {
        [SerializeField] private Button placeButton;
        
        public override void Initialize(int positionY, int positionX)
        {
            base.Initialize(positionY, positionX);
            cellSpriteRenderer.color = Color.green;
            placeButton.onClick.AddListener(() => GameManager.Instance.OnClickCellButton(this));
        }

        public override void PlaceDefenceItem(DefenceItem item)
        {
            PlacedDefenceItem = item;
            Debug.Log($"Defence Item placed at Row: {YIndex}, Column: {XIndex}");
        }
    }
}