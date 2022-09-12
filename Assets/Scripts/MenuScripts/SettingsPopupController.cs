using AxGrid.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPopupController : PopupController
{
	[SerializeField]
	protected Image soundImg;
	[SerializeField]
	protected List<Sprite> soundSprites;
	[SerializeField]
	protected Image musicImg;
	[SerializeField]
	protected List<Sprite> musicSprites;

	[OnStart]
	private void OnStart()
	{
		container.SetActive(false);
		GameModel.EventManager.AddEvent("OpenSettingPopup", OpenSettingPopup);
	}

	[OnDestroy]
	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(OpenSettingPopup);
	}

	private void OpenSettingPopup(EventArgs args)
	{
		OnOpen();
	}

	public void OnSound()
	{
		GameModel.Model.IsSoundOn = !GameModel.Model.IsSoundOn;
		soundImg.sprite = GameModel.Model.IsSoundOn ? soundSprites[0] : soundSprites[1];
	}

	public void OnMusic()
	{
		GameModel.Model.IsMusicOn = !GameModel.Model.IsMusicOn;
		musicImg.sprite = GameModel.Model.IsMusicOn ? musicSprites[0] : musicSprites[1];
	}

	public void OnConnect()
	{
		OnClose();
		GameModel.EventManager.Invoke("OpenConnectPopup");
	}

	public void OnSupport()
	{

	}

	public void OnService()
	{

	}
	public void OnCreators()
	{

	}

	public void OnMainMenu()
	{
		OnClose();
		SceneManager.LoadScene("MenuScene");
	}
}