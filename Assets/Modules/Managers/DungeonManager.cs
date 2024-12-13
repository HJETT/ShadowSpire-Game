using System.Collections;
using System.Linq;
using Dungeon.Drawers;
using Dungeon.Drawers.Rooms;
using Dungeon.Drawers.Terrain;
using Dungeon.Generation;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Managers
{
    public class DungeonManager : MonoBehaviour
    {
        #region Ground

        [Header("Ground")]
        [SerializeField]
        private Tilemap groundMap;

        [SerializeField]
        private TileBase groundTile;

        #endregion

        #region Walls

        [Header("Walls")]
        [SerializeField]
        private Tilemap wallMap;

        // TOP    = 0b0001 = 1
        // RIGHT  = 0b0010 = 2
        // BOTTOM = 0b0100 = 4
        // LEFT   = 0b1000 = 8
        [SerializeField]
        private TileBase[] wallTiles;

        #endregion

        #region Doors

        [Header("Doors")]
        [SerializeField]
        private TileBase doorTile;

        #endregion

        #region Entities

        [Header("Entities")]

        [SerializeField]
        private EntranceEntity entrance;

        [SerializeField]
        private ExitEntity exit;

        #endregion

        #region Enemies

        [Header("Enemies")]
        [SerializeField]
        private GameObject enemyPrefab;

        [SerializeField]
        private GameObject spikesPrefab;

        #endregion

        #region Treasures

        [Header("Treasures")]
        [SerializeField]
        private GameObject potionPrefab;

        #endregion

        #region UI

        [Header("UI")]
        [SerializeField]
        private Graphic blackout;

        [SerializeField]
        private TextMeshProUGUI transitionLevelText;
        private string originalLevelText;

        private IEnumerator BlackoutFadeIn(int transitionTicks)
        {
            var blackoutColor = blackout.color;
            blackoutColor.a = 0;

            blackout.color = blackoutColor;
            blackout.gameObject.SetActive(true);

            for (int i = 1; i <= transitionTicks; i++)
            {
                blackoutColor.a = 1f / transitionTicks * i;
                blackout.color = blackoutColor;
                yield return new WaitForSeconds(0.2f);
            }
        }

        private IEnumerator BlackoutFadeOut(int transitionTicks)
        {
            var blackoutColor = blackout.color;
            blackoutColor.a = 1;

            blackout.color = blackoutColor;
            blackout.gameObject.SetActive(true);

            for (int i = 1; i <= transitionTicks; i++)
            {
                blackoutColor.a = 1f - 1f / transitionTicks * i;
                blackout.color = blackoutColor;
                yield return new WaitForSeconds(0.2f);
            }

            blackout.gameObject.SetActive(false);
        }

        #endregion

        #region Generation

        private Drawer[] DrawerPipeline;
        private IDungeonReceive[] Receivers;

        public DungeonResult StartLevel(DungeonSettings settings, PlayerEntity player)
        {
            // Generate level
            var random = new System.Random(settings.Seed);
            var lvl = DungeonGenerator.Generate(random, settings);

            // Create drawers
            DrawerPipeline = new Drawer[]
            {
                // Terrain
                new WallDrawer(lvl, wallMap, wallTiles),
                new DoorDrawer(lvl, wallMap, doorTile),
                new EntranceExitDrawer(lvl, entrance, exit, player),
                new GroundDrawer(lvl, groundMap, groundTile),

                // Rooms
                new EnemyRoomDrawer(lvl, enemyPrefab, GameManager.Instance.allEnemies),
                new TreasureRoomDrawer(lvl, potionPrefab),
                new SpikesRoomDrawer(lvl, spikesPrefab),
            };

            // Process the level
            lvl.Player = player;
            lvl.Random = random;
            lvl.Grid = Drawer.CreateEmpty(lvl.Rooms);
            lvl.Height = lvl.Grid.GetLength(0);
            lvl.Width = lvl.Grid.GetLength(1);

            foreach (var drawer in DrawerPipeline)
                drawer.Process(lvl.Rooms);

            // Compute graphs
            lvl.TileGraph = PathFindingManager.ComputeTileGraph(lvl);

            // Draw the level
            foreach (var drawer in DrawerPipeline)
                drawer.Draw(lvl.Rooms);

            // Notify all receivers
            Receivers = FindObjectsOfType<MonoBehaviour>().Where(m => m is IDungeonReceive).Select(m => m as IDungeonReceive).ToArray();

            foreach (var receiver in Receivers)
                receiver.OnLevelStart(lvl);

            return lvl;
        }

        public IEnumerator EndLevel(int currentLevel, int nextLevel, System.Func<IEnumerator> callback = null)
        {
            const int BLACKOUT_TICKS = 4;

            yield return new WaitForSeconds(0.2f);

            yield return BlackoutFadeIn(BLACKOUT_TICKS);

            yield return new WaitForSeconds(0.6f);

            originalLevelText ??= transitionLevelText.text;

            transitionLevelText.gameObject.SetActive(true);
            transitionLevelText.text = string.Format(originalLevelText, currentLevel);

            yield return new WaitForSeconds(1.5f);

            transitionLevelText.text = string.Format(originalLevelText, nextLevel);

            yield return new WaitForSeconds(2f);

            transitionLevelText.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.2f); // Level end animation

            ClearDungeon();

            yield return null; // Wait 1 frame

            if (callback != null)
                yield return callback.Invoke();

            yield return BlackoutFadeOut(BLACKOUT_TICKS);

            yield return new WaitForSeconds(0.2f);
        }

        public void ClearDungeon()
        {
            // Clear all drawers
            foreach (var drawer in DrawerPipeline)
                drawer.Clear();
        }

        #endregion
    }
}