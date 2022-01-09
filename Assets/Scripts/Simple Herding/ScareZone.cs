using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareZone : MonoBehaviour
{
    public Dog dog;
    public float sheepRunSpeed;

    float defaultSpeed;

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;

        if (go.layer == LayerMask.NameToLayer("Sheep"))
        {
            var sheep = go.GetComponent<SheepController>();

            if (!dog.sheepsAround.Contains(go.GetComponent<SheepController>()))
                dog.sheepsAround.Add(go.GetComponent<SheepController>());

            sheep.dogPosition = transform.position;
            defaultSpeed = go.GetComponent<Fright>().RunSpeed;
            go.GetComponent<Fright>().RunSpeed = sheepRunSpeed;
            sheep.isScared = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (var sheep in dog.sheepsAround)
        {
            if (sheep.isScared)
                Debug.DrawRay(transform.position, sheep.transform.position - transform.position, Color.white);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var go = other.gameObject;

        if (go.layer == LayerMask.NameToLayer("Sheep"))
        {
            go.GetComponent<SheepController>().isScared = false;
            go.GetComponent<Fright>().RunSpeed = defaultSpeed;
        }
    }
}
