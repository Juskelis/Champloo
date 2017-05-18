// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33229,y:32719,varname:node_1873,prsc:2|emission-1749-OUT,alpha-603-OUT,voffset-9182-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32551,y:32729,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1086,x:32812,y:32818,cmnt:RGB,varname:node_1086,prsc:2|A-4805-RGB,B-5983-RGB,C-5376-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:32551,y:32915,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32551,y:33079,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1749,x:33025,y:32818,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-1086-OUT,B-603-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:32812,y:32992,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-5983-A,C-5376-A;n:type:ShaderForge.SFN_Vector4Property,id:5037,x:31020,y:33086,ptovrint:False,ptlb:WorldPosObstacle,ptin:_WorldPosObstacle,varname:node_5037,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_ValueProperty,id:1416,x:31512,y:33067,ptovrint:False,ptlb:ObstacleDistanceLimit,ptin:_ObstacleDistanceLimit,varname:node_1416,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_TexCoord,id:9903,x:32445,y:33349,varname:node_9903,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:3893,x:32812,y:33228,varname:node_3893,prsc:2|A-373-OUT,B-8537-OUT;n:type:ShaderForge.SFN_Multiply,id:9182,x:33019,y:33228,varname:node_9182,prsc:2|A-3893-OUT,B-445-OUT;n:type:ShaderForge.SFN_Vector3,id:445,x:32812,y:33349,varname:node_445,prsc:2,v1:1,v2:0,v3:0;n:type:ShaderForge.SFN_Power,id:8537,x:32634,y:33349,varname:node_8537,prsc:2|VAL-9903-V,EXP-489-OUT;n:type:ShaderForge.SFN_Vector1,id:489,x:32445,y:33491,varname:node_489,prsc:2,v1:2;n:type:ShaderForge.SFN_Sin,id:4001,x:31853,y:33349,varname:node_4001,prsc:2|IN-9136-OUT;n:type:ShaderForge.SFN_Multiply,id:7497,x:32035,y:33349,varname:node_7497,prsc:2|A-3324-OUT,B-4001-OUT;n:type:ShaderForge.SFN_Add,id:373,x:32634,y:33228,cmnt:Horizontal Push,varname:node_373,prsc:2|A-5859-OUT,B-969-OUT;n:type:ShaderForge.SFN_Slider,id:9383,x:31512,y:33227,ptovrint:False,ptlb:WindBend,ptin:_WindBend,varname:node_9383,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Multiply,id:3324,x:31853,y:33208,varname:node_3324,prsc:2|A-9383-OUT,B-3578-OUT;n:type:ShaderForge.SFN_Get,id:3578,x:31648,y:33292,varname:node_3578,prsc:2|IN-1378-OUT;n:type:ShaderForge.SFN_Set,id:9604,x:31669,y:33067,varname:ObstacleDistanceLimitValue,prsc:2|IN-1416-OUT;n:type:ShaderForge.SFN_Time,id:1768,x:31423,y:33292,varname:node_1768,prsc:2;n:type:ShaderForge.SFN_Add,id:5304,x:32238,y:33349,varname:node_5304,prsc:2|A-7497-OUT,B-5197-OUT;n:type:ShaderForge.SFN_Multiply,id:5197,x:32035,y:33475,varname:node_5197,prsc:2|A-4073-OUT,B-8347-OUT;n:type:ShaderForge.SFN_Cos,id:4073,x:31853,y:33475,varname:node_4073,prsc:2|IN-3864-OUT;n:type:ShaderForge.SFN_Vector1,id:8347,x:31853,y:33599,varname:node_8347,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Time,id:6687,x:31246,y:33475,varname:node_6687,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7495,x:31423,y:33475,varname:node_7495,prsc:2|A-6687-T,B-3002-OUT;n:type:ShaderForge.SFN_Add,id:3864,x:31669,y:33475,varname:node_3864,prsc:2|A-7495-OUT,B-1961-OUT;n:type:ShaderForge.SFN_Add,id:9136,x:31669,y:33349,varname:node_9136,prsc:2|A-1768-T,B-448-OUT;n:type:ShaderForge.SFN_Root2,id:3002,x:31279,y:33598,varname:node_3002,prsc:2;n:type:ShaderForge.SFN_ObjectPosition,id:8138,x:31020,y:33227,varname:node_8138,prsc:2;n:type:ShaderForge.SFN_Set,id:4723,x:31380,y:33166,varname:ObstaclePos,prsc:2|IN-5037-XYZ;n:type:ShaderForge.SFN_Get,id:4773,x:31249,y:32868,varname:node_4773,prsc:2|IN-4723-OUT;n:type:ShaderForge.SFN_Distance,id:7298,x:31488,y:32729,varname:node_7298,prsc:2|A-4332-OUT,B-4773-OUT;n:type:ShaderForge.SFN_Subtract,id:4040,x:31488,y:32868,varname:node_4040,prsc:2|A-4332-OUT,B-4773-OUT;n:type:ShaderForge.SFN_Normalize,id:9873,x:32023,y:32868,varname:node_9873,prsc:2|IN-4040-OUT;n:type:ShaderForge.SFN_Get,id:425,x:31467,y:32671,varname:node_425,prsc:2|IN-9604-OUT;n:type:ShaderForge.SFN_Min,id:673,x:31666,y:32729,varname:node_673,prsc:2|A-425-OUT,B-7298-OUT;n:type:ShaderForge.SFN_Subtract,id:123,x:31848,y:32729,varname:node_123,prsc:2|A-8305-OUT,B-673-OUT;n:type:ShaderForge.SFN_Relay,id:8305,x:31725,y:32671,varname:node_8305,prsc:2|IN-425-OUT;n:type:ShaderForge.SFN_Multiply,id:3688,x:32219,y:32729,varname:node_3688,prsc:2|A-2332-OUT,B-9873-OUT;n:type:ShaderForge.SFN_Set,id:7146,x:32393,y:32729,varname:FullOffset,prsc:2|IN-3688-OUT;n:type:ShaderForge.SFN_Get,id:3662,x:31832,y:33107,varname:node_3662,prsc:2|IN-7146-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5859,x:32238,y:33202,varname:node_5859,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-2229-OUT;n:type:ShaderForge.SFN_Relay,id:969,x:32445,y:33299,varname:node_969,prsc:2|IN-5304-OUT;n:type:ShaderForge.SFN_Add,id:26,x:31208,y:33227,varname:node_26,prsc:2|A-8138-XYZ,B-8944-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:8944,x:31020,y:33374,ptovrint:False,ptlb:PositionOffset,ptin:_PositionOffset,varname:node_8944,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Set,id:8609,x:31380,y:33227,varname:Root,prsc:2|IN-26-OUT;n:type:ShaderForge.SFN_Get,id:4332,x:31249,y:32729,varname:node_4332,prsc:2|IN-8609-OUT;n:type:ShaderForge.SFN_Get,id:448,x:31402,y:33416,varname:node_448,prsc:2|IN-8609-OUT;n:type:ShaderForge.SFN_Get,id:1961,x:31402,y:33598,varname:node_1961,prsc:2|IN-8609-OUT;n:type:ShaderForge.SFN_Divide,id:2332,x:32023,y:32729,cmnt:0..1,varname:node_2332,prsc:2|A-123-OUT,B-1186-OUT;n:type:ShaderForge.SFN_Relay,id:1186,x:31907,y:32671,varname:node_1186,prsc:2|IN-8305-OUT;n:type:ShaderForge.SFN_Multiply,id:2229,x:32035,y:33107,varname:node_2229,prsc:2|A-3662-OUT,B-5866-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6234,x:31512,y:33141,ptovrint:False,ptlb:BendAmount,ptin:_BendAmount,varname:node_6234,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Set,id:1378,x:31669,y:33141,varname:Bend,prsc:2|IN-6234-OUT;n:type:ShaderForge.SFN_Get,id:5866,x:31832,y:33158,varname:node_5866,prsc:2|IN-1378-OUT;proporder:4805-5983-5037-1416-9383-8944-6234;pass:END;sub:END;*/

