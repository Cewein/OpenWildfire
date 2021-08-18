Shader "Hidden/renderingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define PI 3.141592653589793
            #define TWOPI 6.283185307179586
            #define HALFPI 1.570796326794896
            #define INV_SQRT_2 0.7071067811865476

            #define MAX_ALPHA_PER_UNIT_DIST 2.5
            #define QUIT_ALPHA 0.99
            #define QUIT_ALPHA_L 0.95

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewfloattor : TEXCOORD1;
            };
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                float3 viewfloattor = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewfloattor = mul(unity_CameraToWorld, float4(viewfloattor, 0));
                return o;
            }

            sampler2D _MainTex;
            float3 minBounds;
            float3 maxBounds;
            int nbStep;
            int unitPerSide;
            float threashold;

            
            uniform StructuredBuffer<float> smokeDensity;


            //http://www.jcgt.org/published/0007/03/04/paper-lowres.pdf
            bool hitBox(float3 bmin, float3 bmax, float3 rayOrigin, float3 invRayDir, out float distToBox, out float distInsideBox)
            {
                float3 t0 = (bmin - rayOrigin) * invRayDir;
                float3 t1 = (bmax - rayOrigin) * invRayDir;

                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);

                float d1 = max(tmin.x, max(tmin.y, tmin.z));
                float d2 = min(tmax.x, min(tmax.y, tmax.z));

                distToBox = max(0, d1);
                distInsideBox = max(0, d2 - distToBox);

                return max(tmin.x, max(tmin.y, tmin.z)) <= min(tmax.x, min(tmax.y, tmax.z));
            }

            //compute the local Coord of a given point from the minBound
            //max bound is here to get the length of the axis x, y and z
            //we clamp so we are always in the cube
            float3 pointToLocalCoord(float3 minBound, float3 maxBound, float3 insidePoint)
            {
                float3 axisLength = maxBound - minBound;

                float3 uvw = (insidePoint - minBound) / axisLength;

                return uvw * (unitPerSide - 1);

            }

            uint flatten(uint3 coord)
            {
                return coord.x + unitPerSide * (coord.y + unitPerSide * coord.z);
            }

            void readLMN(in float3 lmn, out float density, out float lightAmount)
            {
                lightAmount = 1.0;
                density = smokeDensity[flatten(uint3(lmn))];
            }

            //lut for smoke color map
            float3 colormap(float t) {
                return .5 + .3 * cos(TWOPI * (t + float3(0.1, 0.1, 0.1)));
            }

            float4 blendOnto(float4 cFront, float4 cBehind) {
                return cFront + (1.0 - cFront.a) * cBehind;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 bgcol = tex2D(_MainTex, i.uv);
                float4 col = float4(0.0, 0.0, 0.0, 0.0);

                float3 rayOrigin = _WorldSpaceCameraPos;
                float viewLength = length(i.viewfloattor);
                float3 rayDir = i.viewfloattor / viewLength;
                
                float dist = 0.0;
                float insideDist = 0.0;

                col.rgb = col.rgb;

                //look if we hit the domain and the get the distance to the domain and the length travel inside the domain
                bool asHit = hitBox(minBounds, maxBounds, rayOrigin, 1.0/rayDir, dist, insideDist);
                
                float stepSize = insideDist / float(nbStep);

                for(int i = 0; i < nbStep && asHit; i++)
                {
                    float3 localcoord = pointToLocalCoord(minBounds, maxBounds, rayOrigin + rayDir*dist);
                    localcoord = localcoord - frac(localcoord);

                    float density;
                    float lightAmount;
                    readLMN(localcoord, density, lightAmount);

                    float3 cfrag = colormap(0.7 * density + 0.8);


                    float calpha = density * MAX_ALPHA_PER_UNIT_DIST * stepSize;
                    float4 ci = clamp(float4(cfrag * lightAmount, 1.0) * calpha, 0.0, 1.0);
                    col = blendOnto(col, ci);

                    dist += stepSize;

                }

                float finalA = clamp(col.a / QUIT_ALPHA, 0.0, 1.0);
                col *= (finalA / (col.a + 1e-5));


                float3 finalColor = blendOnto(col, bgcol).rgb;

                return float4(finalColor, 1.0);
                
            }
            ENDCG
        }
    }
}
