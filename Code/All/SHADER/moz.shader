Shader "Unlit/moz"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

            Cull Off
            ZWrite On
            ZTest Always
            ZClip FALSE

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			float4 _MainTex_TexelSize;// (x, y, z, w) = (1/width, 1/height, width, height)
            float4 _MainTex_ST;

            float4 color;
			float moz;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				float2 fragCoord = i.uv * _MainTex_TexelSize.zw;
				int m = (int)moz;

                fixed4 col = tex2D(_MainTex,
                                   float2((int)fragCoord.x / m * m, (int)fragCoord.y / m * m)
                                   * _MainTex_TexelSize.xy);

                return col * color;
            }
            ENDCG
        }
    }
}
