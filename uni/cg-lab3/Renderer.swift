//
//  Renderer.swift
//  CG_Lab3
//
//  Created by Mykyta Diachyna on 14.12.2022.
//

// Our platform independent renderer class

import Metal
import MetalKit
import simd

// The 256 byte aligned size of our uniform structure
let alignedUniformsSize = (MemoryLayout<Uniforms>.size + 0xFF) & -0x100

let maxBuffersInFlight = 3

enum RendererError: Error {
    case badVertexDescriptor
}

enum PipelineStateType {
    case normal
    case skybox
    case sphere
}

class Renderer: NSObject, MTKViewDelegate {

    public let device: MTLDevice
    let commandQueue: MTLCommandQueue
    var dynamicUniformBuffer: MTLBuffer
    var uniformTruncatedBuffer: MTLBuffer
    var pipelineState: MTLRenderPipelineState
    var skyboxPipelineState: MTLRenderPipelineState
    var depthState: MTLDepthStencilState
    
    var skybox: MTLTexture
    
    var textures: [MTLTexture] = []
    var axisRotationSpeed: [Float] = []
    var orbitRotationSpeed: [Float] = []
    var currentAxisRotation: [Float] = []
    var currentOrbitRotation: [Float] = []
    var axes: [SIMD3<Float>] = []
    
    var prevRotationX: Float = 0.0
    var prevRotationY: Float = 0.0
    var prevRotationZ: Float = 0.0
    
    var rotationX: Float = 0.0
    var rotationY: Float = 0.0
    var rotationZ: Float = 0.0
    
    var rotationMatrix: matrix_float4x4 = matrix_float4x4()
    
    var translations: [matrix_float4x4] = []
    
    let inFlightSemaphore = DispatchSemaphore(value: maxBuffersInFlight)

    var uniformBufferOffset = 0

    var uniformBufferIndex = 0

    var uniforms: UnsafeMutablePointer<Uniforms>
    var uniformTruncated: UnsafeMutablePointer<UniformTruncated>

    var projectionMatrix: matrix_float4x4 = matrix_float4x4()
    
    var meshes: [MTKMesh] = []
    var skyboxMesh: MTKMesh

