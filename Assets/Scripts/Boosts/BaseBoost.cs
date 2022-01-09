using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseBoost : MonoBehaviour
{
    public int boostTime;

    bool takingAway = false;
    int secondsLeft = -1;
    GameObject Player;

    private void Update()
    {
        if (takingAway == false && secondsLeft > 0)
        {
            StartCoroutine(TimerTake());
        }

        if (secondsLeft == 0)
        {
            Debug.Log(Player.name);
            OnBoostEnd(Player);
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            secondsLeft = boostTime;
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            Player = other.gameObject;
            OnBoostStart(Player);
        }
    }

    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;
        takingAway = false;
    }

    public virtual void OnBoostStart(GameObject player){ }

    public virtual void OnBoostEnd(GameObject player){ }
}
