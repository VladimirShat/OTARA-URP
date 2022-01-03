using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareZone : MonoBehaviour
{
    public Dog dog;
    public float sheepRunningSpeed;

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;

        if (go.layer == LayerMask.NameToLayer("Sheep"))
        {
            if (!dog.sheepsAround.Contains(go.GetComponent<Sheep>()))
                dog.sheepsAround.Add(go.GetComponent<Sheep>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var go = other.gameObject;

        if (go.layer == LayerMask.NameToLayer("Sheep"))
        {
            var sheep = go.GetComponent<Sheep>();
            sheep.dogPosition = transform.position;
            sheep.RunSpeed = sheepRunningSpeed;
            sheep.runing = true;
        }

        foreach (var sheep in dog.sheepsAround)
        {
            if (sheep.runing)
                Debug.DrawRay(transform.position, sheep.transform.position - transform.position, Color.white);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var go = other.gameObject;

        if (go.layer == LayerMask.NameToLayer("Sheep"))
        {
            go.GetComponent<Sheep>().runing = false;
        }
    }
}