    init?(metalKitView: MTKView) {
        self.device = metalKitView.device!
        self.commandQueue = self.device.makeCommandQueue()!
        
        rotationMatrix = matrix4x4_translation(0, 0, 0)

        metalKitView.depthStencilPixelFormat = MTLPixelFormat.depth32Float_stencil8
        metalKitView.colorPixelFormat = MTLPixelFormat.bgra8Unorm_srgb
        metalKitView.sampleCount = 1

        let mtlVertexDescriptor = Renderer.buildMetalVertexDescriptor()

        do {
            pipelineState = try Renderer.buildRenderPipelineWithDevice(device: device,
                                                       metalKitView: metalKitView,
                                                       mtlVertexDescriptor: mtlVertexDescriptor,
                                                       type: .normal)
            skyboxPipelineState = try Renderer.buildRenderPipelineWithDevice(device: device, metalKitView: metalKitView, mtlVertexDescriptor: mtlVertexDescriptor, type: .skybox)
        } catch {
            print("Unable to compile render pipeline state.  Error info: \(error)")
            return nil
        }

        let depthStateDescriptor = MTLDepthStencilDescriptor()
        depthStateDescriptor.depthCompareFunction = MTLCompareFunction.less
        depthStateDescriptor.isDepthWriteEnabled = true
        self.depthState = device.makeDepthStencilState(descriptor:depthStateDescriptor)!
        
        let desc = MTKModelIOVertexDescriptorFromMetal(Renderer.buildMetalVertexDescriptor())
        var attribute = desc.attributes[0] as! MDLVertexAttribute
        attribute.name = MDLVertexAttributePosition
        attribute = desc.attributes[1] as! MDLVertexAttribute
        attribute.name = MDLVertexAttributeTextureCoordinate
        let mtkBufferAllocator = MTKMeshBufferAllocator(device: device)
        
        var ofs: Float = -5.25
        for name in ["bus", "chair", "chinese_coin", "doom", "flower", "measuring_tape", "shoe", "vase"] {
//        for name in ["chair", "chinese_coin", "doom", "flower"] {
            let assetUrl = URL.init(filePath: Bundle.main.bundlePath + "/Contents/Resources/Models/" + name + "/" + name + ".obj")

            let asset = MDLAsset(url: assetUrl, vertexDescriptor: desc, bufferAllocator: mtkBufferAllocator)
            
            let localMeshes: [MTKMesh]
            do {
                localMeshes = try MTKMesh.newMeshes(asset: asset, device: device).metalKitMeshes
            } catch let error {
                fatalError("\(error)")
            }
                        
            for mesh in localMeshes {
                var normals: [Float] = []
                let vertexBufferLength = mesh.vertexBuffers[BufferIndex.meshNormals.rawValue].length
                let vertexPointer = mesh.vertexBuffers[BufferIndex.meshPositions.rawValue].buffer.contents()
                
                var map = Dictionary<SIMD3<Float>, [SIMD3<Float>]>()
                
                for submesh in mesh.submeshes {
                    // TODO: go through the faces and add to normals of vertices, then normalize
                    let indexPointer = submesh.indexBuffer.buffer.contents()
                    let vertexCount = submesh.indexCount / 3
                    for i in 0..<vertexCount {
                        let i0 = indexPointer.load(fromByteOffset: i * 12, as: UInt32.self)
                        let i1 = indexPointer.load(fromByteOffset: i * 12 + 4, as: UInt32.self)
                        let i2 = indexPointer.load(fromByteOffset: i * 12 + 8, as: UInt32.self)
                        
                        let v0x = vertexPointer.load(fromByteOffset: Int(i0) * 12, as: Float.self)
                        let v0y = vertexPointer.load(fromByteOffset: Int(i0) * 12 + 4, as: Float.self)
                        let v0z = vertexPointer.load(fromByteOffset: Int(i0) * 12 + 8, as: Float.self)
                        let v0 = SIMD3<Float>(v0x, v0y, v0z)
                        
                        let v1x = vertexPointer.load(fromByteOffset: Int(i1) * 12, as: Float.self)
                        let v1y = vertexPointer.load(fromByteOffset: Int(i1) * 12 + 4, as: Float.self)
                        let v1z = vertexPointer.load(fromByteOffset: Int(i1) * 12 + 8, as: Float.self)
                        let v1 = SIMD3<Float>(v1x, v1y, v1z)
                        
                        let v2x = vertexPointer.load(fromByteOffset: Int(i2) * 12, as: Float.self)
                        let v2y = vertexPointer.load(fromByteOffset: Int(i2) * 12 + 4, as: Float.self)
                        let v2z = vertexPointer.load(fromByteOffset: Int(i2) * 12 + 8, as: Float.self)
                        let v2 = SIMD3<Float>(v2x, v2y, v2z)
                        
                        // TODO: if you have a lighting bug, it's here
                        let vec0 = v1 - v0
                        let vec1 = v2 - v0
                        let normal = simd_normalize(simd_cross(vec0, vec1))
                        
                        if (map[v0] == nil) {
                            map[v0] = []
                        }
                        map[v0]?.append(normal)
                        
                        if (map[v1] == nil) {
                            map[v1] = []
                        }
                        map[v1]?.append(normal)
                        
                        if (map[v2] == nil) {
                            map[v2] = []
                        }
                        map[v2]?.append(normal)
                    }
                }
                
                let vertexCount = vertexBufferLength / 12
                for i in 0..<vertexCount {
                    let vx = vertexPointer.load(fromByteOffset: i * 12, as: Float.self)
                    let vy = vertexPointer.load(fromByteOffset: i * 12 + 4, as: Float.self)
                    let vz = vertexPointer.load(fromByteOffset: i * 12 + 8, as: Float.self)
                    let v = SIMD3<Float>(vx, vy, vz)
                    
                    var normal = SIMD3<Float>(0, 0, 0)
                    for vec in map[v]! {
                        normal += vec
                    }
                    normal = simd_normalize(normal)
                    
                    normals.append(normal.x)
                    normals.append(normal.y)
                    normals.append(normal.z)
                }
                
                let normalsPointer = mesh.vertexBuffers[BufferIndex.meshNormals.rawValue].buffer.contents()
                normalsPointer.copyMemory(from: normals, byteCount: vertexBufferLength)
            }
            
            translations.append(matrix4x4_translation(ofs, 0.0, -6.0))
            axisRotationSpeed.append(Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 0.05))))
            orbitRotationSpeed.append(Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 0.05))))
