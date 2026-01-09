using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumerableAnchors;

namespace InteractiveTutorial
{
    public class BallUnavailableStepBackCondition : IStepBackCondition
    {
        private BallController _target;
        private List<BallStepBackInfo> _ballsInfo;
        private BallController[] _stashedBalls;
        private Stash _stash;
        private InteractiveTutorialMessage _messageReference;
        private InteractiveTutorialMessage _temporaryMessage;
        private string _stringReference;
        private float _messageLifeTime;
        private EnumerableAnchor _anchor;

        public List<BallStepBackInfo> BallsInfo => _ballsInfo;

        public BallUnavailableStepBackCondition(BallController targetBall, Stash stash,
            InteractiveTutorialMessage interactiveMessage, EnumerableAnchor anchor = EnumerableAnchor.MiddleCenter,
                float messageLifeTime = 3f, string stringReference = "Locale.InteractiveTutorial.StepBackDefaultMessage")
        {
            _target = targetBall;
            _stash = stash;
            _messageReference = interactiveMessage;
            _stringReference = stringReference;
            _messageLifeTime = messageLifeTime;
            _anchor = anchor;
        }

        public void SetupStepBackInfo() => FormStepBackInfo();

        public bool Check()
        {
            bool answer = _target == null || (!_target.CanBeMovedByMouse() && _target.GameStatus != BallGameStatus.OnStash);
            return answer;
        }

        public IEnumerator StepBack()
        {
            if (_ballsInfo == null) yield break;
            int counter = 0;
            foreach (BallStepBackInfo info in _ballsInfo)
            {
                counter++;
                if (info.Ball == null) continue;
                info.Ball.DropVelocity();
                yield return new WaitForFixedUpdate();
                info.Ball.transform.position = info.Position;
                info.Ball.SetBallColoring(info.Coloring);
                info.Ball.SetBallSide(info.Side);
            }
            _stash.ClearStash();
            foreach (BallController ball in _stashedBalls)
            {
                if (ball == null) continue;
                _stash.AddNewBall(ball);
                yield return new WaitForFixedUpdate();
                ball.transform.position = _stash.InsisibleBallPosition.position;
            }
            TryToShowMessage();
        }

        private void TryToShowMessage()
        {
            if (_temporaryMessage != null) return;
            _temporaryMessage = _messageReference.CreateTemporaryCopy(_messageLifeTime);
            _temporaryMessage.Show(_anchor, _stringReference);
        }

        private void FormStepBackInfo()
        {
            if (_ballsInfo != null || _stashedBalls != null) return;
            _ballsInfo = new List<BallStepBackInfo>();
            _stashedBalls = _stash.GetBalls();
            BallController[] balls = GameObject.FindObjectsByType<BallController>(FindObjectsSortMode.None);
            foreach (BallController b in balls)
                _ballsInfo.Add(new BallStepBackInfo(b, b.transform.position, b.Coloring, b.GameStatus, b.Side));
        }
    }
}