using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DotsPool : MonoBehaviour
{
	[SerializeField]
	protected GameObject dotPrefab;

	public int DotsCount => Dots != null ? Dots.Count : 0;
	public List<Vector2> Dots { get; private set; }

	private List<GameObject> dotsPool;
	private int maxDots = 26;
	private float dotGap = 0.32f;

	public void CreateDotsPool()
	{
		Dots = new List<Vector2>();
		dotsPool = new List<GameObject>();

		var i = 0;
		var alpha = 1.0f / maxDots;
		var startAlpha = 1.0f;
		while (i < maxDots)
		{
			GameObject dot = Instantiate(dotPrefab, transform);
			var sp = dot.GetComponent<SpriteRenderer>();
			var c = sp.color;

			c.a = startAlpha - alpha;
			startAlpha -= alpha;
			sp.color = c;

			dot.SetActive(false);
			dotsPool.Add(dot);
			i++;
		}
	}

	public void ClearDots()
	{
		Dots.Clear();
	}

	public void DotsDisable()
	{
		foreach (GameObject d in dotsPool)
			d.SetActive(false);
	}

	public void AddDot(Vector2 dot)
	{
		Dots.Add(dot);
	}

	public void ReplaceFinalDot(Vector2 dot)
	{
		Dots.RemoveAll(x => x.y > dot.y);
		Dots.Add(dot);
	}

	public void DrawPaths()
	{
		if (Dots.Count > 1)
		{
			DotsDisable();

			int index = 0;
			for (var i = 1; i < Dots.Count; i++)
			{
				DrawSubPath(i - 1, i, ref index);
			}
		}
	}

	private void DrawSubPath(int start, int end, ref int index)
	{
		var pathLength = Vector2.Distance(Dots[start], Dots[end]);
		int numDots = Mathf.RoundToInt((float)pathLength / dotGap);
		float dotProgress = 1.0f / numDots;

		var p = 0.0f;
		while (p < 1)
		{
			var px = Dots[start].x + p * (Dots[end].x - Dots[start].x);
			var py = Dots[start].y + p * (Dots[end].y - Dots[start].y);

			if (index < maxDots)
			{
				var d = dotsPool[index];
				d.transform.position = new Vector2(px, py);
				d.SetActive(true);
				index++;
			}

			p += dotProgress;
		}
	}
}
