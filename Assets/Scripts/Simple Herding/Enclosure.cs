using System.Collections;
using TMPro;
using UnityEngine;

public class Enclosure : MonoBehaviour
{
    public TextMeshProUGUI ScoresText;
    public int scoresPerSheep = 1;

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;

        if (go.layer == LayerMask.NameToLayer("Sheep"))
        {
            GameManager.Instance.AddScores(scoresPerSheep);

            StartCoroutine(HideSheep(go));

            ScoresText.text = "Score: " + GameManager.Instance.Scores;
        }
    }

    IEnumerator HideSheep(GameObject sheepObject)
    {
        sheepObject.layer = LayerMask.NameToLayer("Default");

        yield return new WaitForSeconds(1);

        sheepObject.GetComponent<SheepController>().enabled = false;

        yield return new WaitForSeconds(1);

        FindObjectOfType<Dog>().sheepsAround.Remove(sheepObject.GetComponent<SheepController>());
        Destroy(sheepObject);
    }
}
