using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionBoost : BaseBoost
{
    public GameObject dogModel;
    public GameObject sheepModel;


    Dog dog;
    List<Fright> sheepsTouched = new List<Fright>();
    IEnumerator corAttraction;

    public override void OnBoostStart(GameObject player)
    {
        dog = player.GetComponent<Dog>();
        corAttraction = Attraction();
        StartCoroutine(corAttraction);
        sheepModel.SetActive(true);
        dogModel.SetActive(false);
    }

    public override void OnBoostEnd(GameObject player)
    {
        StopCoroutine(corAttraction);
        dog = null;
        foreach (var sheepTouched in sheepsTouched)
        {
            sheepTouched.GetComponent<Folower>().enabled = false;
            if(!sheepTouched.GetComponent<SheepController>().isCatched)
                sheepTouched.GetComponent<SheepController>().enabled = true;
        }
        sheepsTouched.Clear();
        sheepModel.SetActive(false);
        dogModel.SetActive(true);
    }

    IEnumerator Attraction()
    {
        while (true)
        {
            if (dog.sheepsAround.Count > 0)
                foreach (var sheep in dog.sheepsAround)
                {
                    if (!sheepsTouched.Contains(sheep.GetComponent<Fright>()) || sheepsTouched.Count == 0) sheepsTouched.Add(sheep.GetComponent<Fright>());
                    sheep.GetComponent<Folower>().enabled = true;
                    sheep.GetComponent<SheepController>().enabled = false;
                    sheep.GetComponent<Graze>().enabled = false;
                    sheep.GetComponent<Fright>().enabled = false;
                }

            yield return null;
        }
    }
}