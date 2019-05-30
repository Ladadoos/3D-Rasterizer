namespace Template
{
    public class SurfaceTexture
    {
        public Texture diffuse;
        public Texture normal;

        public SurfaceTexture(Texture diffuse, Texture normal)
        {
            this.diffuse = diffuse;
            this.normal = normal;
        }
    }
}
