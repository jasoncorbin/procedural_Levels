using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HallwayDirection{Undefined, Top, Right, Bottom, Left}   

public static class HallwayDirectionExtensions
{
    private static Color yellow = new Color(1,1,0,1);
    public static readonly Dictionary<HallwayDirection, Color> DirectionToColorMap = new Dictionary<HallwayDirection, Color>()
    {
        {HallwayDirection.Top, Color.cyan},
        {HallwayDirection.Right, Color.magenta},
        {HallwayDirection.Bottom, Color.green},
        {HallwayDirection.Left, yellow}
    };

    public static Color GetColor(this HallwayDirection direction)
    {
        return DirectionToColorMap.TryGetValue(direction, out Color color) ? color : Color.white;
    }

    public static Dictionary<Color, HallwayDirection> GetColorToDictionary()
    {
        return DirectionToColorMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    }

    public static HallwayDirection GetOppositeDirection(this HallwayDirection direction)
    {
        Dictionary<HallwayDirection, HallwayDirection> oppositesDirectionMap = new Dictionary<HallwayDirection, HallwayDirection>()
        {
            {HallwayDirection.Top, HallwayDirection.Bottom},
            {HallwayDirection.Right, HallwayDirection.Left},
            {HallwayDirection.Bottom, HallwayDirection.Top},
            {HallwayDirection.Left, HallwayDirection.Right}
        };
        return oppositesDirectionMap.TryGetValue(direction, out HallwayDirection oppositeDirection) ? oppositeDirection : HallwayDirection.Undefined;
    }

}