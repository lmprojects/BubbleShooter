using UnityEngine;

public class NextBulletHandler : MonoBehaviour
{
	[SerializeField]
	protected GameObject[] colorsGO;

	public Ball.BALL_TYPE GetBallType { get; private set; }

	public void SetType(Ball.BALL_TYPE type)
	{
		foreach (var go in colorsGO)
			go.SetActive(false);

		this.GetBallType = type;
		colorsGO[((int)type) -1].SetActive(true);
	}

	public void SetNextBullet()
	{ 
		GetBallType = (Ball.BALL_TYPE)Random.Range(2, Ball.GetBallTypesCount);
		SetType(GetBallType);
	}

	public void SetNextBullet(Ball.BALL_TYPE new_type)
	{
		GetBallType = new_type;
		SetType(GetBallType);
	}
}