//            axisRotationSpeed.append(0.0)
//            orbitRotationSpeed.append(0.0)
            axes.append(SIMD3<Float>(Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 1.0))), Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 1.0))), Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 1.0)))))
            currentAxisRotation.append(0.0)
            currentOrbitRotation.append(0.0)
            ofs += 1.5
            
            meshes.append(contentsOf: localMeshes)
            
            let textureUrl = URL.init(filePath: Bundle.main.bundlePath + "/Contents/Resources/Models/" + name + "/" + name + ".jpg")
            let texture: MTLTexture
            do {
                texture = try Renderer.loadTexture(device: device, url: textureUrl)
            } catch {
                print("Unable to load texture at url: " + textureUrl.absoluteString)
                return nil
            }

            textures.append(texture)
        }
        
        translations.append(matrix4x4_translation(ofs, 0.0, -6.0))
        axisRotationSpeed.append(Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 0.05))))
        orbitRotationSpeed.append(Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 0.05))))
//            axisRotationSpeed.append(0.0)
//            orbitRotationSpeed.append(0.0)
        axes.append(SIMD3<Float>(Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 1.0))), Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 1.0))), Float.random(in: Range<Float>.init(uncheckedBounds: (0.0, 1.0)))))
        currentAxisRotation.append(0.0)
        currentOrbitRotation.append(0.0)
        
        let skyboxUrl = URL.init(filePath: Bundle.main.bundlePath + "/Contents/Resources/Models/skybox.png")
        do {
            skybox = try Renderer.loadTexture(device: device, url: skyboxUrl)
        } catch {
            print("Unable to load skybox texture at url: " + skyboxUrl.absoluteString)
            return nil
        }
        
        let mdlVertexDescriptor = MTKModelIOVertexDescriptorFromMetal(mtlVertexDescriptor)

        guard let attributes = mdlVertexDescriptor.attributes as? [MDLVertexAttribute] else {
            return nil
        }
        attributes[VertexAttribute.position.rawValue].name = MDLVertexAttributePosition
        attributes[VertexAttribute.position.rawValue].format = .float3
        attributes[VertexAttribute.normal.rawValue].name = MDLVertexAttributeNormal
        attributes[VertexAttribute.normal.rawValue].format = .float3
        attributes[VertexAttribute.texcoord.rawValue].name = MDLVertexAttributeTextureCoordinate
        attributes[VertexAttribute.texcoord.rawValue].format = .float2
        
        let skyboxMdlMesh = MDLMesh.newBox(withDimensions: SIMD3<Float>(80.0, 80.0, 80.0), segments: SIMD3<UInt32>(1, 1, 1), geometryType: .triangles, inwardNormals: true, allocator: mtkBufferAllocator)
        var texCoords: [Float] = [
            0.75, 0.33333,
            0.5, 0.33333,
            0.75, 0.66667,
            0.5, 0.66667,
            0.25, 0.33333,
            0.0, 0.33333,
            0.25, 0.66667,
            0.0, 0.66667,
            0.25, 0.33333,
            0.5, 0.33333,
            0.25, 0.0,
            0.5, 0.0,
            0.25, 1.0,
            0.5, 1.0,
            0.25, 0.66667,
            0.5, 0.66667,
            1.0, 0.33333,
            0.75, 0.33333,
            1.0, 0.66667,
            0.75, 0.66667,
            0.5, 0.33333,
            0.25, 0.33333,
            0.5, 0.66667,
            0.25, 0.66667
        ]
        for _ in 0..<42 {
            texCoords.append(0.0)
        }
        skyboxMdlMesh.vertexDescriptor = mdlVertexDescriptor
        print(skyboxMdlMesh.vertexBuffers.count)
        skyboxMdlMesh.vertexBuffers[BufferIndex.meshGenerics.rawValue].fill(Data.init(bytes: texCoords, count: texCoords.count * MemoryLayout.size(ofValue: texCoords[0])), offset: 0)
        
        do {
            skyboxMesh = try MTKMesh(mesh: skyboxMdlMesh, device: device)
        } catch {
            print("Error creating MTKMesh for skybox")
            return nil
        }
        
        let uniformBufferSize = alignedUniformsSize * maxBuffersInFlight * meshes.count

        self.dynamicUniformBuffer = self.device.makeBuffer(length:uniformBufferSize,
                                                           options:[MTLResourceOptions.storageModeShared])!

        self.dynamicUniformBuffer.label = "UniformBuffer"

        uniforms = UnsafeMutableRawPointer(dynamicUniformBuffer.contents()).bindMemory(to:Uniforms.self, capacity:1)
        
        self.uniformTruncatedBuffer = self.device.makeBuffer(
            length: MemoryLayout<UniformTruncated>.size,
            options:[MTLResourceOptions.storageModeShared])!

        self.uniformTruncatedBuffer.label = "UniformTruncatedBuffer"

        uniformTruncated = UnsafeMutableRawPointer(uniformTruncatedBuffer.contents()).bindMemory(to: UniformTruncated.self, capacity: 1)

        super.init()

    }

    class func buildMetalVertexDescriptor() -> MTLVertexDescriptor {
        // Create a Metal vertex descriptor specifying how vertices will by laid out for input into our render
        //   pipeline and how we'll layout our Model IO vertices

        let mtlVertexDescriptor = MTLVertexDescriptor()

        mtlVertexDescriptor.attributes[VertexAttribute.position.rawValue].format = MTLVertexFormat.float3
        mtlVertexDescriptor.attributes[VertexAttribute.position.rawValue].offset = 0
        mtlVertexDescriptor.attributes[VertexAttribute.position.rawValue].bufferIndex = BufferIndex.meshPositions.rawValue

        mtlVertexDescriptor.attributes[VertexAttribute.texcoord.rawValue].format = MTLVertexFormat.float2
        mtlVertexDescriptor.attributes[VertexAttribute.texcoord.rawValue].offset = 0
        mtlVertexDescriptor.attributes[VertexAttribute.texcoord.rawValue].bufferIndex = BufferIndex.meshGenerics.rawValue
        
        mtlVertexDescriptor.attributes[VertexAttribute.normal.rawValue].format = MTLVertexFormat.float3
        mtlVertexDescriptor.attributes[VertexAttribute.normal.rawValue].offset = 0
        mtlVertexDescriptor.attributes[VertexAttribute.normal.rawValue].bufferIndex = BufferIndex.meshNormals.rawValue

        mtlVertexDescriptor.layouts[BufferIndex.meshPositions.rawValue].stride = 12
        mtlVertexDescriptor.layouts[BufferIndex.meshPositions.rawValue].stepRate = 1
        mtlVertexDescriptor.layouts[BufferIndex.meshPositions.rawValue].stepFunction = MTLVertexStepFunction.perVertex

        mtlVertexDescriptor.layouts[BufferIndex.meshGenerics.rawValue].stride = 8
        mtlVertexDescriptor.layouts[BufferIndex.meshGenerics.rawValue].stepRate = 1
        mtlVertexDescriptor.layouts[BufferIndex.meshGenerics.rawValue].stepFunction = MTLVertexStepFunction.perVertex
        
        mtlVertexDescriptor.layouts[BufferIndex.meshNormals.rawValue].stride = 12
        mtlVertexDescriptor.layouts[BufferIndex.meshNormals.rawValue].stepRate = 1
        mtlVertexDescriptor.layouts[BufferIndex.meshNormals.rawValue].stepFunction = MTLVertexStepFunction.perVertex

        return mtlVertexDescriptor
    }

    class func buildRenderPipelineWithDevice(device: MTLDevice,
                                             metalKitView: MTKView,
                                             mtlVertexDescriptor: MTLVertexDescriptor,
                                             type: PipelineStateType) throws -> MTLRenderPipelineState {
        /// Build a render state pipeline object

        let library = device.makeDefaultLibrary()
        
        let fragment: String
        let vertex: String
        switch (type) {
        case .normal:
            vertex = "vertexShader"
            fragment = "fragmentShader"
            break
        case .skybox:
            vertex = "skyboxVertexShader"
            fragment = "skyboxFragmentShader"
            break
        case .sphere:
            vertex = "sphereVertexShader"
            fragment = "sphereFragmentShader"
            break
        }

        let vertexFunction = library?.makeFunction(name: vertex)
        let fragmentFunction = library?.makeFunction(name: fragment)

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

    class func loadTexture(device: MTLDevice,
                           url: URL) throws -> MTLTexture {
        /// Load texture data with optimal parameters for sampling

        let textureLoader = MTKTextureLoader(device: device)

//        let textureLoaderOptions = [
//            MTKTextureLoader.Option.textureUsage: NSNumber(value: MTLTextureUsage.shaderRead.rawValue),
//            MTKTextureLoader.Option.textureStorageMode: NSNumber(value: MTLStorageMode.`private`.rawValue)
//        ]

//        return try textureLoader.newTexture(URL: url, options: textureLoaderOptions)
        return try textureLoader.newTexture(URL: url)

    }

    private func updateDynamicBufferState() {
        /// Update the state of our uniform buffers before rendering

        uniformBufferIndex = (uniformBufferIndex + 1) % maxBuffersInFlight

        uniformBufferOffset = alignedUniformsSize * uniformBufferIndex * meshes.count

        uniforms = UnsafeMutableRawPointer(dynamicUniformBuffer.contents() + uniformBufferOffset).bindMemory(to:Uniforms.self, capacity:1)
    }

    private func updateGameState() {
        /// Update any game state before rendering
        ///
        /// let rotationChangeX = rotationX - prevRotationX
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
        
        uniformTruncated[0].projectionMatrix = projectionMatrix
        uniformTruncated[0].scalingMatrix = rotationMatrix
        
        for i in 0..<meshes.count {
            uniforms[i].projectionMatrix = projectionMatrix
            
            uniforms[i].modelViewMatrix = simd_mul(translations[i], matrix4x4_rotation(radians: currentAxisRotation[i], axis: axes[i]))
            uniforms[i].modelViewMatrix = simd_mul(matrix4x4_translation(0.0, 0.0, 6.0), uniforms[i].modelViewMatrix)
            uniforms[i].modelViewMatrix = simd_mul(matrix4x4_rotation(radians: currentOrbitRotation[i], axis: SIMD3<Float>(0.0, 1.0, 0.0)), uniforms[i].modelViewMatrix)
            uniforms[i].modelViewMatrix = simd_mul(rotationMatrix, uniforms[i].modelViewMatrix)
            uniforms[i].modelViewMatrix = simd_mul(matrix4x4_translation(0.0, 0.0, -6.0), uniforms[i].modelViewMatrix)
            
            uniforms[i].reflectivity = SIMD4<Float>(0.9, 0.9, 0.9, 1.0)
            uniforms[i].lightPosition0 = SIMD4<Float>(0.0, 0.0, -6.0, 1.0)
            uniforms[i].lightIntensity0 = SIMD4<Float>(0.3, 0.3, 0.3, 1.0)
            uniforms[i].lightPosition1 = SIMD4<Float>(-5.0, 6.0, -8.0, 1.0)
            uniforms[i].lightIntensity1 = SIMD4<Float>(0.6, 0.6, 0.6, 1.0)
            
//            uniforms[i].modelViewMatrix = simd_mul(translations[i], simd_mul(matrix4x4_rotation(radians: rotation, axis: SIMD3<Float>(0.0, 1.0, 0.0)), matrix4x4_scaling(scaleX: scale, scaleY: scale, scaleZ: scale)))
        }
//        uniforms[0].projectionMatrix = projectionMatrix
//
//        let rotationAxis = SIMD3<Float>(1, 1, 0)
//        let modelMatrix = matrix4x4_rotation(radians: rotation, axis: rotationAxis)
//        let viewMatrix = matrix4x4_translation(0.0, 0.0, -8.0)
//        uniforms[0].modelViewMatrix = simd_mul(viewMatrix, modelMatrix)
//        rotation += 0.01
        
        for i in 0..<meshes.count {
            currentOrbitRotation[i] += orbitRotationSpeed[i]
            currentAxisRotation[i] += axisRotationSpeed[i]
        }
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
                    
                    renderEncoder.setRenderPipelineState(pipelineState)
                    
                    renderEncoder.setDepthStencilState(depthState)
                    
                    renderEncoder.setRenderPipelineState(skyboxPipelineState)
                    
                    renderEncoder.setVertexBuffer(uniformTruncatedBuffer, offset: 0, index: BufferIndex.uniforms.rawValue)
                    
                    for (index, element) in skyboxMesh.vertexDescriptor.layouts.enumerated() {
                        guard let layout = element as? MDLVertexBufferLayout else {
                            return
                        }
                        
                        if layout.stride != 0 {
                            let buffer = skyboxMesh.vertexBuffers[index]
                            renderEncoder.setVertexBuffer(buffer.buffer, offset: buffer.offset, index: index)
                        }
                    }
                    
                    renderEncoder.setFragmentTexture(skybox, index: TextureIndex.color.rawValue)
                    
                    for submesh in skyboxMesh.submeshes {
                        renderEncoder.drawIndexedPrimitives(
                            type: submesh.primitiveType,
                            indexCount: submesh.indexCount,
                            indexType: submesh.indexType,
                            indexBuffer: submesh.indexBuffer.buffer,
                            indexBufferOffset: submesh.indexBuffer.offset)
                    }
                    
                    renderEncoder.setRenderPipelineState(pipelineState)
                    
                    renderEncoder.setVertexBuffer(dynamicUniformBuffer, offset: uniformBufferOffset, index: BufferIndex.uniforms.rawValue)
                    renderEncoder.setFragmentBuffer(dynamicUniformBuffer, offset: uniformBufferOffset, index: BufferIndex.uniforms.rawValue)
                                        
                    for i in 0..<meshes.count {
                        let mesh = meshes[i]
                        
                        renderEncoder.setVertexBufferOffset(uniformBufferOffset + i * MemoryLayout<Uniforms>.size, index: BufferIndex.uniforms.rawValue)
                        
                        for (index, element) in mesh.vertexDescriptor.layouts.enumerated() {
                            guard let layout = element as? MDLVertexBufferLayout else {
                                return
                            }
                            
                            if layout.stride != 0 {
                                let buffer = mesh.vertexBuffers[index]
                                renderEncoder.setVertexBuffer(buffer.buffer, offset:buffer.offset, index: index)
                            }
                        }
                        
                        renderEncoder.setFragmentTexture(textures[i], index: TextureIndex.color.rawValue)
                        
                        for submesh in mesh.submeshes {
                            renderEncoder.drawIndexedPrimitives(
                                type: submesh.primitiveType,
                                indexCount: submesh.indexCount,
                                indexType: submesh.indexType,
                                indexBuffer: submesh.indexBuffer.buffer,
                                indexBufferOffset: submesh.indexBuffer.offset)
                            
                        }
                    }
                    
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
        projectionMatrix = matrix_perspective_right_hand(fovyRadians: radians_from_degrees(65), aspectRatio:aspect, nearZ: 0.1, farZ: 100.0)
                
//        projectionMatrix = matrix_perspective_orthographic(l: -4.0, r: 4.0, b: -4.0, t: 4.0, near: 0.1, far: 100.0)
//        projectionMatrix = matrix_perspective_right_hand(fovyRadians: radians_from_degrees(160), aspectRatio:aspect, nearZ: 0.1, farZ: 100.0)
    }
}

