using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsManager : MonoBehaviour
{
    #region Singleton
    private static BallsManager _instance;
    public static BallsManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion
    [SerializeField]
    private Ball ballPrefab;

    private Ball initialBall;

    private Rigidbody2D initialBallRb;

    public float initialBallSpeed = 250;

    public List<Ball> Balls { get; set; }

    private void Start()
    {
        InitBall();
    }

    private void Update()
    {
        if(!GameManager.Instance.IsGameStarted)
        {
            Vector3 paddlePosition = Paddle.Instance.gameObject.transform.position;
            Vector3 ballPosition = new Vector3(paddlePosition.x, paddlePosition.y + 0.27f, 0);
            initialBall.transform.position = ballPosition;

            if( Input.GetMouseButtonDown(0))
            {
                initialBallRb.isKinematic = false;
                initialBallRb.AddForce(new Vector2(0, initialBallSpeed));
                GameManager.Instance.IsGameStarted = true;
            }
        }
    }

    private void InitBall()
    {
        Vector3 paddlePosition = Paddle.Instance.gameObject.transform.position;
        Vector3 startingPosition = new Vector3(paddlePosition.x, paddlePosition.y + 0.27f, 0); 
        initialBall = Instantiate(ballPrefab, startingPosition, Quaternion.identity);
        initialBallRb = initialBall.GetComponent<Rigidbody2D>();

        this.Balls = new List<Ball>
        {
            initialBall
        };
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Ball")
        {
            Rigidbody2D ballRb = coll.gameObject.GetComponent<Rigidbody2D>();
            Vector3 hitPoint = coll.contacts[0].point;
            Vector3 paddleCentre = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y);

            ballRb.velocity = Vector2.zero;

            float difference = paddleCentre.x - hitPoint.x;

            if (hitPoint.x < paddleCentre.x)
            {
                ballRb.AddForce(new Vector2(-(Mathf.Abs(difference * 200)), initialBallSpeed));
            }
            else
            {
                ballRb.AddForce(new Vector2(Mathf.Abs(difference * 200), initialBallSpeed));
            }

        }
    }
}
