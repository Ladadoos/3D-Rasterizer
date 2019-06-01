using System.IO;
using System.Collections.Generic;
using OpenTK;
using System;

namespace Template
{
    // mesh and loader based on work by JTalton; http://www.opentk.com/node/642

    public class MeshLoader
    {
        public bool Load(Mesh mesh, string fileName)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(fileName))
                {
                    Load(mesh, streamReader);
                    streamReader.Close();
                    return true;
                }
            } catch { return false; }
        }

        char[] splitCharacters = new char[] { ' ' };

        List<Vector3> vertices;
        List<Vector3> normals;
        List<Vector2> texCoords;
        List<Mesh.ObjVertex> objVertices;
        List<Mesh.ObjTriangle> objTriangles;
        List<Mesh.ObjQuad> objQuads;

        void Load(Mesh mesh, TextReader textReader)
        {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            texCoords = new List<Vector2>();
            objVertices = new List<Mesh.ObjVertex>();
            objTriangles = new List<Mesh.ObjTriangle>();
            objQuads = new List<Mesh.ObjQuad>();
            string line;
            while ((line = textReader.ReadLine()) != null)
            {
                line = line.Trim(splitCharacters);
                line = line.Replace("  ", " ");
                string[] parameters = line.Split(splitCharacters);
                switch (parameters[0])
                {
                    case "p": // point
                        break;
                    case "v": // vertex
                        float x = float.Parse(parameters[1]);
                        float y = float.Parse(parameters[2]);
                        float z = float.Parse(parameters[3]);
                        vertices.Add(new Vector3(x, y, z));
                        break;
                    case "vt": // texCoord
                        float u = float.Parse(parameters[1]);
                        float v = float.Parse(parameters[2]);
                        texCoords.Add(new Vector2(u, v));
                        break;
                    case "vn": // normal
                        float nx = float.Parse(parameters[1]);
                        float ny = float.Parse(parameters[2]);
                        float nz = float.Parse(parameters[3]);
                        normals.Add(new Vector3(nx, ny, nz));
                        break;
                    case "f":
                        switch (parameters.Length)
                        {
                            case 4:
                                Mesh.ObjTriangle objTriangle = new Mesh.ObjTriangle();
                                objTriangle.Index0 = ParseFaceParameter(parameters[1]);
                                objTriangle.Index1 = ParseFaceParameter(parameters[2]);
                                objTriangle.Index2 = ParseFaceParameter(parameters[3]);
                                objTriangles.Add(objTriangle);
                                break;
                            case 5:
                                Mesh.ObjQuad objQuad = new Mesh.ObjQuad();
                                objQuad.Index0 = ParseFaceParameter(parameters[1]);
                                objQuad.Index1 = ParseFaceParameter(parameters[2]);
                                objQuad.Index2 = ParseFaceParameter(parameters[3]);
                                objQuad.Index3 = ParseFaceParameter(parameters[4]);
                                objQuads.Add(objQuad);
                                break;
                        }
                        break;
                }
            }

            CalculateBoundingSphere(mesh);
            CalculateTangentAndBitangents();

            mesh.vertices = objVertices.ToArray();
            mesh.triangles = objTriangles.ToArray();
            mesh.quads = objQuads.ToArray();
            vertices = null;
            normals = null;
            texCoords = null;
            objVertices = null;
            objTriangles = null;
            objQuads = null;
        }

        private void CalculateBoundingSphere(Mesh mesh)
        {
            float minX = 0; float minY = 0; float minZ = 0;
            float maxX = 0; float maxY = 0; float maxZ = 0;
            for(int i = 0; i < objVertices.Count; i++)
            {
                Vector3 vec = objVertices[i].Vertex;
                if (vec.X < minX) { minX = vec.X; }
                if (vec.Y < minY) { minY = vec.Y; }
                if (vec.Z < minZ) { minZ = vec.Z; }
                if (vec.X > maxX) { maxX = vec.X; }
                if (vec.Y > maxY) { maxY = vec.Y; }
                if (vec.Z > maxZ) { maxZ = vec.Z; }
            }

            Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
            float radius = Math.Max(Math.Max(Math.Abs(maxX - minX), Math.Abs(maxY - minY)), Math.Abs(maxZ - minZ));
            mesh.hitboxCenter = center;
            mesh.hitboxRadius = radius;
        }

        private void CalculateTangentAndBitangents()
        {
            for (int i = 0; i < objTriangles.Count; i++)
            {
                Mesh.ObjVertex vert1 = objVertices[objTriangles[i].Index0];
                Mesh.ObjVertex vert2 = objVertices[objTriangles[i].Index1];
                Mesh.ObjVertex vert3 = objVertices[objTriangles[i].Index2];

                Vector3 v0 = vert1.Vertex;
                Vector3 v1 = vert2.Vertex;
                Vector3 v2 = vert3.Vertex;

                Vector2 uv0 = vert1.TexCoord;
                Vector2 uv1 = vert2.TexCoord;
                Vector2 uv2 = vert3.TexCoord;

                Vector3 deltaPos1 = v1 - v0;
                Vector3 deltaPos2 = v2 - v0;

                Vector2 deltaUV1 = uv1 - uv0;
                Vector2 deltaUV2 = uv2 - uv0;

                float r = 1 / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                Vector3 tangent = Vector3.Normalize((deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r);
                Vector3 bitangent = Vector3.Normalize((deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r);

                objVertices[objTriangles[i].Index0] = new Mesh.ObjVertex()
                {
                    TexCoord = vert1.TexCoord,
                    Normal = vert1.Normal,
                    Vertex = vert1.Vertex,
                    Tangent = tangent,
                    Bitangent = bitangent
                };

                objVertices[objTriangles[i].Index1] = new Mesh.ObjVertex()
                {
                    TexCoord = vert2.TexCoord,
                    Normal = vert2.Normal,
                    Vertex = vert2.Vertex,
                    Tangent = tangent,
                    Bitangent = bitangent
                };

                objVertices[objTriangles[i].Index2] = new Mesh.ObjVertex()
                {
                    TexCoord = vert3.TexCoord,
                    Normal = vert3.Normal,
                    Vertex = vert3.Vertex,
                    Tangent = tangent,
                    Bitangent = bitangent
                };
            }
        }

        char[] faceParamaterSplitter = new char[] { '/' };
        int ParseFaceParameter(string faceParameter)
        {
            Vector3 vertex = new Vector3();
            Vector2 texCoord = new Vector2();
            Vector3 normal = new Vector3();
            string[] parameters = faceParameter.Split(faceParamaterSplitter);
            int vertexIndex = int.Parse(parameters[0]);
            if (vertexIndex < 0) vertexIndex = vertices.Count + vertexIndex;
            else vertexIndex = vertexIndex - 1;
            vertex = vertices[vertexIndex];
            if (parameters.Length > 1) if (parameters[1] != "")
                {
                    int texCoordIndex = int.Parse(parameters[1]);
                    if (texCoordIndex < 0) texCoordIndex = texCoords.Count + texCoordIndex;
                    else texCoordIndex = texCoordIndex - 1;
                    texCoord = texCoords[texCoordIndex];
                }
            if (parameters.Length > 2)
            {
                int normalIndex = int.Parse(parameters[2]);
                if (normalIndex < 0) normalIndex = normals.Count + normalIndex;
                else normalIndex = normalIndex - 1;
                normal = normals[normalIndex];
            }
            return AddObjVertex(ref vertex, ref texCoord, ref normal);
        }

        int AddObjVertex(ref Vector3 vertex, ref Vector2 texCoord, ref Vector3 normal)
        {
            Mesh.ObjVertex newObjVertex = new Mesh.ObjVertex();
            newObjVertex.Vertex = vertex;
            newObjVertex.TexCoord = texCoord;
            newObjVertex.Normal = normal;
            objVertices.Add(newObjVertex);
            return objVertices.Count - 1;
        }
    }
}
