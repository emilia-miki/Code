//
//  Renderer.swift
//  CG_Lab2
//
//  Created by Mykyta Diachyna on 06.12.2022.
//

// Our platform independent renderer class

import Metal
import MetalKit
import simd

// The 256 byte aligned size of our uniform structure
let alignedUniformsSize = (MemoryLayout<Uniforms>.size + 0xFF) & -0x100

let maxBuffersInFlight = 3
let cameraDistance: Float = 8.0

enum RendererError: Error {
    case badVertexDescriptor
}

enum MeshType {
    case cube
    case plane
    case sphere
}

class Renderer: NSObject, MTKViewDelegate {

    public let device: MTLDevice
    let commandQueue: MTLCommandQueue
    var dynamicUniformBuffer: MTLBuffer
    var pipelineState: MTLRenderPipelineState
    var simplePipelineState: MTLRenderPipelineState
    var depthState: MTLDepthStencilState
    var surfaceBuffer: MTLBuffer
    var colorsBuffer: MTLBuffer
    var surfaceCount: Int

    let inFlightSemaphore = DispatchSemaphore(value: maxBuffersInFlight)

    var uniformBufferOffset = 0

    var uniformBufferIndex = 0

    var uniforms: UnsafeMutablePointer<Uniforms>

    var projectionMatrix: matrix_float4x4 = matrix_float4x4()
    var orthographicMatrix: matrix_float4x4 = matrix_float4x4()
    var currentMatrix: matrix_float4x4 = matrix_float4x4()
    var isOrtho: Bool = false
    var rotationMatrix: matrix_float4x4 = matrix4x4_translation(0, 0, 0)

    var prevRotationX: Float = 0
    var prevRotationY: Float = 0
    var prevRotationZ: Float = 0
    
    var rotationX: Float = 0
    var rotationY: Float = 0
    var rotationZ: Float = 0

    var meshes: [MTKMesh] = []
    var translations: [matrix_float4x4] = []
    
    func setOrthographic() {
        isOrtho = true
        currentMatrix = orthographicMatrix
    }
    
    func setProjection() {
        isOrtho = false
        currentMatrix = projectionMatrix
    }
    
    class func generateSurfaceVertices(leftX: Float, rightX: Float, leftZ: Float, rightZ: Float, step: Float) -> [Float] {
        var surface: [Float] = []
        
        let xSteps = (Int) ((Float) (rightX - leftX) / step)
        let zSteps = (Int) ((Float) (rightZ - leftZ) / step)
        for i in 0..<xSteps {
            let x = leftX + step * Float(i)
            let xNext = leftX + step * (Float(i) + 1.0)
            for j in 0..<zSteps {
                let z = leftZ + step * Float(j)
                let zNext = leftZ + step * (Float(j) + 1.0)

                surface.append(sinf(x) + cosf(z))
                surface.append(x)
                surface.append(z)

                surface.append(sinf(xNext) + cosf(z))
                surface.append(xNext)
                surface.append(z)

                surface.append(sinf(x) + cosf(zNext))
                surface.append(x)
                surface.append(zNext)

                surface.append(sinf(x) + cosf(zNext))
                surface.append(x)
                surface.append(zNext)
                
                surface.append(sinf(xNext) + cosf(z))
                surface.append(xNext)
                surface.append(z)

                surface.append(sinf(xNext) + cosf(zNext))
                surface.append(xNext)
                surface.append(zNext)
                
                surface.append(sinf(x) + cosf(zNext))
                surface.append(x)
                surface.append(zNext)
                
                surface.append(sinf(xNext) + cosf(z))
                surface.append(xNext)
                surface.append(z)
                
                surface.append(sinf(x) + cosf(z))
                surface.append(x)
                surface.append(z)
                
                surface.append(sinf(xNext) + cosf(zNext))
                surface.append(xNext)
                surface.append(zNext)
                
                surface.append(sinf(xNext) + cosf(z))
                surface.append(xNext)
                surface.append(z)

                surface.append(sinf(x) + cosf(zNext))
                surface.append(x)
                surface.append(zNext)
            }
        }

