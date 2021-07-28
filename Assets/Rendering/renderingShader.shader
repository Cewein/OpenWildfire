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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewVector : TEXCOORD1;
            };
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));
                return o;
            }

            sampler2D _MainTex;
            float3 minBounds;
            float3 maxBounds;
            int nbStep;
            float threashold;


            // cf. Dave Hoskins https://www.shadertoy.com/view/4djSRW
            float hash13(float3 p3) {
                p3 = frac(p3 * .1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

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


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 rayOrigin = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;
                
                float dist = 0.0;
                float insideDist = 0.0;

                col.rgb = col.rgb;

                //look if we hit the domain and the get the distance to the domain and the length travel inside the domain
                bool asHit = hitBox(minBounds, maxBounds, rayOrigin, 1.0/rayDir, dist, insideDist);
                
                if (asHit)
                {
                    col.rgb -= insideDist * threashold;
                }


                return col;
                
            }
            ENDCG
        }
    }
}
