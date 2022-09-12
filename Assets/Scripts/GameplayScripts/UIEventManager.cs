using AxGrid.Base;
using AxGrid.Path;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEventManager : MonoBehaviourExt
{
	[SerializeField]
	protected GameObject GOMsg;
	[SerializeField]
	protected TextMeshProUGUI blueBottleText;
	[SerializeField]
	protected TextMeshProUGUI greenBottleText;
	[SerializeField]
	protected TextMeshProUGUI redBottleText;
	[SerializeField]
	protected Image lineProgress;

	private int blueBotlCnt = 0;
	private int greenBotlCnt = 0;
	private int redBotlCnt = 0;
	private int lineCnt = 0;

	[OnStart]
	private void OnStart()
	{
		GameModel.EventManager.AddEvent("game_over_show", ShowGOMsg);
		GameModel.EventManager.AddEvent("bottleCountMinus", BottleCountMinus);
		GameModel.EventManager.AddEvent("updateLineProgress", UpdateLineProgress);
		GameModel.EventManager.AddEvent("OnPauseOn", OnPauseOn);
		GameModel.EventManager.AddEvent("OnPauseOff", OnPauseOff);
		ResetBottleCounts();
	}

	[OnDestroy]

	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(ShowGOMsg);
		GameModel.EventManager.RemoveEvent(BottleCountMinus);
		GameModel.EventManager.RemoveEvent(UpdateLineProgress);
		GameModel.EventManager.RemoveEvent(OnPauseOn);
		GameModel.EventManager.RemoveEvent(OnPauseOff);
	}

	private void OnPauseOn(EventArgs args)
	{
		GameModel.EventManager.Invoke("lock_screen");
	}

	private void OnPauseOff(EventArgs args)
	{
		Path = new CPath();
		Path.Wait(0.5f).Action(() => GameModel.EventManager.Invoke("unlock_screen"));
	}

	private void UpdateLineProgress(EventArgs args)
	{
		Debug.Log(lineCnt + " " + (int)args.Args[0] + " = " + (lineCnt + (int)args.Args[0]));
		lineCnt += (int)args.Args[0];
		lineProgress.fillAmount = lineCnt / (float)Grid.rowCount;

		if (lineCnt == Grid.rowCount)
		{
			GameModel.EventManager.Invoke("lock_screen");

			Map map = GameModel.Model.maps[GameModel.Model.currentLevel - 1];
			int blueBotlGoalCnt = blueBotlCnt;
			int greenBotlGoalCnt = 0;
			int redBotlGoalCnt = map.bottleRedGoal;

			float starsP = 0;
			starsP += blueBotlCnt > blueBotlGoalCnt ? 1 / 3 : (blueBotlCnt * 100) / blueBotlGoalCnt;
			starsP += greenBotlCnt > greenBotlGoalCnt ? 1 / 3 : (greenBotlCnt * 100) / greenBotlGoalCnt;
			starsP += redBotlCnt > redBotlGoalCnt ? 1 / 3 : (redBotlCnt * 100) / redBotlGoalCnt;

			int level = GameModel.Model.currentLevel;
			if (GameModel.Model.levelsStars.ContainsKey(level))
				GameModel.Model.levelsStars[level] = starsP;
			else
				GameModel.Model.levelsStars.Add(level, starsP);

			if (GameModel.Model.levelsPoints.ContainsKey(level))
				GameModel.Model.levelsPoints[level] = Random.Range(0, 20000);
			else
				GameModel.Model.levelsPoints.Add(level, Random.Range(0, 20000));

			GameModel.EventManager.Invoke("OpenLevelPopup", this, level, blueBotlCnt, redBotlCnt, greenBotlCnt); //DonePopup.SetActive(true);

			GameModel.Model.OpenNewLevel();
		}
	}

	public void Continue()
	{
		lineCnt = 0;
		lineProgress.fillAmount = 0;
		ResetBottleCounts();
		GameModel.EventManager.Invoke("unlock_screen");
		GameModel.EventManager.Invoke("rebuild_grid");
		GameModel.EventManager.Invoke("CloseLevelPopup");
	}

	public void Restart()
	{
		GameModel.Model.BlockLastLevel();
		lineCnt = 0;
		lineProgress.fillAmount = 0;
		ResetBottleCounts();
		GameModel.EventManager.Invoke("unlock_screen");
		GameModel.EventManager.Invoke("rebuild_grid");
		GameModel.EventManager.Invoke("CloseLevelPopup");
	}

	private void ResetBottleCounts()
	{
		blueBotlCnt = 0;
		greenBotlCnt = 0;
		redBotlCnt = 0;

		blueBottleText.text = blueBotlCnt.ToString();
		greenBottleText.text = greenBotlCnt.ToString();
		redBottleText.text = redBotlCnt.ToString();
	}

	private void BottleCountMinus(EventArgs args)
	{
		Bottle.BottleType type = (Bottle.BottleType)args.Args[0];
		switch(type)
		{
			case Bottle.BottleType.Bottle_blue:
				blueBotlCnt++;
				if(blueBotlCnt >= 0)
					blueBottleText.text = blueBotlCnt.ToString();
				break;
			case Bottle.BottleType.Bottle_green:
				greenBotlCnt++;
				if(greenBotlCnt >= 0)
					greenBottleText.text = greenBotlCnt.ToString();
				break;
			case Bottle.BottleType.Bottle_red:
				redBotlCnt++;
				if(redBotlCnt >= 0)
					redBottleText.text = redBotlCnt.ToString();
				break;
		}
	}

	private void ShowGOMsg(EventArgs args)
	{
		GameModel.EventManager.Invoke("lock_screen");
		GOMsg.SetActive(true);
	}
	public void HideGOMsg()
	{
		lineCnt = 0;
		lineProgress.fillAmount = 0;
		ResetBottleCounts();
		GameModel.EventManager.Invoke("unlock_screen");
		GameModel.EventManager.Invoke("rebuild_grid");
		GOMsg.SetActive(false);
	}
}
