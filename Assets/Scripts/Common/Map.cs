using LitJson;
using System;
using System.Collections.Generic;

public class Map
{
    public List<int> data { get; set; }
    public int height { get; set; }
    public int width { get; set; }
    public int cost { get; set; }
    public int bottleBlueDuration { get; set; }
    public int bottleGreenDuration { get; set; }
    public int bottleRedDuration { get; set; }
    public int bottleBlueGoal { get; set; }
    public int bottleGreenGoal { get; set; }
    public int bottleRedGoal { get; set; }
}