using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAiO;

namespace ConsoleApp1.VF3
{

    public class VF3State
    {
        public bool[] core_1;
        public bool[] core_2;

        public int[] t_1;
        public int[] t_2;

        public Graph g1, g2;
        public Stack<(int, int)> Mappings = new();


        public VF3State(Graph g1, Graph g2)
        {
            this.g1 = g1;
            this.g2 = g2;

            core_1 = new bool[g1.size];
            core_2 = new bool[g2.size];
            t_1 = new int[g1.size];
            t_2 = new int[g2.size];
            Array.Fill(t_1, 0);
            Array.Fill(t_2, 0);
        }

        public bool IsGoal()
        {
            return Mappings.Count == g1.size;
        }

        public void AddMapping(int node1, int node2)
        {
            if(core_1[node1] || core_2[node2])
                throw new InvalidOperationException("Trying to map already mapped nodes");
            core_1[node1] = true;
            core_2[node2] = true;
            foreach (var n in g1.OutNeighbours(node1))
            {
                t_1[n]++;
            }
            foreach (var n in g2.OutNeighbours(node2))
            {
                t_2[n]++;
            }
            Mappings.Push((node1, node2));
        }

        public void Backtrack()
        {
            var (node1, node2) = Mappings.Pop();
            core_1[node1] = false;
            core_2[node2] = false;
            foreach (var n in g1.OutNeighbours(node1))
            {
                t_1[n]--;
            }
            foreach (var n in g2.OutNeighbours(node2))
            {
                t_2[n]--;
            }
        }

        public static int[] PreprocessDegreeProbability(Graph g)
        {
            int[] degrees = new int[g.size];
            int maxDegree = 0;
            for (int i = 0; i < g.size; i++)
            {
                degrees[i] = g.OutDegree(i);
                if (degrees[i] > maxDegree)
                    maxDegree = degrees[i];
            }
            int[] degreeProbability = new int[maxDegree];
            for (int i = 0; i < g.size; i++)
            {
                degreeProbability[degrees[i]]++;
            }
            return degreeProbability;
        }

        public static int NodeMappingDegree(List<int> nodeExplorationSequence, Graph g1, int node)
        {
            int degree = 0;

            for(int i = 0; i<nodeExplorationSequence.Count; i++)
            {
                if (g1.HasEdge(node, nodeExplorationSequence[i]) || g1.HasEdge(nodeExplorationSequence[i], node))
                    degree++;
            }

            return degree;
        }

        public static int FindNextNode(List<int> nodeExSeq, Graph g1)
        {
            int maxDegree = -1;
            int nextNode = -1;
            for (int i = 0; i < g1.size; i++)
            {
                if (!nodeExSeq.Contains(i))
                {
                    int degree = NodeMappingDegree(nodeExSeq, g1, i);
                    if (degree > maxDegree)
                    {
                        maxDegree = degree;
                        nextNode = i;
                    }
                }
            }
            return nextNode;
        }

        public static List<int> ComputeExplorationSequence(Graph g)
        {
            List<int> sequence = new List<int>();

            while(sequence.Count < g.size)
            {

            }

            return sequence;
        }
    }
}
