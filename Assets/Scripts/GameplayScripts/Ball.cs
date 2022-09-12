using AxGrid.Base;
using AxGrid.Path;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ball : MonoBehaviourExt
{
	[SerializeField]
	protected GameObject[] colorsGO;
	[SerializeField]
	protected Color[] colors;
	[SerializeField]
	protected Transform resizer;
	[SerializeField]
	protected ParticleSystem particle;
	[SerializeField]
	protected List<ParticleSystem> particleBoom;
	[SerializeField]
	protected Rigidbody2D rb;
	[SerializeField]
	protected Collider2D coll;

	public enum BALL_TYPE
	{
		NONE = 0,
		bubble,
		pink,
		yellow,
		purple,
		green,
		red,
		blue,
		grey
	}
	public static int GetBallTypesCount = 9;

	#region Properties
	public int GetRow { get; private set; }
	public int GetColumn { get; private set; }
	public BALL_TYPE GetBallType { get; private set; }

	public bool IsVisited { get; set; }
	public bool IsConnected { get; set; }
	public bool IsNotHidden { get; set; }
	public bool IsHideAnimStart { get; set; }
	public bool IsVisible { get; set; }
	public Bottle Bottle { get; set; }
	public Transform Shooter { get; set; }
	#endregion

	private Vector3 ballPosition;
	private Grid grid;

	[OnAwake]
	private void OnAwake()
	{
		rb.simulated = false;
	}

	public void SetBallPosition(Grid grid, int column, int row)
	{
		this.grid = grid;
		this.GetColumn = column;
		this.GetRow = row;

		ballPosition = new Vector3((column * grid.TileSize) - grid.GridOffset, row * grid.TileSize, 0);

		if (row % 2 != 0)
			ballPosition.x += grid.TileSize * 0.5f;

		transform.localPosition = ballPosition;

		foreach (var go in colorsGO)
			go.SetActive(false);
	}

	public void SetType(BALL_TYPE type)
	{
		foreach (var go in colorsGO)
			go.SetActive(false);

		this.GetBallType = type;

		if (type == BALL_TYPE.NONE)
			return;

		colorsGO[((int)type)-1].SetActive(true);
	}

	private void ResetBall(bool isLastBall)
	{
		particle.Stop();
		particleBoom.ForEach(x => x.Stop());
		IsNotHidden = false;
		resizer.localScale = Vector3.one;
		resizer.localPosition = Vector3.zero;
		if (isLastBall)
			grid.CheckForAddLine();
		gameObject.SetActive(false);
	}

	public void BallHideAnim(bool isLastBall)
	{
		IsHideAnimStart = true;
		if (gameObject.activeSelf)
			Splush(isLastBall);
	}

	public void Splush(bool isLastBall)
	{
		particleBoom[0].startColor = colors[(int)GetBallType];
		particleBoom[1].startColor = colors[(int)GetBallType];
		Path = CPath.Create()
			.Action(() =>
			{
				coll.enabled = false;
				particleBoom.ForEach(x => x.Play(true));
			}).EasingLinear(0.3f, 1, 0, value =>
			{
				resizer.transform.localScale = new Vector3(value, value, 1);
			})
			.Action(() => particleBoom.ForEach(x => x.Stop()))
			.Wait(0.5f).Action(() =>
			{
				coll.enabled = true;
				rb.simulated = false;
				ResetBall(isLastBall);
			});
	}

	public void BallFallAnim(bool isLastBall)
	{
		if (gameObject.activeSelf)
		{
			if (GetBallType == BALL_TYPE.bubble && Bottle != null)
				Bottle.ZoomBottle();
			Сrush(isLastBall);
		}
	}

	public void Сrush(bool isLastBall)
	{
		particle.startColor = colors[(int)GetBallType];
		rb.simulated = true;
		float time = Vector3.Distance(transform.position, Shooter.position) * 0.13f;
		Path = CPath.Create()
			.Action(() =>
			{
				coll.enabled = false;
				rb.simulated = true;
			}).Wait(0.2f)
			.Action(() =>
			{
				particle.Play(true);
			}).EasingLinear(time, 1, 0, value =>
			{
				resizer.transform.localScale = new Vector3(value, value, 1);
			})
			.Action(() => particle.Stop())
			.Wait(0.7f).Action(() =>
			{
				coll.enabled = true;
				rb.simulated = false;
				ResetBall(isLastBall);
			});

	}
}