// Generic matrix math utility functions
func matrix4x4_rotation(radians: Float, axis: SIMD3<Float>) -> matrix_float4x4 {
    let unitAxis = normalize(axis)
    let ct = cosf(radians)
    let st = sinf(radians)
    let ci = 1 - ct
    let x = unitAxis.x, y = unitAxis.y, z = unitAxis.z
    return matrix_float4x4.init(columns:(vector_float4(    ct + x * x * ci, y * x * ci + z * st, z * x * ci - y * st, 0),
                                         vector_float4(x * y * ci - z * st,     ct + y * y * ci, z * y * ci + x * st, 0),
                                         vector_float4(x * z * ci + y * st, y * z * ci - x * st,     ct + z * z * ci, 0),
                                         vector_float4(                  0,                   0,                   0, 1)))
}

func matrix4x4_scaling(scaleX: Float, scaleY: Float, scaleZ: Float) -> matrix_float4x4 {
    return matrix_float4x4.init(columns:(
        vector_float4(scaleX, 0, 0, 0),
        vector_float4(0, scaleY, 0, 0),
        vector_float4(0, 0, scaleZ, 0),
        vector_float4(0, 0, 0, 1)))
}

func matrix4x4_translation(_ translationX: Float, _ translationY: Float, _ translationZ: Float) -> matrix_float4x4 {
    return matrix_float4x4.init(columns:(vector_float4(1, 0, 0, 0),
                                         vector_float4(0, 1, 0, 0),
                                         vector_float4(0, 0, 1, 0),
                                         vector_float4(translationX, translationY, translationZ, 1)))
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

func matrix_perspective_right_hand(fovyRadians fovy: Float, aspectRatio: Float, nearZ: Float, farZ: Float) -> matrix_float4x4 {
    let ys = 1 / tanf(fovy * 0.5)
    let xs = ys / aspectRatio
    let zs = farZ / (nearZ - farZ)
    return matrix_float4x4.init(columns:(vector_float4(xs,  0, 0,   0),
                                         vector_float4( 0, ys, 0,   0),
                                         vector_float4( 0,  0, zs, -1),
                                         vector_float4( 0,  0, zs * nearZ, 0)))
}

func radians_from_degrees(_ degrees: Float) -> Float {
    return (degrees / 180) * .pi
}
