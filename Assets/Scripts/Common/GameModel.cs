using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameModel
{
	private static GameModel model;
	public static GameModel Model
	{
		get
		{
			if(model == null)
				model = new GameModel();
			return model;
		}
	}

	private static EventManager eventManager;
	public static EventManager EventManager
	{
		get
		{
			if (eventManager == null)
				eventManager = new EventManager();
			return eventManager;
		}
	}

	public List<Map> maps = new List<Map>();
	public List<int> openedLevels = new List<int>() { 1 };
	public int currentLevel = 1;
	public Dictionary<int, int> levelsPoints = new Dictionary<int, int>() { { 1, 0} };
	public Dictionary<int, float> levelsStars = new Dictionary<int, float>() { { 1, 2} };

	public bool IsSoundOn = true;
	public bool IsMusicOn = true;

	private int coinCnt = 100;
	public int CoinCnt
	{
		set
		{
			coinCnt = value;
			EventManager.Invoke("SetCoins", this, coinCnt);
		}
		get
		{
			return coinCnt;
		}
	}

	public void OpenNewLevel()
	{
		currentLevel++;
		openedLevels.Add(currentLevel);
		levelsPoints.Add(currentLevel, 0);
		levelsStars.Add(currentLevel, 0);
	}

	public void BlockLastLevel()
	{
		openedLevels.Remove(currentLevel);
		levelsPoints.Remove(currentLevel);
		levelsStars.Remove(currentLevel);
		currentLevel--;
	}
}
