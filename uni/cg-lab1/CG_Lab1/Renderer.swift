//
//  Renderer.swift
//  CG_Lab1
//
//  Created by Mykyta Diachyna on 01.12.2022.
//

// Our platform independent renderer class

import Metal
import MetalKit
import simd

class Renderer: NSObject, MTKViewDelegate {

    public let device: MTLDevice
    let commandQueue: MTLCommandQueue
    var vertexBuffer: MTLBuffer
    var vertexColorBuffer: MTLBuffer
    var translationBuffer: MTLBuffer
    var translationMatrix: matrix_float4x4
    var pipelineState: MTLRenderPipelineState
    public var x: Float
    public var y: Float
    
    let vertexData: [Float] = [
        -0.078125, 0.125, 0.0,
         0.359375, 0.125, 0.0,
         0.359375, -0.3125, 0.0,
         
         -0.296875, 0.34375, 0.0,
         -0.296875, -0.09375, 0.0,
         0.140625, -0.09375, 0.0,
         
         0.359375, 0.125, 0.0,
         0.609375, 0.125, 0.0,
         0.609375, 0.375, 0.0,
         
         -0.296875, 0.46875, 0.0,
         -0.046875, 0.46875, 0.0,
         -0.046875, 0.71875, 0.0,
         
         -0.296875, 0.21875, 0.0,
         -0.546875, 0.21875, 0.0,
         -0.546875, 0.46875, 0.0,
         
         -0.296875, 0.21875, 0.0,
         -0.296875, 0.46875, 0.0,
         -0.546875, 0.46875, 0.0,
         
         0.359375, -0.3125, 0.0,
         0.609375, -0.3125, 0.0,
         0.359375, -0.09375, 0.0,
         
         0.359375, -0.3125, 0.0,
         0.609375, -0.3125, 0.0,
         0.359375, -0.53125, 0.0,
         
         -0.421875, -0.09375, 0.0,
         -0.421875, -0.25, 0.0,
         -0.546875, -0.25, 0.0,
         
         -0.296875, -0.09375, 0.0,
         -0.296875, -0.25, 0.0,
         -0.171875, -0.09375, 0.0,
         
         -0.421875, -0.25, 0.0,
         -0.296875, -0.09375, 0.0,
         -0.421875, -0.09375, 0.0,
         
         -0.421875, -0.25, 0.0,
         -0.296875, -0.09375, 0.0,
         -0.296875, -0.25, 0.0,
         
         // lines
         -0.078125, 0.125, 0.0,
         0.359375, 0.125, 0.0,
         0.359375, 0.125, 0.0,
         0.359375, -0.3125, 0.0,
         0.359375, -0.3125, 0.0,
         -0.078125, 0.125, 0.0,

         -0.296875, 0.34375, 0.0,
         -0.296875, -0.09375, 0.0,
         -0.296875, -0.09375, 0.0,
         0.140625, -0.09375, 0.0,
         0.140625, -0.09375, 0.0,
         -0.296875, 0.34375, 0.0,

         0.359375, 0.125, 0.0,
         0.609375, 0.125, 0.0,
         0.609375, 0.125, 0.0,
         0.609375, 0.375, 0.0,
         0.609375, 0.375, 0.0,
         0.359375, 0.125, 0.0,

         -0.296875, 0.46875, 0.0,
         -0.046875, 0.46875, 0.0,
         -0.046875, 0.46875, 0.0,
         -0.046875, 0.71875, 0.0,
         -0.046875, 0.71875, 0.0,
         -0.296875, 0.46875, 0.0,

         -0.546875, 0.21875, 0.0,
         -0.546875, 0.46875, 0.0,

         -0.296875, 0.46875, 0.0,
         -0.546875, 0.46875, 0.0,

         0.359375, -0.3125, 0.0,
         0.359375, -0.09375, 0.0,
         0.609375, -0.3125, 0.0,
         0.359375, -0.09375, 0.0,

         0.359375, -0.3125, 0.0,
         0.359375, -0.53125, 0.0,
         0.609375, -0.3125, 0.0,
         0.359375, -0.53125, 0.0,

         -0.421875, -0.09375, 0.0,
         -0.546875, -0.25, 0.0,
         -0.421875, -0.25, 0.0,
         -0.546875, -0.25, 0.0,

         -0.296875, -0.09375, 0.0,
         -0.171875, -0.09375, 0.0,
         -0.296875, -0.25, 0.0,
         -0.171875, -0.09375, 0.0,

         -0.296875, -0.09375, 0.0,
         -0.421875, -0.09375, 0.0,

         -0.421875, -0.25, 0.0,
         -0.296875, -0.25, 0.0
    ]
    
