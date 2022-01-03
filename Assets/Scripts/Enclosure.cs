using System.Collections;
using TMPro;
using UnityEngine;

public class Enclosure : MonoBehaviour
{
    public TextMeshProUGUI ScoresText;

    int scores;

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;

        if (go.layer == LayerMask.NameToLayer("Sheep"))
        {
            scores++;
            go.layer = LayerMask.NameToLayer("Default");
            StartCoroutine(StopSheep(go));
            StartCoroutine(HideSheep(go));
            ScoresText.text = "Score: " + scores;
        }
    }

    IEnumerator HideSheep(GameObject go)
    {
        yield return new WaitForSeconds(2);
        FindObjectOfType<Dog>().sheepsAround.Remove(go.GetComponent<Sheep>());
        Destroy(go);
    }
    IEnumerator StopSheep(GameObject go)
    {
        yield return new WaitForSeconds(1);
        go.GetComponent<Sheep>().enabled = false;
    }
}
