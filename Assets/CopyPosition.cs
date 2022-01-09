using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public Transform transformToCopy;

    void Update()
    {
        transform.position = transformToCopy.position;
    }
}
