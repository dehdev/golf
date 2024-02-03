Shader "Custom/ShootLineRenderer"
{
    Properties{
        _Tex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1) // Add a color property with default white
    }
   
    SubShader{
       
        Ztest Always
        Blend SrcAlpha OneMinusSrcAlpha
       
        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct input{
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
 
            struct output{
                float4 vertex : SV_POSITION;
                float4 texcoord : TEXCOORD0;
                fixed4 color : COLOR0; // Add color to the output structure
            };
 
            uniform sampler2D _Tex;
            uniform fixed4 _Color; // Uniform for the color property
 
            // ---------------------------------------------------SHADER METHODS
           
            output vert(input i){
                output o;
                UNITY_SETUP_INSTANCE_ID(i);
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.texcoord = i.texcoord;
                o.color = _Color; // Pass the color to the fragment shader
                return o;
            }
 
            fixed4 frag(output o) : SV_TARGET{
                // Multiply texture color by the input color
                fixed4 texColor = tex2D(_Tex, o.texcoord);
                return texColor * o.color;
            }
            ENDCG
        }
    }
}
