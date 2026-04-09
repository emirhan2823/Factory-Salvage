using UnityEngine;
using FactorySalvage.Core;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Renders layered background: sky gradient, parallax clouds/mountains, grass.
    /// Attach to a scene GameObject — creates child renderers at Start.
    /// </summary>
    public class EnvironmentRenderer : MonoBehaviour
    {
        #region Fields

        [Header("Config")]
        [SerializeField] private float _skyWidth = 60f;
        [SerializeField] private float _skyHeight = 40f;
        [SerializeField] private int _cloudCount = 5;
        [SerializeField] private int _mountainCount = 3;

        private Transform _cameraTransform;
        private Transform _skyTransform;
        private Transform[] _clouds;
        private Transform[] _mountains;
        private float[] _cloudSpeeds;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            _cameraTransform = Camera.main?.transform;
            CreateSky();
            CreateMountains();
            CreateClouds();
            CreateGrassDetails();
        }

        private void Update()
        {
            if (_cameraTransform == null) return;

            float camX = _cameraTransform.position.x;
            float camY = _cameraTransform.position.y;

            // Sky follows camera
            if (_skyTransform != null)
            {
                _skyTransform.position = new Vector3(camX, camY, 0f);
            }

            // Parallax mountains (30% camera speed)
            if (_mountains != null)
            {
                foreach (var m in _mountains)
                {
                    if (m == null) continue;
                    var pos = m.localPosition;
                    pos.x += (camX * 0.3f - pos.x) * 0.02f;
                    m.localPosition = pos;
                }
            }

            // Clouds move + parallax (10% camera speed)
            if (_clouds != null)
            {
                for (int i = 0; i < _clouds.Length; i++)
                {
                    if (_clouds[i] == null) continue;
                    var pos = _clouds[i].position;
                    pos.x -= _cloudSpeeds[i] * Time.deltaTime;

                    // Wrap clouds
                    if (pos.x < camX - _skyWidth * 0.6f)
                    {
                        pos.x = camX + _skyWidth * 0.6f;
                        pos.y = Random.Range(6f, 10f);
                    }

                    _clouds[i].position = pos;
                }
            }
        }

        #endregion

        #region Private Methods

        private void CreateSky()
        {
            var skyGo = new GameObject("Sky");
            skyGo.transform.SetParent(transform);
            _skyTransform = skyGo.transform;

            var sr = skyGo.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.CreateSkyGradient(128, 128);
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = new Vector2(_skyWidth, _skyHeight);
            sr.sortingOrder = -10;
        }

        private void CreateMountains()
        {
            _mountains = new Transform[_mountainCount];

            for (int i = 0; i < _mountainCount; i++)
            {
                var go = new GameObject($"Mountain_{i}");
                go.transform.SetParent(transform);

                float x = -10f + i * 15f;
                float y = 1f + Random.Range(-0.5f, 0.5f);
                go.transform.position = new Vector3(x, y, 0f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = CreateMountainSprite(i);
                sr.sortingOrder = -8;

                _mountains[i] = go.transform;
            }
        }

        private void CreateClouds()
        {
            _clouds = new Transform[_cloudCount];
            _cloudSpeeds = new float[_cloudCount];

            for (int i = 0; i < _cloudCount; i++)
            {
                var go = new GameObject($"Cloud_{i}");
                go.transform.SetParent(transform);

                float x = Random.Range(-15f, 25f);
                float y = Random.Range(6f, 10f);
                go.transform.position = new Vector3(x, y, 0f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = CreateCloudSprite();
                sr.sortingOrder = -9;
                sr.color = ColorPalette.WithAlpha(ColorPalette.CloudWhite, Random.Range(0.3f, 0.6f));

                float scale = Random.Range(0.8f, 1.5f);
                go.transform.localScale = new Vector3(scale, scale * 0.6f, 1f);

                _clouds[i] = go.transform;
                _cloudSpeeds[i] = Random.Range(0.15f, 0.4f);
            }
        }

        private void CreateGrassDetails()
        {
            var grassParent = new GameObject("GrassDetails");
            grassParent.transform.SetParent(transform);

            // Grass blades along ground top
            for (float x = -12f; x < 22f; x += 0.3f)
            {
                var go = new GameObject("Grass");
                go.transform.SetParent(grassParent.transform);
                go.transform.position = new Vector3(x + Random.Range(-0.1f, 0.1f), 0.1f, 0f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = CreateGrassBladeSprite();
                sr.sortingOrder = 0;
                sr.color = Color.Lerp(ColorPalette.GrassTop, ColorPalette.GrassBody, Random.Range(0f, 1f));

                float scaleX = Random.Range(0.6f, 1.2f);
                float scaleY = Random.Range(0.5f, 1.3f);
                go.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
        }

        private Sprite CreateCloudSprite()
        {
            const int w = 32, h = 16;
            var pixels = new Color[w * h];

            // Fluffy cloud shape (overlapping circles)
            DrawCircle(pixels, w, h, 10, 8, 6, Color.white);
            DrawCircle(pixels, w, h, 18, 9, 5, Color.white);
            DrawCircle(pixels, w, h, 24, 7, 4, Color.white);
            DrawCircle(pixels, w, h, 14, 11, 4, Color.white);

            return MakeSprite(pixels, w, h, 32f);
        }

        private Sprite CreateMountainSprite(int index)
        {
            const int w = 64, h = 48;
            var pixels = new Color[w * h];

            var color = index % 2 == 0 ? ColorPalette.MountainFar : ColorPalette.MountainNear;
            var lightColor = ColorPalette.Lighten(color, 0.1f);

            // Mountain triangle
            int peakX = w / 2 + (index - 1) * 5;
            int peakY = h - 4;

            for (int y = 0; y < peakY; y++)
            {
                float t = (float)y / peakY;
                int halfWidth = (int)((1f - t) * (w / 2));
                int left = peakX - halfWidth;
                int right = peakX + halfWidth;
                var rowColor = Color.Lerp(color, lightColor, t * 0.3f);
                for (int x = Mathf.Max(0, left); x < Mathf.Min(w, right); x++)
                {
                    pixels[y * w + x] = rowColor;
                }
            }

            // Snow cap on top 20%
            for (int y = (int)(peakY * 0.8f); y < peakY; y++)
            {
                float t = (float)(y - peakY * 0.8f) / (peakY * 0.2f);
                int halfWidth = (int)((1f - (float)y / peakY) * (w / 2));
                int left = peakX - halfWidth;
                int right = peakX + halfWidth;
                for (int x = Mathf.Max(0, left); x < Mathf.Min(w, right); x++)
                {
                    pixels[y * w + x] = Color.Lerp(pixels[y * w + x], Color.white, 0.6f);
                }
            }

            return MakeSprite(pixels, w, h, 32f);
        }

        private Sprite CreateGrassBladeSprite()
        {
            const int w = 4, h = 8;
            var pixels = new Color[w * h];

            // Simple blade shape
            SetPixel(pixels, w, h, 1, 0, ColorPalette.GrassBody);
            SetPixel(pixels, w, h, 2, 0, ColorPalette.GrassBody);
            SetPixel(pixels, w, h, 1, 1, ColorPalette.GrassBody);
            SetPixel(pixels, w, h, 2, 1, ColorPalette.GrassBody);
            SetPixel(pixels, w, h, 1, 2, ColorPalette.GrassTop);
            SetPixel(pixels, w, h, 2, 2, ColorPalette.GrassTop);
            SetPixel(pixels, w, h, 1, 3, ColorPalette.GrassTop);
            SetPixel(pixels, w, h, 2, 3, ColorPalette.GrassTop);
            SetPixel(pixels, w, h, 1, 4, ColorPalette.GrassTop);
            SetPixel(pixels, w, h, 1, 5, ColorPalette.Lighten(ColorPalette.GrassTop, 0.1f));

            return MakeSprite(pixels, w, h, 16f);
        }

        // Helpers
        private static void SetPixel(Color[] px, int w, int h, int x, int y, Color c)
        {
            if (x >= 0 && x < w && y >= 0 && y < h) px[y * w + x] = c;
        }

        private static void DrawCircle(Color[] px, int w, int h, int cx, int cy, int r, Color c)
        {
            for (int y = cy - r; y <= cy + r; y++)
                for (int x = cx - r; x <= cx + r; x++)
                    if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r)
                        SetPixel(px, w, h, x, y, c);
        }

        private static Sprite MakeSprite(Color[] pixels, int w, int h, float ppu)
        {
            var tex = new Texture2D(w, h);
            tex.SetPixels(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), ppu);
        }

        #endregion
    }
}
