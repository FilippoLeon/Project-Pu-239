using MoonSharp.Interpreter;
using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MoonSharpUserData]
public class World : Emitter {

    Tile[,] tiles;

    public Dictionary<string, SimplePriorityQueue<Job>> jobs = new Dictionary<string, SimplePriorityQueue<Job>>();

    public static string category = "world";
    public override string Category() { return category; }

    public int sizeX, sizeY;

    List<EntityAnimated> characters = new List<EntityAnimated>();
    Emitter selected;
    public Tile selectedTile;


    public bool Paused
    {
        get
        {
            return paused;
        }
        set
        {
            paused = value;
        }
    }

    private bool paused = true;
    private int speed = 1;
    private int elapsed = 0;

    private int elapsedLong = 0;
    private int elapsedLongLong = 0;
    static private int ticLength = 4;
    static private int longTicLength = ticLength * 60;
    static private int longLongTicLength = longTicLength * 5;


    public static int[] speedRatio = new int[] { 1, 2, 4 };
    private static int maxSpeed = speedRatio.Length - 1;
    private static int minSpeed = 0;

    public EntityRegistry registry;

    public void IncreaseSpeed()
    {
        Speed = Math.Min(Speed + 1, maxSpeed);
    }

    public void DecreaseSpeed()
    {
        Speed = Math.Max(Speed - 1, minSpeed);
    }
    public void CycleSpeed()
    {
        if (Speed == maxSpeed) Speed = minSpeed;
        else Speed = Math.Min(Speed + 1, maxSpeed);
    }

    public void SetSpeed(int newSpeed)
    {
        Speed = Math.Min(Math.Max(newSpeed, minSpeed), maxSpeed);
    }

    public Emitter Selected
    {
        get
        {
            return selected;
        }

        set
        {
            selected = value;
        }
    }

    public int Speed
    {
        get
        {
            return speed;
        }

        internal set
        {
            speed = value;
            SppedChanged(speed);
        }
    }
    

    internal void Update()
    {
        if (paused) return;
        elapsed += 1;
        elapsedLong += 1;
        elapsedLongLong += 1;
        if (elapsed >= ticLength / speedRatio[Speed])
        {
            Tic();
            if (elapsedLong >= longTicLength / speedRatio[Speed])
            {
                TicLong();
                if (elapsedLongLong >= longLongTicLength / speedRatio[Speed])
                {
                    TicLongLong();
                    elapsedLongLong = 0;
                }
                elapsedLong = 0;
            }
            elapsed = 0;
        }
    }

    [MoonSharpUserData]
    public struct Coord : IFormattable
    {
        public int x, y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        private static World.Coord[] directions =
        {
            new Coord(0, 1),
            new Coord(-1, 0),
            new Coord(0, -1),
            new Coord(1, 0),
            new Coord(-1, 1),
            new Coord(-1, -1),
            new Coord(1, -1),
            new Coord(1, 1)
        };

        private static int[] order = {
            0,4,1,5,2,6,3,7
        };

        public static World.Coord Direction(int i)
        {
            return directions[i];
        }
        public static World.Coord DirectionOrdered(int i)
        {
            return directions[order[i]];
        }

        public static Coord operator+(Coord one, Coord other)
        {
            return new Coord(one.x + other.x, one.y + other.y);
        }

        public static Coord Top = directions[0];
        public static Coord Left = directions[1];
        public static Coord Bottom = directions[2];
        public static Coord Right = directions[3];
        public static Coord TopLeft = directions[4];
        public static Coord BottomLeft = directions[5];
        public static Coord BottomRight = directions[6];
        public static Coord TopRight = directions[7];

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public static implicit operator Coord(Vector2 vector)
        {
            return new Coord((int) vector.x, (int) vector.y);
        }

        public static implicit operator Coord(Vector3 vector)
        {
            return new Coord((int) vector.x, (int) vector.y);
        }

        public static Coord operator-(Coord left, Coord right)
        {
            return new Coord(left.x - right.x, left.y - right.y);
        }

        public Coord Rotate()
        {
            return new Coord(-y, x);
        }

        public string ToString(string format = null, IFormatProvider formatProvider = null)
        {
            return String.Format("{0}x{1}", x, y);
        }
    }
    
    internal void SetRegistry()
    {
        registry = new EntityRegistry();
    }

    internal EntityAnimated GetCharacter(string name)
    {
        return characters[0];
    }

