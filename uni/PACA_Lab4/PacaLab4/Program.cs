using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PacaLab4
{
    internal class Graph
    {
        private enum Color
        {
            White,
            Gray,
            Black
        }

        private class Vertex
        {
            public int Index { get; }
            public Color Color { get; set; }
            public Vertex Predecessor { get; set; }
            public int Discovery { get; set; }
        
            public Vertex(int index)
            {
                Index = index;
                Color = Color.White;
                Predecessor = null;
                Discovery = int.MaxValue;
            }
        }

        private class Pair
        {
            public int first;
            public int second;

            public Pair(int x, int y)
            {
                first = x;
                second = y;
                if (x > y) (first, second) = (second, first);
            }
        }
        
        private class Road
        {
            Pair pair;
            public int weight;
            List<Pair> connections;

            public Road(int x, int y, int weight)
            {
                pair = new Pair(x, y);
                connections = new List<Pair>();
                this.weight = weight;
            }

            public void addConnection(int x, int y)
            {
                connections.Add(new Pair(x, y));
            }
        }

        private readonly List<Dictionary<int, int>> _adjList; // HashTable<Destination, Weight>
        private readonly List<Vertex> _vertices;
        private readonly HashSet<int> _machines;
        private readonly Dictionary<int, Road> _roads;
        private readonly Dictionary<int, HashSet<int>> _connections;
        private bool[] _visited;
        private int _n;

        public int GetPairKey(int x, int y)
        {
            return x * _n + y;
        }

        public Graph(List<List<int>> roads, HashSet<int> machines)
        {
            _adjList = new List<Dictionary<int, int>>();
            _vertices = new List<Vertex>();  
            _machines = machines;
            foreach (var road in roads) Add(road[0], road[1], road[2]);
            _n = _adjList.Count;
            _visited = new bool[_n];
        }

        private void Add(int x, int y, int weight)
        {
            if (x >= _adjList.Count || y >= _adjList.Count)
            {
                int capacity;
                if (x > y)
                {
                    capacity = x + 1;
                }
                else
                {
                    capacity = y + 1;
                }

                var n = _adjList.Count;
                for (var i = n; i < capacity; i++)
                {
                    _adjList.Add(new Dictionary<int, int>());
                    _vertices.Add(new Vertex(i));
                }
            }

            _adjList[x].Add(y, weight);
            _adjList[y].Add(x, weight);
            _roads.Add(GetPairKey(x, y), new Road(x, y, weight)));
        }

        public int MinTime()
        {
            var path = new List<int>();
            foreach (var machine in _machines)
            {
                for (var i = 0; i < _n; i++)
                {
                    _visited[i] = false;
                }
                Visit(machine, machine, machine, ref path);
            }

            var roadsToDestroy = new HashSet<int>();
            foreach (var i in _machines)
            {
                foreach (var j in _machines)
                {
                    if (i >= j) continue;
                    roadsToDestroy.Add(_connections[GetPairKey(i, j)].Min(Weight));
                }
            }
            

            return time;
        }

        private int Weight(int road)
        {
            return _roads[road].weight;
        }

        private int Visit(int source, int current, int weakest, ref List<int> path)
        {
            _visited[current] = true;
            foreach (var (next, _) in _adjList[current])
            {
                if (_visited[next])
                {
                    continue;
                }
                
                path.Add(GetPairKey(current, next));

                if (_machines.Contains(next))
                {
                    _connections.Add(GetPairKey(source, next), new HashSet<int>(path));
                }

                var ret = Visit(source, next, weakest, ref path);
                if (ret != current && ret != -1)
                {
                    return ret;
                }
            }

            return -1;
        }
    }

    internal static class Result
    {
        /*
         * Complete the 'minTime' function below.
         *
         * The function is expected to return an INTEGER.
         * The function accepts following parameters:
         *  1. 2D_INTEGER_ARRAY roads
         *  2. INTEGER_ARRAY machines
         */

        public static int MinTime(List<List<int>> roads, HashSet<int> machines)
        {
            var graph = new Graph(roads, machines);
            return graph.MinTime();
        }
    }

    internal static class Solution
    {
        public static void Main(string[] args)
        {
            var textReader = new StreamReader("/Users/mykytko/Downloads/test2.txt");

            var firstMultipleInput = textReader.ReadLine()?.TrimEnd().Split(' ');

            var n = Convert.ToInt32(firstMultipleInput?[0]);

            var k = Convert.ToInt32(firstMultipleInput?[1]);

            var roads = new List<List<int>>();

            for (var i = 0; i < n - 1; i++)
            {
                roads.Add(textReader.ReadLine()?.TrimEnd().Split(' ').Select(roadsTemp => Convert.ToInt32(roadsTemp))
                    .ToList());
            }

            var machines = new HashSet<int>();

            for (var i = 0; i < k; i++)
            {
                var machinesItem = Convert.ToInt32(textReader.ReadLine()?.Trim());
                machines.Add(machinesItem);
            }

            var sw = new Stopwatch();
            sw.Start();
            var result = Result.MinTime(roads, machines);
            sw.Stop();
            Console.WriteLine("Execution took " + sw.Elapsed + " s.");

            Console.WriteLine(result);

            textReader.Close();
        }
    }
}