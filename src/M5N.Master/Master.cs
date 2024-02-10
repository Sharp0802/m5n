namespace M5N.Master;

public class Master
{
    public Master(MasterChannel user0, MasterChannel user1)
    {
        User0 = user0;
        User1 = user1;
    }

    public MasterChannel User0 { get; }
    public MasterChannel User1 { get; }

    public int Run()
    {
        return 0;
    }
}