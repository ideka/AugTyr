Shader "Custom/FullTransparency"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

		SubShader
	{
		Pass
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM

			#pragma vertex VertexShaderFunction
			#pragma fragment PixelShaderFunction
		
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct VertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct VertexToPixelData
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			VertexToPixelData VertexShaderFunction(VertexData input)
			{
				VertexToPixelData output;
				output.position = UnityObjectToClipPos(input.position);
				output.uv = input.uv;
				return output;
			}

			float4 PixelShaderFunction(VertexToPixelData input) : SV_Target
			{
				return float4(0, 0, 0, 0);
			}

			ENDCG
		}
	}
}
