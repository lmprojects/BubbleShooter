using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelsController : MonoBehaviour
{
	[SerializeField]
	protected GameObject loader;
	[SerializeField]
	protected GameObject levelPref;

	private float startX;
	private float startY;
	public float stepX = 190f;
	public float stepY = 50f;

	private void Start()
	{
		StartCoroutine(StartLevelScene());
	}

	private IEnumerator StartLevelScene()
	{
		loader.SetActive(true);
		yield return new WaitForSeconds(1f);
		loader.SetActive(false);
		Create();
	}

	private void Create()
	{
		Level l = levelPref.GetComponent<Level>();
		l.CurLevel = 1;
		l.ResetAll();
		l.IsAvailable = GameModel.Model.openedLevels.Contains(l.CurLevel);
		if (GameModel.Model.openedLevels.Contains(l.CurLevel))
			l.SetStars(GameModel.Model.levelsStars[l.CurLevel]);

		startX = levelPref.transform.localPosition.x;
		startY = levelPref.transform.localPosition.y;
		CreateLevels(20);
	}

	private void CreateLevels(int count)
	{
		int cnt = 1;
		for(int i = 1; i < count; i++)
		{
			if (cnt == 4 || cnt == 8)
				startY += 200f;
			GameObject go = Instantiate(levelPref, transform);
			float x = cnt < 5 ? startX + cnt * stepX : startX + (8 - cnt) * stepX;
			float y = cnt == 4 || cnt == 8 ? startY - 100f + cnt * stepY : startY + cnt * stepY;
			go.transform.localPosition = new Vector3(x, y, 0);
			cnt++;
			if (cnt == 9)
			{
				cnt = 1;
				startY = go.transform.localPosition.y + 100f;
			}
			
			Level lvl = go.GetComponent<Level>();
			lvl.CurLevel = i + 1;
			lvl.ResetAll();
			lvl.IsAvailable = GameModel.Model.openedLevels.Contains(lvl.CurLevel);
			if (GameModel.Model.openedLevels.Contains(lvl.CurLevel))
				lvl.SetStars(GameModel.Model.levelsStars[lvl.CurLevel]);
		}
	}
}
