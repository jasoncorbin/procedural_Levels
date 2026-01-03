using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;
using System;
using UnityEngine.UI;

[Serializable]
public class RoomTemplate
{   
    [SerializeField] string name;
    [SerializeField] int numberOfRooms;
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;
    [SerializeField] Texture2D layoutTexture;

    public int NumberOfRooms { get => numberOfRooms; }
    public int RoomWidthMin { get => roomWidthMin; }
    public int RoomWidthMax { get => roomWidthMax; }
    public int RoomLengthMin { get => roomLengthMin; }
    public int RoomLengthMax { get => roomLengthMax; }
    public Texture2D LayoutTexture { get => layoutTexture; }
    
    public RectInt GenerateRoomCandiateRect(Random random)
    {
        if(layoutTexture != null)
        {
            return new RectInt{width = layoutTexture.width, height = layoutTexture.height};
        }
        RectInt roomRect = new RectInt{
            width = random.Next(roomWidthMin, roomWidthMax),
            height = random.Next(roomLengthMin, roomLengthMax)
        };
        return roomRect;
    }
}
