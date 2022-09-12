using AxGrid.Base;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RayCastShooter : MonoBehaviourExt
{
	[SerializeField]
	protected GameObject[] colorsGO;
	[SerializeField]
	protected Ball.BALL_TYPE type;
	[SerializeField]
	protected DotsPool dots_pool; 
	[SerializeField]
	protected Bullet bullet;
	[SerializeField]
	protected Grid grid;
	[SerializeField]
	protected NextBulletHandler nextBullet;
	[SerializeField]
	protected Animator anim;

	public List<Vector2> bulletPath => dots_pool.Dots;
	public static Vector3 ShooterPos;

	private bool isNotShoot;

	[OnStart]
	private void OnStart()
	{
		ShooterPos = transform.position;
		dots_pool.CreateDotsPool();
		bullet.SetParams(this, grid);

		//select initial type
		type = Ball.BALL_TYPE.pink;
		UpdateColorType();
		nextBullet.SetNextBullet();
	}

	private void UpdateColorType()
	{
		foreach (var go in colorsGO)
		{
			go.SetActive(false);
		}
		colorsGO[((int)type) - 1].SetActive(true);
	}

	public void HandleTouchUp(Vector2 touch) //after touch or mouse down
	{
		if (bullet.IsBulletActive || dots_pool.DotsCount < 2 || isNotShoot)
			return;

		Ball finalBall = grid.GetFinalBall(dots_pool.Dots[dots_pool.Dots.Count - 1]);
		if (finalBall != null)
		{
			dots_pool.ReplaceFinalDot(finalBall.transform.position);
			bullet.FinalBall = finalBall;
		}
		ClearShotPath(); 
		bullet.BulletGo(type, transform.position);
		UpdateBulletType();
	}

	private void UpdateBulletType()
	{
		Path.Action(() =>
		{
			anim.SetFloat("next_bullet", (int)nextBullet.GetBallType);
			anim.SetTrigger("isNextBullet");
		})
			.Wait(0.1f)
			.Action(() =>
			{
				type = nextBullet.GetBallType;
				UpdateColorType();
				nextBullet.SetNextBullet();
			});
	}

	public void HandleTouchMove(Vector2 touch) //move touch or mouse
	{
		if (bullet.IsBulletActive || isNotShoot)
			return;

		dots_pool.ClearDots();

		Vector2 point = Camera.main.ScreenToWorldPoint(touch);
		Vector2 direction = new Vector2(point.x - transform.position.x, Mathf.Abs(point.y - transform.position.y));
		if (direction.y < 0.2f)
			direction = new Vector2(direction.x, 0.2f);

		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
		if (hit.collider != null)
		{
			dots_pool.AddDot(transform.position); //add first dot

			if (hit.collider.tag == "SideWall")
			{
				DoRayCast(hit, direction);
			}
			else
			{
				dots_pool.AddDot(hit.point); //add last dot
				dots_pool.DrawPaths();
			}
		}
	}

	public void ClearShotPath()
	{
		dots_pool.DotsDisable();
	}

	private void DoRayCast(RaycastHit2D previousHit, Vector2 directionIn)
	{
		dots_pool.AddDot(previousHit.point);

		var normal = Mathf.Atan2(previousHit.normal.y, previousHit.normal.x);
		var newDirection = normal + (normal - Mathf.Atan2(directionIn.y, directionIn.x));
		var reflection = new Vector2(-Mathf.Cos(newDirection), -Mathf.Sin(newDirection));
		var newCastPoint = previousHit.point + (0.25f * reflection);

		var hit2 = Physics2D.Raycast(newCastPoint, reflection);
		if (hit2.collider != null)
		{
			if (hit2.collider.tag == "SideWall")
			{
				DoRayCast(hit2, reflection);
			}
			else
			{
				dots_pool.AddDot(hit2.point);
				dots_pool.DrawPaths();
			}
		}
		else
		{
			dots_pool.DrawPaths();
		}
	}

	private void OnMouseDown()
	{
		isNotShoot = true;
	}

	private void OnMouseUp()
	{
		ClearShotPath();
		SwitchBall();
		isNotShoot = false;
	}

	public void SwitchBall()
	{
		Path.Action(() =>
		{
			anim.SetFloat("next_bullet", (int)nextBullet.GetBallType);
			anim.SetTrigger("isSwitch");
		});/*.Wait(0.49f)
			.Action(() =>
			{
				Ball.BALL_TYPE tmp_type = type;
				type = nextBullet.GetBallType;
				nextBullet.SetNextBullet(tmp_type);
				UpdateColorType();
			});*/
	}


	public void SwitchBalls()
	{
		Ball.BALL_TYPE tmp_type = type;
		type = nextBullet.GetBallType;
		nextBullet.SetNextBullet(tmp_type);
		UpdateColorType();
	}
}
