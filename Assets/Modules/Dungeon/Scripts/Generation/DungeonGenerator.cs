using System;
using System.Collections.Generic;
using Dungeon.Drawers;
using Dungeon.Drawers.Terrain;
using UnityEngine;
using Random = System.Random;

namespace Dungeon.Generation
{
    public class DungeonGenerator
    {
        private Random random;
        private DungeonSettings settings;

        private DungeonGenerator(Random random, DungeonSettings settings)
        {
            this.random = random;
            this.settings = settings;
            this.settings.MinimumRoomWidth++; // Add right wall
            this.settings.MinimumRoomHeight++; // Add down wall
        }

        public static DungeonResult Generate(Random random, DungeonSettings settings) => new DungeonGenerator(random, settings).Generate();

        private DungeonResult Generate()
        {
            var root = new Room
            {
                Width = settings.Width,
                Height = settings.Height
            };

            // Slices the root into smaller rooms
            SlicesRooms(root, settings.SliceCount, settings.MinimumRoomWidth, settings.MinimumRoomHeight);

            // Compile all the rooms
            Room[] rooms = CompileRooms(root);

            Room entrance = EntranceExitDrawer.FindEntrance(rooms);
            entrance.Type = RoomType.ENTRANCE;

            Room exit = EntranceExitDrawer.FindExit(rooms, entrance);
            exit.Type = RoomType.EXIT;

            // Find adjacent rooms
            Dictionary<Room, HashSet<Room>> adjacentRooms = FindAdjacentRooms(rooms);

            if (rooms.Length > 1)
                CutConnections(rooms, adjacentRooms, entrance);

            // Find room types
            FindEnemyRooms(rooms);
            FindTreasureRooms(rooms);
            FindSpikesRooms(rooms);

            // Package results
            return new DungeonResult
            {
                Index = settings.Index,
                Rooms = rooms,
                AdjacentRooms = adjacentRooms,
                Entrance = entrance,
                Exit = exit,
                Width = settings.Width,
                Height = settings.Height,
            };
        }

        #region Rooms

        /// <summary>
        /// Slices the given room into smaller rooms
        /// </summary>
        private void SlicesRooms(Room root, int sliceCount, int minRoomWidth, int minRoomHeight)
        {
            var rooms = new Queue<Room>();
            rooms.Enqueue(root);

            var slicesLeft = sliceCount;

            while (rooms.Count > 0 && slicesLeft > 0)
            {
                var room = rooms.Dequeue();

                // Split
                bool sliced = room.Split(random, minRoomWidth, minRoomHeight);

                if (sliced)
                {
                    foreach (var item in room.Children)
                        rooms.Enqueue(item);
                    slicesLeft--;
                }
            }
        }

        /// <summary>
        /// Finds all the rooms from the given root
        /// </summary>
        private Room[] CompileRooms(Room root)
        {
            Stack<Room> roomsToExplore = new();
            roomsToExplore.Push(root);

            List<Room> rooms = new();

            while (roomsToExplore.Count > 0)
            {
                var room = roomsToExplore.Pop();

                // If has children, keep going
                if (room.Children != null)
                {
                    foreach (var item in room.Children)
                        roomsToExplore.Push(item);
                    continue;
                }

                rooms.Add(room);
            }

            rooms.Reverse();

            return rooms.ToArray();
        }

        #endregion

        #region Doors

        /// <summary>
        /// Creates a map of all the rooms and all the rooms they are connected to
        /// </summary>
        private Dictionary<Room, HashSet<Room>> FindAdjacentRooms(Room[] rooms)
        {
            var roomLinks = new Dictionary<Room, HashSet<Room>>();

            foreach (var firstRoom in rooms)
            {
                foreach (var secondRoom in rooms)
                {
                    // If same room, skip
                    if (firstRoom == secondRoom)
                        continue;

                    if (!firstRoom.IsAdjacent(secondRoom))
                        continue;

                    if (!roomLinks.ContainsKey(firstRoom))
                        roomLinks.Add(firstRoom, new());

                    if (!roomLinks.ContainsKey(secondRoom))
                        roomLinks.Add(secondRoom, new());

                    roomLinks[firstRoom].Add(secondRoom);
                    roomLinks[secondRoom].Add(firstRoom);
                }
            }

            return roomLinks;
        }

