using PlayerIOClient;
namespace PenguinSdk
{

    public class PenguinCoinDoor : PenguinBlock
    {
        public int CoinsRequired { get; set; }

        public override void Build(PenguinMap map, Connection c)
        {
            c.Send(map.WorldKey,
                Z,
                X,
                Y,
                ID,
                CoinsRequired
            );
        }

        public PenguinCoinDoor(int x, int y, int coinsRequired)
            : base(x, y, 0, PenguinIds.Action.Doors.Coin, 0)
        {
            this.CoinsRequired = coinsRequired;
        }
    }
}
