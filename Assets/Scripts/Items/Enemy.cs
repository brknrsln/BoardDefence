using DG.Tweening;
using Interfaces;
using UnityEngine;

namespace Items
{
    public abstract class Enemy : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer enemySpriteRenderer;
        [SerializeField] private TMPro.TMP_Text hp;
        
        protected int TypeIndex;

        private Constants.ObjectItemType ObjectItemType { get; set; }
        private int Health { get; set; }
        private float SpeedInSeconds { get; set; }

        private int CurrentRow { get; set; }
        private int CurrentColumn { get; set; }

        private int _currentHealth;

        protected virtual void Awake()
        {
            Initialize();
        }

        private void Move()
        {
            var targetCell = GameManager.Instance.board.GetCell(Constants.BoardRows - 1, CurrentColumn);
            
            if (!targetCell) return;
            
            transform.DOMoveY(
                targetCell.transform.position.y, Constants.BoardRows / SpeedInSeconds)
                .SetEase(Ease.Linear)
                .OnComplete(GameManager.Instance.EnemiesReachedBase);
        }

        private void Initialize()
        {
            var data = Constants.EnemyDataDictionary[TypeIndex];
            Health = data.Health;
            SpeedInSeconds = data.Speed;
            ObjectItemType = data.ObjectItemType;
            _currentHealth = Health;
            
            UpdateHpText();
        }
        
        public void Initialize(int row, int column)
        {
            CurrentRow = row;
            CurrentColumn = column;
            _currentHealth = Health;
            
            UpdateHpText();
            
            if (GameManager.Instance && GameManager.Instance.board)
            {
                var startCell = GameManager.Instance.board.GetCell(row, column);
                if (startCell)
                {
                    transform.position = startCell.transform.position;
                }
                else
                {
                    Debug.LogError($"Start cell is null for enemy at ({row}, {column})");
                }
            }
            else
            {
                Debug.LogError("GameManager or Board instance is null. Cannot initialize enemy position.");
            }

            if (enemySpriteRenderer)
            {
                enemySpriteRenderer.color = Color.red;
            }
            
            Move();
            
            Debug.Log($"{typeof(Constants.ObjectItemType)} initialized at Row: {CurrentRow}, Column: {CurrentColumn} with Health: {_currentHealth}");
        }
        
        public void TakeDamage(int damageAmount)
        {
            _currentHealth -= damageAmount;
            
            UpdateHpText();
            
            Debug.Log($"{typeof(Constants.ObjectItemType)} took {damageAmount} damage. Remaining Health: {_currentHealth}");

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void UpdateHpText()
        {
            if (hp)
            {
                hp.text = "HP: " + _currentHealth;
            }
        }

        private void Die()
        {
            Debug.Log($"{ObjectItemType} died!");
            transform.DOKill();
            
            if (GameManager.Instance)
            {
                GameManager.Instance.EnemyDied(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnSpawnFromPool()
        {
            _currentHealth = Health;
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            transform.DOKill();
            gameObject.SetActive(false);
        }
    }
}