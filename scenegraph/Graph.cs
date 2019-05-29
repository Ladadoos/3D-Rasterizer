using System.Collections.Generic;

namespace Template
{
    class GraphTree<T>
    {
        public List<GraphNode<T>> rootNodes = new List<GraphNode<T>>();
    }

    class GraphNode<T>
    {
        public GraphNode<T> parentNode;
        public List<GraphNode<T>> childrenNodes;
        public T data;

        public GraphNode(T data)
        {
            this.data = data;
            childrenNodes = new List<GraphNode<T>>();
        }

        public void AddChild(GraphNode<T> node)
        {
            childrenNodes.Add(node);
            node.parentNode = this;
        }

        public void RemoveChild(GraphNode<T> node)
        {
            childrenNodes.Remove(node);
            node.parentNode = null;
        }
    }
}
