Shader "Custom/Unit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainColor("MainColor", Color) = (1, 1, 1, 1)
		_RimWidth("Rim Width", Range(0.0001, 0.3)) = 0.05
		_RimStrength("Rim Strength", Range(0, 1)) = 1
		_RimColor("Rim Color", Color) = (0.1, 0.1, 0.1, 1)
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
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _RimWidth;
			float _RimStrength;
			fixed4 _RimColor;
			fixed4 _MainColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= _MainColor;
				fixed4 colMul = fixed4(1, 1, 1, 1) - _RimColor;
				float2 uv = i.uv - float2(0.5, 0.5);
				float edge = 0.5 - _RimWidth;
				float x = 1 - saturate(abs(uv.x) - edge) / _RimWidth;
				float y = 1 - saturate(abs(uv.y) - edge) / _RimWidth;
				//colMul = fixed4(1, 1, 1, 1) - 
				col = col * x * y + (1 - x * y) * _RimColor;
				col.a = 1;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
