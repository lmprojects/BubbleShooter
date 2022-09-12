using AxGrid.Base;
using AxGrid.Path;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupController : MonoBehaviourExt
{
	[SerializeField]
	protected GameObject container;
	[SerializeField]
	protected Button openBtn;

	public void OnOpen()
	{
		GameModel.EventManager.Invoke("OnPauseOn");
		GameModel.EventManager.Invoke("BlockLevelsBtn");
		if (openBtn != null)
			openBtn.interactable = false;
		container.SetActive(true);
		SmoothShow();
	}

	private void SmoothShow()
	{
		List<Image> imgs = GetComponentsInChildren<Image>().ToList();
		List<TextMeshProUGUI> tmps = GetComponentsInChildren<TextMeshProUGUI>().ToList();
		float duration = 0.2f;

		Path = new CPath()
			.Add((p) =>
			{
				float progress = Mathf.Clamp01(p.DeltaF / duration);
				foreach(Image img in imgs)
					img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Lerp(0, 1, progress));
				foreach (TextMeshProUGUI tmp in tmps)
					tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, Mathf.Lerp(0, 1, progress));
				return p.DeltaF < duration ? Status.Continue : Status.OK;
			});
	}

	public void OnClose()
	{
		GameModel.EventManager.Invoke("OnPauseOff");
		GameModel.EventManager.Invoke("UnblockLevelsBtn");
		if (openBtn != null)
			openBtn.interactable = true;
		container.SetActive(false);
	}
}
