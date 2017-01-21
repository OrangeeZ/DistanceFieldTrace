// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/WarFogPostEffect"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_WarFogTexture("Texture", 2D) = "white" {}
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
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			sampler2D _MainTex;
			sampler2D_float _WarFogTexture;
			sampler2D_float _CameraDepthTexture;
			float4x4 _World2Texture;
			float4x4 _ViewProjectInverse;
			float4x4 _Camera2World;
			half _WarFogBrightness;
			fixed _MaxFieldDistance;
			float3 _WorldTracerPosition;

            float3 GetWorldPosition(float2 uv)
            {
                #if UNITY_UV_STARTS_AT_TOP
                    uv.y = 1 - uv.y;
                #endif

                float depth = (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv.xy));
				depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, (depth));

                float4 clipSpacePosition = 0;
                clipSpacePosition.xy = uv.xy * 2 - 1;
                clipSpacePosition.z = depth;
                clipSpacePosition.w = 1;

                float4 worldPosition = mul(_ViewProjectInverse, clipSpacePosition);
                worldPosition /= worldPosition.w;

                return worldPosition.xyz;
            }

			float SampleDistanceField(float3 worldPosition)
			{
    			half2 worldCoords = mul(_World2Texture, worldPosition).xz - float2(0.5, 0.5);

				return tex2D(_WarFogTexture, worldCoords).r * _MaxFieldDistance;// pow(tex2D(_WarFogTexture, worldCoords).r, 1.2) * _MaxFieldDistance;// - 1 / _MaxFieldDistance;
			}

			fixed4 frag(v2f i) : SV_Target
			{
			    float2 uv = i.uv;
				fixed4 color = tex2D(_MainTex, uv);

                float threshold = 0.0001;

                float3 worldPosition = GetWorldPosition(uv);
                float3 direction = (_WorldTracerPosition - worldPosition);
                direction.y = 0;

                //return (direction / _MaxFieldDistance).xyzz;

                //return ((direction / _MaxFieldDistance).xyzy + 1) / 2;

                float distanceToLight = abs(length(direction));
                direction /= distanceToLight;

				float distanceFieldValue = 0;//SampleDistanceField(worldPosition);

				//float depth = (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv.xy));
				//depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, (depth));
				
				//return Linear01Depth(depth);

                //return lerp(color, sqrt(SampleDistanceField(worldPosition) / _MaxFieldDistance), 0.5);

                fixed itr = 0;
                for (; itr < 20; ++itr)
                {
					distanceFieldValue = SampleDistanceField(worldPosition);

                    if (distanceFieldValue < threshold) return 0;//distanceToLight / _MaxFieldDistance;

					worldPosition += direction * distanceFieldValue;

                    distanceToLight -= distanceFieldValue;
					
					if (distanceToLight <= 0) return color;
                }

				return 0;// lerp(color, fixed4(0, 0, 0, 0), saturate(10 * pow(distanceToLight / _MaxFieldDistance, 1)));

                return fixed4(0.5, 0.5, 0.5, 1);

				return lerp(float4(0, 0, 0, 0), color, distanceFieldValue * _WarFogBrightness);
			}
			ENDCG
		}
	}
}
