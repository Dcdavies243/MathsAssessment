using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Assessment
{
    public struct directionalLightSource
    {
        public Vector3 diffuseColor; // the main color of the light
        public Vector3 specularColor; // highlight color
        public Vector3 direction; // direction the light is shining in
    }
    public class camera3d
    {
        public Vector3 position;
        public Vector3 target;
        public Vector3 whichWayIsUp = Vector3.Up;
        public float fieldOfView = MathHelper.ToRadians(45);
        public float aspectRatio = 4f / 3f;
        public float nearPlane = 1f;
        public float farPlane = 10000f;
        public Matrix ViewMatrix()
        { // Set the position of the camera and tell it what to look at
            return Matrix.CreateLookAt(position, target, whichWayIsUp);
        }
        public Matrix ProjectionMatrix()
        { // Set up a matrix to project the camera image onto the viewport
            return Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio,
            nearPlane, farPlane);
        }
        public BoundingFrustum viewarea()
        {
            return new BoundingFrustum(ViewMatrix() * ProjectionMatrix());
        }
    }
    public class object3d
    {
        public Vector3 rotation;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;
        public bool visible = false;
        public Model mesh;
        public Matrix[] transforms;
        public float Alpha = 1;
        public Vector3 collisionScale = Vector3.One;
        public float scale = 1;
        public bool Lit = true;
        public Vector3 storedPos;
        public BoundingBox hitBox
        {
            get
            {
                BoundingBox b = new BoundingBox();
                ///////////////////////////////////////////////////////////////////
                //
                // CODE FOR TASK 3 SHOULD BE ENTERED HERE
                // ----------------------------------------------------------------

                //Find center first, by starting at the meshs world position
                //then adding na offset if the meshs center also happens to be ofset - scalled by meshs scale;

                b.Min = position + mesh.Meshes[0].BoundingSphere.Center + new Vector3(-5, 0, 0);

                //Then move this center to the top left corner by subtracting half the size of the mesh
                //Calculated by its radius and scaled by visual and collision scales

                b.Min.X -= (mesh.Meshes[0].BoundingSphere.Radius) * collisionScale.X * scale;
                b.Min.Y -= (mesh.Meshes[0].BoundingSphere.Radius) * collisionScale.Y * scale;
                b.Min.Z -= (mesh.Meshes[0].BoundingSphere.Radius) * collisionScale.Z * scale;

                //Find the max (the opposite corner) by adding on the mesh size, scaled
                b.Max.X = b.Min.X + mesh.Meshes[0].BoundingSphere.Radius * 2 * collisionScale.X * scale;
                b.Max.Y = b.Min.Y + mesh.Meshes[0].BoundingSphere.Radius * 2 * collisionScale.Y * scale;
                b.Max.Z = b.Min.Z + mesh.Meshes[0].BoundingSphere.Radius * 2 * collisionScale.Z * scale;

                //
                ///////////////////////////////////////////////////////////////////
                return b;
            }
        }
        internal void LoadModel(ContentManager content, string modelName)
        {
            mesh = content.Load<Model>(modelName);
            transforms = new Matrix[mesh.Bones.Count];
            mesh.CopyAbsoluteBoneTransformsTo(transforms);
            visible = true;
        }
        public void Draw(camera3d cam, directionalLightSource light)
        {
            if (!visible) return; // dont render hidden meshes
            foreach (ModelMesh mesh in mesh.Meshes) // loop through the mesh in the 3d model, drawing each one in turn.
             {
                foreach (BasicEffect effect in mesh.Effects) // This loop then goes through every effect in each mesh.
                {   
                    effect.World = transforms[mesh.ParentBone.Index]; // begin dealing with transforms to render the object into the game world
                                                                      // The following effects allow the object to be drawn in the correct place, with the correct rotation and scale.

                    ///////////////////////////////////////////////////////////////////
                    //
                    // CODE FOR TASK 1 SHOULD BE ENTERED HERE
                    // -----------------------------------------------------------------
                    //
                    // To render we need three things - world matrix, view matrix and projection matrix
                    // But we actually start in model space - this is when our world starts before transforms

                    //--------------------------------------
                    // MESH BASE MATRIX 
                    //--------------------------------------
                    // Our meshes start with world - model space, so we use our transforms array
                    effect.World = transforms[mesh.ParentBone.Index];

                    //--------------------------------------
                    // WORLD MATRIX 
                    //--------------------------------------
                    // Transform from model space to world space in order - scale, rotation, translation.

                    // 1. Scale
                    //Scale our model by multiplying the world matrix by a scale matrix
                    //XNA does this for us using CreateScale()

                    effect.World *= Matrix.CreateScale(scale);

                    // 2. Rotation
                    // Rotate our model in the game world
                    effect.World *= Matrix.CreateRotationX(rotation.X); // Rotate the x axis
                    effect.World *= Matrix.CreateRotationY(rotation.Y); // Rotate the y axis
                    effect.World *= Matrix.CreateRotationZ(rotation.Z); // Rotate the z axis

                    // 3. Translation / Position
                    // Move our model to the correct place in the game world
                    effect.World *= Matrix.CreateTranslation(position);

                    //--------------------------------------
                    // VIEW MATRIX 
                    //--------------------------------------
                    // This puts the model in relation to where our cam is, and direction of our cam.
                    effect.View = Matrix.CreateLookAt(cam.position, cam.target, cam.whichWayIsUp);

                    //--------------------------------------
                    // PROJECTION MATRIX 
                    //--------------------------------------
                    // Projection changes from view space (3D) to screen space (2D)

                    // Can be either orthographic or perspective

                    // Perspective
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(cam.fieldOfView, cam.aspectRatio, cam.nearPlane, cam.farPlane);

                    //Orthographic
                    //effect.Projection = Matrix.CreateOrthographic(1600f, 900f, 1f, 10000f);
                    
                    ///////////////////////////////////////////////////////////////////



                    // the following effects are related to lighting and texture  settings, feel free to tweak them to see what happens.
                    effect.LightingEnabled = true;
                    effect.Alpha = Alpha; //  amount of transparency
                    effect.AmbientLightColor = new Vector3(0.25f); // fills in dark areas with a small amount of light
                    effect.DiffuseColor = new Vector3(0.1f);
                    // Diffuse is the standard colour method
                    effect.DirectionalLight0.Enabled = true; // allows a directional light
                    effect.DirectionalLight0.DiffuseColor = light.diffuseColor; // the directional light's main colour
                    effect.DirectionalLight0.SpecularColor = light.specularColor; // the directional light's colour used for highlights
                    effect.DirectionalLight0.Direction = light.direction; // the direction of the light
                    effect.EmissiveColor = new Vector3(0.15f);
                }
                mesh.Draw(); // draw the current mesh using the effects.
            }
        }
    }

    public static class BoundingRenderer
    {
        static GraphicsDevice gfx;
        static VertexBuffer sphereVertexBuffer;
        static VertexBuffer cubeVertexBuffer;
        static BasicEffect effect;
        static int tessellation = 16;
        public static void InitializeGraphics(GraphicsDevice graphicsDevice)
        {
            gfx = graphicsDevice;
            effect = new BasicEffect(gfx);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;
            VertexPositionColor[] sphereVerts = new VertexPositionColor[tessellation *
            3 + 2];
            int index = 0;
            float step = MathHelper.TwoPi / (float)tessellation;
            for (float a = 0; a <= MathHelper.TwoPi; a += step) //create circle on the XY plane first
                sphereVerts[index++] = new VertexPositionColor(new
                Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f), Color.White);
            for (float a = 0; a <= MathHelper.TwoPi; a += step) //next the XZ  circle
                sphereVerts[index++] = new VertexPositionColor(new
                Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)), Color.White);
            sphereVerts[index++] = sphereVerts[index - tessellation - 1]; // close the circle
        for (float a = 0; a <= MathHelper.TwoPi; a += step) //finally the YZ circle
        sphereVerts[index++] = new VertexPositionColor(new Vector3(0f,
        (float)Math.Cos(a), (float)Math.Sin(a)), Color.White);

            sphereVerts[index++] = sphereVerts[index - tessellation - 1]; // close the circle
            sphereVertexBuffer = new VertexBuffer(graphicsDevice,
            typeof(VertexPositionColor), sphereVerts.Length, BufferUsage.None);
            sphereVertexBuffer.SetData(sphereVerts);
            int[] boxpos = new int[] { 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1,
0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0,
0, 1, 0 };
            VertexPositionColor[] boxVerts = new VertexPositionColor[17];
            for (index = 0; index < boxpos.Length; index += 3)
                boxVerts[index / 3] = new VertexPositionColor(new
                Vector3(boxpos[index], boxpos[index + 1], boxpos[index + 2]), Color.White);
            cubeVertexBuffer = new VertexBuffer(graphicsDevice,
            typeof(VertexPositionColor), boxVerts.Length, BufferUsage.None);
            cubeVertexBuffer.SetData(boxVerts);
        }
        public static void RenderBox(BoundingBox box, Matrix view, Matrix projection,
        Color wireColour)
        {
            gfx.SetVertexBuffer(cubeVertexBuffer);
            effect.World =
            Matrix.CreateScale(box.Max - box.Min) *
            Matrix.CreateTranslation(box.Min);
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = wireColour.ToVector3();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gfx.DrawPrimitives(PrimitiveType.LineStrip, 0, 16);
                // draw first circle
            }
        }
        public static void RenderSphere(BoundingSphere sphere, Matrix view, Matrix
        projection, Color wireColour)
        {
            gfx.SetVertexBuffer(sphereVertexBuffer);
            effect.World = Matrix.CreateScale(sphere.Radius) *
            Matrix.CreateTranslation(sphere.Center);
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = wireColour.ToVector3();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gfx.DrawPrimitives(PrimitiveType.LineStrip, 0, tessellation);
                // draw first circle
                gfx.DrawPrimitives(PrimitiveType.LineStrip, tessellation,
                tessellation); // draw second circle
                gfx.DrawPrimitives(PrimitiveType.LineStrip, (tessellation) * 2 + 1,
                tessellation); // draw third circle
            }
        }
    }
}
