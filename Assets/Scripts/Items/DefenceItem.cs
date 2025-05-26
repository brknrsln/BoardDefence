using System.Collections.Generic;
using Board;
using DG.Tweening;
using Interfaces;
using UnityEngine;

namespace Items
{
    public abstract class DefenceItem : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer defenceSpriteRenderer;
        [SerializeField] private GameObject bulletPrefab;

        private int Damage { get; set; }
        private int Range { get; set; }
        private float AttackIntervalInSeconds { get; set; }
        public Constants.AttackDirection AttackDirection { get; protected set; }

        private readonly List<BulletItem> _bulletItems = new();

        private float _attackDurationInSeconds;
        
        protected int TypeIndex;
        private Sequence _attackSequence;
        
        protected virtual void Awake()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            var data = Constants.DefenceItemDataDictionary[TypeIndex];
            Damage = data.Damage;
            Range = data.Range;
            AttackIntervalInSeconds = data.AttackIntervalInSeconds;
            AttackDirection = data.AttackDirection;

            _attackDurationInSeconds = AttackIntervalInSeconds / Range;

            if (!bulletPrefab) return;
            
            GameObject bulletGO;

            BulletItem bulletItem;
            
            switch (AttackDirection)
            {
                case Constants.AttackDirection.All:
                    for (var i = 0; i < 4; i++)
                    {
                        bulletGO = Instantiate(bulletPrefab, transform);
                        bulletItem = bulletGO.GetComponent<BulletItem>();
                        bulletItem.Initialize(Damage, _attackDurationInSeconds, (Constants.AttackDirection)i);
                        _bulletItems.Add(bulletGO.GetComponent<BulletItem>());
                    }
                    break;
                default:
                    bulletGO = Instantiate(bulletPrefab, transform);
                    bulletItem = bulletGO.GetComponent<BulletItem>();
                    bulletItem.Initialize(Damage, _attackDurationInSeconds, AttackDirection);
                    _bulletItems.Add(bulletItem);
                    break;
            }
        }
        
        public void Initialize(Cell cell)
        {
            TargetCells(cell);

            if (defenceSpriteRenderer)
            {
                defenceSpriteRenderer.color = Color.blue;
            }

            transform.position = cell.transform.position;

            StartAttack();
        }

        private void StartAttack()
        {
            if (_attackSequence != null && _attackSequence.IsActive())
            {
                _attackSequence.Kill();
            }
            
            _attackSequence = DOTween.Sequence();
            _attackSequence.Pause();

            foreach (var bulletItem in _bulletItems)
            {
                var bulletMoveSequence = bulletItem.StartMove();
                _attackSequence.Join(bulletMoveSequence);
            }
            
            _attackSequence.Play();
        }


        private void TargetCells(Cell cell)
        {
            var targetRow = cell.yIndex;
            var targetColumn = cell.xIndex;
            
            var upperRow = targetRow - Range;
            if (upperRow < 0)
            {
                upperRow = 0; // Ensure we don't go out of bounds
            }
            var lowerRow = targetRow + Range;
            if (lowerRow >= Constants.BoardRows)
            {
                lowerRow = Constants.BoardRows - 1; // Ensure we don't go out of bounds
            }
            var leftColumn = targetColumn - Range;
            if (leftColumn < 0)
            {
                leftColumn = 0; // Ensure we don't go out of bounds
            }
            var rightColumn = targetColumn + Range;
            if (rightColumn >= Constants.BoardColumns)
            {
                rightColumn = Constants.BoardColumns - 1; // Ensure we don't go out of bounds
            }

            Cell targetCell;
            BulletItem bulletItem;

            switch (AttackDirection)
            {
                case Constants.AttackDirection.Forward:
                    targetCell = GameManager.Instance.board.GetCell(upperRow, targetColumn);
                    bulletItem = _bulletItems[0];
                    bulletItem.SetTargetCell(targetCell.transform);
                    break;
                case Constants.AttackDirection.Backward:
                    targetCell = GameManager.Instance.board.GetCell(lowerRow, targetColumn);
                    bulletItem = _bulletItems[0];
                    bulletItem.SetTargetCell(targetCell.transform);
                    break;
                case Constants.AttackDirection.Left:
                    targetCell = GameManager.Instance.board.GetCell(targetRow, leftColumn);
                    bulletItem = _bulletItems[0];
                    bulletItem.SetTargetCell(targetCell.transform);
                    break;
                case Constants.AttackDirection.Right:
                    targetCell = GameManager.Instance.board.GetCell(targetRow, rightColumn);
                    bulletItem = _bulletItems[0];
                    bulletItem.SetTargetCell(targetCell.transform);
                    break;
                case Constants.AttackDirection.All:
                    targetCell = GameManager.Instance.board.GetCell(upperRow, targetColumn);
                    bulletItem = _bulletItems[0];
                    bulletItem.SetTargetCell(targetCell.transform);
                    targetCell = GameManager.Instance.board.GetCell(targetRow, leftColumn);
                    bulletItem = _bulletItems[1];
                    bulletItem.SetTargetCell(targetCell.transform);
                    targetCell = GameManager.Instance.board.GetCell(lowerRow, targetColumn);
                    bulletItem = _bulletItems[2];
                    bulletItem.SetTargetCell(targetCell.transform);
                    targetCell = GameManager.Instance.board.GetCell(targetRow, rightColumn);
                    bulletItem = _bulletItems[3];
                    bulletItem.SetTargetCell(targetCell.transform);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(AttackDirection), AttackDirection, null);
            }
        }
        
        public void StopAttack()
        {
            foreach (var bulletItem in _bulletItems)
            {
                bulletItem.DOKill();
                Destroy(bulletItem.gameObject);
            }
            
            if (_attackSequence != null && _attackSequence.IsActive())
            {
                _attackSequence.Kill();
            }
        }

        public void OnSpawnFromPool()
        {
            gameObject.SetActive(true);
            
            if (_attackSequence != null && _attackSequence.IsActive())
            {
                _attackSequence.Kill();
            }
        }

        public void OnReturnToPool()
        {
            gameObject.SetActive(false);
            
            if (_attackSequence != null && _attackSequence.IsActive())
            {
                _attackSequence.Kill();
            }
            
            if (defenceSpriteRenderer)
            {
                defenceSpriteRenderer.color = Color.white;
            }
            
            transform.localScale = Vector3.one;
        }
    }
}