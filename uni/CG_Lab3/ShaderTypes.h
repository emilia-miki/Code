//
//  ShaderTypes.h
//  CG_Lab3
//
//  Created by Mykyta Diachyna on 14.12.2022.
//

//
//  Header containing types and enum constants shared between Metal shaders and Swift/ObjC source
//
#ifndef ShaderTypes_h
#define ShaderTypes_h

#ifdef __METAL_VERSION__
#define NS_ENUM(_type, _name) enum _name : _type _name; enum _name : _type
typedef metal::int32_t EnumBackingType;
#else
#import <Foundation/Foundation.h>
typedef NSInteger EnumBackingType;
#endif

#include <simd/simd.h>

typedef NS_ENUM(EnumBackingType, BufferIndex)
{
    BufferIndexMeshPositions = 0,
    BufferIndexMeshGenerics  = 1,
    BufferIndexMeshNormals = 2,
    BufferIndexUniforms      = 3
};

typedef NS_ENUM(EnumBackingType, VertexAttribute)
{
    VertexAttributePosition  = 0,
    VertexAttributeTexcoord  = 1,
    VertexAttributeNormal = 2
};

typedef NS_ENUM(EnumBackingType, TextureIndex)
{
    TextureIndexColor    = 0,
};

typedef struct
{
    matrix_float4x4 projectionMatrix;
    matrix_float4x4 modelViewMatrix;
    simd_float4 lightPosition0;
    simd_float4 lightIntensity0;
    simd_float4 lightPosition1;
    simd_float4 lightIntensity1;
    simd_float4 reflectivity;
} Uniforms;

typedef struct
{
    matrix_float4x4 projectionMatrix;
    matrix_float4x4 scalingMatrix;
} UniformTruncated;

#endif /* ShaderTypes_h */

