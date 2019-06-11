using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Assessment
{
    class basicShape
    {
        protected GraphicsDevice device;
        protected BasicEffect basicEffect;
        protected Texture2D faceTexture;
        protected VertexBuffer vertexBuffer;
        public Vector3 scale = Vector3.One;
        protected int numberOfPrimitives;
        protected PrimitiveType primitiveType;
        protected basicShape() { } // exists purely to simplify the creation of subclasses
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public basicShape(GraphicsDevice graphicsDevice)
        {
            device = graphicsDevice;
            basicEffect = new BasicEffect(device);
            SetUpVertices();
        }
        public void LoadContent(ContentManager content, string assetname)
        {
            faceTexture = content.Load<Texture2D>(assetname);
            basicEffect.Texture = faceTexture;
            basicEffect.TextureEnabled = true;
        }
        public virtual void SetUpVertices()
        {
            numberOfPrimitives = 0;
        }
        public virtual void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            basicEffect.World = Matrix.Identity;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(vertexBuffer);
                device.DrawPrimitives(primitiveType, 0, numberOfPrimitives);
            }
        }

        public virtual void Draw(Matrix viewMatrix, Matrix projectionMatrix, Effect effect)
        {
            effect.CurrentTechnique = effect.Techniques["BasicEffect_Texture"];
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["WorldInverseTranspose"].SetValue(viewMatrix);
            effect.Parameters["WorldViewProj"].SetValue(projectionMatrix);
            effect.Parameters["Texture"].SetValue(faceTexture);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                device.SetVertexBuffer(vertexBuffer);
                device.DrawPrimitives(primitiveType, 0, numberOfPrimitives);
            }
        }
    }
    class basicCuboid : basicShape
    {
        public basicCuboid(GraphicsDevice graphics) : base(graphics) { }
        public BoundingBox collisionbox;
        public void SetUpVertices(Vector3 position)
        {
            List<VertexPositionNormalTexture> verticesList = new
            List<VertexPositionNormalTexture>();
            //front
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new
            Vector3(1, 0, 0), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 0), new
            Vector3(0, 0, 1), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 0), new
            Vector3(0, 1, 0), new Vector2(1, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 0), new
            Vector3(1, 0, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 1, 0), new
            Vector3(0, 0, 1), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 0), new
            Vector3(0, 1, 0), new Vector2(1, 0)));
            //back
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 1), new
            Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 1), new
            Vector3(0, 0, 1), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 1), new
            Vector3(1, 0, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 1), new
            Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 1, 1), new
            Vector3(0, 0, 1), new Vector2(1, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 1), new
            Vector3(1, 0, 0), new Vector2(1, 1)));
            //left
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 0), new
            Vector3(0, 1, 0), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 1), new
            Vector3(0, 0, 1), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new
            Vector3(1, 0, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 0), new
            Vector3(0, 1, 0), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 1), new
            Vector3(0, 0, 1), new Vector2(1, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 1), new
            Vector3(1, 0, 0), new Vector2(0, 0)));
            //right
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 1, 0), new
            Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 0), new
            Vector3(1, 0, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 1), new
            Vector3(0, 0, 1), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 1, 0), new
            Vector3(0, 1, 0), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 1), new
            Vector3(1, 0, 0), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 1, 1), new
            Vector3(0, 0, 1), new Vector2(1, 0)));
            //top
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 0), new
            Vector3(0, 1, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 1, 0), new
            Vector3(1, 0, 0), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 1), new
            Vector3(0, 0, 1), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 1, 0), new
            Vector3(0, 1, 0), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 1, 1), new
            Vector3(1, 0, 0), new Vector2(1, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 1, 1), new
            Vector3(0, 0, 1), new Vector2(0, 0)));
            //bottom
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new
            Vector3(0, 1, 0), new Vector2(0, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 1), new
            Vector3(0, 0, 1), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 0), new
            Vector3(1, 0, 0), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 0), new
            Vector3(0, 1, 0), new Vector2(1, 1)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 1), new
            Vector3(0, 0, 1), new Vector2(0, 0)));
            verticesList.Add(new VertexPositionNormalTexture(new Vector3(1, 0, 1), new
            Vector3(1, 0, 0), new Vector2(1, 0)));
            for (int vert = 0; vert < verticesList.Count; vert++)
            {
                VertexPositionNormalTexture temp = verticesList[vert];
                temp.Position *= scale;
                temp.Position += position;
                verticesList[vert] = temp;
            }
            collisionbox = new BoundingBox(position, (position + scale));
            vertexBuffer = new VertexBuffer(device,
            typeof(VertexPositionNormalTexture), verticesList.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
            numberOfPrimitives = verticesList.Count();
            primitiveType = PrimitiveType.TriangleList;
        }
    }
}