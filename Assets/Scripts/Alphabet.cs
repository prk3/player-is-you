using System;
using UnityEngine;

public class Alphabet
{
    /**
     * Holds alphabet texture. Will be initialized in GetTexture.
     */
    private static Texture2D _texture;
    
    /**
     * Array mapping ascii codes to character positions and widths.
     * First 32 characters are skipped (control codes should not be shown anyways).
     * Character is not defined if value = 0.
     * First 11 bits store x offset (0 - 2047).
     * Remaining 5 bits store character width - 1 (1-32)
     */
    private static readonly ushort[] PositionsAndWidths =
    {
        0, // SPACE
        852 << 5 | 12, // !
        0, // "
        0, // #
        0, // $
        0, // %
        0, // &
        0, // '
        0, // (
        0, // )
        0, // *
        0, // +
        894 << 5 | 10, // ,
        0, // -
        884 << 5 | 10, // .
        0, // /
        612 << 5 | 24, // 0
        636 << 5 | 24, // 1
        660 << 5 | 24, // 2
        684 << 5 | 24, // 3
        708 << 5 | 24, // 4
        732 << 5 | 24, // 5
        756 << 5 | 24, // 6
        780 << 5 | 24, // 7
        804 << 5 | 24, // 8
        828 << 5 | 24, // 9
        0, // :
        0, // ;
        0, // <
        0, // =
        0, // >
        864 << 5 | 20, // ?
        0, // @
          0      | 24, // A
         24 << 5 | 24, // B
         48 << 5 | 24, // C
         72 << 5 | 24, // D
         96 << 5 | 24, // E
        120 << 5 | 24, // F
        144 << 5 | 24, // G
        168 << 5 | 24, // H
        192 << 5 | 10, // I
        204 << 5 | 20, // J
        224 << 5 | 24, // K
        248 << 5 | 20, // L
        268 << 5 | 30, // M
        298 << 5 | 24, // N
        322 << 5 | 24, // O
        346 << 5 | 20, // P
        366 << 5 | 24, // Q
        390 << 5 | 24, // R
        414 << 5 | 24, // S
        438 << 5 | 24, // T
        462 << 5 | 24, // U
        486 << 5 | 24, // V
        510 << 5 | 30, // W
        540 << 5 | 24, // X
        564 << 5 | 24, // Y
        588 << 5 | 24, // Z
        0, // [
        0, // \
        0, // ]
        0, // ^
        0, // _
        0, // `
          0      | 24, // a
         24 << 5 | 24, // b
         48 << 5 | 24, // c
         72 << 5 | 24, // d
         96 << 5 | 24, // e
        120 << 5 | 24, // f
        144 << 5 | 24, // g
        168 << 5 | 24, // h
        192 << 5 | 10, // i
        204 << 5 | 20, // j
        224 << 5 | 24, // k
        248 << 5 | 20, // l
        268 << 5 | 30, // m
        298 << 5 | 24, // n
        322 << 5 | 24, // o
        346 << 5 | 20, // p
        366 << 5 | 24, // q
        390 << 5 | 24, // r
        414 << 5 | 24, // s
        438 << 5 | 24, // t
        462 << 5 | 24, // u
        486 << 5 | 24, // v
        510 << 5 | 30, // w
        540 << 5 | 24, // x
        564 << 5 | 24, // y
        588 << 5 | 24, // z
        0, // {
        0, // |
        0, // }
        0, // ~
        0, // DEL
    };
    
    /*
     * Returns alphabet texture.
     */
    public static Texture2D GetTexture()
    {
        if (_texture != null) return _texture;
        _texture = Resources.Load<Texture2D>("Textures/alphabet");
        _texture.filterMode = FilterMode.Point;

        return _texture;
    }

    /**
     * Returns position of given character in alphabet texture.
     * Null is returned when character doesn't exist in the texture.
     */
    public static Tuple<int, int> PositionAndWidthOf(char c)
    {
        if (c < 32 || c > 127) return null;
        ushort data = PositionsAndWidths[c - 32];
        return new Tuple<int, int>(data >> 5, data & 31);
    }
}