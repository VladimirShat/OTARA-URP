using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineTest : MonoBehaviour
{
    public int DefaultLayer = 0;
    public int HighlightLayer = 25;

    private void OnEnable()
    {
        gameObject.layer = HighlightLayer;
        Outliner.RegisterObject(gameObject);
    }

    private void OnDisable()
    {
        gameObject.layer = DefaultLayer;
        Outliner.UnregisterObject(gameObject);
    }
}
