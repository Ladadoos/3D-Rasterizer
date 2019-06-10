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
            viewMatrices = Camera.GetSurroundViews(position);
        }
    }

}
