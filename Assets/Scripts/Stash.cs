using DG.Tweening;
using EntryPoint;
using EntryPoint.GameState;
using EntryPoint.Levels;
using PlayerInputs.ObjectsPicker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeightedRandomization;

public class NewOnBoardPositionSide
{
    public Transform NewOnBoardBallPosition;
    public BallSpawnArea BallSpawnArea;
}
/*
* Code here is temporary disabled because it references old UI and Audio system.
* Pending refactoring.
*/
public class Stash : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private List<Transform> _visibleBallsPositions;
    [SerializeField] private Transform _bigOneBallPosition;
    [SerializeField] private Transform _invisibleBallPosition;
    [SerializeField] private Transform _newOnBoardBallPosition;
    [SerializeField] private Transform _newOnBoardBallPositionFlipped;
    [Space(5f)]
    [Header("References")]
    [SerializeField] private BallSpawnArea _ballSpawnArea;
    [SerializeField] private BallSpawnArea _ballSpawnAreaFlipped;
    [Space(5f)]
    [Header("Settings")]
    [SerializeField] private float _ballAnimationSpeed = 0.5f;
    [SerializeField] private float _ballAnimationDelay = 0.5f;
    [Space(5f)]
    [Header("Random balls modificator")]
    [SerializeField, Range(1, 20)] private int _minBallsToSpawn = 3;
    [SerializeField, Range(1, 20)] private int _maxBallsToSpawn = 8;
    [SerializeField] private WeightedItem<GameObject>[] _availableRandomBalls;

    private List<BallController> _balls = new List<BallController>();
    private BallController _currentBigOneBall;
    private Vector3 _prevBigOneBallPos;
    private bool _isBigBallInAnim;
    private GameBoard _gameBoard;
    private ILevelManager _levelManager;
    private IGlobalGameState _globalGameState;

    public BallController CurrentBigOneBall => _currentBigOneBall;
    public Transform InsisibleBallPosition => _invisibleBallPosition;

    private void OnDestroy()
    {
        GameEntryPoint.Instance.RaycastObjectPicker.SceneObjectPicked -= TryToHandleBallClick;
        GameEntryPoint.Instance.SwipeController.SwipeUp -= ReturnBigBallToStash;
        GameEntryPoint.Instance.SwipeController.SwipeDown -= SpawnNewBall;
        _levelManager.LevelBeginChanged -= KillAllBallsTweens;
    }

    private void Start()
    {
        GameEntryPoint.Instance.RaycastObjectPicker.SceneObjectPicked += TryToHandleBallClick;
        GameEntryPoint.Instance.SwipeController.SwipeUp += ReturnBigBallToStash;
        GameEntryPoint.Instance.SwipeController.SwipeDown += SpawnNewBall;
        _levelManager = GameEntryPoint.Instance.LevelManager;
        _globalGameState = GameEntryPoint.Instance.GlobalGameState;
        _levelManager.LevelBeginChanged += KillAllBallsTweens;

        _gameBoard = FindFirstObjectByType<GameBoard>();
        _gameBoard.OnGameBoardFlipped += HandleNewOnBoardPositions;
        HandleNewOnBoardPositions();
        if (_gameBoard == null) throw new NullReferenceException("Stash can not find game board on level");

        StartCoroutine(WaitForDataInitialized());
    }

    private void OnValidate()
    {
        UpdateBalls();
    }

    public void ClearStash()
    {
        _balls.Clear();
    }

    public BallController[] GetBalls()
    {
        return _balls.ToArray();
    }

    public void AddNewBall(BallController ball)
    {
        ball.transform.SetParent(transform, true);
        ball.SetBallStatus(BallGameStatus.OnStash);
        _balls.Add(ball);
        MoveBallsToRight();
    }

    private IEnumerator WaitForDataInitialized()
    {
        yield return ActualPlayerData.Data != null;
        _visibleBallsPositions.Reverse();

        if (ActualPlayerData.Data.RandomBallsInStash == 1
            && _levelManager.IsPlayableLevel(_levelManager.CurrentLevel))
            SpawnRandomBalls();

        UpdateBalls();
        MoveBallsToRight();
    }

    private void SpawnRandomBalls()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }

        transform.DetachChildren();

        int ballsToSpawn = UnityEngine.Random.Range(_minBallsToSpawn, _maxBallsToSpawn);
        for (int i = 0; i < ballsToSpawn; i++)
        {
            GameObject item = WeightsRandom.Pick(_availableRandomBalls);
            GameObject ball = Instantiate(item);
            ball.transform.parent = transform;
        }
    }

    private void KillAllBallsTweens(int level)
    {
        for (int i = 0; i < 3; i++)
        {
            if (_balls.ElementAtOrDefault(i) == null) continue;
            BallController ball = _balls[i];
            Transform ballTransform = _balls[i].gameObject.transform;
            ballTransform.DOKill();
        }
    }

    private void HandleNewOnBoardPositions()
    {
        MeshRenderer meshRenderer_Normal;
        _ballSpawnArea.gameObject.TryGetComponent(out meshRenderer_Normal);
        meshRenderer_Normal.enabled = !_gameBoard.IsFlipped;

        MeshRenderer meshRenderer_Flipped;
        _ballSpawnAreaFlipped.gameObject.TryGetComponent(out meshRenderer_Flipped);
        meshRenderer_Flipped.enabled = _gameBoard.IsFlipped;
    }

    private NewOnBoardPositionSide ChooseNewOnBoardBallPosition()
    {
        NewOnBoardPositionSide answer = new NewOnBoardPositionSide();
        if (_gameBoard.IsFlipped)
        {
            answer.NewOnBoardBallPosition = _newOnBoardBallPositionFlipped;
            answer.BallSpawnArea = _ballSpawnAreaFlipped;
        }
        else
        {
            answer.NewOnBoardBallPosition = _newOnBoardBallPosition;
            answer.BallSpawnArea = _ballSpawnArea;
        }
        return answer;
    }

    private void SpawnNewBall()
    {
        if (_currentBigOneBall == null || _isBigBallInAnim) return;
        Transform bigOneBallTransform = _currentBigOneBall.gameObject.transform;
        NewOnBoardPositionSide newOnBoardPositionSide = ChooseNewOnBoardBallPosition();
        if (newOnBoardPositionSide.BallSpawnArea.IsAreaTaken)
        {
            // Add reaction to taken area
            return;
        }
        Action afterAnimAction = () =>
        {
            bigOneBallTransform.parent = null;
            _currentBigOneBall.SetBallStatus(BallGameStatus.OnBoard);
            //AudioManager.Instance.PlaceNewBallSFX.CopyAndPlay(Vector3.zero);
            _balls.Remove(_currentBigOneBall);
            _currentBigOneBall = null;
            _isBigBallInAnim = false;
            MoveBallsToRight();
        };
        _isBigBallInAnim = true;
        bigOneBallTransform.DOMove(newOnBoardPositionSide.NewOnBoardBallPosition.position, _ballAnimationSpeed)
            .OnComplete(() => afterAnimAction());
    }

    private void ReturnBigBallToStash()
    {
        if (_currentBigOneBall == null || _isBigBallInAnim) return;
        Transform bigOneBallTransform = _currentBigOneBall.gameObject.transform;
        _isBigBallInAnim = true;
        Action afterAnimAction = () =>
        {
            _currentBigOneBall = null;
            _isBigBallInAnim = false;
        };
        bigOneBallTransform.DOMove(_prevBigOneBallPos, _ballAnimationSpeed)
            .OnComplete(() => afterAnimAction());
    }

    private void TryToHandleBallClick(PickedObjectInfo pickedObjectInfo)
    {
        /*if (_globalGameState.Status != GameStatus.Level) return;
        if (UIManager.Instance == null || !UIManager.Instance.IsUIOpenedNow("PauseButtonUI")
            || UIManager.Instance.TutorialFrame.IsTutorialActive) return;
        if (pickedObjectInfo.PerformedButtonAction == MouseButtonAction.Pressed || _currentBigOneBall != null) return;
        BallController ball = pickedObjectInfo.GameObject.GetComponent<BallController>();
        if (ball == null || ball.GameStatus != BallGameStatus.OnStash) return;
        _currentBigOneBall = ball;
        Transform ballTransform = ball.gameObject.transform;
        _prevBigOneBallPos = ballTransform.position;
        ballTransform.DOMove(_bigOneBallPosition.position, _ballAnimationSpeed);*/
    }

    private void MoveBallsToRight()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_balls.ElementAtOrDefault(i) == null) continue;
            BallController ball = _balls[i];
            Transform ballTransform = _balls[i].gameObject.transform;
            ballTransform.DOMoveX(_visibleBallsPositions[i].position.x, _ballAnimationSpeed).SetDelay(i*_ballAnimationDelay);
        }
    }

    private void UpdateBalls()
    {
        _balls.Clear();
        transform.position = _invisibleBallPosition.position;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform ball = transform.GetChild(i);
            BallController ballController = ball.GetComponentInChildren<BallController>();
            ball.position = _invisibleBallPosition.position;
            ballController.SetBallStatus(BallGameStatus.OnStash);
            _balls.Add(ballController);
        }
    }
}
