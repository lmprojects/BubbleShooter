using AxGrid.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GetCoinPopupController : PopupController
{
	[OnStart]
	public void OnStart()
	{
		container.SetActive(false);
		GameModel.EventManager.AddEvent("OpenGetCoinPopup", OpenGetCoinPopup);
	}

	[OnDestroy]
	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(OpenGetCoinPopup);
	}

	private void OpenGetCoinPopup(EventArgs args)
	{
		OnOpen();
	}

	public void ToStore()
	{
		OnClose();
		GameModel.EventManager.Invoke("OpenStorePopup");
	}


	public void ToAdv()
	{
		OnClose();
	}
}
