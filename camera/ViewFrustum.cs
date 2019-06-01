using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class ViewFrustum
    {
        struct FrustumPlane
        {
            public Vector3 point;
            public Vector3 normal;
            public float distanceToOrigin;

            public float GetSignedDistance(Vector3 point)
            {
                return Vector3.Dot(point, normal) - distanceToOrigin;
            }
        }

        private float fov;
        private float aspectRatio;
        private float nearDistance;
        private float farDistance;
        private float nearWidth, nearHeight, farWidth, farHeight;
        private float halfNearWidth, halfNearHeight, halfFarWidth, halfFarHeight;

        private FrustumPlane[] frustumPlanes = new FrustumPlane[6];

        public ViewFrustum(float fov, float aspectRatio, float nearDistance, float farDistance)
        {
            this.fov = fov;
            this.aspectRatio = aspectRatio;
            this.nearDistance = nearDistance;
            this.farDistance = farDistance;

            nearHeight = (float)(2 * Math.Tan(fov / 2) * nearDistance);
            halfNearHeight = nearHeight / 2;
            nearWidth = nearHeight * aspectRatio;
            halfNearWidth = nearWidth / 2;
            farHeight = (float)(2 * Math.Tan(fov / 2) * farDistance);
            halfFarHeight = farHeight / 2;
            farWidth = farHeight * aspectRatio;
            halfFarWidth = farWidth / 2;
        }

        public Matrix4 CreateProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearDistance, farDistance);
        }

        public void UpdateFrustumPoints(Camera camera)
        {
            Vector3 zAxis = -camera.forward;
            Vector3 xAxis = camera.right;
            Vector3 yAxis = Vector3.Cross(zAxis, xAxis);

            Vector3 nearCenter = camera.position - zAxis * nearDistance;
            Vector3 farCenter = camera.position - zAxis * farDistance;

            frustumPlanes[0].normal = -zAxis;
            frustumPlanes[0].point = nearCenter; //near plane
            frustumPlanes[0].distanceToOrigin = Vector3.Dot(frustumPlanes[0].normal, frustumPlanes[0].point);

            frustumPlanes[1].normal = zAxis;
            frustumPlanes[1].point = farCenter; //far plane
            frustumPlanes[1].distanceToOrigin = Vector3.Dot(frustumPlanes[1].normal, frustumPlanes[1].point);

            Vector3 aux, normal;

            aux = (nearCenter + yAxis * halfNearHeight) - camera.position;
            aux.Normalize();
            normal = Vector3.Cross(aux, xAxis);
            frustumPlanes[2].normal = normal;
            frustumPlanes[2].point = (nearCenter + yAxis * halfNearHeight); //top plane
            frustumPlanes[2].distanceToOrigin = Vector3.Dot(frustumPlanes[2].normal, frustumPlanes[2].point);

            aux = (nearCenter - yAxis * halfNearHeight) - camera.position;
            aux.Normalize();
            normal = Vector3.Cross(xAxis, aux);
            frustumPlanes[3].normal = normal;
            frustumPlanes[3].point = (nearCenter - yAxis * halfNearHeight); //bottom plane
            frustumPlanes[3].distanceToOrigin = Vector3.Dot(frustumPlanes[3].normal, frustumPlanes[3].point);

            aux = (nearCenter - xAxis * halfNearWidth) - camera.position;
            aux.Normalize();
            normal = Vector3.Cross(aux, yAxis);
            frustumPlanes[4].normal = normal;
            frustumPlanes[4].point = (nearCenter - xAxis * halfNearWidth); //left plane
            frustumPlanes[4].distanceToOrigin = Vector3.Dot(frustumPlanes[4].normal, frustumPlanes[4].point);

            aux = (nearCenter + xAxis * halfNearWidth) - camera.position;
            aux.Normalize();
            normal = Vector3.Cross(yAxis, aux);
            frustumPlanes[5].normal = normal;
            frustumPlanes[5].point = (nearCenter + xAxis * halfNearWidth); //right plane
            frustumPlanes[5].distanceToOrigin = Vector3.Dot(frustumPlanes[5].normal, frustumPlanes[5].point);
        }

        public bool IsSphereInFrustum(Vector3 position, float radius)
        {
            for(int i = 0; i < 6; i++)
            {
                if(frustumPlanes[i].GetSignedDistance(position) < -radius)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
