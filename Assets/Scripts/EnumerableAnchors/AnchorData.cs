using UnityEngine;

namespace EnumerableAnchors
{
    public readonly struct AnchorData
    {
        public readonly Vector2 Min;
        public readonly Vector2 Max;
        public readonly Vector2 Pivot;
        public readonly Vector2 Offset;

        public AnchorData(Vector2 min, Vector2 max, Vector2 pivot, Vector2 offset)
        {
            Min = min;
            Max = max;
            Pivot = pivot;
            Offset = offset;
        }
    }
}