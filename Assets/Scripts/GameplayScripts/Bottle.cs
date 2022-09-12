using AxGrid.Base;
using AxGrid.Path;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Bottle : MonoBehaviourExt
{
	[SerializeField]
	protected List<Sprite> bottleSprites;
	[SerializeField]
	protected SpriteRenderer spriteR;
	[SerializeField]
	protected Animator anim;

	public enum BottleType
	{
		Bottle_blue,
		Bottle_green,
		Bottle_red
	}

	public static int GetBottleTypeCount = 3;

	public bool IsEnable { get; set; }
	public Ball CurBall { get; set; }

	private float timerDuration;
	private BottleType curType;
	private bool OnPause;

	private float pauseStartTime = 0;
	private float pauseEndTime = 0;

	[OnStart]
	private void OnStart()
	{
		GameModel.EventManager.AddEvent("OnPauseOn", OnPauseOn);
		GameModel.EventManager.AddEvent("OnPauseOff", OnPauseOff);
	}

	[OnDestroy]
	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(OnPauseOn);
		GameModel.EventManager.RemoveEvent(OnPauseOff);
	}

	private void OnPauseOn(EventArgs args)
	{
		OnPause = true; 
		pauseStartTime = Path.DeltaF;
	}

	private void OnPauseOff(EventArgs args)
	{
		OnPause = false;
		pauseEndTime = Path.DeltaF;
	}

	public void SetPosition(Vector3 pos)
	{
		transform.position = pos;
	}

	public void SetType(BottleType type, float duration)
	{
		timerDuration = duration;
		curType = type;
		spriteR.sprite = bottleSprites[(int)curType];

		if (Path == null)
			Path = new CPath();
		Path.Clear();
		Path.Add(() =>
		{
			if (CurBall.IsVisible)
			{
				StartTimer();
				return Status.OK;
			}
			return Status.Continue;
		});

	}

	private void StartTimer()
	{
		if (Path == null)
			Path = new CPath();
		Path.Clear();
		float partTime = timerDuration / 7;
		float curPartTime = 0;
		int anim_index = 0;
		float duration = 0.5f;
		float curTime = 0;
		Path.Add((p) =>
		{
			if (OnPause)
			{
				return Status.Continue;
			}
			else
			{
				curTime = pauseStartTime + p.DeltaF - pauseEndTime;
				float progress = Mathf.Clamp01(curTime / timerDuration);
				if (curTime >= curPartTime)
				{
					curPartTime += partTime;
					anim_index++;
					anim.SetFloat("time", anim_index);
				}
				return curTime < timerDuration ? Status.Continue : Status.OK;
			}
		})
			.Add((p) =>
			{
				float progress = Mathf.Clamp01(p.DeltaF / duration);
				transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress);
				return p.DeltaF < duration ? Status.Continue : Status.OK;
			})
			.Action(() => ResetBottle());
	}

	public void ZoomBottle()
	{
		anim.SetFloat("time", 8);
		float duration = 0.5f;
		if (Path == null)
			Path = new CPath();
		Path.Clear();
		Path.Add((p) =>
		{
			float progress = Mathf.Clamp01(p.DeltaF / duration);
			transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, progress);
			return p.DeltaF < duration ? Status.Continue : Status.OK;
		})
			.Add((p) =>
			{
				float progress = Mathf.Clamp01(p.DeltaF / duration);
				transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.zero, progress);
				return p.DeltaF < duration ? Status.Continue : Status.OK;
			})
			.Action(() =>
			{
				ResetBottle();
				GameModel.EventManager.Invoke("bottleCountMinus", this, curType);
			});
	}

	private void ResetBottle()
	{
		anim.SetFloat("time", 0);
		gameObject.SetActive(false);
		transform.localScale = Vector3.one;
		IsEnable = true;
		CurBall.Bottle = null;
	}
}