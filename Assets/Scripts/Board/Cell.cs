using Items;
using UnityEngine;

namespace Board
{
    public abstract class Cell : MonoBehaviour
    {
        [SerializeField] internal SpriteRenderer cellSpriteRenderer; 
        
        protected int XIndex;
        protected int YIndex;

        protected DefenceItem PlacedDefenceItem;
        
        public virtual void Initialize(int positionY, int positionX)
        {
            YIndex = positionY;
            XIndex = positionX;
            
        }
        
        public virtual void PlaceDefenceItem(DefenceItem item)
        {
        }

        public void RemoveDefenceItem()
        {
            PlacedDefenceItem = null;
        }
    }
}