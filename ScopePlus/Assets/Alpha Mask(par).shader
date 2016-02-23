Shader "Unlit/AlphaMaskPar" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _AlphaTex ("Alpha mask (R)", 2D) = "white" {}
  //  _AlphaVal ("AlphaVal", Range (0,1) ) = 1.0
    //alp ("alpha", float) = 0.5
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
   
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
   
    Pass {  
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                //float2 texcoordA : TEXCOORD1; // alpha uv
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                //half2 texcoordA : TEXCOORD1; // alpha uv
            };

            sampler2D _MainTex;
            sampler2D _AlphaTex;
           	//fixed alp;
           //	float _AlphaVal;
            float4 _MainTex_ST;
           // float4 _AlphaTex_ST; // alpha uv

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
               // o.texcoordA = TRANSFORM_TEX(v.texcoordA, _AlphaTex); // alpha uv
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, (i.texcoord)/2);
                fixed4 col2 = tex2D(_AlphaTex, i.texcoord/2+0.5);
               
                return fixed4(col.r, col.g, col.b, col2.r);
            }
        ENDCG
    }
}

}