        return surface
    }
    
    class func generateSurfaceColors(leftX: Float, rightX: Float, leftZ: Float, rightZ: Float, step: Float) -> [CChar] {
        var colors: [CChar] = []
        
        let xSteps = (Int) ((Float) (rightX - leftX) / step)
        let zSteps = (Int) ((Float) (rightZ - leftZ) / step)
        for _ in 0..<xSteps {
            for _ in 0..<zSteps {
                for _ in 0..<6 {
                    colors.append(0)
                    colors.append(0)
                    colors.append(0)

                    colors.append(1)
                    colors.append(1)
                    colors.append(1)
                }
            }
        }

        return colors
    }

    init?(metalKitView: MTKView) {
        self.device = metalKitView.device!
        self.commandQueue = self.device.makeCommandQueue()!

        metalKitView.depthStencilPixelFormat = MTLPixelFormat.depth32Float_stencil8
        metalKitView.colorPixelFormat = MTLPixelFormat.bgra8Unorm_srgb
        metalKitView.sampleCount = 1

        let mtlVertexDescriptor = Renderer.buildMetalVertexDescriptor()

        do {
            pipelineState = try Renderer.buildRenderPipelineWithDevice(
                device: device,
                metalKitView: metalKitView,
                mtlVertexDescriptor: mtlVertexDescriptor,
                forMeshes: true)
            simplePipelineState = try Renderer.buildRenderPipelineWithDevice(device: device, metalKitView: metalKitView, mtlVertexDescriptor: mtlVertexDescriptor, forMeshes: false)
        } catch {
            print("Unable to compile render pipeline state.  Error info: \(error)")
            return nil
        }

        let depthStateDescriptor = MTLDepthStencilDescriptor()
        depthStateDescriptor.depthCompareFunction = MTLCompareFunction.less
        depthStateDescriptor.isDepthWriteEnabled = true
        self.depthState = device.makeDepthStencilState(descriptor:depthStateDescriptor)!

        do {
            meshes.append(
                try Renderer.buildMesh(
                    device: device,
                    mtlVertexDescriptor: mtlVertexDescriptor,
                    meshType: .cube,
                    showFaces: true))
            meshes.append(
                try Renderer.buildMesh(
                    device: device,
                    mtlVertexDescriptor: mtlVertexDescriptor,
                    meshType: .sphere,
                    showFaces: false))
            for _ in 0..<2 {
                meshes.append(
                    try Renderer.buildMesh(
                    device: device,
                    mtlVertexDescriptor: mtlVertexDescriptor,
                    meshType: .plane,
                    showFaces: true))
            }
        } catch {
            print("Unable to build MetalKit Mesh. Error info: \(error)")
            return nil
        }
        
        translations.append(matrix4x4_translation(-4.0, 0.0, -8.0))
        translations.append(matrix4x4_translation(4.0, 0.0, -8.0))
        translations.append(simd_mul(matrix4x4_translation(0, 0, -8.0), matrix4x4_rotation(radians: .pi / 2, axis: SIMD3<Float>(0.0, 0.0, 1.0))))
        translations.append(simd_mul(matrix4x4_translation(0, 0, -8.0), matrix4x4_rotation(radians: -.pi / 2, axis: SIMD3<Float>(0.0, 0.0, 1.0))))
        
        let uniformBufferSize = alignedUniformsSize * maxBuffersInFlight * (meshes.count + 1)

        self.dynamicUniformBuffer = self.device.makeBuffer(length:uniformBufferSize,
                                                           options:[MTLResourceOptions.storageModeShared])!

        self.dynamicUniformBuffer.label = "UniformBuffer"

        uniforms = UnsafeMutableRawPointer(dynamicUniformBuffer.contents()).bindMemory(to:Uniforms.self, capacity:1)
        
        let surface = Renderer.generateSurfaceVertices(leftX: -8.0, rightX: 8.0, leftZ: -8.0, rightZ: 8.0, step: 0.05)
        surfaceBuffer = self.device.makeBuffer(bytes: surface, length: surface.count * MemoryLayout.size(ofValue: surface[0]), options: .storageModeShared)!
        colorsBuffer = self.device.makeBuffer(bytes: Renderer.generateSurfaceColors(leftX: -8.0, rightX: 8.0, leftZ: -8.0, rightZ: 8.0, step: 0.05), length: surface.count / 3)!
        surfaceCount = surface.count / 3
        
        super.init()

    }
    
