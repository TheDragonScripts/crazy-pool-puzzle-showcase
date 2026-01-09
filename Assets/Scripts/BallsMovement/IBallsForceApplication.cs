using System;

namespace BallsMovement
{
    public interface IBallsForceApplication
    {
        event Action ForceApplied;
    }
}