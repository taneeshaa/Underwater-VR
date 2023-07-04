Shader "Hidden/CameraUnderwaterEffect"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _DepthStart ("Depth Start Distance", float) = 1;
        [HideInInspector] _DepthEnd ("Depth End Distance", float) = 300;
        [HideInInspector] _DepthColor ("Depth Color Distance", Color) = (1, 1, 1, 1);
    }
        SubShader
    {
        //Disable backface culling(Cull off),
        // depth buffer waiting during rendering (ZWrite Off),
        // Always draw a pixel regardless of depth (Ztest always)
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _CameraDepthTexture, _MainTex;
            float _DepthStart, _DepthEnd;
            fixed4 _DepthColor;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            //We add an extra screenPos attribute to the vertex data, and compute the
            //screen position of each vertex in the vert() function below.

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }


            //This function is run on every pixel that is seen on camera
            //Hence, it is responsible for applying the post-processing effects onto
            //the image that the camera receives.
            fixed4 frag(v2f i) : SV_Target
            {
                //we sample the pixel in i.screenPos from _CameraDepthTexture, then convert it into 
                //linear depth (depth is stored non-linearly) that is clamped between 0 and 1
                float depth = LinearEyeDepth(text2D(_CameraDepthTexture, i.screenPos.xy));

                //clip the depth between 0 and 1 again, where 1 is if the pixel is further
                //than _DepthEnd, and 0 if the pixel is nearer than _DepthStart
                depth = saturate((depth - _DepthStart) / _DepthEnd);

                //scale the intensity of the depth colour based on its depth by lerping it
                //between the original pixel colour and our colour based on the depthValue of the pixel
                fixed4 col = text2D(_MainText, i.screenPos);
                return lerp(cool, _DepthColor, depth);
            }
            ENDCG
        }
    }
}
