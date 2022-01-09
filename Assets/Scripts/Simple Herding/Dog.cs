using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    [HideInInspector]
    public List<SheepController> sheepsAround;
    public SphereCollider sphCollider;
    float defaultRadius;

    private void Start()
    {
        sheepsAround = new List<SheepController>();

        if (sphCollider == null)
            sphCollider = GetComponent<SphereCollider>();
        defaultRadius = sphCollider.radius;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCoroutine(Bark(4, 0.2f));
        }

        if(sphCollider.radius == 4)
        {
            StartCoroutine(Bark(defaultRadius, 0.1f));
        }
    }

    IEnumerator Bark(float endValue, float duration)
    {
        float time = 0;
        float startValue = sphCollider.radius;

        while (time < duration)
        {
            sphCollider.radius = Mathf.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        sphCollider.radius = endValue;
    }
}