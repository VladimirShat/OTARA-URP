using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    [HideInInspector]
    public List<Sheep> sheepsAround;
    SphereCollider sphCollider;
    float defaultRadius;

    private void Start()
    {
        sheepsAround = new List<Sheep>();
        sphCollider = GetComponent<SphereCollider>();
        defaultRadius = sphCollider.radius;
    }

    private void Update()
    {
        sphCollider.radius = Mathf.Lerp(sphCollider.radius, defaultRadius, 0.1f);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(Bark());
        }
    }

    IEnumerator Bark()
    {
        sphCollider.radius *= 2f;

        foreach (var sheep in sheepsAround)
        {
            if (sheep.runing)
                sheep.Scared();
        }

        yield return null;
        StopCoroutine(Bark());
    }
}