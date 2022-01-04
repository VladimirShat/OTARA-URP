using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public int Scores { get; private set; } = 0;

	public void AddScores(int addedScores)
	{
		Scores += addedScores;
	}
}