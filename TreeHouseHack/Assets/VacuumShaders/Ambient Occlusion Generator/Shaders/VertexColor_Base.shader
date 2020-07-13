Shader "VacuumShaders/Vertex Color/Base" 
{
    SubShader 
	{
		Cull Off

        Pass 
		{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"
			
            struct vertexInput 
			{
                float4 vertex : POSITION;
                fixed4 color : COLOR;
            };

            struct fragmentInput
			{
                float4 position : SV_POSITION;
                float4 color : TEXCOORD0;
            };

            fragmentInput vert(vertexInput v)
			{
                fragmentInput o = (fragmentInput)0;

                o.position = UnityObjectToClipPos (v.vertex);
                                
				o.color = v.color;

				return o;
            }

            fixed4 frag(fragmentInput i) : SV_Target 
			{
				return i.color;
            }
            ENDCG
        }
    }
}
