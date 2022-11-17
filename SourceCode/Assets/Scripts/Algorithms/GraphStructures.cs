
using mehmetsrl.Algorithms.Graph;
using System.Collections.Generic;
using UnityEngine;

namespace mehmetsrl.Algorithms.DataStructures {

    public class Tile_Vector2Int<T>
    {
        static uint occupiedTile = 0;
        public static uint OccupiedTile { get { return occupiedTile; } private set { occupiedTile = value; } }
        T holder;
        public Vector2Int position { get; private set; }
        public Vector2Int size { get; private set; }

        public T Holder { 
            get { return holder; } 
            set {
                //Fills the holder and arrange occupied tiles
                if (holder == null && value != null)
                {
                    for (int i = 0; i < size.x * size.y; i++)
                        OccupiedTile++;
                }
                else if (holder != null && value == null)
                {
                    for (int i = 0; i < size.x * size.y; i++)
                        OccupiedTile--;
                }
                holder = value; 
            } 
        }

        public Tile_Vector2Int(T holder, Vector2Int position, Vector2Int size)
        {
            this.holder = holder;
            this.position = position;
            this.size = size;
        }
    }
    public class GraphDataStruct_Vector2IntBoard<T> : IHasNeighbours<Tile_Vector2Int<T>>
    {
        Tile_Vector2Int<T>[,] tiles;
        static Vector2Int[] neigbourDirections =
        new Vector2Int[] {
                        new Vector2Int(1,-1),
                        new Vector2Int(1,0),
                        new Vector2Int(1,1),
                        new Vector2Int(-1,-1),
                        new Vector2Int(-1,0),
                        new Vector2Int(-1,1),
                        new Vector2Int(0,1),
                        new Vector2Int(0,-1),
                        };

        public GraphDataStruct_Vector2IntBoard(ref Tile_Vector2Int<T>[,] tiles)
        {
            this.tiles = tiles;
        }

        bool TileWithinBounds(Vector2Int tilePos)
        {
            return (tilePos.x > 0 && tilePos.x < tiles.GetLength(0) && tilePos.y > 0 && tilePos.y < tiles.GetLength(1));
        }

        public IEnumerable<Tile_Vector2Int<T>> Neighbours(Tile_Vector2Int<T> tile)
        {
            List<Tile_Vector2Int<T>> neighbours = new List<Tile_Vector2Int<T>>();
            foreach (var neigbourDirection in neigbourDirections)
            {
                Vector2Int candidateNeigbourPos = tile.position + neigbourDirection;
                if (TileWithinBounds(candidateNeigbourPos))
                {
                    Tile_Vector2Int<T> candidateNeigbourPivot = tiles[candidateNeigbourPos.x, candidateNeigbourPos.y];
                    Tile_Vector2Int<T>[] candidateNeigbourArea = new Tile_Vector2Int<T>[tile.size.x* tile.size.y];

                    if (candidateNeigbour.Holder == null)
                        neighbours.Add(candidateNeigbour);
                }
            }

            return neighbours;
        }
    }
}
