#ifndef VACUUM_SHADERS_VC_VERTEXLIT_CGINC
#define VACUUM_SHADERS_VC_VERTEXLIT_CGINC


//Variables//////////////////////////////////
fixed4 _Color;

#ifdef V_VC_COLOR_AND_TEXTURE_ON
	sampler2D _MainTex;
	half4 _MainTex_ST;
	half2 _V_VC_MainTex_Scroll;
#endif

#ifdef V_VC_RENDERING_MODE_CUTOUT
	half _Cutoff; 
#endif

#include "UnityCG.cginc"

////////////////////////////////////////////////////////////////////////////
//																		  //
//Struct    															  //
//																		  //
////////////////////////////////////////////////////////////z////////////////
struct v2f  
{  
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;	

	#ifdef V_VC_LIGHTMAP_ON
		half2 lm : TEXCOORD1;
	#else
		half4 vLight : TEXCOORD1;
	#endif		
	
	fixed4 vColor : TEXCOORD2;	

	//FOG
	UNITY_FOG_COORDS(5)	
};

 
////////////////////////////////////////////////////////////////////////////
//																		  //
//Vertex    															  //
//																		  //
////////////////////////////////////////////////////////////////////////////
// Used in Vertex pass: Calculates diffuse lighting from lightCount lights. Specifying true to spotLight is more expensive
// to calculate but lights are treated as spot lights otherwise they are treated as point lights.
float3 V_VC_ShadeVertexLightsFull(float4 vertex, float3 normal, int lightCount, bool spotLight)
{
	float3 viewpos = UnityObjectToViewPos(vertex);
	float3 viewN = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normal));

	float3 lightColor = 0;// UNITY_LIGHTMODEL_AMBIENT.xyz;

	for (int i = 0; i < lightCount; i++) {
		float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
		float lengthSq = dot(toLight, toLight);

		// don't produce NaNs if some vertex position overlaps with the light
		lengthSq = max(lengthSq, 0.000001);

		toLight *= rsqrt(lengthSq);

		float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
		if (spotLight)
		{
			float rho = max(0, dot(toLight, unity_SpotDirection[i].xyz));
			float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
			atten *= saturate(spotAtt);
		}

		float diff = max(0, dot(viewN, toLight));
		lightColor += unity_LightColor[i].rgb * (diff * atten);
	}
	return lightColor;
}

v2f vert (appdata_full v) 
{   
	v2f o;
	UNITY_INITIALIZE_OUTPUT(v2f,o); 

	
	o.pos = UnityObjectToClipPos(v.vertex); 

	#ifdef V_VC_COLOR_AND_TEXTURE_ON
		o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
		o.uv.xy += _V_VC_MainTex_Scroll.xy * _Time.x;
	#endif


	#ifdef V_VC_LIGHTMAP_ON
		o.lm = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#else
		o.vLight = float4(V_VC_ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1);	
	#endif

	
	o.vColor = v.color;	

	//FOG
	UNITY_TRANSFER_FOG(o, o.pos);

	return o; 
}


////////////////////////////////////////////////////////////////////////////
//																		  //
//Fragment    															  //
//																		  //
////////////////////////////////////////////////////////////////////////////
fixed4 frag (v2f i) : SV_Target 
{
	fixed4 retColor = i.vColor;

	//Main Texture
	#ifdef V_VC_COLOR_AND_TEXTURE_ON
		half4 mainTex = tex2D(_MainTex, i.uv.xy);
				
		retColor *= mainTex * _Color;
	#endif
	 
	//Cutout
	#ifdef V_VC_RENDERING_MODE_CUTOUT
		clip(retColor.a - _Cutoff);
	#endif


	#ifdef V_VC_LIGHTMAP_ON
		half3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lm));

		retColor.rgb *= lm.rgb;

	#else 
		retColor *= i.vLight;
	#endif
			   
	
	//Fog
	UNITY_APPLY_FOG(i.fogCoord, retColor);

	//Alpha
	#if !defined(V_VC_RENDERING_MODE_CUTOUT) && !defined(V_VC_RENDERING_MODE_TRANSPARENT)
		UNITY_OPAQUE_ALPHA(retColor.a);
	#endif

	return retColor;
} 

#endif 
