Shader "UltraEffects/Fisheye"
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
            };

			// Standard vertex shader.
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            uniform sampler2D _MainTex;
            uniform float2 _focalLength;
            uniform float4 _distortion;

            float2 distort(float2 xy, float fx, float fy, float cx, float cy, float k1, float k2, float k3, float k4)
            {
                xy = (xy - float2(cx, cy)) / float2(fx, fy);
                float r = length(xy);
                
                float theta = atan(r);
                float theta_d = theta * (1.0 + k1 * pow(theta, 2.0) + k2 * pow(theta, 4.0) + k3 * pow(theta, 6.0) + k4 * pow(theta, 8.0));
                
                float r_d = tan(theta_d);
                
                float2 xy_d = xy / r * r_d;
                return xy_d * float2(fx, fy) + float2(cx, cy);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = 2.0 * i.uv - 1.0;

				if (length(uv) >= 1.0)
				{
					discard;
				}

                float w = _ScreenParams.x;
                float h = _ScreenParams.y;
				float2 xy_d = distort(uv, _focalLength.x/w, _focalLength.y/h, 0.0, 0.0, _distortion.x, _distortion.y, _distortion.z, _distortion.w);
                float2 uv_d = xy_d * 0.5 + 0.5;
                fixed4 col = tex2D(_MainTex, uv_d);
				return col;
            }
            ENDCG
        }
    }
}