using OpenTK;

namespace Template
{
    public class PointLight : Light
    {
        public CubeDepthMap depthCube;
        public Matrix4[] viewMatrices = new Matrix4[6];

        public PointLight(Mesh mesh, SurfaceTexture texture, Vector3 position, Vector3 rotationInAngle, Vector3 scale)
            : base(mesh, texture, position, rotationInAngle, scale)
        {

        }

        public void CreateDepth(CubeDepthMap depthMap)
        {
            this.depthCube = depthMap;
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), 1, 1, 1000, out projectionMatrix);
        }

        public override void Update()
        {
            base.Update();

            Vector3 position = globalTransform.ExtractTranslation();
            viewMatrices[0] = Matrix4.LookAt(position, position + new Vector3(1, 0, 0), new Vector3(0, -1, 0));
            viewMatrices[1] = Matrix4.LookAt(position, position + new Vector3(-1, 0, 0), new Vector3(0, -1, 0));
            viewMatrices[2] = Matrix4.LookAt(position, position + new Vector3(0, 1, 0), new Vector3(0, 0, 1));
            viewMatrices[3] = Matrix4.LookAt(position, position + new Vector3(0, -1, 0), new Vector3(0, 0, -1));
            viewMatrices[4] = Matrix4.LookAt(position, position + new Vector3(0, 0, 1), new Vector3(0, -1, 0));
            viewMatrices[5] = Matrix4.LookAt(position, position + new Vector3(0, 0, -1), new Vector3(0, -1, 0));
        }
    }

}
