using UnityEngine;

public class TileMod : MonoBehaviour
{
    /**
     * Maps neighbours byte to tile position in mod tilemap.
     * The following weights are applied to subject's neighbours:
     * [ 1 ] [ 2 ] [ 4 ]
     * [ 8 ]       [ 16]
     * [ 32] [ 64] [128]
     */
    private static byte[] _modPositions =
    {
        (6 << 4) + 5, //   0
        (6 << 4) + 5, //   1
        (2 << 4) + 0, //   2
        (2 << 4) + 0, //   3
        (6 << 4) + 5, //   4
        (6 << 4) + 5, //   5
        (2 << 4) + 0, //   6
        (2 << 4) + 0, //   7
        (1 << 4) + 0, //   8
        (1 << 4) + 0, //   9
        (2 << 4) + 1, //  10
        (6 << 4) + 0, //  11
        (1 << 4) + 0, //  12
        (1 << 4) + 0, //  13
        (2 << 4) + 1, //  14
        (6 << 4) + 0, //  15
        
        (3 << 4) + 0, //  16
        (3 << 4) + 0, //  17
        (3 << 4) + 1, //  18
        (3 << 4) + 1, //  19
        (3 << 4) + 0, //  20
        (3 << 4) + 0, //  21
        (7 << 4) + 0, //  22
        (7 << 4) + 0, //  23
        (5 << 4) + 5, //  24
        (5 << 4) + 5, //  25
        (2 << 4) + 3, //  26
        (6 << 4) + 2, //  27
        (5 << 4) + 5, //  28
        (5 << 4) + 5, //  29
        (2 << 4) + 2, //  30
        (6 << 4) + 1, //  31
        
        // 5th bit is the bottom left corner
        // the following 32 numbers are the same as elements [0, 31]
        (6 << 4) + 5, //  32
        (6 << 4) + 5, //  33
        (2 << 4) + 0, //  34
        (2 << 4) + 0, //  35
        (6 << 4) + 5, //  36
        (6 << 4) + 5, //  37
        (2 << 4) + 0, //  38
        (2 << 4) + 0, //  39
        (1 << 4) + 0, //  40
        (1 << 4) + 0, //  41
        (2 << 4) + 1, //  42
        (6 << 4) + 0, //  43
        (1 << 4) + 0, //  44
        (1 << 4) + 0, //  45
        (2 << 4) + 1, //  46
        (6 << 4) + 0, //  47
        
        (3 << 4) + 0, //  48
        (3 << 4) + 0, //  49
        (3 << 4) + 1, //  50
        (3 << 4) + 1, //  51
        (3 << 4) + 0, //  52
        (3 << 4) + 0, //  53
        (7 << 4) + 0, //  54
        (7 << 4) + 0, //  55
        (5 << 4) + 5, //  56
        (5 << 4) + 5, //  57
        (2 << 4) + 3, //  58
        (6 << 4) + 2, //  59
        (5 << 4) + 5, //  60
        (5 << 4) + 5, //  61
        (2 << 4) + 2, //  62
        (6 << 4) + 1, //  63
        
        (0 << 4) + 0, //  64
        (0 << 4) + 0, //  65
        (4 << 4) + 5, //  66
        (4 << 4) + 5, //  67
        (0 << 4) + 0, //  68
        (0 << 4) + 0, //  69
        (4 << 4) + 5, //  70
        (4 << 4) + 5, //  71
        (1 << 4) + 1, //  72
        (1 << 4) + 1, //  73
        (1 << 4) + 3, //  74
        (1 << 4) + 2, //  75
        (1 << 4) + 1, //  76
        (1 << 4) + 1, //  77
        (1 << 4) + 3, //  78
        (1 << 4) + 2, //  79
        
        (0 << 4) + 1, //  80
        (0 << 4) + 1, //  81
        (3 << 4) + 3, //  82
        (3 << 4) + 3, //  83
        (0 << 4) + 1, //  84
        (0 << 4) + 1, //  85
        (7 << 4) + 2, //  86
        (7 << 4) + 2, //  87
        (0 << 4) + 3, //  88
        (0 << 4) + 3, //  89
        (7 << 4) + 5, //  90
        (2 << 4) + 5, //  91
        (0 << 4) + 3, //  92
        (0 << 4) + 3, //  93
        (3 << 4) + 5, //  94
        (3 << 4) + 4, //  95
        
        (0 << 4) + 0, //  96
        (0 << 4) + 0, //  97
        (4 << 4) + 5, //  98
        (4 << 4) + 5, //  99
        (0 << 4) + 0, // 100
        (0 << 4) + 0, // 101
        (4 << 4) + 5, // 102
        (4 << 4) + 5, // 103
        (5 << 4) + 0, // 104
        (5 << 4) + 0, // 105
        (5 << 4) + 2, // 106
        (5 << 4) + 1, // 107
        (5 << 4) + 0, // 108
        (5 << 4) + 0, // 109
        (5 << 4) + 2, // 110
        (5 << 4) + 1, // 111
        
        (0 << 4) + 1, // 112
        (0 << 4) + 1, // 113
        (3 << 4) + 3, // 114
        (3 << 4) + 3, // 115
        (0 << 4) + 1, // 116
        (0 << 4) + 1, // 117
        (7 << 4) + 2, // 118
        (7 << 4) + 2, // 119
        (0 << 4) + 2, // 120
        (0 << 4) + 2, // 121
        (1 << 5) + 2, // 122
        (2 << 5) + 4, // 123
        (0 << 4) + 2, // 124
        (0 << 4) + 2, // 125
        (7 << 4) + 4, // 126
        (7 << 4) + 4, // 126
        (7 << 4) + 3, // 127
        
        // 7th bit in the bottom right corner
        // the following 64 numbers are the same as elements [0, 63]
        (6 << 4) + 5, // 128
        (6 << 4) + 5, // 129
        (2 << 4) + 0, // 130
        (2 << 4) + 0, // 131
        (6 << 4) + 5, // 132
        (6 << 4) + 5, // 133
        (2 << 4) + 0, // 134
        (2 << 4) + 0, // 135
        (1 << 4) + 0, // 136
        (1 << 4) + 0, // 137
        (2 << 4) + 1, // 138
        (6 << 4) + 0, // 139
        (1 << 4) + 0, // 140
        (1 << 4) + 0, // 141
        (2 << 4) + 1, // 142
        (6 << 4) + 0, // 143
        
        (3 << 4) + 0, // 144
        (3 << 4) + 0, // 145
        (3 << 4) + 1, // 146
        (3 << 4) + 1, // 147
        (3 << 4) + 0, // 148
        (3 << 4) + 0, // 149
        (7 << 4) + 0, // 150
        (7 << 4) + 0, // 151
        (5 << 4) + 5, // 152
        (5 << 4) + 5, // 153
        (2 << 4) + 3, // 154
        (6 << 4) + 2, // 155
        (5 << 4) + 5, // 156
        (5 << 4) + 5, // 157
        (2 << 4) + 2, // 158
        (6 << 4) + 1, // 159
        
        // repeating [0, 31] here
        (6 << 4) + 5, // 160
        (6 << 4) + 5, // 161
        (2 << 4) + 0, // 162
        (2 << 4) + 0, // 163
        (6 << 4) + 5, // 164
        (6 << 4) + 5, // 165
        (2 << 4) + 0, // 166
        (2 << 4) + 0, // 167
        (1 << 4) + 0, // 168
        (1 << 4) + 0, // 169
        (2 << 4) + 1, // 170
        (6 << 4) + 0, // 171
        (1 << 4) + 0, // 172
        (1 << 4) + 0, // 173
        (2 << 4) + 1, // 174
        (6 << 4) + 0, // 175
        
        (3 << 4) + 0, // 176
        (3 << 4) + 0, // 177
        (3 << 4) + 1, // 178
        (3 << 4) + 1, // 179
        (3 << 4) + 0, // 180
        (3 << 4) + 0, // 181
        (7 << 4) + 0, // 182
        (7 << 4) + 0, // 183
        (5 << 4) + 5, // 184
        (5 << 4) + 5, // 185
        (2 << 4) + 3, // 186
        (6 << 4) + 2, // 187
        (5 << 4) + 5, // 188
        (5 << 4) + 5, // 189
        (2 << 4) + 2, // 190
        (6 << 4) + 1, // 191
        
        // repeating [64, 79] here
        (0 << 4) + 0, // 192
        (0 << 4) + 0, // 193
        (4 << 4) + 5, // 194
        (4 << 4) + 5, // 195
        (0 << 4) + 0, // 196
        (0 << 4) + 0, // 197
        (4 << 4) + 5, // 198
        (4 << 4) + 5, // 199
        (1 << 4) + 1, // 200
        (1 << 4) + 1, // 201
        (1 << 4) + 3, // 202
        (1 << 4) + 2, // 203
        (1 << 4) + 1, // 204
        (1 << 4) + 1, // 205
        (1 << 4) + 3, // 206
        (1 << 4) + 2, // 207
        
        (4 << 4) + 0, // 208
        (4 << 4) + 0, // 209
        (3 << 4) + 2, // 210
        (3 << 4) + 2, // 211
        (4 << 4) + 0, // 212
        (4 << 4) + 0, // 213
        (7 << 4) + 1, // 214
        (7 << 4) + 1, // 215
        (4 << 4) + 2, // 216
        (4 << 4) + 2, // 217
        (0 << 4) + 5, // 218
        (4 << 4) + 4, // 219
        (4 << 4) + 2, // 220
        (4 << 4) + 2, // 221
        (0 << 4) + 4, // 222
        (4 << 4) + 3, // 223
        
        (0 << 4) + 0, // 224
        (0 << 4) + 0, // 225
        (4 << 4) + 5, // 226
        (4 << 4) + 5, // 227
        (0 << 4) + 0, // 228
        (0 << 4) + 0, // 229
        (4 << 4) + 5, // 230
        (4 << 4) + 5, // 231
        (5 << 4) + 0, // 232
        (5 << 4) + 0, // 233
        (5 << 4) + 2, // 234
        (5 << 4) + 1, // 235
        (5 << 4) + 0, // 236
        (5 << 4) + 0, // 237
        (5 << 4) + 2, // 238
        (5 << 4) + 1, // 239
        
        (4 << 4) + 0, // 240
        (4 << 4) + 0, // 241
        (3 << 4) + 2, // 242
        (3 << 4) + 2, // 243
        (4 << 4) + 0, // 244
        (4 << 4) + 0, // 245
        (7 << 4) + 1, // 246
        (7 << 4) + 1, // 247
        (4 << 4) + 1, // 248
        (4 << 4) + 1, // 249
        (1 << 4) + 4, // 250
        (6 << 4) + 3, // 251
        (4 << 4) + 1, // 252
        (4 << 4) + 1, // 253
        (5 << 4) + 3, // 254
        
        // blocks all around: regular render
    };

