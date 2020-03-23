using System;
using UnityEngine;
using _Scripts.Utility;

[Serializable]
public class Decoration
{
    [SerializeField] private Sprite decoration;
    [SerializeField] private Position2D decorationPosition;

    public Sprite GetDecorationSprites()
    {
        return decoration;
    }

    public Position2D GetDecorationPositions()
    {
        return decorationPosition;
    }
}
