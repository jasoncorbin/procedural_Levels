using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data;
using Random = System.Random;


public class LayoutGeneratorRooms : MonoBehaviour
{
    [SerializeField] int seed = Environment.TickCount;
    [SerializeField] RoomLevelLayoutConfiguration levelConfig;


    [SerializeField] GameObject levelLayoutDisplay;
    [SerializeField] List<Hallway> openDoorways;

    Random random;
    Level level;
    Dictionary<RoomTemplate, int> availableRooms;

    [ContextMenu("Generate Level Layout")]
    
    public void GenerateLayout()
    {
        random = new Random(seed);
        availableRooms = levelConfig.GetAvailableRooms();
        openDoorways = new List<Hallway>();
        level = new Level(levelConfig.Width, levelConfig.Length);

        RoomTemplate startingRoomTemplate = availableRooms.Keys.ElementAt(random.Next(0, availableRooms.Count));
        RectInt roomRect = GetStartRoomRect(startingRoomTemplate);
        Room room = CreateNewRroom(roomRect, startingRoomTemplate);
        List<Hallway> hallways = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, levelConfig.DoorDistanceFromEdge);
        hallways.ForEach(h => h.StartRoom = room);
        hallways.ForEach(h => openDoorways.Add(h));
        level.AddRoom(room);