    let vertexColorData: [Float] = [
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,
        0.639, 0.792, 0.243, 1.0,

         // lines
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0,
        0.0, 0.0, 0.0, 1.0
    ]

    init?(metalKitView: MTKView) {
        self.device = metalKitView.device!
        
        // load shaders
        let defaultLibrary = self.device.makeDefaultLibrary()
        let fragmentProgram = defaultLibrary!.makeFunction(name: "basic_fragment")
        let vertexProgram = defaultLibrary!.makeFunction(name: "basic_vertex")

        // create a MTLRenderPipelineState
        let pipelineStateDescriptor = MTLRenderPipelineDescriptor()
        pipelineStateDescriptor.vertexFunction = vertexProgram
        pipelineStateDescriptor.fragmentFunction = fragmentProgram
        pipelineStateDescriptor.colorAttachments[0].pixelFormat = .bgra8Unorm
        do {
            try self.pipelineState = self.device.makeRenderPipelineState(
                descriptor: pipelineStateDescriptor)
        } catch let error {
            print("Failed to create pipeline state, error \(error)")
            exit(-1)
        }
        
        // create a MTLCommandQueue
        self.commandQueue = self.device.makeCommandQueue()!
        
        // load vertex buffers
        let dataSize = vertexData.count * MemoryLayout.size(ofValue: vertexData[0])
        let vertexColorSize = vertexColorData.count * MemoryLayout.size(ofValue: vertexColorData[0])
        
        self.vertexBuffer = self.device.makeBuffer(
            bytes: vertexData,
            length: dataSize,
            options: .storageModeShared)!
        self.vertexBuffer.label = "vertices"
        
        self.vertexColorBuffer = self.device.makeBuffer(
            bytes: vertexColorData, length: vertexColorSize, options: [])!
        self.vertexColorBuffer.label = "colors"
        
        self.translationMatrix = matrix4x4_translation(0.0, 0.0, 0.0)
        let matrixSize = MemoryLayout.size(ofValue: translationMatrix)
        self.translationBuffer = self.device.makeBuffer(length: matrixSize, options: [])!
        let matrices = [translationMatrix]
        memcpy(self.translationBuffer.contents(), matrices, matrixSize)
        
        // initialize position to 0
        x = 0.0
        y = 0.0
        
        super.init()

    }

    func draw(in view: MTKView) {
        // update the position of the figure
        self.translationMatrix = matrix4x4_translation(x, y, 0.0)
        let matrixSize = MemoryLayout.size(ofValue: translationMatrix)
        let matrices = [translationMatrix]
        memcpy(self.translationBuffer.contents(), matrices, matrixSize)
        
        // describe a render pass
        let renderPassDescriptor = MTLRenderPassDescriptor()

        guard let drawable = view.currentDrawable else {return}
        renderPassDescriptor.colorAttachments[0].texture = drawable.texture
        renderPassDescriptor.colorAttachments[0].loadAction = .clear
        renderPassDescriptor.colorAttachments[0].clearColor =
            MTLClearColor(red: 216.0/255.0, green: 55.0/255.0,
            blue: 50.0/255.0, alpha: 1.0)
        
        // create and populate a command buffer
        let commandBuffer = commandQueue.makeCommandBuffer()!
        let renderEncoder = commandBuffer.makeRenderCommandEncoder(descriptor: renderPassDescriptor)!
        renderEncoder.setRenderPipelineState(pipelineState)
        renderEncoder.setVertexBuffer(vertexBuffer, offset: 0, index: 0)
        renderEncoder.setVertexBuffer(vertexColorBuffer, offset: 0, index: 1)
        renderEncoder.setVertexBuffer(translationBuffer, offset: 0, index: 2)
        
        let trianglesSize = 36
        let linesSize = 48
        
        renderEncoder.drawPrimitives(type: .triangle, vertexStart: 0, vertexCount: trianglesSize)
        renderEncoder.drawPrimitives(type: .line, vertexStart: trianglesSize, vertexCount: linesSize)
 
        renderEncoder.endEncoding()
        commandBuffer.present(drawable)
        
        // commit the command buffer
        commandBuffer.commit()
    }
        
    func mtkView(_ view: MTKView, drawableSizeWillChange size: CGSize) {
        
    }
        
}

func matrix4x4_translation(_ translationX: Float, _ translationY: Float, _ translationZ: Float) -> matrix_float4x4 {
    return matrix_float4x4.init(columns:(vector_float4(1, 0, 0, 0),
                                         vector_float4(0, 1, 0, 0),
                                         vector_float4(0, 0, 1, 0),
                                         vector_float4(translationX, translationY, translationZ, 1)))
}
