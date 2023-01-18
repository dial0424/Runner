Shader "Unlit/blur"
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
			float blur;

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
                //fixed4 col = tex2D(_MainTex, i.uv);

				float4 s = 0;
				for (int n = -4; n < 5; n++)
				{
					float4  sum  = tex2D(_MainTex, float2(fragCoord.x - 4 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
							sum += tex2D(_MainTex, float2(fragCoord.x - 3 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
							sum += tex2D(_MainTex, float2(fragCoord.x - 2 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
							sum += tex2D(_MainTex, float2(fragCoord.x - 1 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
							sum += tex2D(_MainTex, float2(fragCoord.x + 0 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
							sum += tex2D(_MainTex, float2(fragCoord.x + 1 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
							sum += tex2D(_MainTex, float2(fragCoord.x + 2 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
							sum += tex2D(_MainTex, float2(fragCoord.x + 3 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
							sum += tex2D(_MainTex, float2(fragCoord.x + 4 * blur, fragCoord.y + n * blur) * _MainTex_TexelSize.xy);
					sum /= 9;
					s += sum;
				}
				s /= 9;

                //float grey = col.r * 0.25 + col.g * 0.6 + col.b * 0.15;
                //col.rgb = grey;

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return s * color;
            }
            ENDCG
        }
    }
}
