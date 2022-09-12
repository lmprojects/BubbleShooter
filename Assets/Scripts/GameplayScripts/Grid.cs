using AxGrid.Base;
using AxGrid.Path;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviourExt
{
	[SerializeField]
	protected GameObject loader;
	[SerializeField]
	protected float tile_size = 0.68f;
	[SerializeField]
	protected float changeTypeRate = 0.5f;
	[SerializeField]
	protected int emptyLines = 16;
	[SerializeField]
	protected GameObject gridBallGO;
	[SerializeField]
	protected BottlePool bottlePool;
	[SerializeField]
	protected Transform shooter;

	public static int rowCount;
	public float TileSize => tile_size;
	public float GridOffset { get; set; } = 0;

	protected int rows_count = 20;
	protected int columns_count = 14;

	protected List<List<Ball>> gridBalls;
	private List<Ball> matchList;
	private Map map;
	private int topEmptyLine;
	private int bottomUnvisibleLine = 17;
	private const int alwaysVisibleCount = 4;
	private const int fullScreenBallsCount = 11;
	private float finishPoint;

	[OnStart]
	private void OnStart()
	{
		Path = new CPath();

		GameModel.EventManager.AddEvent("rebuild_grid", RebuildGrid);
		GameModel.EventManager.AddEvent("move_grid", MoveGrid);

		topEmptyLine = emptyLines - 1;
		matchList = new List<Ball>();

		MapRebuild();
		BuildGrid();
	}

	[OnDestroy]

	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(RebuildGrid);
		GameModel.EventManager.RemoveEvent(MoveGrid);
	}

	private void MapRebuild()
	{
		map = GameModel.Model.maps[GameModel.Model.currentLevel - 1];

		GameModel.Model.CoinCnt -= map.cost;

		columns_count = map.width;
		rowCount = map.height;
		rows_count = map.height + emptyLines;
	}	

	private void BuildGrid()
	{
		gridBalls = new List<List<Ball>>();

		GridOffset = (columns_count * tile_size) * 0.5f;
		GridOffset -= tile_size * 0.5f;

		int index = 0;
		int ballIndex = 0;
		for (int row = 0; row < rows_count; row++)
		{
			var rowBalls = new List<Ball>();
			for (int column = columns_count - 1; column >= 0; column--)
			{
				GameObject item = Instantiate(gridBallGO);
				item.name = "Bubble " + ballIndex;
				ballIndex++;
				Ball ball = item.GetComponent<Ball>();

				ball.Shooter = shooter;
				ball.SetBallPosition(this, column, row);
				ball.IsNotHidden = true;
				ball.IsVisible = true;
				if (gridBalls.Count < emptyLines)
				{
					ball.SetType((Ball.BALL_TYPE)UnityEngine.Random.Range(1, Ball.GetBallTypesCount));
					ball.gameObject.SetActive(false);
					ball.IsNotHidden = false;
				}
				else
				{
					ball.SetType((Ball.BALL_TYPE)map.data[index]);
					index++;
					if (ball.GetBallType == Ball.BALL_TYPE.NONE)
					{
						ball.IsNotHidden = false;
						ball.gameObject.SetActive(false);
					}

					if (gridBalls.Count >= emptyLines + alwaysVisibleCount)
						ball.IsVisible = false;

					if(ball.GetBallType == Ball.BALL_TYPE.bubble)
					{
						Bottle bottle = bottlePool.GetFreeBottle();
						Bottle.BottleType b_type = (Bottle.BottleType)Random.Range(0, Bottle.GetBottleTypeCount);
						bottle.SetPosition(ball.transform.position);
						bottle.CurBall = ball;
						ball.Bottle = bottle;
						int timer = 0;
						switch(b_type)
						{
							default:
							case Bottle.BottleType.Bottle_blue:
								timer = map.bottleBlueDuration;
								break;
							case Bottle.BottleType.Bottle_green:
								timer = map.bottleGreenDuration;
								break;
							case Bottle.BottleType.Bottle_red:
								timer = map.bottleRedDuration;
								break;
						}
						bottle.SetType(b_type, timer);
					}
				}

				ball.transform.parent = gameObject.transform;
				rowBalls.Insert(0, ball);
			}

			gridBalls.Add(rowBalls);
		}

		Vector3 p = transform.localPosition;
		p.y = -4.87f;
		transform.localPosition = p;
		finishPoint = transform.position.y;

		Path.Wait(0.5f).Action(() => loader.SetActive(false));
	}

	public void RebuildGrid(EventArgs args)
	{
		MapRebuild();
		bottlePool.ResetPool();
		int index = 0;
		for (int row = 0; row < rows_count; row++)
		{
			for (int column = columns_count - 1; column >= 0; column--)
			{
				Ball ball = gridBalls[row][column];
				ball.IsNotHidden = true;
				ball.IsVisible = true;
				ball.IsHideAnimStart = false;
				if (row < emptyLines)
				{
					ball.SetType((Ball.BALL_TYPE)UnityEngine.Random.Range(1, Ball.GetBallTypesCount));
					ball.gameObject.SetActive(false);
					ball.IsNotHidden = false;
				}
				else
				{
					ball.gameObject.SetActive(true);
					ball.SetType((Ball.BALL_TYPE)map.data[index]);
					index++;
					if (ball.GetBallType == Ball.BALL_TYPE.NONE)
					{
						ball.IsNotHidden = false;
						ball.gameObject.SetActive(false);
					}

					if (row >= emptyLines + alwaysVisibleCount)
						ball.IsVisible = false;

					if (ball.GetBallType == Ball.BALL_TYPE.bubble)
					{
						Bottle bottle = bottlePool.GetFreeBottle();
						Bottle.BottleType b_type = (Bottle.BottleType)Random.Range(0, Bottle.GetBottleTypeCount);
						bottle.SetPosition(ball.transform.position);
						bottle.CurBall = ball;
						ball.Bottle = bottle; 
						
						int timer = 0;
						switch (b_type)
						{
							default:
							case Bottle.BottleType.Bottle_blue:
								timer = map.bottleBlueDuration;
								break;
							case Bottle.BottleType.Bottle_green:
								timer = map.bottleGreenDuration;
								break;
							case Bottle.BottleType.Bottle_red:
								timer = map.bottleRedDuration;
								break;
						}
						bottle.SetType(b_type, timer);
					}
				}
			}
		}

		Vector3 p = transform.localPosition;
		p.y = -4.87f;
		transform.localPosition = p;
		finishPoint = transform.position.y;
		topEmptyLine = emptyLines + 1; 
		bottomUnvisibleLine = 17;
	}

	public void AddBall(Ball collisionBall, Bullet bullet)
	{
		if (collisionBall.GetRow >= bottomUnvisibleLine)
			return;
		Ball minBall = collisionBall;
		minBall.SetType(bullet.GetBallType);
		minBall.gameObject.SetActive(true);
		minBall.IsNotHidden = true;
		minBall.IsHideAnimStart = false;
		minBall.IsVisible = true;

		CheckMatchesForBall(minBall);

		if (collisionBall.GetRow <= bottomUnvisibleLine - fullScreenBallsCount + 1 && !minBall.IsHideAnimStart)
			GameModel.EventManager.Invoke("game_over_show");
	}

	public void CheckMatchesForBall(Ball ball)
	{
		matchList.Clear();

		foreach (List<Ball> r in gridBalls)
		{
			foreach (Ball b in r)
			{
				b.IsVisited = false;
			}
		}

		//search for matches around ball
		List<Ball> initialResult = GetMatches(ball);
		matchList.AddRange(initialResult.FindAll(x => x.IsVisible && !x.IsHideAnimStart));

		while (true)
		{
			bool allVisited = true;
			for (int i = matchList.Count - 1; i >= 0; i--)
			{
				Ball b = matchList[i];
				if (!b.IsVisited)
				{
					AddMatches(GetMatches(b).FindAll(x => x.IsVisible && !x.IsHideAnimStart));
					allVisited = false;
				}
			}

			if (allVisited)
			{
				if (matchList.Count > 2)
				{
					for(int k = 0; k < matchList.Count; k++)
					{
						matchList[k].BallHideAnim(false);//(k == matchList.Count - 1);
					}

					CheckForDisconnected();

					//remove disconnected balls
					List<Ball> disconnectedBalls = new List<Ball>();
					int i = gridBalls.Count - 1;
					while (i >= 0)
					{
						foreach(Ball b in gridBalls[i])
						{
							if (!b.IsConnected && !b.IsHideAnimStart && b.IsNotHidden)
							{
								disconnectedBalls.Add(b);
							}
						}
						i--;
					}

					for(int k = 0; k < disconnectedBalls.Count; k++)
						disconnectedBalls[k].BallFallAnim(k == disconnectedBalls.Count - 1);

					if (disconnectedBalls.Count == 0)
						StartCoroutine(CheckAdd());

				}
				return;
			}
		}
	}

	private IEnumerator CheckAdd()
	{
		yield return new WaitForSeconds(1f);
		CheckForAddLine();
	}

	private void CheckForDisconnected()
	{
		//set all balls as disconnected
		foreach (List<Ball> r in gridBalls)
		{
			foreach (Ball b in r)
			{
				b.IsConnected = false;
			}
		}
		//connect visible balls in last row 
		foreach (Ball b in gridBalls[rows_count - 1])
		{
			if (b.IsNotHidden)
				b.IsConnected = true;
		}

		//now set connect property on the rest of the balls
		int i = rows_count - 1;
		while (i >= 0)
		{
			foreach (Ball b in gridBalls[i])
			{
				if (b.IsNotHidden && !b.IsHideAnimStart)
				{
					List<Ball> neighbors = BallActiveNeighbors(b);
					bool connected = false;

					foreach (Ball n in neighbors)
					{
						if (n.IsConnected && !n.IsHideAnimStart)
						{
							connected = true;
							break;
						}
					}

					if (connected)
					{
						b.IsConnected = true;
						foreach (Ball n in neighbors)
						{
							if (n.IsNotHidden && !n.IsHideAnimStart)
							{
								n.IsConnected = true;
							}
						}
					}
				}
			}
			i--;
		}
	}

	private List<Ball> GetMatches(Ball ball)
	{
		ball.IsVisited = true;
		List<Ball> result = new List<Ball>() { ball };
		List<Ball> activeBalls = BallActiveNeighbors(ball);

		foreach (Ball b in activeBalls)
		{
			if (b.GetBallType == ball.GetBallType)
			{
				result.Add(b);
			}
		}

		return result;
	}

	private void AddMatches(List<Ball> matches)
	{
		foreach (Ball b in matches)
		{
			if (!matchList.Contains(b))
				matchList.Add(b);
		}
	}

	private List<Ball> BallActiveNeighbors(Ball ball)
	{
		List<Ball> result = new List<Ball>();
		//right
		if (ball.GetColumn + 1 < columns_count)
		{
			if (gridBalls[ball.GetRow][ball.GetColumn + 1].IsNotHidden)
				result.Add(gridBalls[ball.GetRow][ball.GetColumn + 1]);
		}

		//left
		if (ball.GetColumn - 1 >= 0)
		{
			if (gridBalls[ball.GetRow][ball.GetColumn - 1].IsNotHidden)
				result.Add(gridBalls[ball.GetRow][ball.GetColumn - 1]);
		}
		//bottom (bot right/left)
		if (ball.GetRow - 1 >= 0)
		{
			if (gridBalls[ball.GetRow - 1][ball.GetColumn].IsNotHidden)
				result.Add(gridBalls[ball.GetRow - 1][ball.GetColumn]);
		}

		//top(top right\left)
		if (ball.GetRow + 1 < gridBalls.Count)
		{
			if (gridBalls[ball.GetRow + 1][ball.GetColumn].IsNotHidden)
				result.Add(gridBalls[ball.GetRow + 1][ball.GetColumn]);
		}

		//top-left
		if (ball.GetRow % 2 == 0)
		{
			if (ball.GetRow - 1 >= 0 && ball.GetColumn - 1 >= 0)
			{
				if (gridBalls[ball.GetRow - 1][ball.GetColumn - 1].IsNotHidden)
					result.Add(gridBalls[ball.GetRow - 1][ball.GetColumn - 1]);
			}
		}

		//top-right
		if (ball.GetRow % 2 != 0)
		{
			if (ball.GetRow - 1 >= 0 && ball.GetColumn + 1 < columns_count)
			{
				if (gridBalls[ball.GetRow - 1][ball.GetColumn + 1].IsNotHidden)
					result.Add(gridBalls[ball.GetRow - 1][ball.GetColumn + 1]);
			}
		}
		//bottom-left
		if (ball.GetRow % 2 == 0)
		{
			if (ball.GetRow + 1 < gridBalls.Count && ball.GetColumn - 1 >= 0)
			{
				if (gridBalls[ball.GetRow + 1][ball.GetColumn - 1].IsNotHidden)
					result.Add(gridBalls[ball.GetRow + 1][ball.GetColumn - 1]);
			}
		}

		//bottom-right
		if (ball.GetRow % 2 != 0)
		{
			if (ball.GetRow + 1 < gridBalls.Count && ball.GetColumn + 1 < columns_count)
			{
				if (gridBalls[ball.GetRow + 1][ball.GetColumn + 1].IsNotHidden)
					result.Add(gridBalls[ball.GetRow + 1][ball.GetColumn + 1]);
			}
		}
		return result;
	}

	public void CheckForAddLine()
	{
		int curELine = -1;
		for (int k = gridBalls.Count - 1; k >= 0; k--)
		{
			if (gridBalls[k].All(x => !x.IsNotHidden) && curELine == -1)
			{
				curELine = k;
			}
			if (gridBalls[k].All(x => !x.IsVisible))
			{
				bottomUnvisibleLine = k;
			}
		}
		Debug.Log("Check for move grid. TopEmptyLine " + topEmptyLine + " curELine " + curELine);
		if (curELine >= 0 && curELine > topEmptyLine)
		{
			GameModel.EventManager.Invoke("updateLineProgress", this, curELine - topEmptyLine);
			topEmptyLine = curELine;

			int vLine = bottomUnvisibleLine - 1 - topEmptyLine;
			int moveCount = alwaysVisibleCount - vLine;
			if (moveCount > 0)
			{
				MoveGrid(moveCount);
			}

		}
	}

	private void MoveGrid(EventArgs args)
	{
		if (bottomUnvisibleLine >= rows_count)
			return;
		MoveGrid(1);
	}

	private void MoveGrid(int countRow)
	{
		Debug.Log("MoveGrid " + countRow);
		if (bottomUnvisibleLine + countRow >= rows_count)
			countRow = rows_count - bottomUnvisibleLine;
		finishPoint -= 0.54f * countRow;
		Path.EasingCubicEaseOut(0.4f, transform.position.y, finishPoint, value =>
		{
			transform.position = new Vector3(transform.position.x, value, transform.position.z);
		});

		for (int i = 0; i < gridBalls.Count; i++)
		{
			if (i <= bottomUnvisibleLine - 1 + countRow)
				gridBalls[i].ForEach(x => x.IsVisible = true);
			else
				gridBalls[i].ForEach(x => x.IsVisible = false);
		}
		bottomUnvisibleLine += countRow;

		if (bottomUnvisibleLine - topEmptyLine >= fullScreenBallsCount)
			GameModel.EventManager.Invoke("game_over_show");
	}

	public Ball GetFinalBall(Vector2 last_dot)
	{
		Ball minBall = null;
		float minDistance = 10000.0f;
		for(int i = 0; i < gridBalls.Count; i++)
		{
			foreach (Ball n in gridBalls[i])
			{
				float d = Vector2.Distance(n.transform.position, last_dot);
				if (d < minDistance && !n.IsNotHidden)
				{
					minDistance = d;
					minBall = n;
				}
			}
		}
		return minBall;
	}
}