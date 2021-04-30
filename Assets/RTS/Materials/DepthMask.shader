Shader "Peter/DarkEffect"
{
    Properties
    {
      _MainTex("Texture", 2D) = "white" {}
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

          // Maximum number of tracking objects
          #define ItemSize 9

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

          sampler2D _MainTex;

          fixed4 _DarkColor;
          float _SmoothLength;
          fixed _ItemCnt;
          float4 _Item[ItemSize];

          v2f vert(appdata v)
          {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
          }

          fixed CalcAlpha(float4 vt, float4 pt)
          {
            if (pt.z < 0)
            {
              return 1;
            }

            float distPow2 = pow(vt.x - pt.x, 2) + pow(vt.y - pt.y, 2);
            float dist = (distPow2 > 0) ? sqrt(distPow2) : 0;

            float smoothLength = _SmoothLength;
            if (smoothLength < 0)
            {
              smoothLength = 0;
            }

            float maxValue = pt.z;
            float minValue = pt.z - smoothLength;
            if (minValue < 0)
            {
              minValue = 0;
              smoothLength = pt.z;
            }

            if (dist <= minValue)
            {
              return 0;
            }
            else if (dist > maxValue)
            {
              return 1;
            }

            fixed retVal = (dist - minValue) / smoothLength;

            return retVal;
          }

          fixed4 frag(v2f i) : SV_Target
          {
            fixed alphaVal = 1;
            fixed tmpVal = 1;

            for (fixed index = 0; index < _ItemCnt; ++index)
            {
              tmpVal = CalcAlpha(i.vertex, _Item[index]);
              if (tmpVal < alphaVal)
              {
                alphaVal = tmpVal;
              }
            }

            alphaVal *= _DarkColor.a;

            return tex2D(_MainTex, i.uv) * (1 - alphaVal) + _DarkColor * alphaVal;
          }

          ENDCG
        }
    }
}