using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Visual preview when placing a machine. Green = valid, Red = invalid.
    /// </summary>
    public class MachineGhost : MonoBehaviour
    {
        #region Fields

        [SerializeField] private SpriteRenderer _spriteRenderer;

        private static readonly Color ValidColor = new(0f, 1f, 0f, 0.5f);
        private static readonly Color InvalidColor = new(1f, 0f, 0f, 0.5f);

        #endregion

        #region Public Methods

        public void SetValid(bool isValid)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = isValid ? ValidColor : InvalidColor;
            }
        }

        public void SetSprite(Sprite sprite)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sprite = sprite;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdatePosition(Vector3 worldPos)
        {
            transform.position = worldPos;
        }

        #endregion
    }
}
