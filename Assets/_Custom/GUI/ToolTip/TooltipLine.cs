using UnityEngine;

public class TooltipLine
{
    public string Left;
    public string Right;
    public Color color = Color.white;

    public TooltipLine(string left, string right = "", Color? color = null)
    {
        Left = left;
        Right = right;
        if (color != null)
            this.color = (Color)color;
    }
}