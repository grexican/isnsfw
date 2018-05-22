using ServiceStack.DataAnnotations;

namespace IsNsfw.Model
{
    [EnumAsInt]
    public enum LinkEventType : byte
    {
        View,
        Preview,
        ClickThrough,
        TurnBack
    }
}
