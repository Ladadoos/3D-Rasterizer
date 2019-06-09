using OpenTK;

namespace Template
{
    public class SpotLight : Light
    {
        public DepthMap depthMap;
        public Vector3 direction;
        public float cutOffAngle;

        public SpotLight(Mesh mesh, SurfaceTexture texture, Vector3 position, Vector3 rotationInAngle, Vector3 scale)
            : base(mesh, texture, position, rotationInAngle, scale)
        {
            viewMatrices = new Matrix4[1];
            direction = new Vector3(0, 1, 0);
            cutOffAngle = 30.5F;
        }

        public void CreateDepth(DepthMap depthMap)
        {
            this.depthMap = depthMap;
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), 1, 1, 1000, out projectionMatrix);
        }

        public override void Update()
        {
            base.Update();

            Vector3 position = globalTransform.ExtractTranslation();
            viewMatrices[0] = Matrix4.LookAt(position, position + direction, new Vector3(0, -1, 0));
        }
    }
}
