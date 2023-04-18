//
//  Shaders.metal
//  CG_Lab1
//
//  Created by Mykyta Diachyna on 01.12.2022.
//

// File for Metal kernel and shader functions

#include <metal_stdlib>
#include <simd/simd.h>

using namespace metal;

struct VertexInOut
{
    float4 position [[ position ]];
    float4 color;
};

vertex VertexInOut basic_vertex(
        uint vid [[ vertex_id ]],
        constant packed_float3* position [[ buffer(0) ]],
        constant packed_float4* color [[ buffer(1) ]],
        constant matrix_float4x4 &translationMatrix [[ buffer(2) ]]) {
            VertexInOut outVertex;
            
            outVertex.position = translationMatrix * float4(position[vid], 1.0);
            outVertex.color = color[vid];
            
            return outVertex;
}

fragment half4 basic_fragment(VertexInOut inFrag [[stage_in]]) {
    return half4(inFrag.color);
}
