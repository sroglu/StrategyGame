
using mehmetsrl.Algorithms.Graph;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mehmetsrl.Algorithms.DataStructures {

    /// <summary>
    /// Generic Vector2Int node.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tile_Vector2Int<T>
    {
        static uint occupiedTile = 0;
        public static uint OccupiedTile { get { return occupiedTile; } private set { occupiedTile = value; } }
        T holder;
        public Vector2Int position { get; private set; }

        public T Holder { 
            get { return holder; } 
            set {
                if (holder == null && value != null) OccupiedTile++;
                else if (holder != null && value == null) OccupiedTile--;
                holder = value; 
            } 
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="position"></param>
        public Tile_Vector2Int(T holder, Vector2Int position)
        {
            this.holder = holder;
            this.position = position;
        }

        /// <summary>
        /// Equal function must be implemented for comparisons
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var otherTile = obj as Tile_Vector2Int<T>;

            if (otherTile == null) return false;

            return (this.position == otherTile.position);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    /// <summary>
    /// Data struct for algorithms that uses neigbourhood relation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GraphDataStruct_Vector2IntBoard<T> : IHasNeighbours<Tile_Vector2Int<T>>
    {
        Tile_Vector2Int<T>[,] tiles;
        static readonly Vector2Int[] neigbourDirections =
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

        /// <summary>
        /// Constructor method. In case of any changes it takes ref of tiles
        /// </summary>
        /// <param name="tiles"></param>
        public GraphDataStruct_Vector2IntBoard(ref Tile_Vector2Int<T>[,] tiles)
        {
            this.tiles = tiles;
        }

        /// <summary>
        /// Checks the node is within the gameboard bounds.
        /// </summary>
        /// <param name="tilePos"></param>
        /// <returns></returns>
        bool TileWithinBounds(Vector2Int tilePos)
        {
            return (tilePos.x >= 0 && tilePos.x < tiles.GetLength(0) && tilePos.y >= 0 && tilePos.y < tiles.GetLength(1));
        }

        /// <summary>
        /// Return neigbours of a spesific node.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public IEnumerable<Tile_Vector2Int<T>> Neighbours(Tile_Vector2Int<T> tile)
        {
            List<Tile_Vector2Int<T>> neighbours = new List<Tile_Vector2Int<T>>();
            foreach (var neigbourDirection in neigbourDirections)
            {
                Vector2Int candidateNeigbourPos = tile.position + neigbourDirection;
                if (TileWithinBounds(candidateNeigbourPos))
                {
                    Tile_Vector2Int<T> candidateNeigbour = tiles[candidateNeigbourPos.x, candidateNeigbourPos.y];

                    if (candidateNeigbour.Holder == null)
                    {
                        neighbours.Add(candidateNeigbour);
                    }
                }
            }

            return neighbours.AsEnumerable();
        }
    }
}
