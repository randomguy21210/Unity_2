using System;
using UnityEngine;

public static class Navigation
{
    public static PlayerController player;
    public static float weaponDistance = 1f;
    public static float enemyRange = 4f;
    public static int mapx = 60;
    public static int mapy = 60;
    public static int[,] directions = new int[mapx,mapy];
    public static float tilesize = 0.5f;
    //L,R,D,U
    public static Vector2Int[] diri = {new Vector2Int(-1,0),new Vector2Int(1,0),new Vector2Int(0,-1),new Vector2Int(0,1)};
    public static Vector2[] oppdir = {new Vector2(1,0),new Vector2(-1,0),new Vector2(0,1), new Vector2(0,-1)};
}
