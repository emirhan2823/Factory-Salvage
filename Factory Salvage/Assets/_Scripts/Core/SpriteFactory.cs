using System.Collections.Generic;
using UnityEngine;
using FactorySalvage.Data;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Procedural sprite generation with shapes, shading, and personality.
    /// All sprites use flat colors with lighter top / darker bottom for depth.
    /// Cached to avoid regenerating identical sprites.
    /// </summary>
    public static class SpriteFactory
    {
        #region Cache

        private static readonly Dictionary<int, Sprite> _cache = new();

        public static void ClearCache()
        {
            _cache.Clear();
        }

        #endregion

        #region Building Sprites

        public static Sprite CreateBuilding(BuildingCategory category, Color baseColor, int level = 1)
        {
            int key = HashCode(nameof(CreateBuilding), baseColor, (int)category, level);
            if (_cache.TryGetValue(key, out var cached)) return cached;

            const int w = 32, h = 48;
            var pixels = new Color[w * h];
            var light = ColorPalette.Lighten(baseColor, 0.15f);
            var dark = ColorPalette.Darken(baseColor, 0.15f);
            var outline = ColorPalette.Darken(baseColor, 0.3f);

            switch (category)
            {
                case BuildingCategory.Resource:
                    DrawHouseShape(pixels, w, h, baseColor, light, dark, outline);
                    break;
                case BuildingCategory.Processing:
                    DrawFactoryShape(pixels, w, h, baseColor, light, dark, outline);
                    break;
                case BuildingCategory.Defense:
                    DrawTowerShape(pixels, w, h, baseColor, light, dark, outline);
                    break;
                case BuildingCategory.Military:
                    DrawBarracksShape(pixels, w, h, baseColor, light, dark, outline);
                    break;
                default:
                    DrawHouseShape(pixels, w, h, baseColor, light, dark, outline);
                    break;
            }

            // Level dots at bottom
            if (level > 1)
            {
                int dots = Mathf.Min(level - 1, 5);
                int startX = (w - dots * 4) / 2;
                for (int i = 0; i < dots; i++)
                {
                    DrawCircle(pixels, w, h, startX + i * 4 + 1, 2, 1, ColorPalette.TextGold);
                }
            }

            var sprite = MakeSprite(pixels, w, h, new Vector2(0.5f, 0.1f));
            _cache[key] = sprite;
            return sprite;
        }

        private static void DrawHouseShape(Color[] px, int w, int h, Color body, Color light, Color dark, Color outline)
        {
            // Body (rectangle, bottom 60%)
            int bodyTop = h * 6 / 10;
            DrawRect(px, w, h, 3, 2, w - 4, bodyTop, body);
            // Lighter top half of body
            DrawRect(px, w, h, 4, bodyTop - 4, w - 5, bodyTop, light);
            // Darker bottom
            DrawRect(px, w, h, 3, 2, w - 4, 5, dark);

            // Roof (triangle)
            int roofBottom = bodyTop;
            int roofTop = h - 3;
            int roofPeak = w / 2;
            for (int y = roofBottom; y < roofTop; y++)
            {
                float t = (float)(y - roofBottom) / (roofTop - roofBottom);
                int halfWidth = (int)((1f - t) * (w / 2 - 2));
                int left = roofPeak - halfWidth;
                int right = roofPeak + halfWidth;
                for (int x = left; x <= right; x++)
                {
                    var roofColor = Color.Lerp(dark, light, t * 0.5f + 0.3f);
                    SetPixel(px, w, h, x, y, roofColor);
                }
            }

            // Outline
            DrawOutline(px, w, h, 3, 2, w - 4, bodyTop, outline);

            // Window (2x2 yellow)
            DrawRect(px, w, h, 8, bodyTop - 8, 12, bodyTop - 4, ColorPalette.WithAlpha(ColorPalette.ProjectileYellow, 0.8f));
            DrawRect(px, w, h, w - 13, bodyTop - 8, w - 9, bodyTop - 4, ColorPalette.WithAlpha(ColorPalette.ProjectileYellow, 0.8f));

            // Door
            DrawRect(px, w, h, w / 2 - 2, 2, w / 2 + 2, 8, dark);
        }

        private static void DrawFactoryShape(Color[] px, int w, int h, Color body, Color light, Color dark, Color outline)
        {
            // Main body (flat roof)
            DrawRect(px, w, h, 2, 2, w - 3, h * 7 / 10, body);
            DrawRect(px, w, h, 3, h * 5 / 10, w - 4, h * 7 / 10, light);
            DrawRect(px, w, h, 2, 2, w - 3, 5, dark);

            // Flat roof
            DrawRect(px, w, h, 1, h * 7 / 10, w - 2, h * 7 / 10 + 3, dark);

            // Chimney
            DrawRect(px, w, h, w - 8, h * 7 / 10, w - 5, h - 4, dark);
            // Smoke dots
            SetPixel(px, w, h, w - 7, h - 3, ColorPalette.CloudGray);
            SetPixel(px, w, h, w - 6, h - 2, ColorPalette.CloudWhite);

            // Gear icon (circle in center)
            DrawCircle(px, w, h, w / 2, h * 4 / 10, 4, ColorPalette.WithAlpha(Color.white, 0.3f));
            DrawCircle(px, w, h, w / 2, h * 4 / 10, 2, dark);

            DrawOutline(px, w, h, 2, 2, w - 3, h * 7 / 10, outline);
        }

        private static void DrawTowerShape(Color[] px, int w, int h, Color body, Color light, Color dark, Color outline)
        {
            // Tapered body (wider at bottom)
            for (int y = 2; y < h - 5; y++)
            {
                float t = (float)(y - 2) / (h - 7);
                int halfWidth = (int)Mathf.Lerp(w / 2 - 2, w / 2 - 5, t);
                int cx = w / 2;
                var rowColor = Color.Lerp(dark, light, t * 0.6f);
                for (int x = cx - halfWidth; x <= cx + halfWidth; x++)
                {
                    SetPixel(px, w, h, x, y, rowColor);
                }
            }

            // Crenellations (battlement)
            int topY = h - 5;
            for (int i = 0; i < 5; i++)
            {
                int bx = w / 2 - 8 + i * 4;
                DrawRect(px, w, h, bx, topY, bx + 2, topY + 3, light);
            }

            // Arrow slit
            DrawRect(px, w, h, w / 2 - 1, h / 2, w / 2 + 1, h / 2 + 4, ColorPalette.Darken(dark, 0.2f));

            // Base platform
            DrawRect(px, w, h, 2, 0, w - 3, 3, dark);
        }

        private static void DrawBarracksShape(Color[] px, int w, int h, Color body, Color light, Color dark, Color outline)
        {
            // Wide building
            DrawRect(px, w, h, 2, 2, w - 3, h * 6 / 10, body);
            DrawRect(px, w, h, 3, h * 4 / 10, w - 4, h * 6 / 10, light);

            // Flat roof with flag
            DrawRect(px, w, h, 1, h * 6 / 10, w - 2, h * 6 / 10 + 2, dark);

            // Flag pole
            DrawRect(px, w, h, w / 2, h * 6 / 10, w / 2 + 1, h - 3, outline);
            // Flag
            DrawRect(px, w, h, w / 2 + 1, h - 7, w / 2 + 6, h - 3, ColorPalette.EnemyBase);

            // Shield icon
            DrawCircle(px, w, h, w / 2, h * 3 / 10, 3, ColorPalette.WithAlpha(Color.white, 0.3f));

            DrawOutline(px, w, h, 2, 2, w - 3, h * 6 / 10, outline);
        }

        #endregion

        #region Enemy Sprites

        public static Sprite CreateEnemy(bool isFast, Color baseColor, bool isBoss = false)
        {
            int key = HashCode(nameof(CreateEnemy), baseColor, isFast ? 1 : 0, isBoss ? 1 : 0);
            if (_cache.TryGetValue(key, out var cached)) return cached;

            int size = isBoss ? 48 : 28;
            var pixels = new Color[size * size];
            var light = ColorPalette.Lighten(baseColor, 0.2f);
            var dark = ColorPalette.Darken(baseColor, 0.2f);

            if (isFast)
            {
                // Narrow, tall shape
                DrawEllipse(pixels, size, size, size / 2, size / 2 - 2, size / 3, size / 2 - 3, baseColor);
                DrawEllipse(pixels, size, size, size / 2, size / 2 + 2, size / 3 - 1, size / 3, light);
            }
            else
            {
                // Round blob shape
                DrawCircle(pixels, size, size, size / 2, size / 2 - 2, size / 2 - 3, baseColor);
                // Lighter top half
                DrawCircle(pixels, size, size, size / 2, size / 2 + 1, size / 3, light);
            }

            // Eyes (white with black pupils)
            int eyeY = size / 2 + (isBoss ? 4 : 2);
            int eyeSpacing = isBoss ? 6 : 3;
            DrawCircle(pixels, size, size, size / 2 - eyeSpacing, eyeY, isBoss ? 3 : 2, ColorPalette.EnemyEye);
            DrawCircle(pixels, size, size, size / 2 + eyeSpacing, eyeY, isBoss ? 3 : 2, ColorPalette.EnemyEye);
            // Pupils
            SetPixel(pixels, size, size, size / 2 - eyeSpacing, eyeY, Color.black);
            SetPixel(pixels, size, size, size / 2 + eyeSpacing, eyeY, Color.black);

            // Mouth (angry line for fast, round for normal)
            int mouthY = eyeY - (isBoss ? 5 : 3);
            if (isFast)
            {
                // Angry V mouth
                for (int i = -2; i <= 2; i++)
                {
                    SetPixel(pixels, size, size, size / 2 + i, mouthY + Mathf.Abs(i) / 2, dark);
                }
            }
            else
            {
                // Round mouth
                SetPixel(pixels, size, size, size / 2 - 1, mouthY, dark);
                SetPixel(pixels, size, size, size / 2, mouthY, dark);
                SetPixel(pixels, size, size, size / 2 + 1, mouthY, dark);
            }

            // Boss horns
            if (isBoss)
            {
                int hornY = size / 2 + size / 3;
                DrawRect(pixels, size, size, size / 2 - 10, hornY, size / 2 - 8, hornY + 6, dark);
                DrawRect(pixels, size, size, size / 2 + 8, hornY, size / 2 + 10, hornY + 6, dark);
            }

            // Dark bottom shadow
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size / 4; y++)
                {
                    int idx = y * size + x;
                    if (pixels[idx].a > 0.1f)
                    {
                        pixels[idx] = Color.Lerp(pixels[idx], dark, 0.4f);
                    }
                }
            }

            float ppu = isBoss ? 48f : 28f;
            var sprite = MakeSprite(pixels, size, size, new Vector2(0.5f, 0.25f), ppu);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Projectile Sprites

        public static Sprite CreateProjectile(Color baseColor)
        {
            int key = HashCode(nameof(CreateProjectile), baseColor, 0, 0);
            if (_cache.TryGetValue(key, out var cached)) return cached;

            const int s = 10;
            var pixels = new Color[s * s];
            var bright = ColorPalette.Lighten(baseColor, 0.3f);

            // Diamond shape
            int cx = s / 2, cy = s / 2;
            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    int dx = Mathf.Abs(x - cx);
                    int dy = Mathf.Abs(y - cy);
                    if (dx + dy <= 3)
                    {
                        var c = (dx + dy <= 1) ? bright : baseColor;
                        SetPixel(pixels, s, s, x, y, c);
                    }
                }
            }

            var sprite = MakeSprite(pixels, s, s, new Vector2(0.5f, 0.5f), 10f);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Base Sprite

        public static Sprite CreateBase(float healthPercent = 1f)
        {
            int key = HashCode(nameof(CreateBase), Color.blue, (int)(healthPercent * 10), 0);
            if (_cache.TryGetValue(key, out var cached)) return cached;

            const int w = 48, h = 64;
            var pixels = new Color[w * h];

            var bodyColor = Color.Lerp(ColorPalette.HealthRed, ColorPalette.StoneGray, healthPercent);
            var light = ColorPalette.Lighten(bodyColor, 0.1f);
            var dark = ColorPalette.Darken(bodyColor, 0.15f);

            // Main wall
            DrawRect(pixels, w, h, 4, 2, w - 5, h * 7 / 10, bodyColor);
            DrawRect(pixels, w, h, 5, h * 5 / 10, w - 6, h * 7 / 10, light);
            DrawRect(pixels, w, h, 4, 2, w - 5, 6, dark);

            // Crenellations
            int topY = h * 7 / 10;
            for (int i = 0; i < 7; i++)
            {
                int bx = 6 + i * 6;
                if (bx + 3 < w - 5)
                    DrawRect(pixels, w, h, bx, topY, bx + 3, topY + 4, light);
            }

            // Gate arch
            int gateX = w / 2;
            DrawRect(pixels, w, h, gateX - 4, 2, gateX + 4, 12, dark);
            DrawCircle(pixels, w, h, gateX, 12, 4, dark);

            // Windows
            for (int i = 0; i < 3; i++)
            {
                int wx = 10 + i * 12;
                DrawRect(pixels, w, h, wx, h * 4 / 10, wx + 3, h * 4 / 10 + 4,
                    ColorPalette.WithAlpha(ColorPalette.ProjectileYellow, 0.6f));
            }

            // Flag
            DrawRect(pixels, w, h, w / 2, topY + 4, w / 2 + 1, h - 3, dark);
            DrawRect(pixels, w, h, w / 2 + 1, h - 8, w / 2 + 7, h - 4,
                healthPercent > 0.5f ? ColorPalette.TowerGreen : ColorPalette.EnemyBase);

            var sprite = MakeSprite(pixels, w, h, new Vector2(0.5f, 0.05f), 32f);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Environment Sprites

        public static Sprite CreateSkyGradient(int width = 256, int height = 256)
        {
            int key = HashCode(nameof(CreateSkyGradient), Color.blue, width, height);
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / height;
                var color = Color.Lerp(ColorPalette.SkyTop, ColorPalette.SkyBottom, t);
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = color;
                }
            }

            var sprite = MakeSprite(pixels, width, height, Vector2.one * 0.5f, 32f);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite CreateSlotIndicator(bool isDefense)
        {
            int key = HashCode(nameof(CreateSlotIndicator), isDefense ? Color.red : Color.white, 0, 0);
            if (_cache.TryGetValue(key, out var cached)) return cached;

            const int w = 32, h = 48;
            var pixels = new Color[w * h];
            var borderColor = isDefense
                ? ColorPalette.WithAlpha(ColorPalette.EnemyBase, 0.25f)
                : ColorPalette.SlotEmpty;

            // Dashed border
            for (int x = 2; x < w - 2; x++)
            {
                if (x % 4 < 2)
                {
                    SetPixel(pixels, w, h, x, 2, borderColor);
                    SetPixel(pixels, w, h, x, h - 3, borderColor);
                }
            }
            for (int y = 2; y < h - 2; y++)
            {
                if (y % 4 < 2)
                {
                    SetPixel(pixels, w, h, 2, y, borderColor);
                    SetPixel(pixels, w, h, w - 3, y, borderColor);
                }
            }

            // Plus icon in center
            var plusColor = ColorPalette.WithAlpha(borderColor, 0.4f);
            DrawRect(pixels, w, h, w / 2 - 1, h / 2 - 4, w / 2 + 1, h / 2 + 4, plusColor);
            DrawRect(pixels, w, h, w / 2 - 4, h / 2 - 1, w / 2 + 4, h / 2 + 1, plusColor);

            var sprite = MakeSprite(pixels, w, h, new Vector2(0.5f, 0.1f));
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite CreateResourceIcon(string resourceId)
        {
            Color iconColor = resourceId switch
            {
                "wood" => ColorPalette.ResWood,
                "iron" => ColorPalette.ResIron,
                "steel" => ColorPalette.ResSteel,
                "gold" => ColorPalette.ResGold,
                _ => Color.white
            };

            int key = HashCode(nameof(CreateResourceIcon), iconColor, 0, 0);
            if (_cache.TryGetValue(key, out var cached)) return cached;

            const int s = 16;
            var pixels = new Color[s * s];
            var light = ColorPalette.Lighten(iconColor, 0.2f);

            // Diamond/gem shape
            DrawCircle(pixels, s, s, s / 2, s / 2, s / 2 - 2, iconColor);
            DrawCircle(pixels, s, s, s / 2 - 1, s / 2 + 1, s / 4, light);

            var sprite = MakeSprite(pixels, s, s, Vector2.one * 0.5f, 16f);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Drawing Helpers

        private static void SetPixel(Color[] pixels, int w, int h, int x, int y, Color c)
        {
            if (x >= 0 && x < w && y >= 0 && y < h)
                pixels[y * w + x] = c;
        }

        private static void DrawRect(Color[] pixels, int w, int h, int x0, int y0, int x1, int y1, Color c)
        {
            for (int y = Mathf.Max(0, y0); y <= Mathf.Min(h - 1, y1); y++)
                for (int x = Mathf.Max(0, x0); x <= Mathf.Min(w - 1, x1); x++)
                    pixels[y * w + x] = c;
        }

        private static void DrawOutline(Color[] pixels, int w, int h, int x0, int y0, int x1, int y1, Color c)
        {
            for (int x = x0; x <= x1; x++) { SetPixel(pixels, w, h, x, y0, c); SetPixel(pixels, w, h, x, y1, c); }
            for (int y = y0; y <= y1; y++) { SetPixel(pixels, w, h, x0, y, c); SetPixel(pixels, w, h, x1, y, c); }
        }

        private static void DrawCircle(Color[] pixels, int w, int h, int cx, int cy, int r, Color c)
        {
            for (int y = cy - r; y <= cy + r; y++)
                for (int x = cx - r; x <= cx + r; x++)
                    if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r)
                        SetPixel(pixels, w, h, x, y, c);
        }

        private static void DrawEllipse(Color[] pixels, int w, int h, int cx, int cy, int rx, int ry, Color c)
        {
            for (int y = cy - ry; y <= cy + ry; y++)
                for (int x = cx - rx; x <= cx + rx; x++)
                {
                    float dx = (float)(x - cx) / rx;
                    float dy = (float)(y - cy) / ry;
                    if (dx * dx + dy * dy <= 1f)
                        SetPixel(pixels, w, h, x, y, c);
                }
        }

        private static Sprite MakeSprite(Color[] pixels, int w, int h, Vector2 pivot, float ppu = 32f)
        {
            var tex = new Texture2D(w, h);
            tex.SetPixels(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), pivot, ppu);
        }

        private static int HashCode(string method, Color c, int a, int b)
        {
            unchecked
            {
                int hash = method.GetHashCode();
                hash = hash * 31 + c.GetHashCode();
                hash = hash * 31 + a;
                hash = hash * 31 + b;
                return hash;
            }
        }

        #endregion
    }
}
