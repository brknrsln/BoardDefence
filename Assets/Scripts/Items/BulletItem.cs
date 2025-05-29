using DG.Tweening;
using UnityEngine;

namespace Items
{
    public class BulletItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bulletSpriteRenderer;
        [SerializeField] private TMPro.TMP_Text bulletText;

        private int _damage;
        private int _range;
        private float _speed;
        private Vector2 _targetPosition;
        private Constants.AttackDirection _attackDirection;

        private Sequence _attackSequence;
        
        public void Initialize(int damage, int range, float speed, Constants.AttackDirection attackDirection)
        {
            _damage = damage;
            _range = range;
            _speed = speed;
            _attackDirection = attackDirection;
            
            if (bulletText)
            {
                bulletText.text = _damage.ToString();
            }
            else
            {
                Debug.LogWarning("Bullet Text is not assigned in BulletItem.");
            }
        }

        private void SetTarget()
        {
            var targetPoint = _range * Constants.CellSpacing;
            
            switch (_attackDirection)
            {
                case Constants.AttackDirection.Forward:
                    _targetPosition = new Vector2(transform.position.x, transform.position.y + targetPoint);
                    break;
                case Constants.AttackDirection.Backward:
                    _targetPosition = new Vector2(transform.position.x, transform.position.y - targetPoint);
                    break;
                case Constants.AttackDirection.Left:
                    _targetPosition = new Vector2(transform.position.x - targetPoint, transform.position.y);
                    break;
                case Constants.AttackDirection.Right:
                    _targetPosition = new Vector2(transform.position.x + targetPoint, transform.position.y);
                    break;
            }
        }

        public Sequence StartMove()
        {
            SetTarget();
            
            bulletSpriteRenderer.color = Color.magenta;
            
            if (_attackSequence != null && _attackSequence.IsActive())
            {
                _attackSequence.Kill();
            }
            
            _attackSequence = DOTween.Sequence();
            _attackSequence.Pause();

            _attackSequence.Append(transform.DOMove(_targetPosition, _speed).SetEase(Ease.Linear));

            _attackSequence.SetLoops(-1, LoopType.Restart);
            return _attackSequence;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy")) return;
            
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_damage);
            }
        }
    }
}