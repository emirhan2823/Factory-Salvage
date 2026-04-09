using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Simple procedural animations for sprites. No sprite sheets needed.
    /// Attach to any GameObject with a SpriteRenderer.
    /// </summary>
    public class SpriteAnimator : MonoBehaviour
    {
        #region Fields

        [SerializeField] private AnimationType _animationType = AnimationType.None;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _amplitude = 0.05f;

        private Vector3 _originalPosition;
        private Vector3 _originalScale;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private float _timer;
        private float _flashTimer;
        private bool _isFlashing;

        // Spawn pop
        private float _spawnTimer;
        private bool _isSpawning;

        // Death shrink
        private float _deathTimer;
        private bool _isDying;
        private System.Action _onDeathComplete;

        #endregion

        #region Enums

        public enum AnimationType
        {
            None,
            IdleBounce,
            IdlePulse,
            WalkBob
        }

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            _originalPosition = transform.localPosition;
            _originalScale = transform.localScale;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
        }

        private void Update()
        {
            _timer += Time.deltaTime * _speed;

            // Spawn pop animation
            if (_isSpawning)
            {
                _spawnTimer += Time.deltaTime * 4f;
                if (_spawnTimer < 1f)
                {
                    // Scale: 0 → 1.2 → 1.0
                    float t = _spawnTimer;
                    float scale = t < 0.6f
                        ? Mathf.Lerp(0f, 1.3f, t / 0.6f)
                        : Mathf.Lerp(1.3f, 1f, (t - 0.6f) / 0.4f);
                    transform.localScale = _originalScale * scale;
                }
                else
                {
                    transform.localScale = _originalScale;
                    _isSpawning = false;
                }
                return;
            }

            // Death shrink
            if (_isDying)
            {
                _deathTimer += Time.deltaTime * 5f;
                float scale = Mathf.Lerp(1f, 0f, _deathTimer);
                transform.localScale = _originalScale * Mathf.Max(0f, scale);
                if (_deathTimer >= 1f)
                {
                    _isDying = false;
                    _onDeathComplete?.Invoke();
                }
                return;
            }

            // Hit flash
            if (_isFlashing)
            {
                _flashTimer -= Time.deltaTime;
                if (_flashTimer <= 0f)
                {
                    _isFlashing = false;
                    if (_spriteRenderer != null) _spriteRenderer.color = _originalColor;
                }
            }

            // Continuous animation
            switch (_animationType)
            {
                case AnimationType.IdleBounce:
                    var bouncePos = _originalPosition;
                    bouncePos.y += Mathf.Sin(_timer * 2f) * _amplitude;
                    transform.localPosition = bouncePos;
                    break;

                case AnimationType.IdlePulse:
                    float pulse = 1f + Mathf.Sin(_timer * 3f) * _amplitude;
                    transform.localScale = _originalScale * pulse;
                    break;

                case AnimationType.WalkBob:
                    var bobPos = _originalPosition;
                    bobPos.y += Mathf.Abs(Mathf.Sin(_timer * 8f)) * _amplitude;
                    transform.localPosition = bobPos;
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void SetAnimation(AnimationType type, float speed = 1f, float amplitude = 0.05f)
        {
            _animationType = type;
            _speed = speed;
            _amplitude = amplitude;
        }

        public void PlaySpawnPop()
        {
            _isSpawning = true;
            _spawnTimer = 0f;
            transform.localScale = Vector3.zero;
        }

        public void PlayHitFlash(float duration = 0.12f)
        {
            if (_spriteRenderer == null) return;
            _isFlashing = true;
            _flashTimer = duration;
            _spriteRenderer.color = Color.white;
        }

        public void PlayDeathShrink(System.Action onComplete = null)
        {
            _isDying = true;
            _deathTimer = 0f;
            _onDeathComplete = onComplete;
        }

        #endregion
    }
}
