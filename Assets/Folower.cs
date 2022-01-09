using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Folower : MonoBehaviour
{
    [Range(0, 10)]
    public float Speed = 1f;

    SheepController sheepController;
    float distanceToTarget;
    Transform dog;

    private void Start()
    {
        sheepController = GetComponent<SheepController>();
        dog = transform.parent.GetComponent<SheepsManager>().Dog;
        GetComponent<Folower>().enabled = false;
    }

    void Update()
    {
        var target = dog.position;
        Vector3 xzTargetPos = target;
        xzTargetPos.y = transform.position.y;
        Vector3 relativePos = xzTargetPos - transform.position;
        var rot = Quaternion.LookRotation(relativePos);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 3 * Time.deltaTime);

        if (sheepController.CanGoForward())
        {
            if (Vector3.Distance(transform.position, target) > 2)
            {
                //rotates perpendicular to the ground
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                sheepController.controller.SimpleMove(forward * Speed);
            }
        }
    }
}