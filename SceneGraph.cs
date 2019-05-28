using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Template
{
    class SceneGraph
    {
        public GraphTree<Mesh> hierarchy;

        public void Render(Camera camera, ModelShader shader, Texture texture)
        {
            Matrix4 viewMatrix = camera.GetViewMatrix();
            Matrix4 projMatrix = camera.GetProjectionMatrix();
            Matrix4 viewProjMatrix = viewMatrix * projMatrix;

            for (int i = 0; i < hierarchy.rootNodes.Count; i++)
            {
                RenderNode(hierarchy.rootNodes[i], viewProjMatrix, shader, texture);
            }
        }

        public void RenderNode(GraphNode<Mesh> node, Matrix4 vpm, ModelShader sa, Texture te)
        {
            Matrix4 tm = node.data.GetTransformationMatrix();
            GraphNode<Mesh> n = node;
            while(n.parentNode != null)
            {
                tm *= n.parentNode.data.GetTransformationMatrix();
                n = n.parentNode;
            }

            node.data.Render(sa, tm * vpm, te);

            for (int i = 0; i < node.childrenNodes.Count; i++)
            {
                RenderNode(node.childrenNodes[i], vpm, sa, te);
            }
        }
    }

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
