using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
	[SerializeField]
	protected GameObject starsContainer;
	[SerializeField]
	protected List<Image> stars;
	[SerializeField]
	protected GameObject bonus;
	[SerializeField]
	protected TextMeshProUGUI lvlText;
	[SerializeField]
	protected Image lvlSprite;
	[SerializeField]
	protected List<Sprite> levelSprites;

	private int lvl;
	public int CurLevel { 
		get
		{
			return lvl;
		}
		set
		{
			lvl = value;
			lvlText.text = lvl.ToString();
		} 
	}

	private bool isAvailable;
	public bool IsAvailable
	{
		get
		{
			return isAvailable;
		}
		set
		{
			isAvailable = value;
			starsContainer.SetActive(isAvailable);
			lvlSprite.sprite = isAvailable ? levelSprites[0] : levelSprites[1];
			lvlText.gameObject.SetActive(isAvailable);
		}
	}

	private bool isBlocked;

	private void Start()
	{
		GameModel.EventManager.AddEvent("BlockLevelsBtn", BlockLevelsBtn);
		GameModel.EventManager.AddEvent("UnblockLevelsBtn", UnblockLevelsBtn);
	}

	private void BlockLevelsBtn(EventArgs args)
	{
		isBlocked = true;
	}

	private void UnblockLevelsBtn(EventArgs args)
	{
		isBlocked = false;
	}

	public void ResetAll()
	{
		bonus.SetActive(false);
	}

	private void OnMouseUp()
	{
		if (isBlocked)
			return;

		GameModel.EventManager.Invoke("BlockLevelsBtn");
		GameModel.EventManager.Invoke("OpenLevelPopup", this, CurLevel);
	}

	public void SetStars(float points)
	{
		stars.ForEach(x => x.fillAmount = 0);
		if (points < 1)
			stars[0].fillAmount = points;
		else if (points >= 1 && points < 2)
		{
			stars[0].fillAmount = 1;
			stars[1].fillAmount = points - 1;
		}
		if (points >= 2)
		{
			stars[0].fillAmount = 1;
			stars[1].fillAmount = 1;
			stars[2].fillAmount = points - 2;
		}
	}
}
