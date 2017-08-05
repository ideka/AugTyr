Shader "Custom/MakeTransparent" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TransparentColorKey ("Transparent Color Key", Color) = (0,0,0,1)
		_TransparencyMargin ("Transparency Margin", Float) = 0.01 
	}
	SubShader {
		Pass {
			Tags { "RenderType"="Opaque" }
			LOD 200
		
			CGPROGRAM

			#pragma vertex VertexShaderFunction
			#pragma fragment PixelShaderFunction
		
			#include "UnityCG.cginc"

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
				output.position = UnityObjectToClipPos (input.position);
				output.uv = input.uv;
				return output;
			}
		
			sampler2D _MainTex;
			float3 _TransparentColorKey;
			float _TransparencyMargin;

			float4 PixelShaderFunction(VertexToPixelData input) : SV_Target
			{
				float4 color = tex2D(_MainTex, input.uv);
			
				float deltaR = abs(color.r - _TransparentColorKey.r);
				float deltaG = abs(color.g - _TransparentColorKey.g);
				float deltaB = abs(color.b - _TransparentColorKey.b);

				if (deltaR < _TransparencyMargin && deltaG < _TransparencyMargin && deltaB < _TransparencyMargin)
				{
					return float4(0.0f, 0.0f, 0.0f, 0.0f);
				}

				return float4(0.0f, 0.0f, 0.0f, 0.0f);
			}
			ENDCG
		}
	}
}
