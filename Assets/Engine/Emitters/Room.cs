public class Room : Emitter
{
    int id;
    static int idCounter = 0;

    static Room defaultRoom = new Room();
    static Room impassableRoom = new Room(-1);

    public int Id
    {
        get
        {
            return id;
        }
    }

    private Room(int id)
    {
        this.id = id;
    }

    public Room()
    {
        id = idCounter++;
    }

    public static Room Default()
    {
        return defaultRoom;
    }
    public static Room Impassable()
    {
        return impassableRoom;
    }
}