        /// <summary>
        /// Removes some doors to make a path of rooms
        /// </summary>
        private void CutConnections(Room[] rooms, Dictionary<Room, HashSet<Room>> adjacentRooms, Room entrance)
        {
            // Initialize all the depths to -1
            foreach (var room in rooms)
                room.Depth = -1;

            // Set the start depth to 0
            entrance.Depth = 0;

            Stack<Room> roomsToExplore = new();
            roomsToExplore.Push(entrance);

            while (roomsToExplore.Count > 0)
            {
                var currentRoom = roomsToExplore.Peek();

                // Find all the neighbors that have not been explored
                var neighbors = new List<Room>();

                foreach (var adjacentRoom in adjacentRooms[currentRoom])
                {
                    if (adjacentRoom.Depth != -1)
                        continue;

                    neighbors.Add(adjacentRoom);
                }

                // If no unexplored neighbor, continue
                if (neighbors.Count == 0)
                {
                    roomsToExplore.Pop();
                    continue;
                }

                // Pick a random unexplored neighbor
                var nextRoom = neighbors[random.Next(0, neighbors.Count)];

                // Increase it's depth by 1
                nextRoom.Depth = currentRoom.Depth + 1;

                // Add next room to the stack
                roomsToExplore.Push(nextRoom);
            }

            var highLoop = CreateLoop(
                rooms,
                adjacentRooms,
                (c, f) => c.Depth > f.Depth,
                (c, f) => c.Depth < f.Depth
            );

            var lowLoop = CreateLoop(
                rooms,
                adjacentRooms,
                (c, f) => c.Depth < f.Depth,
                (c, f) => c.Depth > f.Depth
            );

            // Remove extra doors
            foreach (var (room, otherRooms) in adjacentRooms)
            {
                IEnumerator<Room> _rooms = otherRooms.GetEnumerator();
                List<Room> roomsToRemove = new();

                while (_rooms.MoveNext())
                {
                    // If the difference is higher than 1
                    if (Mathf.Abs(room.Depth - _rooms.Current.Depth) > 1)
                        roomsToRemove.Add(_rooms.Current);
                }
                _rooms.Dispose();

                foreach (var item in roomsToRemove)
                    otherRooms.Remove(item);
            }

            // Create the door between the loops
            if (settings.AddHighLoop)
            {
                adjacentRooms[highLoop.Item1].Add(highLoop.Item2);
                adjacentRooms[highLoop.Item2].Add(highLoop.Item1);
            }

            if (settings.AddLowLoop)
            {
                adjacentRooms[lowLoop.Item1].Add(lowLoop.Item2);
                adjacentRooms[lowLoop.Item2].Add(lowLoop.Item1);
            }
        }

        private (Room, Room) CreateLoop(
            Room[] rooms,
            Dictionary<Room, HashSet<Room>> adjacentRooms,
            Func<Room, Room, bool> roomToStartFrom,
            Func<Room, Room, bool> neighborToChoose
        )
        {
            // Find the room with the highest depth
            Room furthestRoom = rooms[0];

            for (int i = 1; i < rooms.Length; i++)
            {
                if (roomToStartFrom.Invoke(rooms[i], furthestRoom))
                    furthestRoom = rooms[i];
            }

            // Find the neighbor with the lowest depth
            IEnumerator<Room> furthestRoomNeighbors = adjacentRooms.GetValueOrDefault(furthestRoom).GetEnumerator();
            furthestRoomNeighbors.MoveNext();

            Room lowestNeighbor = furthestRoomNeighbors.Current;

            while (furthestRoomNeighbors.MoveNext())
            {
                if (neighborToChoose.Invoke(furthestRoomNeighbors.Current, lowestNeighbor))
                    lowestNeighbor = furthestRoomNeighbors.Current;
            }
            furthestRoomNeighbors.Dispose();

            return (furthestRoom, lowestNeighbor);
        }

        #endregion

        #region Type

        public const int ENEMY_ROOM_INDEX = 2;
        public const int WEAPON_INDEX = 2;
        public const int TREASURE_ROOM_INDEX = 3;
        public const int SPIKES_ROOM_INDEX = 4;

        private void FindEnemyRooms(Room[] rooms)
        {
            if (settings.Index < ENEMY_ROOM_INDEX)
                return;

            var validRooms = new List<Room>();

            foreach (var room in rooms)
            {
                // If too close from the entrance, skip
                if (room.Depth <= 1 && settings.Index > ENEMY_ROOM_INDEX)
                    continue;

                validRooms.Add(room);
            }

            // If not valid room, skip
            if (validRooms.Count == 0)
                return;

            int count = rooms.Length / 3;

            if (count >= validRooms.Count)
                return;

            for (int i = 0; i < count; i++)
            {
                int rdmIndex = random.Next(0, validRooms.Count);
                (validRooms[^1], validRooms[rdmIndex]) = (validRooms[rdmIndex], validRooms[^1]);
            }

            for (int i = 0; i < count; i++)
                validRooms[^(count + 1)].Type = RoomType.ENEMY;
        }

        private void FindTreasureRooms(Room[] rooms)
        {
            if (settings.Index < TREASURE_ROOM_INDEX)
                return;

            // Find deepest
            Room deepest = null;

            for (int i = 0; i < rooms.Length; i++)
            {
                var room = rooms[i];

                if (room.Type != RoomType.NORMAL)
                    continue;

                if (deepest == null || room.Depth > deepest.Depth)
                    deepest = room;
            }

            if (deepest == null)
                return;

            deepest.Type = RoomType.TREASURE;
        }

        private void FindSpikesRooms(Room[] rooms)
        {
            if (settings.Index < TREASURE_ROOM_INDEX)
                return;

            // Find random
            Room randomRoom = null;

            for (int i = 0; i < rooms.Length; i++)
            {
                var room = rooms[i];

                if (room.Type != RoomType.NORMAL)
                    continue;

                if (randomRoom == null)
                {
                    randomRoom = room;
                    continue;
                }

                // 50% replace 50% keep
                if (random.Next(0, 2) == 0)
                    randomRoom = room;
            }

            if (randomRoom == null)
                return;

            randomRoom.Type = RoomType.SPIKES;
        }

        #endregion
    }
}