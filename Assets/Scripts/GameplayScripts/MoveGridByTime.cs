using AxGrid.Base;
using AxGrid.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MoveGridByTime : MonoBehaviourExt
{
	private const float timeDelay = 10f;

	[OnStart]
	private void Init()
	{
		if (Path == null)
			Path = new CPath();
		Move();

		GameModel.EventManager.AddEvent("game_over_show", StopMove);
		GameModel.EventManager.AddEvent("rebuild_grid", RebuildGrid);
		GameModel.EventManager.AddEvent("OnPauseOn", OnPauseOn);
		GameModel.EventManager.AddEvent("OnPauseOff", OnPauseOff);
	}

	[OnDestroy]

	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(StopMove);
		GameModel.EventManager.RemoveEvent(RebuildGrid);
		GameModel.EventManager.RemoveEvent(OnPauseOn);
		GameModel.EventManager.RemoveEvent(OnPauseOff);
	}

	private void OnPauseOn(EventArgs args)
	{
		Path = null;
	}

	private void OnPauseOff(EventArgs args)
	{
		Path = new CPath();
		Move();
	}

	private void RebuildGrid(EventArgs args)
	{
		Path = new CPath();
		Move();
	}

	private void StopMove(EventArgs args)
	{
		Path.Clear();
		Path = null;
	}

	private void Move()
	{
		if (Path == null)
			return;
		Path.Clear();
		Path.Wait(timeDelay)
			.Action(() =>
			{
				GameModel.EventManager.Invoke("move_grid");
				Move();
			});
	}
}
