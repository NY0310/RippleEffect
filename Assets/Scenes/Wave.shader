Shader "Hidden/Wave"
{
	Properties
	{
	    _S2("PhaseVelocity^2", Range(0.0, 0.5)) = 0.2
        [PowerSlider(0.01)]
        _Atten("Attenuation", Range(0.0, 1.0)) = 0.999
        _DeltaUV("Delta UV", Float) = 3
        _AddWaveValue("Add Wave Value", Range(-1.0,1.0)) = 1.0
	}

	CGINCLUDE
			
	#include "UnityCustomRenderTexture.cginc"
          
	half _S2;
	half _Atten;
	float _DeltaUV;
    float _AddWaveValue;
    
	float4 frag(v2f_customrendertexture i) : SV_Target
	{
		float2 uv = i.globalTexcoord;

		//1px 単位計算
		float du = 1.0 / _CustomRenderTextureWidth;
		float dv = 1.0 / _CustomRenderTextureHeight;
		float3 duv = float3(du, dv, 0) * _DeltaUV;

		float2 c = tex2D(_SelfTexture2D, uv);

		// ラジアフィタをかける
		float k = 2.0 * c.r - c.g;
		float p = (k + _S2 * (
		tex2D(_SelfTexture2D, uv - duv.zy).r +
		tex2D(_SelfTexture2D, uv + duv.zy).r +
		tex2D(_SelfTexture2D, uv - duv.xz).r +
		tex2D(_SelfTexture2D, uv + duv.xz).r - 4 * c.r)) * _Atten;

		// 現在状態テクスチャのR成分に、ひとつ前の（過去の）状態をG成分に書き込む。
		return float4(p, c.r, 0 , 0);
	}


    float4 add_wave(v2f_customrendertexture i) : SV_Target
    {
        return float4(_AddWaveValue, 0, 0, 0);
    }


	ENDCG

	SubShader	
	{
		Cull Off ZWrite Off ZTest Always
		pass
		{
		    Name "Update"
		    CGPROGRAM
		    #pragma vertex CustomRenderTextureVertexShader
		    #pragma fragment frag
		    ENDCG
		}

		pass 
		{
			Name "AddWave"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment add_wave
			ENDCG
		}
	}
		
}
