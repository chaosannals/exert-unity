Shader "Fighting/Shadow" {
	Properties{
		_ShadowTexture("Shadow", 2D) = "gray" {}
	}
	SubShader{
		Tags { "Queue"="Transparent-1" "RenderType" = "Transparent" }
		Pass {
			ZWrite Off
			Blend One One, SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 position: SV_POSITION;
				float4 shadow: TEXCOORD0;
			};

			sampler2D _ShadowTexture;
			float4x4 unity_Projector;

			v2f vert(appdata_base v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.shadow = mul(unity_Projector, v.vertex);
				return o;
			}

			float4 frag(v2f i): COLOR {
				float4 shadow = tex2Dproj(_ShadowTexture, i.shadow);
				return shadow;
			}
			ENDCG
		}
	}
}
