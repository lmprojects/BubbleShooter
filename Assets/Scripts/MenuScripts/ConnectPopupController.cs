using AxGrid.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ConnectPopupController : PopupController
{
	private bool isFromSetting;

	[OnStart]
	public void OnStart()
	{
		container.SetActive(false);
		GameModel.EventManager.AddEvent("OpenConnectPopup", OpenConnectPopup);
	}

	[OnDestroy]
	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(OpenConnectPopup);
	}

	private void OpenConnectPopup(EventArgs args)
	{
		OpenPopup();
		isFromSetting = true;
	}

	public void OpenPopup()
	{
		base.OnOpen();
		isFromSetting = false;
	}

	public void ClosePopup()
	{
		base.OnClose();
		if (isFromSetting)
			GameModel.EventManager.Invoke("OpenSettingPopup");
		isFromSetting = false;
	}
}
