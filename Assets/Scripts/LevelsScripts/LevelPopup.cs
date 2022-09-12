using AxGrid.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPopup : PopupController
{
	[SerializeField]
	protected TextMeshProUGUI levelLabel;
	[SerializeField]
	protected GameObject startBtn;
	[SerializeField]
	protected GameObject continueBtn;
	[SerializeField]
	protected GameObject restartBtn;
	[SerializeField]
	protected List<Image> stars;
	[SerializeField]
	protected TextMeshProUGUI score;
	[SerializeField]
	protected List<TextMeshProUGUI> bottleCnt;
	[SerializeField]
	protected GameObject bottleTitleStart;
	[SerializeField]
	protected GameObject bottleTitleEnd;

	public LevelPopupState currentState;
	public enum LevelPopupState
	{
		StartGame,
		EndGame
	}

	private int level = 1;

	[OnStart]
	private void OnStart()
	{
		container.SetActive(false);
		GameModel.EventManager.AddEvent("OpenLevelPopup", OpenLevelPopup);
		GameModel.EventManager.AddEvent("CloseLevelPopup", ClosePopup);
	}

	[OnDestroy]

	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(OpenLevelPopup);
		GameModel.EventManager.RemoveEvent(ClosePopup);
	}

	private void OpenLevelPopup(EventArgs args)
	{
		int level = (int)args.Args[0];
		this.level = level;
		if (!GameModel.Model.openedLevels.Contains(level))
		{
			GameModel.EventManager.Invoke("UnblockLevelsBtn");
			return;
		}

		OnOpen();

		GameModel.Model.currentLevel = level;
		levelLabel.text = "Уровень " + level;
		float starsPoint = GameModel.Model.levelsStars[level];
		if(currentState == LevelPopupState.StartGame)
		{
			score.gameObject.SetActive(false);
			startBtn.SetActive(true);
			continueBtn.SetActive(false);
			restartBtn.SetActive(false); 
			Map map = GameModel.Model.maps[GameModel.Model.currentLevel - 1];
			bottleCnt[0].text = map.bottleBlueGoal.ToString();
			bottleCnt[1].text = map.bottleRedGoal.ToString();
			bottleCnt[2].text = map.bottleGreenGoal.ToString();
			bottleTitleStart.SetActive(true);
			bottleTitleEnd.SetActive(false);
		}
		else
		{
			score.text = GameModel.Model.levelsPoints[level].ToString();
			score.gameObject.SetActive(true);
			startBtn.SetActive(false);
			continueBtn.SetActive(true);
			restartBtn.SetActive(true);
			bottleCnt[0].text = args.Args[1].ToString();
			bottleCnt[1].text = args.Args[2].ToString();
			bottleCnt[2].text = args.Args[3].ToString();
			bottleTitleStart.SetActive(false);
			bottleTitleEnd.SetActive(true);
		}
		SetStars(starsPoint);

		
	}

	private void SetStars(float points)
	{
		stars.ForEach(x => x.fillAmount = 0);
		if (points < 1)
			stars[0].fillAmount = points;
		else if (points >= 1 && points < 2)
		{
			stars[0].fillAmount = 1;
			stars[1].fillAmount = points - 1;
		}
		if (points >= 2)
		{
			stars[0].fillAmount = 1;
			stars[1].fillAmount = 1;
			stars[2].fillAmount = points - 2;
		}
	}

	private void ClosePopup(EventArgs args)
	{
		OnClose();
	}

	public void StartGame()
	{
		if (GameModel.Model.maps[level - 1].cost > GameModel.Model.CoinCnt)
		{
			OnClose();
			GameModel.EventManager.Invoke("OpenStorePopup");
			return;
		}

		GameModel.EventManager.Invoke("UnblockLevelsBtn");
		SceneManager.LoadScene("GameScene");
	}
}
