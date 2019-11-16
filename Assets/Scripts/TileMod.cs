using System.Collections.Generic;
using Entities;
using UnityEngine;

/**
 * Behaviour adding mods (edges/blobs/bitmasking) to game entities.
 * Credits for a very good explanation:
 * https://gamedevelopment.tutsplus.com/tutorials/how-to-use-tile-bitmasking-to-auto-tile-your-level-layouts--cms-25673
 */
public class TileMod : MonoBehaviour
{
    /**
     * Maps neighbours byte to tile position in mod tilemap.
     * The following weights are applied to entity's neighbours:
     * [ 1 ] [ 2 ] [ 4 ]
     * [ 8 ]       [ 16]
     * [ 32] [ 64] [128]
     * This array could be easily optimised: annotations below describe repeating sequences of integers.
     */
    private static readonly byte[] ModPositions =
    {
        (4 << 4) + 0, //   0
        (4 << 4) + 0, //   1
        (2 << 4) + 5, //   2
        (2 << 4) + 5, //   3
        (4 << 4) + 0, //   4
        (4 << 4) + 0, //   5
        (2 << 4) + 5, //   6
        (2 << 4) + 5, //   7
        (1 << 4) + 5, //   8
        (1 << 4) + 5, //   9
        (2 << 4) + 4, //  10
        (6 << 4) + 5, //  11
        (1 << 4) + 5, //  12
        (1 << 4) + 5, //  13
        (2 << 4) + 4, //  14
        (6 << 4) + 5, //  15

        (3 << 4) + 5, //  16
        (3 << 4) + 5, //  17
        (3 << 4) + 4, //  18
        (3 << 4) + 4, //  19
        (3 << 4) + 5, //  20
        (3 << 4) + 5, //  21
        (7 << 4) + 5, //  22
        (7 << 4) + 5, //  23
        (7 << 4) + 1, //  24
        (7 << 4) + 1, //  25
        (2 << 4) + 2, //  26
        (6 << 4) + 3, //  27
        (7 << 4) + 1, //  28
        (7 << 4) + 1, //  29
        (2 << 4) + 3, //  30
        (6 << 4) + 4, //  31

        // 5th bit is the bottom left corner
        // the following 32 numbers are the same as elements [0, 31]
        (4 << 4) + 0, //  32
        (4 << 4) + 0, //  33
        (2 << 4) + 5, //  34
        (2 << 4) + 5, //  35
        (4 << 4) + 0, //  36
        (4 << 4) + 0, //  37
        (2 << 4) + 5, //  38
        (2 << 4) + 5, //  39
        (1 << 4) + 5, //  40
        (1 << 4) + 5, //  41
        (2 << 4) + 4, //  42
        (6 << 4) + 5, //  43
        (1 << 4) + 5, //  44
        (1 << 4) + 5, //  45
        (2 << 4) + 4, //  46
        (6 << 4) + 5, //  47

        (3 << 4) + 5, //  48
        (3 << 4) + 5, //  49
        (3 << 4) + 4, //  50
        (3 << 4) + 4, //  51
        (3 << 4) + 5, //  52
        (3 << 4) + 5, //  53
        (7 << 4) + 5, //  54
        (7 << 4) + 5, //  55
        (7 << 4) + 1, //  56
        (7 << 4) + 1, //  57
        (2 << 4) + 2, //  58
        (6 << 4) + 3, //  59
        (7 << 4) + 1, //  60
        (7 << 4) + 1, //  61
        (2 << 4) + 3, //  62
        (6 << 4) + 4, //  63

        (0 << 4) + 5, //  64
        (0 << 4) + 5, //  65
        (6 << 4) + 1, //  66
        (6 << 4) + 1, //  67
        (0 << 4) + 5, //  68
        (0 << 4) + 5, //  69
        (6 << 4) + 1, //  70
        (6 << 4) + 1, //  71
        (1 << 4) + 4, //  72
        (1 << 4) + 4, //  73
        (1 << 4) + 2, //  74
        (1 << 4) + 3, //  75
        (1 << 4) + 4, //  76
        (1 << 4) + 4, //  77
        (1 << 4) + 2, //  78
        (1 << 4) + 3, //  79

        (0 << 4) + 4, //  80
        (0 << 4) + 4, //  81
        (3 << 4) + 2, //  82
        (3 << 4) + 2, //  83
        (0 << 4) + 4, //  84
        (0 << 4) + 4, //  85
        (7 << 4) + 3, //  86
        (7 << 4) + 3, //  87
        (0 << 4) + 2, //  88
        (0 << 4) + 2, //  89
        (5 << 4) + 0, //  90
        (2 << 4) + 0, //  91
        (0 << 4) + 2, //  92
        (0 << 4) + 2, //  93
        (3 << 4) + 0, //  94
        (3 << 4) + 1, //  95

        (0 << 4) + 5, //  96
        (0 << 4) + 5, //  97
        (6 << 4) + 1, //  98
        (6 << 4) + 1, //  99
        (0 << 4) + 5, // 100
        (0 << 4) + 5, // 101
        (6 << 4) + 1, // 102
        (6 << 4) + 1, // 103
        (5 << 4) + 5, // 104
        (5 << 4) + 5, // 105
        (5 << 4) + 3, // 106
        (5 << 4) + 4, // 107
        (5 << 4) + 5, // 108
        (5 << 4) + 5, // 109
        (5 << 4) + 3, // 110
        (5 << 4) + 4, // 111

        (0 << 4) + 4, // 112
        (0 << 4) + 4, // 113
        (3 << 4) + 2, // 114
        (3 << 4) + 2, // 115
        (0 << 4) + 4, // 116
        (0 << 4) + 4, // 117
        (7 << 4) + 3, // 118
        (7 << 4) + 3, // 119
        (0 << 4) + 3, // 120
        (0 << 4) + 3, // 121
        (1 << 4) + 0, // 122
        (2 << 4) + 1, // 123
        (0 << 4) + 3, // 124
        (0 << 4) + 3, // 125
        (5 << 4) + 1, // 126
        (7 << 4) + 2, // 127

        // 7th bit in the bottom right corner
        // the following 64 numbers are the same as elements [0, 63]
        (4 << 4) + 0, // 128
        (4 << 4) + 0, // 129
        (2 << 4) + 5, // 130
        (2 << 4) + 5, // 131
        (4 << 4) + 0, // 132
        (4 << 4) + 0, // 133
        (2 << 4) + 5, // 134
        (2 << 4) + 5, // 135
        (1 << 4) + 5, // 136
        (1 << 4) + 5, // 137
        (2 << 4) + 4, // 138
        (6 << 4) + 5, // 139
        (1 << 4) + 5, // 140
        (1 << 4) + 5, // 141
        (2 << 4) + 4, // 142
        (6 << 4) + 5, // 143

        (3 << 4) + 5, // 144
        (3 << 4) + 5, // 145
        (3 << 4) + 4, // 146
        (3 << 4) + 4, // 147
        (3 << 4) + 5, // 148
        (3 << 4) + 5, // 149
        (7 << 4) + 5, // 150
        (7 << 4) + 5, // 151
        (7 << 4) + 1, // 152
        (7 << 4) + 1, // 153
        (2 << 4) + 2, // 154
        (6 << 4) + 3, // 155
        (7 << 4) + 1, // 156
        (7 << 4) + 1, // 157
        (2 << 4) + 3, // 158
        (6 << 4) + 4, // 159

        // repeating [0, 31] here
        (4 << 4) + 0, // 160
        (4 << 4) + 0, // 161
        (2 << 4) + 5, // 162
        (2 << 4) + 5, // 163
        (4 << 4) + 0, // 164
        (4 << 4) + 0, // 165
        (2 << 4) + 5, // 166
        (2 << 4) + 5, // 167
        (1 << 4) + 5, // 168
        (1 << 4) + 5, // 169
        (2 << 4) + 4, // 170
        (6 << 4) + 5, // 171
        (1 << 4) + 5, // 172
        (1 << 4) + 5, // 173
        (2 << 4) + 4, // 174
        (6 << 4) + 5, // 175

        (3 << 4) + 5, // 176
        (3 << 4) + 5, // 177
        (3 << 4) + 4, // 178
        (3 << 4) + 4, // 179
        (3 << 4) + 5, // 180
        (3 << 4) + 5, // 181
        (7 << 4) + 5, // 182
        (7 << 4) + 5, // 183
        (7 << 4) + 1, // 184
        (7 << 4) + 1, // 185
        (2 << 4) + 2, // 186
        (6 << 4) + 3, // 187
        (7 << 4) + 1, // 188
        (7 << 4) + 1, // 189
        (2 << 4) + 3, // 190
        (6 << 4) + 4, // 191

        // repeating [64, 79] here
        (0 << 4) + 5, // 192
        (0 << 4) + 5, // 193
        (6 << 4) + 1, // 194
        (6 << 4) + 1, // 195
        (0 << 4) + 5, // 196
        (0 << 4) + 5, // 197
        (6 << 4) + 1, // 198
        (6 << 4) + 1, // 199
        (1 << 4) + 4, // 200
        (1 << 4) + 4, // 201
        (1 << 4) + 2, // 202
        (1 << 4) + 3, // 203
        (1 << 4) + 4, // 204
        (1 << 4) + 4, // 205
        (1 << 4) + 2, // 206
        (1 << 4) + 3, // 207

        (4 << 4) + 5, // 208
        (4 << 4) + 5, // 209
        (3 << 4) + 3, // 210
        (3 << 4) + 3, // 211
        (4 << 4) + 5, // 212
        (4 << 4) + 5, // 213
        (7 << 4) + 4, // 214
        (7 << 4) + 4, // 215
        (4 << 4) + 3, // 216
        (4 << 4) + 3, // 217
        (0 << 4) + 0, // 218
        (4 << 4) + 1, // 219
        (4 << 4) + 3, // 220
        (4 << 4) + 3, // 221
        (0 << 4) + 1, // 222
        (4 << 4) + 2, // 223

        (0 << 4) + 5, // 224
        (0 << 4) + 5, // 225
        (6 << 4) + 1, // 226
        (6 << 4) + 1, // 227
        (0 << 4) + 5, // 228
        (0 << 4) + 5, // 229
        (6 << 4) + 1, // 230
        (6 << 4) + 1, // 231
        (5 << 4) + 5, // 232
        (5 << 4) + 5, // 233
        (5 << 4) + 3, // 234
        (5 << 4) + 4, // 235
        (5 << 4) + 5, // 236
        (5 << 4) + 5, // 237
        (5 << 4) + 3, // 238
        (5 << 4) + 4, // 239

        (4 << 4) + 5, // 240
        (4 << 4) + 5, // 241
        (3 << 4) + 3, // 242
        (3 << 4) + 3, // 243
        (4 << 4) + 5, // 244
        (4 << 4) + 5, // 245
        (7 << 4) + 4, // 246
        (7 << 4) + 4, // 247
        (4 << 4) + 4, // 248
        (4 << 4) + 4, // 249
        (1 << 4) + 1, // 250
        (6 << 4) + 2, // 251
        (4 << 4) + 4, // 252
        (4 << 4) + 4, // 253
        (5 << 4) + 2, // 254
        (6 << 4) + 0, // 255
    };

