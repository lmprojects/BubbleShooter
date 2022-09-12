using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BottlePool : MonoBehaviour
{
	[SerializeField]
	protected GameObject bottlePref;

	private List<Bottle> bottlePool;
	private int poolCount = 50;

	private void Awake()
	{
		bottlePool = new List<Bottle>();
		for (int i = 0; i < poolCount; i++)
		{
			GameObject go = Instantiate(bottlePref, transform);
			go.name = "Bottle" + i;
			go.SetActive(false);
			Bottle botl = go.GetComponent<Bottle>();
			botl.IsEnable = true;
			bottlePool.Add(botl);
		}
	}

	public Bottle GetFreeBottle()
	{
		Bottle b = bottlePool.FirstOrDefault(x => x.IsEnable);
		b.IsEnable = false;
		b.gameObject.SetActive(true);
		return b;
	}

	public void ResetPool()
	{
		bottlePool.ForEach(x =>
		{
			x.IsEnable = true;
			x.gameObject.SetActive(false);
			x.CurBall = null;
		});
	}
}
