using Castle.Models.Knight;

namespace Castle.Interfaces
{
    public interface IPeerToPeerService
    {
        void LoadFromDb(IEnumerable<Knight> knights);

        void AddVertex(string vertex);
        void RemoveVertex(string vertex);

        void AddEdge(string start, string end); 
        void RemoveEdge(string start, string end);

        IEnumerable<string> DepthFirstSearch(string start);
        bool PathExists(string start, string end);
        IEnumerable<string> FindPath(string start, string end);

        public void PrintVertexes<T>(IEnumerable<T> search); // DEBUG
    }
}