    /**
     * Hash map storing already computed textures. It speeds up map updates DRAMATICALLY.
     */
    private static Dictionary<(ulong, ulong), Texture2D> _modCache;

    /**
     * Returns (x, y) of tile mod in mod tileset based on tile surroundings (neighbors).
     */
    private static Vector2Int GetModPositionFromNeighbors(byte neighbors)
    {
        byte el = ModPositions[neighbors];

        int y = el & 0b00001111;
        int x = el >> 4;

        return new Vector2Int(x, y);
    }

    /**
     * Constructs new texture based on mod and source texture. Output texture look like mod, but all #ff00ffff pixels
     * are replaced with corresponding pixels from source image.
     */
    public static Texture2D MakeModdedTexture(Texture2D source, Vector2Int sourcePosition, Texture2D mod, Vector2Int modPosition, Vector2Int size)
    {
        // calculate mod hash to see if it has been generated before
        var modId = (
            ((ulong) source.GetHashCode() << 32) + (ulong) mod.GetHashCode(),
            ((ulong) sourcePosition.x << 48) +
            ((ulong) sourcePosition.y << 32) +
            ((ulong) modPosition.x << 16) +
            ((ulong) modPosition.y));

        if (_modCache.ContainsKey(modId))
        {
            return _modCache[modId];
        }

        // texture pixels are laid out left-to-right, bottom-to-top
        // yes, bottom-to-top (Oo)

        Color32[] sourcePixels = source.GetPixels32();
        Color32[] modPixels = mod.GetPixels32();
        Color32[] newPixels = new Color32[size.x * size.y];
        Color32 maskColor = new Color32(255, 0, 255, 255);

        int index = 0;
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var modPixel = modPixels[mod.width * (modPosition.y + y) + modPosition.x + x];

                newPixels[index++] = modPixel.Equals(maskColor)
                    ? sourcePixels[source.width * (sourcePosition.y + y) + sourcePosition.x + x]
                    : modPixel;
            }
        }

        Texture2D newTexture = new Texture2D(size.x, size.y);
        newTexture.filterMode = source.filterMode;
        newTexture.SetPixels32(newPixels);
        newTexture.Apply();
        _modCache.Add(modId, newTexture);
        return newTexture;
    }

    /**
     * Resets mod map. You can call it when a map changes to save memory.
     */
    public static void ClearModCache()
    {
        _modCache = new Dictionary<(ulong, ulong), Texture2D>();
    }

    public Texture2D modTilemap;
    public int modWidth = 32;
    public int modHeight = 32;

    void Start()
    {
        var entity = gameObject.GetComponent<Entity>();
        var map = gameObject.GetComponentInParent<Map>();
        ApplyMod(map.CollectNeighborsByte(entity, true));

        if (_modCache == null)
        {
            _modCache = new Dictionary<(ulong, ulong), Texture2D>();
        }
    }

    /**
     * Applies mod on game object. If object has animated texture, mod applying is handled by that component.
     * Objects with sprite renderer are updated in this function.
     */
    public void ApplyMod(byte neighbors)
    {
        // if gameObject has AnimatedSprite component, let it handle mod applying
        var animatedSpriteComp = gameObject.GetComponent<AnimatedSprite>();
        if (animatedSpriteComp)
        {
            animatedSpriteComp.Mod(
                modTilemap,
                GetModPositionFromNeighbors(neighbors) * new Vector2Int(modWidth, modHeight));
            return;
        }

        // otherwise, just apply mod to SpriteRenderer
        var rendererComp = gameObject.GetComponent<SpriteRenderer>();
        if (rendererComp)
        {
            Sprite s = rendererComp.sprite;

            // new texture should have the same size as old one
            Debug.Assert((int)s.textureRect.width == modWidth);
            Debug.Assert((int)s.textureRect.height == modHeight);

            Texture2D newTexture = MakeModdedTexture(
                s.texture,
                new Vector2Int((int) s.textureRect.x, (int) s.textureRect.y),
                modTilemap,
                GetModPositionFromNeighbors(neighbors) * new Vector2Int(modWidth, modHeight),
                new Vector2Int(modWidth, modHeight));

            newTexture.filterMode = s.texture.filterMode;

            rendererComp.sprite = Sprite.Create(
                newTexture,
                new Rect(0, 0, modWidth, modHeight),
                s.pivot,
                s.pixelsPerUnit);
        }

        Debug.LogError("TileMod could not find editable sprite");
    }
}