    /**
     * Returns (x, y) of tile mod in mod tileset based on tile surroundings (neighbors).
     */
    public static Vector2Int GetModPositionFromNeighbors(byte neighbors)
    {
        byte el = _modPositions[neighbors];

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
        Color32[] sourcePixels = source.GetPixels32();
        Color32[] modPixels = source.GetPixels32();
        Color32[] newPixels = new Color32[size.x * size.y];
        Color32 maskColor = new Color32(255, 0, 255, 255);

        int index = 0;
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var modPixel = modPixels[mod.width * (modPosition.y + y) + modPosition.x + x];
                if (!modPixel.Equals(maskColor))
                {
                    newPixels[index] = modPixel;
                }
                else
                {
                    newPixels[index] = sourcePixels[source.width * (sourcePosition.y + y) + sourcePosition.x + x];
                }

                index++;
            }
        }
        
        Texture2D newTexture = new Texture2D(size.x, size.y);
        newTexture.SetPixels32(newPixels);
        newTexture.Apply();
        return newTexture;
    }

    public Texture2D ModTilemap;
    public int ModWidth = 32;
    public int ModHeight = 32;
    
    public void ApplyMod(byte neighbors)
    {
        // if gameObject has AnimatedSprite component, let it handle mod applying
        var animatedSpriteComp = gameObject.GetComponent<AnimatedSprite>();
        if (animatedSpriteComp)
        {
            animatedSpriteComp.Mod(
                ModTilemap,
                GetModPositionFromNeighbors(neighbors) * new Vector2Int(ModWidth, ModHeight));
            return;
        }
        
        // otherwise, just apply mod to SpriteRenderer
        var rendererComp = gameObject.GetComponent<SpriteRenderer>();
        if (rendererComp)
        {
            Sprite s = rendererComp.sprite;
            
            // new texture should have the same size as old one
            Debug.Assert(s.texture.width == ModWidth);
            Debug.Assert(s.texture.height == ModHeight);
            
            Texture2D newTexture = MakeModdedTexture(
                s.texture,
                new Vector2Int((int) s.textureRect.x, (int) s.textureRect.y),
                ModTilemap,
                GetModPositionFromNeighbors(neighbors) * new Vector2Int(ModWidth, ModHeight),
                new Vector2Int(ModWidth, ModHeight));

            newTexture.filterMode = s.texture.filterMode;

            rendererComp.sprite = Sprite.Create(
                newTexture,
                new Rect(0, 0, ModWidth, ModHeight),
                s.pivot,
                s.pixelsPerUnit);
        }
        
        Debug.LogError("TileMod could not find editable sprite");
    }
}