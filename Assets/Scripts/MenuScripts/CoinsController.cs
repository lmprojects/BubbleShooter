using AxGrid.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinsController : MonoBehaviourExt
{
	[SerializeField]
	protected TextMeshProUGUI text;

	[OnStart]
	private void Init()
	{
		GameModel.EventManager.AddEvent("SetCoins", SetCoins);
		text.text = GameModel.Model.CoinCnt.ToString();
	}

	[OnDestroy]
	private void Destroy()
	{
		GameModel.EventManager.RemoveEvent(SetCoins);
	}

	private void SetCoins(EventArgs args)
	{
		text.text = ((int)args.Args[0]).ToString();
	}
}
