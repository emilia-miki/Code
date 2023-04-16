//
//  Shaders.metal
//  CG_Lab2
//
//  Created by Mykyta Diachyna on 06.12.2022.
//

// File for Metal kernel and shader functions

#include <metal_stdlib>
#include <simd/simd.h>

// Including header shared between this Metal shader code and Swift/C code executing Metal API commands
#import "ShaderTypes.h"

using namespace metal;

typedef struct
{
    float3 position [[attribute(VertexAttributePosition)]];
    char color [[attribute(VertexAttributeColor)]];
} Vertex;

typedef struct
{
    float4 position [[position]];
    char color;
} ColorInOut;

vertex ColorInOut vertexShader(Vertex in [[stage_in]],
                               constant Uniforms & uniforms [[ buffer(BufferIndexUniforms) ]])
{
    ColorInOut out;

    float4 position = float4(in.position, 1.0);
    out.position = uniforms.projectionMatrix * uniforms.modelViewMatrix * position;
    out.color = in.color;

    return out;
}

vertex ColorInOut customVertexShader(unsigned int vid [[vertex_id]], constant packed_float3* vertex_array [[buffer(0)]], constant char* color_array [[buffer(1)]], constant Uniforms & uniforms [[ buffer(2) ]]) {
    ColorInOut out;
    
    float4 position = float4(vertex_array[vid], 1.0);
    out.position = uniforms.projectionMatrix * uniforms.modelViewMatrix * position;
    out.color = color_array[vid];
    
    return out;
}

fragment float4 fragmentShader(ColorInOut in [[stage_in]],
                               constant Uniforms & uniforms [[ buffer(BufferIndexUniforms) ]])
{
    return in.color != 0 ? float4(1.0, 0.5, 0.8, 1.0) : float4(0.0, 0.5, 1.0, 1.0);
}
