using UnityEngine;

namespace FactorySalvage.Core
{
    /// <summary>
    /// Game-wide color palette. All visual elements reference these colors.
    /// Inspired by: Grow Castle (cute/colorful) + Idle Miner (warm underground) + modern flat design.
    /// </summary>
    public static class ColorPalette
    {
        // ── Sky ──
        public static readonly Color SkyTop = Hex("#0B1026");
        public static readonly Color SkyBottom = Hex("#5B8FB9");
        public static readonly Color CloudWhite = Hex("#D4E6F1");
        public static readonly Color CloudGray = Hex("#A9CCE3");
        public static readonly Color MountainFar = Hex("#2C3E6B");
        public static readonly Color MountainNear = Hex("#1A2744");

        // ── Surface ──
        public static readonly Color GrassTop = Hex("#5DAE4B");
        public static readonly Color GrassBody = Hex("#4A8C3F");
        public static readonly Color DirtLight = Hex("#8B7355");
        public static readonly Color DirtDark = Hex("#6B4E2E");
        public static readonly Color PathTan = Hex("#C4A66B");

        // ── Underground Layer 1: Shallow Mine ──
        public static readonly Color Earth1Base = Hex("#8B6B47");
        public static readonly Color Earth1Light = Hex("#A68B6B");
        public static readonly Color Earth1Dark = Hex("#6B4F33");
        public static readonly Color RootBrown = Hex("#5A3D2B");

        // ── Underground Layer 2: Deep Mine ──
        public static readonly Color Rock2Base = Hex("#5A4A3A");
        public static readonly Color Rock2Light = Hex("#7A6A5A");
        public static readonly Color Rock2Dark = Hex("#3A2A1A");
        public static readonly Color CrystalBlue = Hex("#6BC5E8");
        public static readonly Color CrystalPurple = Hex("#9B7BD4");

        // ── Underground Layer 3: Lava Core ──
        public static readonly Color Lava3Base = Hex("#2A1A1A");
        public static readonly Color LavaOrange = Hex("#FF6B2C");
        public static readonly Color LavaYellow = Hex("#FFD93D");
        public static readonly Color EmberRed = Hex("#FF3D1F");
        public static readonly Color LavaDark = Hex("#1A0A0A");

        // ── Buildings ──
        public static readonly Color WoodBrown = Hex("#8B6B47");
        public static readonly Color WoodLight = Hex("#B8976A");
        public static readonly Color WoodDark = Hex("#5A3D2B");

        public static readonly Color StoneGray = Hex("#8899AA");
        public static readonly Color StoneLight = Hex("#AABBCC");
        public static readonly Color StoneDark = Hex("#667788");

        public static readonly Color FurnaceOrange = Hex("#E8712A");
        public static readonly Color FurnaceLight = Hex("#FF9B5A");
        public static readonly Color FurnaceDark = Hex("#B85A1A");

        public static readonly Color TowerGreen = Hex("#3D8B4A");
        public static readonly Color TowerLight = Hex("#5AAE6A");
        public static readonly Color TowerDark = Hex("#2A6B33");

        // ── Enemies ──
        public static readonly Color EnemyBase = Hex("#CC3344");
        public static readonly Color EnemyLight = Hex("#FF5566");
        public static readonly Color EnemyDark = Hex("#992233");
        public static readonly Color EnemyFast = Hex("#9944CC");
        public static readonly Color EnemyFastLight = Hex("#BB66EE");
        public static readonly Color EnemyBoss = Hex("#661122");
        public static readonly Color EnemyEye = Hex("#FFFFFF");

        // ── Projectiles ──
        public static readonly Color ProjectileYellow = Hex("#FFD700");
        public static readonly Color ProjectileCore = Hex("#FFFFAA");
        public static readonly Color MuzzleFlash = Hex("#FFFFFF");

        // ── Resources ──
        public static readonly Color ResWood = Hex("#8B6B47");
        public static readonly Color ResIron = Hex("#9AACBB");
        public static readonly Color ResSteel = Hex("#5577AA");
        public static readonly Color ResGold = Hex("#FFD700");

        // ── UI ──
        public static readonly Color PanelBg = Hex("#1A1A2E");
        public static readonly Color PanelBorder = Hex("#3A3A5E");
        public static readonly Color ButtonGreen = Hex("#2E6E2E");
        public static readonly Color ButtonGreenHover = Hex("#3E8E3E");
        public static readonly Color ButtonRed = Hex("#6E2E2E");
        public static readonly Color ButtonRedHover = Hex("#8E3E3E");
        public static readonly Color ButtonDisabled = Hex("#4A4A4A");
        public static readonly Color TextWhite = Hex("#F0F0F0");
        public static readonly Color TextMuted = Hex("#888899");
        public static readonly Color TextGold = Hex("#FFD700");
        public static readonly Color HealthGreen = Hex("#44CC44");
        public static readonly Color HealthRed = Hex("#CC4444");
        public static readonly Color HealthBg = Hex("#222222");

        // ── Slot Indicators ──
        public static readonly Color SlotEmpty = new(1f, 1f, 1f, 0.12f);
        public static readonly Color SlotHover = new(0.3f, 1f, 0.3f, 0.3f);
        public static readonly Color SlotDefense = new(1f, 0.3f, 0.3f, 0.15f);

        // ── Helpers ──

        public static Color Darken(Color c, float amount)
        {
            return new Color(
                Mathf.Max(0, c.r - amount),
                Mathf.Max(0, c.g - amount),
                Mathf.Max(0, c.b - amount),
                c.a);
        }

        public static Color Lighten(Color c, float amount)
        {
            return new Color(
                Mathf.Min(1, c.r + amount),
                Mathf.Min(1, c.g + amount),
                Mathf.Min(1, c.b + amount),
                c.a);
        }

        public static Color WithAlpha(Color c, float alpha)
        {
            return new Color(c.r, c.g, c.b, alpha);
        }

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var color);
            return color;
        }
    }
}
