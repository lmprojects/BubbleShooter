using AxGrid.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StorePopupController : PopupController
{
	[OnStart]
	private void OnStart()
	{
		container.SetActive(false);
		GameModel.EventManager.AddEvent("OpenStorePopup", OpenStorePopup);
		GameModel.EventManager.AddEvent("CloseStorePopup", CloseStorePopup);
	}

	[OnDestroy]
	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(OpenStorePopup);
	}

	private void OpenStorePopup(EventArgs args)
	{
		OnOpen();
	}

	private void CloseStorePopup(EventArgs args)
	{
		OnClose();
	}
}
