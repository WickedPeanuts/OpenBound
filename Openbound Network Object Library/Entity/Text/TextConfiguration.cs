using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;

namespace OpenBound_Network_Object_Library.Entity.Text
{
    public enum Alignment
    {
        Center, Left, Right
    }

    public enum FontTextType
    {

        // Font Family Masks
        FONT_FAMILY_MASK = 0b1111_0000_0000_0000,

        NotoSans10Family = 0b0001_0000_0000_0000,
        Consolas10Family = 0b0010_0000_0000_0000,

        // Localized fonts
        Consolas10 =
            Consolas10Family + 0b0000_0000_0000_0001,

        NotoSans10 = 
            NotoSans10Family + 0b0000_0000_0000_0001,
        NotoSans10Thai = 
            NotoSans10Family + 0b0000_0000_0000_0010,
        NotoSans10KR = 
            NotoSans10Family + 0b0000_0000_0000_0011,

        Arial12 = 1,
        Consolas10Bold = 3,
        Consolas11 = 4,
        Consolas14 = 5,
        Consolas16 = 6,
        FontAwesome10 = 7,
        FontAwesome11 = 8,
    }

    public class FontFamilyMetadata
    {
        public Vector2 ElementHeightOffset { get; set; }

        public readonly static FrozenDictionary<FontTextType, IReadOnlyList<FontTextType>> FontFamilyInstances = new Dictionary<FontTextType, IReadOnlyList<FontTextType>>()
        {
            { FontTextType.Consolas10Family, [ FontTextType.Consolas10 ] },
            { FontTextType.NotoSans10Family, [ FontTextType.NotoSans10, FontTextType.NotoSans10KR, FontTextType.NotoSans10Thai ] }
        }.ToFrozenDictionary();

        private static Dictionary<FontTextType, FontFamilyMetadata> metadataInformation = new()
        {
            {
                FontTextType.NotoSans10,
                new FontFamilyMetadata {
                    ElementHeightOffset = new Vector2(0, -2),
                }
            },
            {
                FontTextType.NotoSans10Thai,
                new FontFamilyMetadata {
                    ElementHeightOffset = new Vector2(0, -1),
                }
            },
            {
                FontTextType.NotoSans10KR,
                new FontFamilyMetadata {
                    ElementHeightOffset = new Vector2(0, -2),
                }
            },
        };

        private static FontFamilyMetadata genericMetadata = new()
        {
            ElementHeightOffset = Vector2.Zero
        };

        public static FontFamilyMetadata GetMetadata(FontTextType fontType)
        {
            if (metadataInformation.TryGetValue(fontType, out FontFamilyMetadata metadata))
                return metadata;

            return genericMetadata;
        }
    }
}
