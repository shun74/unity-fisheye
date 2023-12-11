Shader "Custom/CheckerPattern"
{
    Properties
    {
        _ColorA("Color A", Color) = (1, 1, 1, 1)
        _ColorB("Color B", Color) = (0, 0, 0, 1)
        _CheckSize("Check Size", Float) = 10.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            fixed4 _ColorA;
            fixed4 _ColorB;
            float _CheckSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Determine the check pattern repetition
                float2 checkPos = floor(i.uv * _CheckSize);
                // Check if the current block is even or odd
                bool isEven = fmod(checkPos.x + checkPos.y, 2.0) < 1.0;

                // Choose the color based on the even/odd check
                fixed4 color = isEven ? _ColorA : _ColorB;

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
