using Balls.Management;
using BallsMovement;
using EntryPoint.GameData;
using EntryPoint.GameRuler;
using EntryPoint.GameState;
using EntryPoint.Levels;
using ModificatedUISystem;
using PlayerInputs.ObjectsPicker;
using PlayerInputs.Swipes;

namespace EntryPoint
{
    public interface IGameEntryPoint
    {
        IGameSaverAndLoader GameSaverAndLoader { get; }
        ILevelSession LevelSession { get; }
        IGlobalGameState GlobalGameState { get; }
        ISwipeController SwipeController { get; }
        IBallsForceApplication BallsForceApplication { get; }
        IRaycastObjectPicker RaycastObjectPicker { get; }
        ILevelManager LevelManager { get; }
        IGameRuler GameRuler { get; }
        IBallsRegistry BallsRegistry { get; }
        IUIFactory UIFactory { get; }
    }
}