namespace Template
{
    public enum MaterialType
    {
        Diffuse = 0,
        Reflective = 1,
        Dieletric = 2
    }

    public class SurfaceTexture
    {
        public Texture diffuse;
        public Texture normal;
        public float shininess;
        public MaterialType materialType;
        public CubeTexture environmentCubeMap;

        public SurfaceTexture(Texture diffuse, Texture normal, float shininess, MaterialType materialType, CubeTexture environmentMap = null)
        {
            this.diffuse = diffuse;
            this.normal = normal;
            this.shininess = shininess;
            this.environmentCubeMap = environmentMap;
            this.materialType = materialType;
        }
    }
}
