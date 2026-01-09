using UnityEngine;

namespace InteractiveTutorial
{
    /// <summary>
    /// Tutorial reference object using to provide all needed data for
    /// interactive tutorial actions correct preformance.
    /// </summary>
    /// <remarks>
    /// Class that implements this interface supposed to be inherited from
    /// <see cref="MonoBehaviour"/>.
    /// </remarks>
    public interface ITutorialReferenceObject
    {
        string UniqueID { get; }
        Transform Transform { get; }
        BallController BallController { get; }
    }
}