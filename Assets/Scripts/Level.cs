using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Level
{
    int width;
    int length;
    List<Hallway> hallways;
    List<Room> rooms;  

    public int Width => width;
    public int Length => length;

    public Hallway[] Hallways => hallways.ToArray();
    public Room[] Room => rooms.ToArray();

    public Level(int width, int length)
    {
        this.width = width;
        this.length = length;
        hallways = new List<Hallway>();
        rooms = new List<Room>();
    }

    public void AddRoom(Room newRoom) => rooms.Add(newRoom);
    public void AddHallway(Hallway hallway) => hallways.Add(hallway);

}
