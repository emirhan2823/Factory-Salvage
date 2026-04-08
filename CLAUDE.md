# Factory Salvage

## Proje Aciklamasi
Mini automation + roguelite hybrid mobil oyun. Hurda bir tesisi ayaga kaldiriyorsun:
parcalari topluyorsun, makineler kuruyorsun, uretim akisi yapiyorsun, enerji yonetiyorsun,
raid/wave olaylarina dayaniyorsun.

## MVP Kapsami
- 4 kaynak tipi
- Conveyor benzeri uretim akisi
- Enerji sistemi
- 1 kucuk map
- Saldiri dalgalari (wave defense)
- Us upgrade sistemi

---

## Tech Stack
- Engine: Unity 2022.3+ LTS
- Render: URP 2D
- Language: C# (.NET Standard 2.1)
- Input: New Input System (touch-first)
- Build: Android (iOS sonra)

## Namespace Kurallari
- Tum script'ler `FactorySalvage.*` namespace'i altinda
- Alt namespace'ler: `.Core`, `.UI`, `.Gameplay`, `.Audio`, `.Data`

## Mimari Kurallar
- **ScriptableObject Pattern**: Tum config ve data SO olarak
- **Event Bus**: `GameEvents` static class uzerinden, UnityEvent veya C# event
- **Object Pooling**: Sik spawn edilen objelerde ZORUNLU
- **MonoBehaviour Lifecycle**: Awake → OnEnable → Start sirasi kritik
- **Singleton YASAK**: ServiceLocator veya SO-based referans kullan

## Klasor Yapisi
```
Assets/
  _Scripts/        # Tum C# kodlari
    Core/          # Singleton-free managers, events
    Gameplay/      # Player, enemies, mechanics
    UI/            # Canvas, panels, HUD
    Audio/         # Audio manager, SFX triggers
    Data/          # ScriptableObjects, save system
  _Data/           # SO asset'leri
  _Prefabs/        # Prefab'lar
  _Scenes/         # Scene dosyalari
  _Art/            # Sprite, animation, tilemap
  _Audio/          # AudioClip dosyalari
```

## Mobil Performans Kurallari
- Texture: Max 1024x1024, compressed (ASTC)
- Draw call: Frame basina < 50
- GC allocation: Frame icinde 0 olmali (cache everything)
- Object pool: Bullet, particle, enemy
- Update(): Mumkunse coroutine veya DOTween ile degistir
- String concatenation YASAK (StringBuilder kullan)

## Kodlama Kurallari
- `[SerializeField] private` tercih et, public field YASAK
- her script'te `#region` kullan: Fields, Properties, Unity Callbacks, Public Methods, Private Methods
- MonoBehaviour olmayan logic ayri class'lara tasi (testability)
- Touch input: `Input.GetTouch` + `EventSystem.current.IsPointerOverGameObject`

## Otonom Mod
- .env ve PlayerPrefs'e secret yazma
- Her feature sonrasi commit at
- Scene degisikliklerini ayri commit at
- Test: PlayMode test zorunlu (EditMode opsiyonel)
