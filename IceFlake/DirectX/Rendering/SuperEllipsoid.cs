using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.D3DCompiler;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Runtime.InteropServices;

namespace Apparat.Renderables
{
    public class SuperEllipsoid : Renderable
    {
        SlimDX.Direct3D11.Buffer vertexBuffer;
        SlimDX.Direct3D11.Buffer indexBuffer;
        DataStream vertices;
        DataStream indices;
        int vertexSizeInBytes;

        InputLayout layout;

        int numVertices = 0;
        int numIndices = 0;

        ShaderSignature inputSignature;
        EffectTechnique technique;
        EffectPass pass;

        Effect effect;
        EffectMatrixVariable tmat;
        EffectVectorVariable mCol;
        EffectVectorVariable wfCol;

        double n1 = 0;
        double n2 = 0;

        float radius = 0.0f;

        public SuperEllipsoid(int numVerticesPerLayer, float radius, float n1, float n2)
        {
            try
            {
                this.n1 = n1;
                this.n2 = n2;
                this.radius = radius;
                using (ShaderBytecode effectByteCode = ShaderBytecode.CompileFromFile(
                    "Shaders/transformEffectWireframe.fx",
                    "Render",
                    "fx_5_0",
                    ShaderFlags.EnableStrictness,
                    EffectFlags.None))
                {
                    effect = new Effect(DeviceManager.Instance.device, effectByteCode);
                    technique = effect.GetTechniqueByIndex(0);
                    pass = technique.GetPassByIndex(0);
                    inputSignature = pass.Description.Signature;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            tmat = effect.GetVariableByName("gWVP").AsMatrix();

            mCol = effect.GetVariableByName("colorSolid").AsVector();
            wfCol = effect.GetVariableByName("colorWireframe").AsVector();

            mCol.Set(new Color4(1, 0, 1, 0));
            wfCol.Set(new Color4(1, 0, 0, 0));

            int numHorizontalVertices = numVerticesPerLayer + 1;
            int numVerticalVertices = numVerticesPerLayer + 1;

            numVertices = numHorizontalVertices * numVerticalVertices;
            vertexSizeInBytes = 12;
            int vertexBufferSizeInBytes = vertexSizeInBytes * numVertices;

            vertices = new DataStream(vertexBufferSizeInBytes, true, true);

            float theta = 0.0f;
            float phi = 0.0f;

            float verticalStep = ((float)Math.PI) / (float)numVerticesPerLayer;
            float horizontalStep = ((float)Math.PI * 2) / (float)numVerticesPerLayer;

            for (int verticalIt = 0; verticalIt < numVerticalVertices; verticalIt++)
            {

                theta = -((float)Math.PI / 2) + verticalStep * verticalIt;

                for (int horizontalIt = 0; horizontalIt < numHorizontalVertices; horizontalIt++)
                {

                    phi = horizontalStep * horizontalIt;

                    double cosTheta = Math.Cos(theta);
                    double sinTheta = Math.Sin(theta);
                    double cosPhi = Math.Cos(phi);
                    double sinPhi = Math.Sin(phi);

                    double powCosTheta = cosTheta == 0.0f ? 0 : Math.Sign(cosTheta) == -1 ? -Math.Pow(-cosTheta, n1) : Math.Pow(cosTheta, n1);
                    double powSinTheta = sinTheta == 0.0f ? 0 : Math.Sign(sinTheta) == -1 ? -Math.Pow(-sinTheta, n1) : Math.Pow(sinTheta, n1);
                    double powCosPhi = cosPhi == 0.0f ? 0 : Math.Sign(cosPhi) == -1 ? -Math.Pow(-cosPhi, n2) : Math.Pow(cosPhi, n2);
                    double powSinPhi = sinPhi == 0.0f ? 0 : Math.Sign(sinPhi) == -1 ? -Math.Pow(-sinPhi, n2) : Math.Pow(sinPhi, n2);


                    double x = radius * powCosTheta * powCosPhi;
                    double y = radius * powCosTheta * powSinPhi;
                    double z = radius * powSinTheta;

                    Vector3 v = new Vector3((float)x, (float)z, (float)y);

                    vertices.Write(v);
                }
            }

            vertices.Position = 0;

            // create the vertex layout and buffer
            var elements = new[] { 
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
            };

            layout = new InputLayout(DeviceManager.Instance.device, inputSignature, elements);
            vertexBuffer = new SlimDX.Direct3D11.Buffer(DeviceManager.Instance.device,
              vertices,
              vertexBufferSizeInBytes,
              ResourceUsage.Default,
              BindFlags.VertexBuffer,
              CpuAccessFlags.None,
              ResourceOptionFlags.None, 0);

            // creating the index buffer
            numIndices = 6 * numVertices;
            indices = new DataStream(2 * numIndices, true, true);

            try
            {
                for (int verticalIt = 0; verticalIt < numVerticesPerLayer; verticalIt++)
                {

                    for (int horizontalIt = 0; horizontalIt < numVerticesPerLayer; horizontalIt++)
                    {
                        short lu = (short)(horizontalIt + verticalIt * (numHorizontalVertices));
                        short ru = (short)((horizontalIt + 1) + verticalIt * (numHorizontalVertices));

                        short ld = (short)(horizontalIt + (verticalIt + 1) * (numHorizontalVertices));
                        short rd = (short)((horizontalIt + 1) + (verticalIt + 1) * (numHorizontalVertices));

                        indices.Write(lu);
                        indices.Write(rd);
                        indices.Write(ld);

                        indices.Write(rd);
                        indices.Write(lu);
                        indices.Write(ru);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            indices.Position = 0;

            indexBuffer = new SlimDX.Direct3D11.Buffer(
                DeviceManager.Instance.device,
                indices,
                2 * numIndices,
                ResourceUsage.Default,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
        }


        public override void render()
        {
            Matrix ViewPerspective = CameraManager.Instance.ViewPerspective;
            tmat.SetMatrix(ViewPerspective);

            // configure the Input Assembler portion of the pipeline with the vertex data
            DeviceManager.Instance.context.InputAssembler.InputLayout = layout;
            DeviceManager.Instance.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            DeviceManager.Instance.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, vertexSizeInBytes, 0));
            DeviceManager.Instance.context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);

            technique = effect.GetTechniqueByName("Render");

            EffectTechniqueDescription techDesc;
            techDesc = technique.Description;

            for (int p = 0; p < techDesc.PassCount; ++p)
            {
                technique.GetPassByIndex(p).Apply(DeviceManager.Instance.context);
                DeviceManager.Instance.context.DrawIndexed(numIndices, 0, 0);
            }
        }

        public override void dispose()
        {
            indexBuffer.Dispose();
            vertexBuffer.Dispose();
            effect.Dispose();
        }
    }
}