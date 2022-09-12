using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
	const int MapCount = 200;
	private void Start()
	{
		GameModel.Model.maps = new List<Map>();
		for(int j = 1; j < MapCount; j++)
		{
			Map map = new Map();
			var json_text = Resources.Load<TextAsset>("Maps/map" + j);
			if (json_text == null || string.IsNullOrEmpty(json_text.text))
			{
				//throw new System.Exception("Level not found!");
				break;
			}
			map = JsonMapper.ToObject<Map>(json_text.text);

			JsonData data = JsonMapper.ToObject(json_text.text);
			map.data = new List<int>();
			for (int i = data["layers"][0]["data"].Count - 1; i >= 0; i--)
			{
				map.data.Add((int)data["layers"][0]["data"][i]);
			}

			JsonData d = JsonMapper.ToObject(json_text.text);
			for (int i = 0; i < d["properties"].Count; i++)
			{
				string name = (string)d["properties"][i]["name"];
				int value = (int)d["properties"][i]["value"];
				switch (name)
				{
					default:
					case "cost":
						map.cost = value;
						break;
					case "bottle_blue":
						map.bottleBlueDuration = value;
						break;
					case "bottle_green":
						map.bottleGreenDuration = value;
						break;
					case "bottle_red":
						map.bottleRedDuration = value;
						break;
					case "bottle_blue_goal":
						map.bottleBlueGoal = value;
						break;
					case "bottle_green_goal":
						map.bottleGreenGoal = value;
						break;
					case "bottle_red_goal":
						map.bottleRedGoal = value;
						break;
				}
			}

			GameModel.Model.maps.Add(map);
		}
	}
}
