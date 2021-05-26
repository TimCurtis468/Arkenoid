using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBall : Collectable
{
    protected override void ApplyEffect()
    {
        foreach(var ball in BallsManager.Instance.Balls)
        {
            ball.StartLightningBall();
        }
    }
}