    func reset() {
        rotationX = 0.01
        prevRotationX = 0.01
        rotationY = 0.0
        prevRotationY = 0.0
        rotationZ = 0.0
        prevRotationZ = 0.0
        rotationMatrix = matrix4x4_rotation(radians: rotationX * 2 * .pi, axis: SIMD3<Float>(0, 1, 0))
    }

    class func buildMetalVertexDescriptor() -> MTLVertexDescriptor {
        // Create a Metal vertex descriptor specifying how vertices will by laid out for input into our render
        //   pipeline and how we'll layout our Model IO vertices

        let mtlVertexDescriptor = MTLVertexDescriptor()

        mtlVertexDescriptor.attributes[VertexAttribute.position.rawValue].format = MTLVertexFormat.float3
        mtlVertexDescriptor.attributes[VertexAttribute.position.rawValue].offset = 0
        mtlVertexDescriptor.attributes[VertexAttribute.position.rawValue].bufferIndex = BufferIndex.meshPositions.rawValue

        mtlVertexDescriptor.attributes[VertexAttribute.color.rawValue].format = MTLVertexFormat.char
        mtlVertexDescriptor.attributes[VertexAttribute.color.rawValue].offset = 0
        mtlVertexDescriptor.attributes[VertexAttribute.color.rawValue].bufferIndex = BufferIndex.meshGenerics.rawValue

        mtlVertexDescriptor.layouts[BufferIndex.meshPositions.rawValue].stride = 12
        mtlVertexDescriptor.layouts[BufferIndex.meshPositions.rawValue].stepRate = 1
        mtlVertexDescriptor.layouts[BufferIndex.meshPositions.rawValue].stepFunction = MTLVertexStepFunction.perVertex

        mtlVertexDescriptor.layouts[BufferIndex.meshGenerics.rawValue].stride = 8
        mtlVertexDescriptor.layouts[BufferIndex.meshGenerics.rawValue].stepRate = 1
        mtlVertexDescriptor.layouts[BufferIndex.meshGenerics.rawValue].stepFunction = MTLVertexStepFunction.perVertex

        return mtlVertexDescriptor
    }

    class func buildRenderPipelineWithDevice(device: MTLDevice,
                                             metalKitView: MTKView,
                                             mtlVertexDescriptor: MTLVertexDescriptor,
                                             forMeshes: Bool) throws -> MTLRenderPipelineState {
        /// Build a render state pipeline object

        let library = device.makeDefaultLibrary()

        let vertexFunction = library?.makeFunction(name: forMeshes ? "vertexShader" : "customVertexShader")
        let fragmentFunction = library?.makeFunction(name: "fragmentShader")

        let pipelineDescriptor = MTLRenderPipelineDescriptor()
        pipelineDescriptor.label = "RenderPipeline"
        pipelineDescriptor.rasterSampleCount = metalKitView.sampleCount
        pipelineDescriptor.vertexFunction = vertexFunction
        pipelineDescriptor.fragmentFunction = fragmentFunction
        pipelineDescriptor.vertexDescriptor = mtlVertexDescriptor

        pipelineDescriptor.colorAttachments[0].pixelFormat = metalKitView.colorPixelFormat
        pipelineDescriptor.depthAttachmentPixelFormat = metalKitView.depthStencilPixelFormat
        pipelineDescriptor.stencilAttachmentPixelFormat = metalKitView.depthStencilPixelFormat

        return try device.makeRenderPipelineState(descriptor: pipelineDescriptor)
    }

