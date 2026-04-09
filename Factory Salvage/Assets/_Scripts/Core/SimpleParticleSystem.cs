using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Lightweight particle pool using SpriteRenderers.
    /// No Unity ParticleSystem overhead — perfect for mobile.
    /// </summary>
    public class SimpleParticleSystem : MonoBehaviour
    {
        #region Fields

        [SerializeField] private int _poolSize = 80;

        private Particle[] _particles;
        private SpriteRenderer[] _renderers;
        private Sprite _particleSprite;

        private static SimpleParticleSystem _instance;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            _instance = this;
            _particleSprite = CreateParticleSprite();
            InitPool();
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        private void Update()
        {
            if (_particles == null || _renderers == null) return;

            for (int i = 0; i < _poolSize; i++)
            {
                ref var p = ref _particles[i];
                if (!p.Active) continue;

                p.Lifetime -= Time.deltaTime;
                if (p.Lifetime <= 0f)
                {
                    p.Active = false;
                    _renderers[i].enabled = false;
                    continue;
                }

                // Move
                p.Velocity.y -= p.Gravity * Time.deltaTime;
                p.Position += p.Velocity * Time.deltaTime;

                // Fade
                float t = p.Lifetime / p.MaxLifetime;
                var color = p.Color;
                color.a = t;
                _renderers[i].color = color;

                // Scale
                float scale = p.Scale * (0.5f + t * 0.5f);
                _renderers[i].transform.position = p.Position;
                _renderers[i].transform.localScale = Vector3.one * scale;
            }
        }

        #endregion

        #region Public Static API

        public static void Emit(Vector3 position, ParticlePreset preset, int count = 5)
        {
            if (_instance == null) return;
            _instance.EmitInternal(position, preset, count);
        }

        #endregion

        #region Presets

        public enum ParticlePreset
        {
            BuildingProduction,
            EnemyDeath,
            ProjectileHit,
            LevelUp,
            WaveStart,
            ResourceCollect
        }

        #endregion

        #region Private Methods

        private void EmitInternal(Vector3 position, ParticlePreset preset, int count)
        {
            if (_particles == null || _renderers == null) return;

            for (int i = 0; i < count; i++)
            {
                int idx = FindInactive();
                if (idx < 0) return;

                ref var p = ref _particles[idx];
                p.Active = true;
                p.Position = position;

                switch (preset)
                {
                    case ParticlePreset.BuildingProduction:
                        p.Color = ColorPalette.TextGold;
                        p.Velocity = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 2.5f), 0f);
                        p.Gravity = -0.5f;
                        p.MaxLifetime = 0.8f;
                        p.Scale = Random.Range(0.15f, 0.3f);
                        break;

                    case ParticlePreset.EnemyDeath:
                        p.Color = ColorPalette.EnemyBase;
                        var dir = Random.insideUnitCircle.normalized * Random.Range(2f, 5f);
                        p.Velocity = new Vector3(dir.x, dir.y + 1f, 0f);
                        p.Gravity = 3f;
                        p.MaxLifetime = 0.5f;
                        p.Scale = Random.Range(0.2f, 0.4f);
                        break;

                    case ParticlePreset.ProjectileHit:
                        p.Color = ColorPalette.ProjectileYellow;
                        var hitDir = Random.insideUnitCircle.normalized * Random.Range(3f, 6f);
                        p.Velocity = new Vector3(hitDir.x, hitDir.y, 0f);
                        p.Gravity = 0f;
                        p.MaxLifetime = 0.3f;
                        p.Scale = Random.Range(0.1f, 0.25f);
                        break;

                    case ParticlePreset.LevelUp:
                        p.Color = Color.Lerp(ColorPalette.TextGold, Color.white, Random.Range(0f, 0.5f));
                        var upDir = Random.insideUnitCircle.normalized * Random.Range(1f, 3f);
                        p.Velocity = new Vector3(upDir.x, Mathf.Abs(upDir.y) + 1f, 0f);
                        p.Gravity = -1f;
                        p.MaxLifetime = 1f;
                        p.Scale = Random.Range(0.15f, 0.35f);
                        break;

                    case ParticlePreset.WaveStart:
                        p.Color = ColorPalette.EnemyBase;
                        p.Velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(2f, 4f), 0f);
                        p.Gravity = 2f;
                        p.MaxLifetime = 1.2f;
                        p.Scale = Random.Range(0.2f, 0.4f);
                        break;

                    case ParticlePreset.ResourceCollect:
                        p.Color = ColorPalette.ResWood;
                        p.Velocity = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0.8f, 1.5f), 0f);
                        p.Gravity = -0.3f;
                        p.MaxLifetime = 0.8f;
                        p.Scale = Random.Range(0.1f, 0.2f);
                        break;
                }

                p.Lifetime = p.MaxLifetime;
                _renderers[idx].enabled = true;
                _renderers[idx].color = p.Color;
                _renderers[idx].transform.position = p.Position;
            }
        }

        private int FindInactive()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                if (!_particles[i].Active) return i;
            }
            return -1;
        }

        private void InitPool()
        {
            _particles = new Particle[_poolSize];
            _renderers = new SpriteRenderer[_poolSize];

            for (int i = 0; i < _poolSize; i++)
            {
                var go = new GameObject($"Particle_{i}");
                go.transform.SetParent(transform);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = _particleSprite;
                sr.sortingOrder = 20;
                sr.enabled = false;

                _renderers[i] = sr;
                _particles[i] = new Particle { Active = false };
            }
        }

        private Sprite CreateParticleSprite()
        {
            const int s = 6;
            var pixels = new Color[s * s];
            // Soft circle
            for (int y = 0; y < s; y++)
                for (int x = 0; x < s; x++)
                {
                    float dx = x - s / 2f + 0.5f;
                    float dy = y - s / 2f + 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist < s / 2f)
                        pixels[y * s + x] = new Color(1f, 1f, 1f, 1f - dist / (s / 2f));
                }

            var tex = new Texture2D(s, s);
            tex.SetPixels(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, s, s), Vector2.one * 0.5f, 6f);
        }

        #endregion

        #region Particle Struct

        private struct Particle
        {
            public bool Active;
            public Vector3 Position;
            public Vector3 Velocity;
            public Color Color;
            public float Lifetime;
            public float MaxLifetime;
            public float Scale;
            public float Gravity;
        }

        #endregion
    }
}
