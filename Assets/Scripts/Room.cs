using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    RectInt area;
    public RectInt Area { get { return area; } }
    public Texture2D LayoutTexture { get; }

    public Room(RectInt area)
    {
        this.area = area;
    }
    public Room(int x, int y, Texture2D layoutTexture)
    {
        area = new RectInt(x, y, layoutTexture.width, layoutTexture.height);
        LayoutTexture = layoutTexture;

    }

    public List<Hallway> CalculateAllPossibleDoorways(int width, int length, int minDistanceFromEdge)
    {
        if (LayoutTexture == null)
            return CalculateAllPossibleDoorwaysForRectangularRoooms(width, length, minDistanceFromEdge);

        return CalculateAllPossibleDoorwayPositions(LayoutTexture);
    }


    public List<Hallway> CalculateAllPossibleDoorwaysForRectangularRoooms(int width, int length, int minDistanceFromEdge)
    {
        List<Hallway> hallwayCandidates = new List<Hallway>();

        int top = length - 1;
        int minX = minDistanceFromEdge;
        int maxX = width - minDistanceFromEdge;
        for (int x = minX; x < maxX; x++)
        {
            hallwayCandidates.Add(new Hallway(HallwayDirection.Bottom, new Vector2Int(x, 0)));
            hallwayCandidates.Add(new Hallway(HallwayDirection.Top, new Vector2Int(x, top)));
        }

        int right = width - 1;
        int minY = minDistanceFromEdge;
        int maxY = length - minDistanceFromEdge;
        for (int y = minY; y < maxY; y++)
        {
            hallwayCandidates.Add(new Hallway(HallwayDirection.Left, new Vector2Int(0, y)));
            hallwayCandidates.Add(new Hallway(HallwayDirection.Right, new Vector2Int(right, y)));
        }


        return hallwayCandidates;

    }

    List<Hallway> CalculateAllPossibleDoorwayPositions(Texture2D layoutTexture)
    {
        List<Hallway> possibleHallwayPositions = new List<Hallway>();
        int width = layoutTexture.width;
        int height = layoutTexture.height;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = layoutTexture.GetPixel(x, y);
                HallwayDirection direction = GetHallwayDirection(pixelColor);
                if (direction != HallwayDirection.Undefined)
                {
                    Hallway hallway = new Hallway(direction, new Vector2Int(x, y));
                    possibleHallwayPositions.Add(hallway);
                }

            }
        }
        return possibleHallwayPositions;

    }

    HallwayDirection GetHallwayDirection(Color Color)
    {
        Dictionary<Color, HallwayDirection> colorToDirectionMap = HallwayDirectionExtensions.GetColorToDictionary();
        return colorToDirectionMap.TryGetValue(Color, out HallwayDirection direction) ? direction : HallwayDirection.Undefined;
    }
}