    class func buildMesh(device: MTLDevice,
                         mtlVertexDescriptor: MTLVertexDescriptor,
                         meshType: MeshType,
                         showFaces: Bool) throws -> MTKMesh {
        /// Create and condition mesh data to feed into a pipeline using the given vertex descriptor

        let metalAllocator = MTKMeshBufferAllocator(device: device)
        let geometryType = showFaces ? MDLGeometryType.triangles : MDLGeometryType.lines

        let mdlMesh: MDLMesh
        switch (meshType) {
        case .plane:
            mdlMesh = MDLMesh.newPlane(withDimensions: SIMD2<Float>(10, 10), segments: SIMD2<UInt32>(10, 10), geometryType: geometryType, allocator: metalAllocator)
        case .cube:
            mdlMesh = MDLMesh.newBox(withDimensions: SIMD3<Float>(2, 2, 2),
                                     segments: SIMD3<UInt32>(2, 2, 2),
                                     geometryType: geometryType,
                                     inwardNormals: false,
                                     allocator: metalAllocator)
        case .sphere:
            mdlMesh = MDLMesh.newEllipsoid(withRadii: SIMD3<Float>(1, 1, 1), radialSegments: 100, verticalSegments: 100, geometryType: geometryType, inwardNormals: false, hemisphere: false, allocator: metalAllocator)
        }
        
        let mdlVertexDescriptor = MTKModelIOVertexDescriptorFromMetal(mtlVertexDescriptor)
        
        guard let attributes = mdlVertexDescriptor.attributes as? [MDLVertexAttribute] else {
            throw RendererError.badVertexDescriptor
        }
        attributes[VertexAttribute.position.rawValue].name = MDLVertexAttributePosition
        attributes[VertexAttribute.color.rawValue].name = MDLVertexAttributeColor
        attributes[VertexAttribute.color.rawValue].format = .char
        
        var colorData: [CChar] = []
        var count: Int = 0
        switch (meshType) {
        case .cube:
            count = 216
            for  _ in 0..<3 {
                for _ in 0..<36 {
                    colorData.append(1)
                }
                for _ in 0..<36 {
                    colorData.append(0)
                }
            }
            break
        case .sphere:
            count = 8192
            for _ in 0..<8192 {
                colorData.append(0)
            }
            break
        case .plane:
            count = 128
            for _ in 0..<128 {
                colorData.append(1)
            }
            break
        }
        
        mdlMesh.addAttribute(withName: MDLVertexAttributeColor, format: .char)

        mdlMesh.vertexBuffers[1] = metalAllocator.newBuffer(with: Data.init(bytes: colorData, count: count), type: .vertex)
        
        mdlMesh.vertexDescriptor = mdlVertexDescriptor

        return try MTKMesh(mesh: mdlMesh, device: device)
    }

    private func updateDynamicBufferState() {
        /// Update the state of our uniform buffers before rendering

        uniformBufferIndex = (uniformBufferIndex + 1) % maxBuffersInFlight

        uniformBufferOffset = alignedUniformsSize * uniformBufferIndex * (meshes.count + 1)

        uniforms = UnsafeMutableRawPointer(dynamicUniformBuffer.contents() + uniformBufferOffset).bindMemory(to:Uniforms.self, capacity:1)
    }

