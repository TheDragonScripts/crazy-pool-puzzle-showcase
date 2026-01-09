using System;

namespace WeightedRandomization
{
    [Serializable]
    public class WeightedItem<T>
    {
        public T Item;
        public float Weight;
    }
}
