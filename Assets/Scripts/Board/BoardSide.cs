using UnityEngine;

public class BoardSide : MonoBehaviour
{
    [SerializeField] private BallSide _side;

    private void OnTriggerEnter(Collider other)
    {
        BallController ball = other.gameObject.GetComponent<BallController>();
        if (ball == null) return;
        ball.SetBallSide(_side);
    }

    private void OnTriggerExit(Collider other)
    {
        BallController ball = other.gameObject.GetComponent<BallController>();
        if (ball == null) return;
        ball.SetBallSide((_side == BallSide.Red) ? BallSide.Blue : BallSide.Red);
    }
}
