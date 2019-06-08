namespace Template
{
    public struct SurfaceTexture
    {
        public Texture diffuse;
        public Texture normal;
        public float shininess;

        public SurfaceTexture(Texture diffuse, Texture normal, float shininess)
        {
            this.diffuse = diffuse;
            this.normal = normal;
            this.shininess = shininess;
        }
    }
}
