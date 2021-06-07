using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    #region Singleton
    private static Paddle _instance;
    public static Paddle Instance => _instance;

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

    private Camera mainCamera;
    private float paddleInitialY;
    private float defaultPaddleWidthInPixels = 200;
    private float defaultLeftClamp = 135;
    private float defaultRightClamp = 410;
    private SpriteRenderer sr;
    private BoxCollider2D boxCol;
    
    // Shooting
    public bool PaddleIsShooting { get; set; }
    public GameObject leftMuzzle;
    public GameObject rightMuzzle;
    public Projectile bulletPrefab;

    // Size
    public bool PaddleIsTransforming { get; set; }

    public float extendShrinkDuration = 50;
    public float paddleWidth = 2.0f;
    public float paddleHeight = 0.28f;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        paddleInitialY = transform.position.y;
        sr = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PaddleMovement();
        UpdateMuzzlePosition();
    }

    private void UpdateMuzzlePosition()
    {
        leftMuzzle.transform.position = new Vector3(this.transform.position.x - (this.sr.size.x /2 ) + 0.1f, this.transform.position.y + 0.2f, this.transform.position.z);
        rightMuzzle.transform.position = new Vector3(this.transform.position.x + (this.sr.size.x / 2) - 0.153f, this.transform.position.y + 0.2f, this.transform.position.z);
    }

    public void StartWidthAnimation(float newWidth)
    {
        StartCoroutine(AnimationPaddleWidth(newWidth));
    }

    private IEnumerator AnimationPaddleWidth(float width)
    {
        this.PaddleIsTransforming = true;
        this.StartCoroutine(ResetPaddleWidthAfterTime(this.extendShrinkDuration));

        if(width > this.sr.size.x)
        {
            float currentWidth = this.sr.size.x;
            while(currentWidth < width)
            {
                currentWidth += Time.deltaTime * 2;
                this.sr.size = new Vector2(currentWidth, paddleHeight);
                boxCol.size = new Vector2(currentWidth, paddleHeight);
                yield return null;
            }
        }
        else
        {
            float currentWidth = this.sr.size.x;
            while(currentWidth > width)
            {
                currentWidth -= Time.deltaTime * 2;
                this.sr.size = new Vector2(currentWidth, paddleHeight);
                boxCol.size = new Vector2(currentWidth, paddleHeight);
                yield return null;
            }
        }
        this.PaddleIsTransforming = false;
    }

    private IEnumerator ResetPaddleWidthAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.StartWidthAnimation(this.paddleWidth);
    }

    private void PaddleMovement()
    {
        float paddleShift = (defaultPaddleWidthInPixels - ((defaultPaddleWidthInPixels / 2) * sr.size.x)) / 2;
        float leftClamp = defaultLeftClamp - paddleShift;
        float rightClamp = defaultRightClamp + paddleShift;
        float mousePositionPixels = Mathf.Clamp(Input.mousePosition.x, leftClamp, rightClamp);
        float mousePositionWorldX = mainCamera.ScreenToWorldPoint(new Vector3(mousePositionPixels, 0, 0)).x;
        transform.position = new Vector3(mousePositionWorldX, paddleInitialY, 0);
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
                ballRb.AddForce(new Vector2(-(Mathf.Abs(difference * 200)), BallsManager.Instance.initialBallSpeed));
            }
            else
            {
                ballRb.AddForce(new Vector2(Mathf.Abs(difference * 200), BallsManager.Instance.initialBallSpeed));
            }
        }
    }

    public void StartShooting()
    {
        if(this.PaddleIsShooting == false)
        {
            this.PaddleIsShooting = true;
            StartCoroutine(StartShootingRoutine());
        }
    }

    public IEnumerator StartShootingRoutine()
    {
        float fireCooldown = 0.5f;
        float fireCooldownleft = 0.0f;

        float shootingDuration = 10.0f;
        float shootingDurationLeft = shootingDuration;

//        Debug.Log("START SHOOTING");

        while(shootingDurationLeft >= 0.0)
        {
            fireCooldownleft -= Time.deltaTime;
            shootingDurationLeft -= Time.deltaTime;

            if(fireCooldownleft <= 0.0f)
            {
                this.Shoot();
                fireCooldownleft = fireCooldown;
//                Debug.Log("Shoot at {time.time}");
            }

            yield return null;
        }

//        Debug.Log("STOPPED SHOOTING");
        this.PaddleIsShooting = false;
        leftMuzzle.SetActive(false);
        rightMuzzle.SetActive(false);
    }

    private void Shoot()
    {
        leftMuzzle.SetActive(false);
        rightMuzzle.SetActive(false);

        leftMuzzle.SetActive(true);
        rightMuzzle.SetActive(true);

        this.SpawnBullet(leftMuzzle);
        this.SpawnBullet(rightMuzzle);
    }

    private void SpawnBullet(GameObject muzzle)
    {
        Vector3 spawnPosition = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y + 0.2f, muzzle.transform.position.y);
        Projectile bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(new Vector2(0, 450.0f));

    }
}