    private func updateGameState() {
        let rotationChangeX = rotationX - prevRotationX
        let rotationChangeY = rotationY - prevRotationY
        let rotationChangeZ = rotationZ - prevRotationZ
        
        if (abs(rotationChangeX) > 0.005) {
            rotationMatrix = simd_mul(matrix4x4_rotation(radians: rotationChangeX * .pi * 2, axis: SIMD3<Float>(0, 1, 0)), rotationMatrix)
        }
        
        if (abs(rotationChangeY) > 0.005) {
            rotationMatrix = simd_mul(matrix4x4_rotation(radians: rotationChangeY * 2 * .pi, axis: SIMD3<Float>(1, 0, 0)), rotationMatrix)
        }
        
        if (abs(rotationChangeZ) > 0.005) {
            rotationMatrix = simd_mul(matrix4x4_rotation(radians: rotationChangeZ * 2 * .pi, axis: SIMD3<Float>(0, 0, 1)), rotationMatrix)
        }
        
        prevRotationX = rotationX
        prevRotationY = rotationY
        prevRotationZ = rotationZ
        
        /// Update any game state before rendering
        for i in 0..<meshes.count {
            uniforms[i].projectionMatrix = currentMatrix
            uniforms[i].modelViewMatrix = simd_mul(matrix4x4_translation(0.0, 0.0, 8.0), translations[i])
            uniforms[i].modelViewMatrix = simd_mul(rotationMatrix, uniforms[i].modelViewMatrix)
            uniforms[i].modelViewMatrix = simd_mul(matrix4x4_translation(0.0, 0.0, -8.0), uniforms[i].modelViewMatrix)
        }
        uniforms[meshes.count].projectionMatrix = currentMatrix
        uniforms[meshes.count].modelViewMatrix = matrix4x4_translation(0, 0, 0)
    }

    func draw(in view: MTKView) {
        /// Per frame updates hare

        _ = inFlightSemaphore.wait(timeout: DispatchTime.distantFuture)
        
        if let commandBuffer = commandQueue.makeCommandBuffer() {
            let semaphore = inFlightSemaphore
            commandBuffer.addCompletedHandler { (_ commandBuffer)-> Swift.Void in
                semaphore.signal()
            }
            
            self.updateDynamicBufferState()
            
            self.updateGameState()
            
            /// Delay getting the currentRenderPassDescriptor until we absolutely need it to avoid
            ///   holding onto the drawable and blocking the display pipeline any longer than necessary
            let renderPassDescriptor = view.currentRenderPassDescriptor
            
            if let renderPassDescriptor = renderPassDescriptor {
                
                /// Final pass rendering code here
                if let renderEncoder = commandBuffer.makeRenderCommandEncoder(descriptor: renderPassDescriptor) {
                    renderEncoder.label = "Primary Render Encoder"
                    
                    renderEncoder.pushDebugGroup("Draw Box")
                    
                    renderEncoder.setCullMode(.back)
                    
                    renderEncoder.setFrontFacing(.counterClockwise)
                    
                    renderEncoder.setDepthStencilState(depthState)
                                    
                    renderEncoder.setVertexBuffer(dynamicUniformBuffer, offset: uniformBufferOffset, index: BufferIndex.uniforms.rawValue)
                    renderEncoder.setFragmentBuffer(dynamicUniformBuffer, offset: uniformBufferOffset, index: BufferIndex.uniforms.rawValue)
                    
                    renderEncoder.setRenderPipelineState(pipelineState)
                    
                    for i in 0..<meshes.count {
                        renderEncoder.setVertexBufferOffset(
                            uniformBufferOffset + i * MemoryLayout<Uniforms>.size,
                            index: BufferIndex.uniforms.rawValue)
                        renderEncoder.setFragmentBufferOffset(
                            uniformBufferOffset + i * MemoryLayout<Uniforms>.size,
                            index: BufferIndex.uniforms.rawValue)
                        
                        for (index, element) in meshes[i].vertexDescriptor.layouts.enumerated() {
                            guard let layout = element as? MDLVertexBufferLayout else {
                                return
                            }
                            
                            if layout.stride != 0 {
                                let buffer = meshes[i].vertexBuffers[index]
                                renderEncoder.setVertexBuffer(buffer.buffer, offset:buffer.offset, index: index)
                            }
                        }
                        
                        for submesh in meshes[i].submeshes {
                            renderEncoder.drawIndexedPrimitives(
                                type: submesh.primitiveType,
                                indexCount: submesh.indexCount,
                                indexType: submesh.indexType,
                                indexBuffer: submesh.indexBuffer.buffer,
                                indexBufferOffset: submesh.indexBuffer.offset)
                        }
                    }
                    
                    renderEncoder.setRenderPipelineState(simplePipelineState)
                    renderEncoder.setVertexBuffer(surfaceBuffer, offset: 0, index: 0)
                    renderEncoder.setVertexBuffer(colorsBuffer, offset: 0, index: 1)
                    renderEncoder.drawPrimitives(type: .triangle, vertexStart: 0, vertexCount: surfaceCount)
                    
                    renderEncoder.popDebugGroup()
                    
                    renderEncoder.endEncoding()
                    
                    if let drawable = view.currentDrawable {
                        commandBuffer.present(drawable)
                    }
                }
            }
            
            commandBuffer.commit()
        }
    }

