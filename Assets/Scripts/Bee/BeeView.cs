using System.Collections;
using Cat;
using Line;
using UnityEngine;
using Utils.Pool;
using Random = UnityEngine.Random;

namespace Bee
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BeeView : MonoBehaviour, IPoolElement<IPool<BeeView>>
    {
        [SerializeField] private float moveSpeed = 4f;

        private Rigidbody2D _rigidbody2D;
        private Transform _target;
        private Transform _transform;
        private Vector3 _targetPosition;
        private Vector3 _lastDirection;
        private bool _targetRage;

        private bool _randomDirection = true;
        private bool _hitSomething;

        private Coroutine _moveCoroutine;
        private IPool<BeeView> _pool;

        public void SetTarget(Transform target)
        {
            _target = target;
            StartMovement();
        }

        private void StartMovement()
        {
            _hitSomething = false;

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            GetNewTarget(_randomDirection);

            _moveCoroutine = StartCoroutine(MoveToTarget(_randomDirection ? 1f : -1f, !_randomDirection));

            _randomDirection = !_randomDirection;
        }

        private void Awake()
        {
            _transform = transform;
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void GetNewTarget(bool randomAround = false)
        {
            if (_lastDirection == Vector3.zero)
            {
                _lastDirection = (_transform.position - _target.position).normalized;
            }

            if (Vector2.Distance(_target.position, _transform.position) >= 5)
            {
                _lastDirection = Vector3.zero;
                _targetPosition = _target.position;
            }
            else
            {
                _targetPosition = randomAround
                    ? _transform.position + Quaternion.Euler(new Vector3(0, 0, Random.Range(-90, 90))) *
                    _lastDirection *
                    (-1 * Random.Range(1f, 3f))
                    : _target.position;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObject collisionGameObject = collision.gameObject;

            ICatView cat = collisionGameObject.GetComponent<ICatView>();

            if (cat != null)
            {
                cat.Kill();
                return;
            }

            if (collisionGameObject.GetComponent<LineView>())
            {
                StartCoroutine(PushToTarget());
                return;
            }

            _hitSomething = true;
        }

        private IEnumerator PushToTarget()
        {
            _rigidbody2D.velocity = _lastDirection * (moveSpeed * 2f);
            yield return new WaitForSeconds(1f);
            StartMovement();
        }

        private IEnumerator MoveToTarget(float time = -1, bool moveUntilHit = false)
        {
            Vector3 currentPosition = _transform.position;

            _lastDirection = (_targetPosition - currentPosition).normalized;

            if (moveUntilHit)
            {
                while (!_hitSomething)
                {
                    _rigidbody2D.velocity = _lastDirection * moveSpeed;
                    yield return null;
                }

                _rigidbody2D.velocity = Vector2.zero;
            }
            else
            {
                _rigidbody2D.velocity = _lastDirection * moveSpeed;
            }

            yield return null;

            if (time > 0)
            {
                yield return new WaitForSeconds(time);
            }

            _rigidbody2D.velocity = Vector2.zero;

            StartMovement();
        }

        public void Spawned(IPool<BeeView> pool) => _pool = pool;

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void Despawn()
        {
            _pool?.Despawn(this);
            _pool = null;
        }

        public Transform GetTransform() => _transform;
    }
}