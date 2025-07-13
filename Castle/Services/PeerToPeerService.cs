using Castle.Interfaces;
using Castle.Models.Knight;

namespace Castle.Services
{
    public class PeerToPeerService : IPeerToPeerService
    {
        private readonly Dictionary<string, HashSet<string>> _adjacencyList = new();
        public void LoadFromDb(IEnumerable<Knight> soldiers)
        {
            soldiers = soldiers.ToArray();

            foreach (var drone in soldiers)
                AddVertex(drone.Metadata.Id);

            foreach (var drone in soldiers)
                if (!string.IsNullOrWhiteSpace(drone.ParentId))
                    AddEdge(drone.ParentId, drone.Metadata.Id);
        }
        public void AddVertex(string vertex)
        {
            if (!_adjacencyList.ContainsKey(vertex))
                _adjacencyList.Add(vertex, new HashSet<string>());
        }
        public void RemoveVertex(string vertex)
        {
            _adjacencyList.Remove(vertex);
        }
        public void AddEdge(string start, string end)
        {
            if (_adjacencyList.TryGetValue(start, out var edges))
                edges.Add(end);
        }
        public void RemoveEdge(string start, string end)
        {
            if (_adjacencyList.TryGetValue(start, out var edges))
                edges.Remove(end);
        }

        public IEnumerable<string> DepthFirstSearch(string start)
        {
            var visited = new HashSet<string>();

            if (!_adjacencyList.ContainsKey(start))
                return visited;

            var stack = new Stack<string>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var vertex = stack.Pop();

                if (visited.Contains(vertex))
                    continue;

                visited.Add(vertex);

                foreach (var neighbour in _adjacencyList[vertex])
                    if (!visited.Contains(neighbour))
                        stack.Push(neighbour);
            }

            return visited;
        }

        public bool PathExists(string start, string end)
        {
            var visited = new HashSet<string>();

            if (!_adjacencyList.ContainsKey(start))
                return false;

            var stack = new Stack<string>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var vertex = stack.Pop();

                if (visited.Contains(vertex))
                    continue;

                if (vertex.Equals(end))  // if the current vertex equals our "end" vertex, a path exists
                    return true;

                visited.Add(vertex);

                foreach (var neighbour in _adjacencyList[vertex])
                    if (!visited.Contains(neighbour))
                        stack.Push(neighbour);
            }

            return false;
        }

        public IEnumerable<string> FindPath(string start, string end)
        {
            var map = new Dictionary<string, string>();

            var queue = new Stack<string>();
            queue.Push(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Pop();

                foreach (var neighbour in _adjacencyList[vertex])
                {
                    if (map.ContainsKey(neighbour))
                        continue;

                    map[neighbour] = vertex;
                    queue.Push(neighbour);
                }
            }

            var path = new List<string>();
            var current = end;

            while (!current.Equals(start))
            {
                path.Add(current);
                current = map[current];
            }

            path.Add(start);
            path.Reverse();

            return path;
        }


        public void PrintVertexes<T>(IEnumerable<T> search) // DEBUG
        {
            var vertexes = search.ToArray();

            Console.WriteLine(">> Printing Vertexes");
            for (var i = 0; i < vertexes.Length; i++)
            {
                Console.Write(i == vertexes.Length - 1
                    ? $"{vertexes[i]}"
                    : $"{vertexes[i]}, ");
            }

            Console.WriteLine();
        }
    }
}
