using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : ScriptableObject
{
    public string title;
    public Color colour;
    public string giverName;
    public string text;
    public string directions;

    public Quest(string nTitle, Color nColour, string nGiverName, string nText, string nDirections)
    {
        title = nTitle;
        colour = nColour;
        giverName = nGiverName;
        text = nText;
        directions = nDirections;
    }
}
