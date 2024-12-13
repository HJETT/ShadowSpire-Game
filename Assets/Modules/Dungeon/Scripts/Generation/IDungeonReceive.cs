namespace Dungeon.Generation
{
    public interface IDungeonReceive
    {
        public void OnLevelStart(DungeonResult level);
        public void OnLevelEnd(DungeonResult level);
    }
}