    internal Tile GetTileAtCoord(Vector2 position)
    {
        return GetTileAt(new Coord(Mathf.FloorToInt(position.x + 0.5f), Mathf.FloorToInt(position.y + 0.5f)));
    }


    public World(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }

    public void Generate()
    {

        tiles = new Tile[sizeX, sizeY];
        for(int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                tiles[i, j] = new Tile(this, new Coord(i, j));
            }
        }

        WorldCreated();
    }

    public void ScheduleJob(Job prototype, object[] parameters)
    {
        Job instance = new Job(prototype);
        if(!jobs.ContainsKey(instance.jobCategory))
        {
            jobs[instance.jobCategory] = new SimplePriorityQueue<Job>();
        }
        jobs[instance.jobCategory].Enqueue(instance, 1);
        instance.Schedule(parameters);

        foreach(IWorldListener listener in listeners)
        {
            listener.JobScheduled(this, instance);
        }
    }
    
    public Tile GetTileAt(Coord coord)
    {
        try
        {
            return tiles[coord.x, coord.y];
        } catch(IndexOutOfRangeException)
        {
            Debug.LogError(String.Format("Invalid coordinate '{0}x{1}'", coord.x, coord.y));
            return null;
        }
    }
    public Tile GetTileAtOrNull(Coord coord)
    {
        if (IsValidCoordinate(coord))
            return tiles[coord.x, coord.y];
        else return null;
    }

    public void Tic()
    {
        Debug.Log("Tic");
        foreach(EntityAnimated entity in characters)
        {
            entity.Tic();
        }
    }

    public void TicLong()
    {
        Debug.Log("LongTic");

    }

    public void TicLongLong()
    {
        Debug.Log("LongLongTic");

    }

    public void PaceEntityAt()
    {
        Emit("OnPlaceEntityAt");
    }

    private bool IsValidCoordinate(Coord coord)
    {
        if (coord.x >= 0 && coord.y >= 0 && coord.x < sizeX && coord.y < sizeY) return true;
        return false;
    }
    
    //// WORLD LISTENERS
    public bool CanInstallAt(EntityBuilding entity, Coord coord)
    {
        if (!IsValidCoordinate(coord)) return false;
        return GetTileAt(coord).CanInstall(entity);
    }

    public void Spawn(Entity entity, Vector2 position)
    {
        if(entity is EntityAnimated)
        {
            characters.Add(entity as EntityAnimated);
        }

        foreach (IEmitterListener listener in listeners)
        {
            if (listener is IWorldListener) ((IWorldListener)listener).Spawn(this, entity, position);
        }

        entity.Spawn(this, position);

    }

    public void Despawn(Entity entity)
    {
        if(entity == null)
        {
            return;
        }
        if (entity is EntityAnimated)
        {
            characters.Remove(entity as EntityAnimated);
        }

        foreach (IEmitterListener listener in listeners)
        {
            if (listener is IWorldListener) ((IWorldListener) listener).Despawn(this, entity);
        }

        entity.Despawn(this);
    }


    public void InstallAt(EntityBuilding entity, Coord coord)
    {
        if(!CanInstallAt(entity, coord)) return;

        Spawn(entity, coord.ToVector2());

        foreach(IEmitterListener listener in listeners)
        {
            if(listener is IWorldListener) ((IWorldListener) listener).InstallAt(this, entity, coord);
        }

        GetTileAt(coord).Install(entity);

        Emit("OnInstallAt");
    }
    public void Uninstall(Coord coord)
    {

        Tile tile = GetTileAt(coord);
        if (tile == null || tile.building == null) return;
        EntityBuilding entity = tile.building;
        
        tile.Uninstall();
        foreach (IEmitterListener listener in listeners)
        {
            if (listener is IWorldListener) ((IWorldListener) listener).Uninstall(this, entity);
        }
        
        Despawn(entity);
        
        Emit("OnUninstallAt");
    }

    private void WorldCreated()
    {
        foreach (IWorldListener listener in listeners)
        {
            if (listener is IWorldListener) ((IWorldListener) listener).WorldCreated(this);
        }
    }

    private void SppedChanged(int speed)
    {

        foreach (IWorldListener listener in listeners)
        {
            if (listener is IWorldListener) ((IWorldListener)listener).SpeedChanged(this, speed, paused);
        }

        Emit("OnSpeedChange", new object[] { this, speed, paused });
    }
}
