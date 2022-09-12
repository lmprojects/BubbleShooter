using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
	[SerializeField]
	protected GameObject[] colorsGO;

	private float bulletProgress = 0.0f;
	private float bulletIncrement = 0.0f;
	private RayCastShooter shooter;
	private Grid grid;

	public Ball.BALL_TYPE GetBallType { get; private set; }
	public bool IsBulletActive => gameObject.activeSelf;
	public Ball FinalBall { get; set; }

	public void SetParams(RayCastShooter shooter, Grid grid)
	{
		this.shooter = shooter;
		this.grid = grid;
	}	

	public void SetType(Ball.BALL_TYPE type)
	{
		foreach (var go in colorsGO)
			go.SetActive(false);

		this.GetBallType = type;
		colorsGO[((int)type) - 1].SetActive(true);
	}

	private void Update()
	{
		List<Vector2> dots = shooter.bulletPath;
		if (gameObject.activeSelf)
		{
			bulletProgress += bulletIncrement;

			if (bulletProgress > 1)
			{
				dots.RemoveAt(0);
				if (dots.Count < 2)
				{
					gameObject.SetActive(false);
					grid.AddBall(FinalBall, this);
					dots.Clear();

					return;
				}
				else
				{
					InitPath(dots);
				}
			}

			var px = dots[0].x + bulletProgress * (dots[1].x - dots[0].x);
			var py = dots[0].y + bulletProgress * (dots[1].y - dots[0].y);

			transform.position = new Vector2(px, py);
		}
	}

	public void BulletGo(Ball.BALL_TYPE new_type, Vector3 pos)
	{
		SetType(new_type);
		gameObject.SetActive(true);
		transform.position = pos;
		InitPath(shooter.bulletPath);
	}

	private void InitPath(List<Vector2> dots)
	{
		var start = dots[0];
		var end = dots[1];
		var length = Vector2.Distance(start, end);
		var iterations = length / 0.22f;
		bulletProgress = 0.0f;
		bulletIncrement = 1.0f / iterations;
	}
}
