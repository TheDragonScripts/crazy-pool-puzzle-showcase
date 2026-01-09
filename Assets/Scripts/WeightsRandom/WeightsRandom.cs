using System;

namespace WeightedRandomization
{
    public static class WeightsRandom
    {
        public static T Pick<T>(WeightedItem<T>[] weights)
        {
            if (weights == null || weights.Length == 0)
            {
                throw new ArgumentNullException("Weights array is null or empty");
            }

            float totalWeightsSum = 0;
            foreach (WeightedItem<T> weighted in weights)
            {
                totalWeightsSum += weighted.Weight;
            }

            float[] normalizedWeights = new float[weights.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                normalizedWeights[i] = weights[i].Weight / totalWeightsSum;
            }

            float randValue = UnityEngine.Random.Range(0f, 1f);
            float totalNormalizedWeightsSum = 0;
            for (int i = 0; i < normalizedWeights.Length; i++)
            {
                float weight = normalizedWeights[i];
                totalNormalizedWeightsSum += weight;
                if (totalNormalizedWeightsSum >= randValue)
                {
                    return weights[i].Item;
                }
            }
            throw new InvalidOperationException($"Weights random can't pick any value. " +
                $"{nameof(totalNormalizedWeightsSum)} = {totalNormalizedWeightsSum} " +
                $"{nameof(randValue)} = {randValue}");
        }
    }
}