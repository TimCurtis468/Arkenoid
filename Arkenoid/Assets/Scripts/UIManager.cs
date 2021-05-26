using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text TargetText;
    public Text ScoreText;
    public Text LivesText;

    public int Score { get; set; }

    private void Awake()
    {
        Brick.OnBrickDesctruction += OnBrickDestruction;
        BricksManager.OnLevelLoaded += OnLevelLoaded;
        GameManager.OnLifeLost += OnLifeLost;
    }

    private void Start()
    {

        OnLifeLost(GameManager.Instance.AvailableLives);
    }

    private void OnLifeLost(int remainingLives)
    {
        LivesText.text = "LIVES: " + remainingLives.ToString();
    }

    private void OnLevelLoaded()
    {
        UpdateRemaingBricksText();
        UpdateScoreText(0);
    }

    private void UpdateScoreText(int increment)
    {
        this.Score += increment;
        string scoreString = this.Score.ToString().PadLeft(5, '0');
        ScoreText.text = "SCORE:\r\n" + scoreString;

    }

    private void OnBrickDestruction(Brick obj)
    {
        UpdateRemaingBricksText();
        UpdateScoreText(10);
    }

    private void UpdateRemaingBricksText()
    {
        TargetText.text = $"TARGET\r\n {BricksManager.Instance.RemainingBricks.Count}/{BricksManager.Instance.InitialBricksCount}";
    }

    private void OnDisable()
    {
        Brick.OnBrickDesctruction -= OnBrickDestruction;
        BricksManager.OnLevelLoaded -= OnLevelLoaded;
    }
}
