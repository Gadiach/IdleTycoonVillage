Shader "UI/TutorialSpotlight"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0,0,0,1)

        _HoleCenter ("Hole Center", Vector) = (0.5,0.5,0,0)
        _HoleSize ("Hole Size", Vector) = (0.2,0.2,0,0)

        _CornerRadius ("Corner Radius", Range(0.0, 0.1)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            fixed4 _Color;
            float4 _HoleCenter;
            float4 _HoleSize;
            float _CornerRadius;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 halfSize = _HoleSize.xy * 0.5;

                float2 position =
                    abs(i.uv - _HoleCenter.xy);

                float2 q =
                    position - halfSize + _CornerRadius;

                float distanceToRoundedRectangle =
                    length(max(q, 0.0)) +
                    min(max(q.x, q.y), 0.0) -
                    _CornerRadius;

                if (distanceToRoundedRectangle < 0)
                    return fixed4(0, 0, 0, 0);

                return i.color;
            }

            ENDCG
        }
    }
}