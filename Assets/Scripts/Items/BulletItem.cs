using DG.Tweening;
using UnityEngine;

namespace Items
{
    public class BulletItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bulletSpriteRenderer;
        [SerializeField] private TMPro.TMP_Text bulletText;

        private int _damage;
        private float _speed;
        private Transform _targetTransform;
        private Constants.AttackDirection _attackDirection;

        private Sequence _attackSequence;
        
        public void Initialize(int damage, float speed, Constants.AttackDirection attackDirection)
        {
            _damage = damage;
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

        public void SetTargetCell(Transform targetCell)
        {
            _targetTransform = targetCell;
            
            bulletSpriteRenderer.color = Color.magenta;
        }

        public Sequence StartMove()
        {
            if (_attackSequence != null && _attackSequence.IsActive())
            {
                _attackSequence.Kill();
            }
            
            _attackSequence = DOTween.Sequence();
            _attackSequence.Pause();

            switch (_attackDirection)
            {
                case Constants.AttackDirection.Forward:
                    _attackSequence.Join(
                        transform.DOMoveY(_targetTransform.position.y, _speed).SetEase(Ease.Linear));
                    break;
                case Constants.AttackDirection.Backward:
                    _attackSequence.Join(
                        transform.DOMoveY(_targetTransform.position.y, _speed).SetEase(Ease.Linear));
                    break;
                case Constants.AttackDirection.Left:
                    _attackSequence.Join(
                        transform.DOMoveX(_targetTransform.position.x, _speed).SetEase(Ease.Linear));
                    break;
                case Constants.AttackDirection.Right:
                    _attackSequence.Join(
                        transform.DOMoveX(_targetTransform.position.x, _speed).SetEase(Ease.Linear));
                    break;
            }

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