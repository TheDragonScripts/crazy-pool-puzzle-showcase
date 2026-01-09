using Balls.Management;
using EntryPoint;
using SpecialBalls;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public enum BallGameStatus
{
    OnStash,
    OnBoard,
}

public enum BallColoring
{
    Coloured,
    Uncoloured,
}

public enum BallSide
{
    Red,
    Blue,
}

/*
 * Code here is temporary disabled because it references old UI and Audio system.
 * Pending refactoring. And the whole class needs be changed.
 */
public class BallController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallAppearance _appearance;
    [SerializeField] private Rigidbody _rigidbody;
    [Space(5f)]
    [Header("Settings")]
    [Tooltip("Add one of the special ball class component to object and set it here")]
    [SerializeField] private Component _speicalBall;
    [SerializeField] private BallGameStatus _gameStatus = BallGameStatus.OnBoard;
    [SerializeField] private BallColoring _coloring = BallColoring.Coloured;
    [SerializeField] private BallSide _side = BallSide.Red;
    [SerializeField] private float _passableImpulseToPlayHitSound = 1f;

    private bool _isImpactBall;
    private IBallsRegistry _ballsRegistry;

    public ISpecialBall SpeicalBall => _speicalBall as ISpecialBall;
    public BallGameStatus GameStatus => _gameStatus;
    public BallColoring Coloring => _coloring;
    public BallSide Side => _side;

    private void Start()
    {
        _ballsRegistry = new BallsRegistry();
        _ballsRegistry.RegisterBall(this);
    }

    private void OnDestroy()
    {
        _ballsRegistry.RemoveBall(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        SpeicalBall.HandleCollision(collision);
        HandleSoundOnCollision(collision);
    }

    public bool CanCountAsWinnable() => SpeicalBall.CanCountAsWinnable();

    public bool CanBeMovedByMouse()
    {
        return SpeicalBall.CanBeMovedByMouse();
    }

    public void DropVelocity()
    {
        if (_rigidbody.isKinematic) return;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    public void ApplyForce(Vector3 force)
    {
        if (CanBeMovedByMouse())
        {
            _isImpactBall = true;
            _rigidbody.AddForce(force);
        }
    }

    public void SetBallStatus(BallGameStatus status)
    {
        _gameStatus = status;
        UpdateBehavioir();
    }

    public void SetBallColoring(BallColoring coloring)
    {
        if (!SpeicalBall.CanBeColoured() || _coloring == coloring) return;
        _coloring = coloring;
        _appearance.SetMaterial(coloring);
        if (_coloring == BallColoring.Coloured)
            AskForCompletionCheck();
    }

    public void SetBallSide(BallSide side)
    {
        _side = side;
        UpdateBehavioir();
    }

    private void UpdateBehavioir()
    {
        _rigidbody.isKinematic = (_gameStatus == BallGameStatus.OnStash) ? true : false;
        _appearance.SetCastingShadows((_gameStatus == BallGameStatus.OnStash) ? false : true);
    }

    private void HandleSoundOnCollision(Collision collision)
    {
        if (!_isImpactBall) return;
        if (_gameStatus == BallGameStatus.OnStash) return;
        if (!IsImpulseEnoughToHitSound(collision.impulse)) return;
        if (collision.gameObject.GetComponent<BallController>() == null) return;
        //AudioManager.Instance.BallHitSFX.CopyAndPlay(collision.transform.position);
        _isImpactBall = false;
    }

    private bool IsImpulseEnoughToHitSound(Vector3 impulse)
    {
        float x = Mathf.Abs(impulse.x);
        float z = Mathf.Abs(impulse.z);
        return x > _passableImpulseToPlayHitSound || z > _passableImpulseToPlayHitSound;
    }

    private void AskForCompletionCheck()
    {
        GameEntryPoint.Instance?.GameRuler?.CheckForCompletion(this);
    }
}
