using AxGrid.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EventsPopupController : PopupController
{
	[OnStart]
	private void OnStart()
	{
		container.SetActive(false);
		GameModel.EventManager.AddEvent("OpenEventsPopup", OpenEventsPopup);
	}

	[OnDestroy]
	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(OpenEventsPopup);
	}

	private void OpenEventsPopup(EventArgs args)
	{
		OnOpen();
	}
}
