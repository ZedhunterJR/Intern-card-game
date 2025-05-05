Shader "UI/DottedOutline"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (0,0,0,1)
        _Spacing ("Dot Spacing", Float) = 20
        _Thickness ("Line Thickness", Float) = 0.02
        _Speed ("Scroll Speed", Float) = 0.5
        _AspectRatio ("Aspect Ratio", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        LOD 100
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color1;
            fixed4 _Color2;
            float _Spacing;
            float _Thickness;
            float _Speed;
            float _AspectRatio; // <--- new manual aspect

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float aspect = _AspectRatio;
                float2 thickness = float2(_Thickness, _Thickness * aspect);

                float isBorder =
                    step(uv.x, thickness.x) +
                    step(1.0 - uv.x, thickness.x) +
                    step(uv.y, thickness.y) +
                    step(1.0 - uv.y, thickness.y);

                isBorder = saturate(isBorder);

                float scroll = _Time * _Speed;
                float2 aspectUV = uv;
                aspectUV.x *= aspect;

                float patternIndex = floor((aspectUV.x + aspectUV.y + scroll) * _Spacing);
                float pattern = fmod(patternIndex, 2.0);

                fixed4 color = lerp(_Color1, _Color2, pattern);
                return isBorder > 0.5 ? color : fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