        Hallway selectedEntryway = openDoorways[random.Next(0, openDoorways.Count)];
        addRooms();
        Debug.Log(selectedEntryway.StartPositionAbsolute);
        DrawLayout(selectedEntryway, roomRect);


    }

    [ContextMenu("Generate New Seed")]

    public void GenerateNewSeed()
    {
        seed = Environment.TickCount;
    }
    
    [ContextMenu("Generate New Seed and Level")]

    public void GenerateNewSeedAndLevel()
    {
        GenerateNewSeed();
        GenerateLayout();
    }

    RectInt GetStartRoomRect(RoomTemplate roomTemplate)
    {
        RectInt roomSize = roomTemplate.GenerateRoomCandiateRect(random);
        int roomWidth = random.Next(roomTemplate.RoomWidthMin, roomTemplate.RoomWidthMax);
        int availableWidthX = levelConfig.Width / 2 - roomWidth;
        int randomX = random.Next(0, availableWidthX);
        int roomX = randomX + (levelConfig.Width / 4);

        int roomLength = random.Next(roomTemplate.RoomLengthMin, roomTemplate.RoomLengthMax);
        int availableLengthY = levelConfig.Length / 2 - roomLength;
        int randomY = random.Next(0, availableLengthY);
        int roomY = randomY + (levelConfig.Length / 4);

        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }

    void DrawLayout(Hallway selectedEntryway, RectInt roomCandidateRect = new RectInt(), bool isDebug = false)
    {
        var renderer = levelLayoutDisplay.GetComponent<Renderer>();

        var layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;

        layoutTexture.Reinitialize(level.Width, level.Length);
        levelLayoutDisplay.transform.localScale = new Vector3(level.Width, level.Length, 1);
        layoutTexture.FillWithColor(Color.black);

        foreach (Room room in level.Room)
        {
            if (room.LayoutTexture != null)
            {
                layoutTexture.DrawTexture(room.LayoutTexture, room.Area);
                continue;
            }
            else
            {
                layoutTexture.DrawRectangle(room.Area, Color.white);
            }
            
        }
        Array.ForEach(level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, Color.gray));
        layoutTexture.ConvertToBlackAndWhite();
        if (isDebug)
        {
            layoutTexture.DrawRectangle(roomCandidateRect, Color.blue);
            openDoorways.ForEach(hallway => layoutTexture.SetPixel(hallway.StartPositionAbsolute.x, hallway.StartPositionAbsolute.y, hallway.StartDirection.GetColor()));
        }

        if (isDebug && selectedEntryway != null)
        {
            layoutTexture.SetPixel(selectedEntryway.StartPositionAbsolute.x, selectedEntryway.StartPositionAbsolute.y, Color.red);
        }

        layoutTexture.SaveAsset();
    }


    Hallway SelectHallwayCandidate(RectInt roomCandidateRect, RoomTemplate roomTemplate, Hallway entryway)
    {
        Room room = CreateNewRroom(roomCandidateRect, roomTemplate, false);
        List<Hallway> candidates = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, levelConfig.DoorDistanceFromEdge);
        HallwayDirection requiredDirection = entryway.StartDirection.GetOppositeDirection();
        List<Hallway> filteredHallwayCandidates = candidates.Where(hallwayCandidate => hallwayCandidate.StartDirection == requiredDirection).ToList();
        return filteredHallwayCandidates.Count > 0 ? filteredHallwayCandidates[random.Next(0, filteredHallwayCandidates.Count)] : null;
    }

    Vector2Int CalculateRoomPosition(Hallway entryway, int roomWidth, int roomLength, int distace, Vector2Int endPosition)
    {
        Vector2Int roomPosition = entryway.StartPositionAbsolute;
        switch (entryway.StartDirection)
        {
            case HallwayDirection.Top:
                roomPosition.x -= roomWidth / 2;
                roomPosition.y += distace;
                break;
            case HallwayDirection.Right:
                roomPosition.x += distace;
                roomPosition.y -= roomLength / 2;
                break;
            case HallwayDirection.Bottom:
                roomPosition.x -= roomWidth / 2;
                roomPosition.y -= (roomLength + distace);
                break;
            case HallwayDirection.Left:
                roomPosition.x -= (roomWidth + distace);
                roomPosition.y -= roomLength / 2;
                break;
        }
        return roomPosition;

    }

    Room ConstructAdjacentRoom(Hallway selectedEntryway)
    {
        RoomTemplate roomTemplate = availableRooms.Keys.ElementAt(random.Next(0, availableRooms.Count));
        RectInt roomCandidateRect = roomTemplate.GenerateRoomCandiateRect(random);

        Hallway selectedExit = SelectHallwayCandidate(roomCandidateRect, roomTemplate, selectedEntryway);
        if (selectedExit == null) { return null; }
        Vector2Int endPosition = HallwayDirection.Undefined == selectedEntryway.StartDirection ? Vector2Int.zero : selectedExit.EndPosition;
        int distance = random.Next(levelConfig.HallwayLengthMin, levelConfig.HallwayLengthMax + 1);
        Vector2Int roomCandidatePosition = CalculateRoomPosition(selectedEntryway, roomCandidateRect.width, roomCandidateRect.height, distance, selectedExit.StartPosition);
        roomCandidateRect.position = roomCandidatePosition;

        if (!IsRoomCandidateValid(roomCandidateRect)) 
        { 
            return null; 
        }

        Room newRoom = CreateNewRroom(roomCandidateRect, roomTemplate);
        selectedEntryway.EndRoom = newRoom;
        selectedEntryway.EndPosition = selectedExit.StartPosition;
        return newRoom;
    }

    void addRooms()
    {
        while (openDoorways.Count > 0 && level.Room.Length < levelConfig.MaxRoomCount && availableRooms.Count > 0)
        {
            int randomIndex = random.Next(0, openDoorways.Count);
            Hallway selectedEntryway = openDoorways[randomIndex];
            openDoorways.RemoveAt(randomIndex);

            Room newRoom = ConstructAdjacentRoom(selectedEntryway);
            if (newRoom == null)
            {
                openDoorways.Remove(selectedEntryway);
                continue;
            }

            level.AddRoom(newRoom);
            level.AddHallway(selectedEntryway);
            selectedEntryway.EndRoom = newRoom;
            List<Hallway> newOpenHallways = newRoom.CalculateAllPossibleDoorways(newRoom.Area.width, newRoom.Area.height, levelConfig.DoorDistanceFromEdge);
            newOpenHallways.ForEach(hallway => hallway.StartRoom = newRoom);

            openDoorways.Remove(selectedEntryway);
            openDoorways.AddRange(newOpenHallways);
        }
    }

    bool IsRoomCandidateValid(RectInt roomCandidateRect)
    {
        RectInt levelRect = new RectInt(1, 1, levelConfig.Width - 2, levelConfig.Length - 2);
        return levelRect.Contains(roomCandidateRect) && !CheckRoomOverlap(roomCandidateRect, level.Room, level.Hallways, levelConfig.MinRoomDistance);
    }

    bool CheckRoomOverlap(RectInt roomCandidateRect, Room[] rooms, Hallway[] hallways, int minRoomDistance)
    {
        RectInt paddedRoomRect = new RectInt
        {
            x = roomCandidateRect.x - minRoomDistance,
            y = roomCandidateRect.y - minRoomDistance,
            width = roomCandidateRect.width + (2 * minRoomDistance),
            height = roomCandidateRect.height + (2 * minRoomDistance)
        };

        foreach (Room room in rooms)
        {
            if (paddedRoomRect.Overlaps(room.Area))
                return true;
        }
        foreach (Hallway hallway in hallways)
        {
            if (paddedRoomRect.Overlaps(hallway.Area))
                return true;
        }

        return false;
    }

    private void UseUpRoomTemplate(RoomTemplate roomTemplate)
    {
        availableRooms[roomTemplate] -= 1;
        if (availableRooms[roomTemplate] == 0)
        {
            availableRooms.Remove(roomTemplate);
        }
    }


    Room CreateNewRroom(RectInt roomCandidateRect, RoomTemplate roomTemplate, bool useUp = true)
    {
        if (useUp)
        {
            UseUpRoomTemplate(roomTemplate);
        }
        if (roomTemplate.LayoutTexture == null)
        {
            return new Room(roomCandidateRect);
        }
        else
        {
            return new Room(roomCandidateRect.x, roomCandidateRect.y, roomTemplate.LayoutTexture);
        }
    }

}