Shader "Shader Forge/Grass2D" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _WorldPosObstacle ("WorldPosObstacle", Vector) = (0,0,0,0)
        _ObstacleDistanceLimit ("ObstacleDistanceLimit", Float ) = 3
        _WindBend ("WindBend", Range(0, 1)) = 0.1
        _PositionOffset ("PositionOffset", Vector) = (0,0,0,0)
        _BendAmount ("BendAmount", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float4 _WorldPosObstacle;
            uniform float _ObstacleDistanceLimit;
            uniform float _WindBend;
            uniform float4 _PositionOffset;
            uniform float _BendAmount;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float ObstacleDistanceLimitValue = _ObstacleDistanceLimit;
                float node_425 = ObstacleDistanceLimitValue;
                float node_8305 = node_425;
                float3 Root = (objPos.rgb+_PositionOffset.rgb);
                float3 node_4332 = Root;
                float3 ObstaclePos = _WorldPosObstacle.rgb;
                float3 node_4773 = ObstaclePos;
                float3 FullOffset = (((node_8305-min(node_425,distance(node_4332,node_4773)))/node_8305)*normalize((node_4332-node_4773)));
                float Bend = _BendAmount;
                float4 node_1768 = _Time + _TimeEditor;
                float4 node_6687 = _Time + _TimeEditor;
                float3 node_9182 = ((((FullOffset*Bend).r+(((_WindBend*Bend)*sin((node_1768.g+Root)))+(cos(((node_6687.g*1.41421356237309504)+Root))*0.25)))*pow(o.uv0.g,2.0))*float3(1,0,0));
                v.vertex.xyz += node_9182;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_603 = (_MainTex_var.a*_Color.a*i.vertexColor.a); // A
                float3 emissive = ((_MainTex_var.rgb*_Color.rgb*i.vertexColor.rgb)*node_603);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_603);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _WorldPosObstacle;
            uniform float _ObstacleDistanceLimit;
            uniform float _WindBend;
            uniform float4 _PositionOffset;
            uniform float _BendAmount;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float ObstacleDistanceLimitValue = _ObstacleDistanceLimit;
                float node_425 = ObstacleDistanceLimitValue;
                float node_8305 = node_425;
                float3 Root = (objPos.rgb+_PositionOffset.rgb);
                float3 node_4332 = Root;
                float3 ObstaclePos = _WorldPosObstacle.rgb;
                float3 node_4773 = ObstaclePos;
                float3 FullOffset = (((node_8305-min(node_425,distance(node_4332,node_4773)))/node_8305)*normalize((node_4332-node_4773)));
                float Bend = _BendAmount;
                float4 node_1768 = _Time + _TimeEditor;
                float4 node_6687 = _Time + _TimeEditor;
                float3 node_9182 = ((((FullOffset*Bend).r+(((_WindBend*Bend)*sin((node_1768.g+Root)))+(cos(((node_6687.g*1.41421356237309504)+Root))*0.25)))*pow(o.uv0.g,2.0))*float3(1,0,0));
                v.vertex.xyz += node_9182;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
