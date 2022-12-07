using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using mehmetsrl.Algorithms.DataStructures;

namespace mehmetsrl.Algorithms.Graph
{
	/// <summary>
	/// Generic Implementation of the A* algorithm.
	/// I found it at https://gist.github.com/THeK3nger/7734169
	/// I added Dijkstra implementation
	/// </summary>
	public class AStar
	{

		#region ProfilingCollection
		// Profiling Info
		static public bool CollectProfiling = false;
		static public Dictionary<string, float> LastRunProfilingInfo = new Dictionary<string, float>();
		//---------------
		#endregion

		/// <summary>
		/// Finds the optimal path between start and destionation TNode.
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="start">Starting Node.</param>
		/// <param name="destination">Destination Node.</param>
		/// <param name="distance">Function to compute distance beween nodes.</param>
		/// <param name="estimate">Function to estimate the remaining cost for the goal.</param>
		/// <typeparam name="TNode">Any class implement IHasNeighbours.</typeparam>
		static public Path<TNode> FindPath<TNode>(
			IHasNeighbours<TNode> dataStructure,
			TNode start,
			TNode destination,
			Func<TNode, TNode, double> distance,
			Func<TNode, TNode, double> estimate)
		{
			// Profiling Information
			float expandedNodes = 0;
			float elapsedTime = 0;
			Stopwatch st = new Stopwatch();
			//----------------------
			var closed = new HashSet<TNode>();
			var queue = new PriorityQueue<double, Path<TNode>>();
			queue.Add(0, new Path<TNode>(start));
			if (CollectProfiling) st.Start();
			while (!queue.IsEmpty)
			{
				var path = queue.PopMin().Value;

				if (closed.Contains(path.LastStep))
					continue;
				if (path.LastStep.Equals(destination))
				{
					if (CollectProfiling)
					{
						st.Stop();
						LastRunProfilingInfo["Expanded Nodes"] = expandedNodes;
						LastRunProfilingInfo["Elapsed Time"] = st.ElapsedTicks;
					}
					return path;
				}
				//UnityEngine.Debug.Log("Last Step: " + path.LastStep);
				closed.Add(path.LastStep);
				expandedNodes++;
				foreach (TNode n in dataStructure.Neighbours(path.LastStep))
				{
					//UnityEngine.Debug.Log ("Neigbours: " + n.ToString ());
					if (!closed.Contains(n))
					{
						double d = distance(path.LastStep, n);
						if (n.Equals(destination))
							d = 0;
						var newPath = path.AddStep(n, d);

						queue.Add(newPath.TotalCost + estimate(n, destination), newPath);
					}
				}
			}
			return null;
		}

	}

	/// <summary>
	/// Interface that rapresent data structures that has the ability to find node neighbours.
	/// </summary>
	public interface IHasNeighbours<T>
	{
		/// <summary>
		/// Gets the neighbours of the instance.
		/// </summary>
		/// <value>The neighbours.</value>
		IEnumerable<T> Neighbours(T node);
	}
}
