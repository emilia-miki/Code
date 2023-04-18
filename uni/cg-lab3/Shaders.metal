//
//  Shaders.metal
//  CG_Lab3
//
//  Created by Mykyta Diachyna on 14.12.2022.
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
    float2 texCoord [[attribute(VertexAttributeTexcoord)]];
    float3 normal [[attribute(VertexAttributeNormal)]];
} Vertex;

typedef struct
{
    float4 position [[position]];
    float2 texCoord;
    float4 lightIntensity;
} ColorInOut;

typedef struct
{
    float4 position [[position]];
    float2 texCoord;
} ColorInOutNoLighting;

vertex ColorInOutNoLighting skyboxVertexShader(Vertex in [[stage_in]],
    constant UniformTruncated & uniforms [[buffer(BufferIndexUniforms)]])
{
    ColorInOutNoLighting out;
    
    matrix_float4x4 projectionMatrix = uniforms.projectionMatrix;
    matrix_float4x4 scalingMatrix = uniforms.scalingMatrix;
    
    float4 position = projectionMatrix * scalingMatrix * float4(in.position, 1.0);
    
    out.position = position;
    out.texCoord = in.texCoord;
    
    return out;
}

vertex ColorInOut vertexShader(Vertex in [[stage_in]],
                               constant Uniforms & uniforms [[ buffer(BufferIndexUniforms) ]])
{
    ColorInOut out;
    
    float4 position = float4(in.position, 1.0);
    float4 normal = float4(in.normal, 1.0);
    
    // Lighting code
    float4 tnorm = normalize(uniforms.projectionMatrix * normal);
    float4 eyeCoords = uniforms.modelViewMatrix * position;
    float4 s0 = normalize(uniforms.lightPosition0 - eyeCoords);
    float4 s1 = normalize(float4(uniforms.lightPosition1 - eyeCoords));
    
    out.lightIntensity = uniforms.lightIntensity0 * uniforms.reflectivity * max(dot(s0, tnorm), 0.0) + uniforms.lightIntensity1 * uniforms.reflectivity * max(dot(s1, tnorm), 0.0);
    out.position = uniforms.projectionMatrix * uniforms.modelViewMatrix * position;
    out.texCoord = in.texCoord;

    return out;
}

fragment float4 skyboxFragmentShader(ColorInOutNoLighting in [[stage_in]], texture2d<half> colorMap [[texture(TextureIndexColor)]])
{
    constexpr sampler colorSampler(mip_filter::linear,
                                   mag_filter::linear,
                                   min_filter::linear);
    
    half4 colorSample = colorMap.sample(colorSampler, in.texCoord.xy);
    
    return float4(colorSample);
}

fragment float4 fragmentShader(ColorInOut in [[stage_in]],
                               constant Uniforms & uniforms [[ buffer(BufferIndexUniforms) ]],
                               texture2d<half> colorMap     [[ texture(TextureIndexColor) ]])
{
    constexpr sampler colorSampler(mip_filter::linear,
                                   mag_filter::linear,
                                   min_filter::linear);

    half4 colorSample = colorMap.sample(colorSampler, in.texCoord.xy);

    return float4(in.lightIntensity * float4(colorSample));
}
