using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class DefenceItemButtonUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text countText;
        [SerializeField] private Button button;
        [SerializeField] private Constants.ObjectItemType defenceObjectItemType;

        private void Awake()
        {
            if (!countText)
            {
                Debug.LogError("Count Text is not assigned in DefenceItemButtonUI.");
            }

            if (!button)
            {
                Debug.LogError("Button is not assigned in DefenceItemButtonUI.");
            }
            
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnEnable()
        {
            UpdateCountText();
        }

        private void OnButtonClick()
        {
            GameManager.Instance.PlaceDefenceItem(defenceObjectItemType);
        }

        private void UpdateCountText()
        {
            countText.text = $"{PoolManager.Instance.GetCountOfTypeInPool(defenceObjectItemType)}";
            button.interactable = PoolManager.Instance.GetCountOfTypeInPool(defenceObjectItemType) > 0;
        }
    }
}