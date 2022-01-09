using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : BaseBoost
{
    public float speedMultiplier = 2f;

    float playerDefaultSpeed;

    public override void OnBoostStart(GameObject player)
    {
        playerDefaultSpeed = player.GetComponent<ThirdPersonController>().maxSpeed;
        player.GetComponent<ThirdPersonController>().maxSpeed *= speedMultiplier;
    }

    public override void OnBoostEnd(GameObject player)
    {
        player.GetComponent<ThirdPersonController>().maxSpeed = playerDefaultSpeed;
    }
}