    func mtkView(_ view: MTKView, drawableSizeWillChange size: CGSize) {
        /// Respond to drawable size or orientation changes here

        let aspect = Float(size.width) / Float(size.height)
        orthographicMatrix = matrix_perspective_orthographic(l: -8.0, r: 8.0, b: -8.0, t: 8.0, near: 0.1, far: 100.0)
        projectionMatrix = matrix_perspective_right_hand(fovyRadians: radians_from_degrees(65), aspectRatio:aspect, nearZ: 0.1, farZ: 100.0)
        currentMatrix = isOrtho ? orthographicMatrix : projectionMatrix
    }
}

// Generic matrix math utility functions
func matrix4x4_rotation(radians: Float, axis: SIMD3<Float>) -> matrix_float4x4 {
    let unitAxis = normalize(axis)
    let ct = cosf(radians)
    let st = sinf(radians)
    let ci = 1 - ct
    let x = unitAxis.x, y = unitAxis.y, z = unitAxis.z
    return matrix_float4x4.init(columns:(
        vector_float4(
            ct + x * x * ci,     y * x * ci + z * st, z * x * ci - y * st, 0),
        vector_float4(
            x * y * ci - z * st, ct + y * y * ci,     z * y * ci + x * st, 0),
        vector_float4(
            x * z * ci + y * st, y * z * ci - x * st, ct + z * z * ci,     0),
        vector_float4(
            0,                   0,                   0,                   1)))
}

func matrix4x4_translation(_ translationX: Float, _ translationY: Float, _ translationZ: Float) -> matrix_float4x4 {
    return matrix_float4x4.init(columns:(vector_float4(1, 0, 0, 0),
                                         vector_float4(0, 1, 0, 0),
                                         vector_float4(0, 0, 1, 0),
                                         vector_float4(translationX, translationY, translationZ, 1)))
}

func matrix_perspective_right_hand(fovyRadians fovy: Float, aspectRatio: Float, nearZ: Float, farZ: Float) -> matrix_float4x4 {
    let ys = 1 / tanf(fovy * 0.5)
    let xs = ys / aspectRatio
    let zs = farZ / (nearZ - farZ)
    return matrix_float4x4.init(columns:(vector_float4(xs,  0, 0,   0),
                                         vector_float4( 0, ys, 0,   0),
                                         vector_float4( 0,  0, zs, -1),
                                         vector_float4( 0,  0, zs * nearZ, 0)))
}

func matrix_perspective_orthographic(l: Float, r: Float, b: Float, t: Float,
                                     near: Float, far: Float)
-> matrix_float4x4 {
    var result = matrix_float4x4()

    result.columns.0[0] = 2 / (r - l)
    result.columns.1[1] = 2 / (t - b)
    result.columns.2[2] = -1 / (far - near)
    result.columns.3[0] = -(r + l) / (r - l)
    result.columns.3[1] = -(t + b) / (t - b)
    result.columns.3[2] = -near / (far - near)
    result.columns.3[3] = 1.0

    return result
}

func radians_from_degrees(_ degrees: Float) -> Float {
    return (degrees / 180) * .pi
}
