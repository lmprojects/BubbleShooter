using UnityEngine;
using System.Collections;
using System.Threading;
using System;

public class GameController : MonoBehaviour
{
	[SerializeField]
	protected RayCastShooter shooter;

	private bool mouseDown = false;
	private bool lockScreen;

	private void Start()
	{
		GameModel.EventManager.AddEvent("lock_screen", LockScreen);
		GameModel.EventManager.AddEvent("unlock_screen", UnlockScreen);
	}

	private void LockScreen(EventArgs args)
	{
		lockScreen = true;
	}

	private void UnlockScreen(EventArgs args)
	{
		lockScreen = false;
		mouseDown = false;
	}

	private void Update()
	{
		if (lockScreen)
		{
			shooter.ClearShotPath();
			return;
		}
		if (Input.touches.Length > 0)
		{
			Touch touch = Input.touches[0];
			if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
				TouchUp(Input.mousePosition);
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
				TouchMove(touch.position);

			TouchMove(touch.position);
			return;
		}
		else if (Input.GetMouseButtonDown(0))
		{
			mouseDown = true;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			mouseDown = false;
			TouchUp(Input.mousePosition);
		}
		else if (mouseDown)
		{
			TouchMove(Input.mousePosition);
		}
	}

	private void TouchUp(Vector2 touch)
	{
		if (shooter == null || lockScreen)
			return;
		Vector2 point = Camera.main.ScreenToWorldPoint(touch);
		if (Vector2.Distance(point, shooter.transform.position) < 0.2f)
			shooter.ClearShotPath();
		else
			shooter.HandleTouchUp(touch);
	}

	private void TouchMove(Vector2 touch)
	{
		if (shooter == null || lockScreen)
			return;
		shooter.HandleTouchMove(touch);
	}


}
