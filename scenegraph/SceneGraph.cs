using OpenTK;
using System.Collections.Generic;

namespace Template
{
    class SceneGraph
    {
        public GraphTree<GameObject> hierarchy;
        public List<Light> lights = new List<Light>();

        private Matrix4 lightViewMatrix, lightProjectionMatrix, lightSpaceMatrix;

        public void PrepareMatrices()
        {
            Matrix4.CreateOrthographicOffCenter(-30, 30, -30, 30, 0.1F, 1000, out lightProjectionMatrix);
            lightViewMatrix = Matrix4.LookAt(lights[0].position, Vector3.Zero, new Vector3(0, 1, 0));
            lightSpaceMatrix = lightViewMatrix * lightProjectionMatrix;
        }

        public void RenderDepthMap(Camera camera, DepthShader shader)
        {
            for (int i = 0; i < hierarchy.rootNodes.Count; i++)
            {
                RenderNodeToDepth(hierarchy.rootNodes[i], lightSpaceMatrix, shader);
            }
        }

        private void RenderNodeToDepth(GraphNode<GameObject> node, Matrix4 viewProjection, DepthShader shader)
        {
            Matrix4 globalTransform = GetGlobalTransformationMatrix(node);
            node.data.RenderDepth(shader, globalTransform, viewProjection);

            for (int i = 0; i < node.childrenNodes.Count; i++)
            {
                RenderNodeToDepth(node.childrenNodes[i], viewProjection, shader);
            }
        }

        public void RenderScene(Camera camera, ModelShader shader, DepthMap depthMap)
        {
            shader.Bind();
            shader.LoadVector3(shader.uniform_ambientlightcolor, new Vector3(0.2F, 0.2F, 0.9F));
            shader.LoadVector3(shader.uniform_lightcolor, lights[0].color);
            shader.LoadVector3(shader.uniform_lightposition, lights[0].position);
            shader.LoadVector3(shader.uniform_camPos, camera.position);
            shader.Unbind();

            Matrix4 viewMatrix = camera.GetViewMatrix();
            Matrix4 projMatrix = camera.GetProjectionMatrix();

            for (int i = 0; i < hierarchy.rootNodes.Count; i++)
            {
                RenderNodeToScene(hierarchy.rootNodes[i], viewMatrix, projMatrix, lightSpaceMatrix, shader, depthMap);
            }
        }

        private void RenderNodeToScene(GraphNode<GameObject> node, Matrix4 cameraView, Matrix4 projection,
            Matrix4 lightMatrix, ModelShader shader, DepthMap depthMap)
        {
            Matrix4 globalTransform = GetGlobalTransformationMatrix(node);
            node.data.RenderScene(shader, globalTransform, cameraView, projection, lightMatrix, depthMap);

            for (int i = 0; i < node.childrenNodes.Count; i++)
            {
                RenderNodeToScene(node.childrenNodes[i], cameraView, projection, lightMatrix, shader, depthMap);
            }
        }

        private Matrix4 GetGlobalTransformationMatrix(GraphNode<GameObject> node)
        {
            Matrix4 globalTransform = node.data.GetLocalTransformationMatrix();
            GraphNode<GameObject> toCheckNode = node;
            while (toCheckNode.parentNode != null)
            {
                globalTransform *= toCheckNode.parentNode.data.GetLocalTransformationMatrix();
                toCheckNode = toCheckNode.parentNode;
            }
            return globalTransform;
        }
    }
}
