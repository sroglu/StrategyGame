using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using mehmetsrl.Algorithms.DataStructures;

namespace mehmetsrl.Algorithms.Graph
{

	/// <summary>
	/// Generic Implementation of the A* algorithm.
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
				var path = queue.DeleteMin().Value;

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
	/// Finds the optimal path between start and destionation TNode.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="start">Starting Node.</param>
	/// <param name="destination">Destination Node.</param>
	/// <param name="distance">Function to compute distance beween nodes.</param>
	/// <typeparam name="TNode">Any class implement IHasNeighbours.</typeparam>
	class Dijkstra
	{
		static public Path<TNode> FindPath<TNode>(IHasNeighbours<TNode> dataStructure, TNode start, TNode destination, Func<TNode, TNode, double> distance)
		{
			var closed = new HashSet<TNode>();
			var queue = new PriorityQueue<double, Path<TNode>>();
			queue.Add(0, new Path<TNode>(start));

			while (!queue.IsEmpty)
			{

				var path = queue.DeleteMin().Value;

				if (closed.Contains(path.LastStep))
					continue;

				if (path.LastStep.Equals(destination))
					return path;

				closed.Add(path.LastStep);

				foreach (TNode n in dataStructure.Neighbours(path.LastStep))
				{
					if (!closed.Contains(n))
					{
						double d = distance(path.LastStep, n);
						if (n.Equals(destination))
							d = 0;
						var newPath = path.AddStep(n, d);

						queue.Add(newPath.TotalCost, newPath);
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
	/// <summary>
	/// Represent a generic Path along a graph.
	/// </summary>
	public class Path<TNode> : IEnumerable<TNode>
	{

		#region PublicProperties
		/// <summary>
		/// Gets the last step.
		/// </summary>
		/// <value>The last step.</value>
		public TNode LastStep { get; private set; }

		/// <summary>
		/// Gets the previous steps.
		/// </summary>
		/// <value>The previous steps.</value>
		public Path<TNode> PreviousSteps { get; private set; }

		/// <summary>
		/// Gets the total cost.
		/// </summary>
		/// <value>The total cost.</value>
		public double TotalCost { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Path`1"/> class.
		/// </summary>
		/// <param name="lastStep">Last step.</param>
		/// <param name="previousSteps">Previous steps.</param>
		/// <param name="totalCost">Total cost.</param>
		Path(TNode lastStep, Path<TNode> previousSteps, double totalCost)
		{
			LastStep = lastStep;
			PreviousSteps = previousSteps;
			TotalCost = totalCost;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Path`1"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		public Path(TNode start) : this(start, null, 0) { }
		#endregion

		/// <summary>
		/// Adds a step to the path.
		/// </summary>
		/// <returns>The new path.</returns>
		/// <param name="step">The step.</param>
		/// <param name="stepCost">The step cost.</param>
		public Path<TNode> AddStep(TNode step, double stepCost)
		{
			//UnityEngine.Debug.Log("step: "+step+"TotalCost: "+TotalCost+" + stepCost  "+stepCost+" -> cost: "+(TotalCost + stepCost));
			return new Path<TNode>(step, this, TotalCost + stepCost);
		}

		#region EnumerableImplementation
		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<TNode> GetEnumerator()
		{
			for (Path<TNode> p = this; p != null; p = p.PreviousSteps)
				yield return p.LastStep;
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

	}
}
