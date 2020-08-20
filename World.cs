// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------


using System;
using System.Collections;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Core;
using System.Diagnostics;
using AssimpSample;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph;

namespace Transformations
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        private AssimpScene prva_formula;
        private AssimpScene druga_formula;

        private int m_width=0;
        private int m_height=0;
        //private OpenGL openGL;
        float db_translate = 0.0f;
        float lb_rotate = 0.0f;

        float prva_komponenta = 1.0f;
        float druga_komponenta = 1.0f;
        float treca_komponenta = 1.0f;

        private float m_sceneDistance = 500.0f;
        private float m_xRotation = 0.0f;
        private float m_yRotation = 0.0f;
        private enum TextureObjects { Put = 0, Metal, Sljunak };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        private uint[] m_textures = null;
        private string[] m_textureFiles = { "..//..//images//put.jpg", "..//..//images//metal2.jpg", "..//..//images//sljunak.jpg" };

        private DispatcherTimer timer1;

        #endregion Atributi

        public float RotationX
        {
            get { return m_xRotation; }
            set
            {
                if (value < 0 || value > 90)
                { return; }

                m_xRotation = value;
            }
        }

        public float PrvaK
        {
            get { return prva_komponenta; }
            set { prva_komponenta = value; }
        }

        public float DrugaK
        {
            get { return druga_komponenta; }
            set { druga_komponenta = value; }
        }

        public float TrecaK
        {
            get { return treca_komponenta; }
            set { treca_komponenta = value; }
        }

        private int anim = 0;
        public int Anim
        {
            get { return anim; }
            set { anim = value; }
        }

        public float lbRotate
        {
            get { return lb_rotate; }
            set { lb_rotate = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        private LookAtCamera lookAtCam;
        private float walkSpeed = 0.1f;
        float mouseSpeed = 0.005f;
        double horizontalAngle = 0f;
        double verticalAngle = 0.0f;

        private Vertex direction;
        private Vertex right;
        private Vertex up;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public float dbTranslate
        {
            get { return db_translate; }
            set { db_translate = value; }
        }

        public float dbScale { get; set; }

        private float pomerajLeve = 0.0f;
        public float PomerajLeve
        {
            get { return pomerajLeve; }
            set { pomerajLeve = value; }
        }

        private float pomerajDesne = 0.0f;

        public float PomerajDesne
        {
            get { return pomerajDesne; }
            set { pomerajDesne = value; }
        }

        private float pomerajSceneY = 0.0f;


        public float PomerajSceneY
        {
            get { return pomerajSceneY; }
            set { pomerajSceneY = value; }
        }

        private float pomerajSceneZ = 0.0f;
        private double db_scale;
        private double novaFormulaScale=0.0f;

        public float PomerajSceneZ
        {
            get { return pomerajSceneZ; }
            set { pomerajSceneZ = value; }
        }

        public double FormulaScale
        {
            get
            {
                return novaFormulaScale;
            }
            set
            {
                novaFormulaScale = value;
            }
        }

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>

        public World(OpenGL gl)
        {
            m_textures = new uint[m_textureCount];
             this.prva_formula = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\prvi"), "beetle.3DS", gl);
            //this.druga_formula = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\drugi"), "Pride_400.3DS", gl);
            //this.prva_formula = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Audi"), "Audi_S3.3ds", gl);
             //this.druga_formula = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Mini"), "Car mini N300114.3DS", gl);
            this.druga_formula = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\treci"), "Car Volkswagen Golf 2009 N280111.3DS", gl);
            Console.WriteLine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models"));

        }


        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); // podesi boju za brisanje ekrana na crnu
            gl.FrontFace(OpenGL.GL_CCW);

            // Osvetljenje
            SetupLighting(gl);

            gl.Enable(OpenGL.GL_NORMALIZE);

            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            // Ukljucivanje ColorTracking mehanizma
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            // Pozivom glColor se definise ambijenta i difuzijalna komponenta
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            //float[] whiteLight = { 1.0f, 1.0f, 1.0f, 1.0f };
            //gl.LightModel(LightModelParameter.Ambient, whiteLight);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);


            prva_formula.LoadScene();
            prva_formula.Initialize();
            druga_formula.LoadScene();
            druga_formula.Initialize();

            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(1);
            timer1.Tick += new EventHandler(UpdateAnimation1);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE); // GL_MDOULATE

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);      // Linear Filtering

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT); // wrapping
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT); // wrapping


                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        private void UpdateAnimation1(object sender, EventArgs e)
        {
            if (PomerajSceneZ < -100f)
            {
                PomerajSceneY += 0.3125f;
                PomerajSceneZ += 5.3125f;
                //Console.WriteLine("Pomeraj po Y: " + PomerajSceneY);

            }
            else if (PomerajSceneY > -100f)
            {
                PomerajSceneZ += 2.0f;
                PomerajSceneY += 0.2f;
            }

            if (PomerajLeve > -65.0f)
            {

                PomerajLeve -= 5f;
            }
            else if(PomerajLeve >-400.0f)
            {
                PomerajLeve -= 10f;
            }
           
            if (PomerajDesne > -65.0f)
            {
                PomerajDesne -=10f;
            }else if (PomerajDesne >-400.0f)
            {
                PomerajDesne -= 5f;
            } else
            {
                Anim = 0;

                PomerajDesne = 0f;
                PomerajLeve = 0f;
                PomerajSceneY = 0f;
                PomerajSceneZ = 0f;
                timer1.Stop();
            }
           

        }

        public void animacija()
        {
            Console.WriteLine("Animacija");
            Anim = 1;
            timer1.Start();



        }

        private void SetupLighting(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 0f, 2f, 0f, 0.0f }; // gore u odnosu na centar scene
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.4f, 0.4f, 0.0f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);


            float[] light1pos = new float[] { -0.5f, 3f, 4f, 1.0f }; // iznad levog bolida
            float[] light1ambient = new float[] { prva_komponenta, druga_komponenta, treca_komponenta, 1.0f };
            float[] light1diffuse = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light1specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };
            float[] smer = { 0.0f, -1.0f, 0.0f };

            // Podesi parametre reflektorkskog izvora 
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);

            float[] light2pos = new float[] { 0.5f, 3f, 4f, 1.0f }; // iznad desnog bolida
            float[] light2ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light2diffuse = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light2specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, light2pos);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_AMBIENT, light2ambient);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_DIFFUSE, light2diffuse);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPECULAR, light2specular);

            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT2);


        }



        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {

            m_height = height;
            m_width = width;
            gl.Viewport(0, 0, width, height); // kreiraj viewport po celom prozoru
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(50, (float)width / height, 1, 20000);
            // gl.Perspective(45f, (double)width / height, 0.1f, 20000f);
            gl.LookAt(0f, 1f, 30f, 0f, 0f, -5f, 0f, 10f, 0f);
            /* gl.MatrixMode(OpenGL.GL_MODELVIEW);
             gl.LoadIdentity();
             lookAtCam = new LookAtCamera();
             lookAtCam.Position = new Vertex(0f, 3f, 0f);
             lookAtCam.Target = new Vertex(0f, 0f, -5f);
             lookAtCam.UpVector = new Vertex(0f,-10f, 0f);
             right = new Vertex(1f, 0f, 0f);
             direction = new Vertex(0f, 0f, 0f);
            lookAtCam.Target = lookAtCam.Position + direction;*/
            // lookAtCam.Project(gl);
            //SetupCamera(gl);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
           
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE); // GL_MODULATE
            float[] light0ambient = new float[] { prva_komponenta, druga_komponenta, treca_komponenta, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_AMBIENT, light0ambient);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Viewport(0, 0, m_width, m_height); // kreiraj viewport po celom prozoru


            //gl.Scale(1f, 1f, 1f);
            gl.PushMatrix();

            if (Anim == 1)
            {
                if (PomerajSceneZ < 80f)
                {
                    gl.Translate(0f, PomerajSceneY, PomerajSceneZ);
                   // gl.Rotate(PomerajSceneZ * 90, 90f, 0f, 0f);
                    
                }
                else
                {
                    gl.Translate(0f, -10f, 50f);
                    gl.Rotate(15f, 7.0f,0.0f, 0.0f);

                }

            }
            gl.Scale(0.5f, 0.5f, 0.5f);
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);

            gl.Color(1.0f, 1.0f, 1.0f);

            // gl.Scale(-3.5f, -3.5f, -3.5f);
            //gl.LookAt(m_cameraX, m_cameraY, m_cameraZ, 0.0f, 0.0f, m_pointZ, 0.0f, 1.0f, 0.0f);

            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
           // DrawGrid(gl);

            DrawPodloga(gl);

            DrawStaza(gl);

            DrawPrviZid(gl);
            DrawDrugiZid(gl);

            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            DrawPrvaFormula(gl);
            DrawDrugaFormula(gl);

            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(1f, 0f, 0f);
            gl.Translate(5f, -8.0f, 0.0f);
            gl.Scale(0.6f, 0.6f, 0.6f);

            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "Predmet: Racunarska grafika");
            gl.Translate(-11.5f, -0.1f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "_______________________");
            gl.Translate(-11.5f, -2.0f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "Sk.god: 2019/20.");
            gl.Translate(-6.7f, -0.1f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "_____________");
            gl.Translate(-6.6f, -2.0f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "Ime: Dijana");
            gl.Translate(-4.5f, -0.1f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "__________");
            gl.Translate(-5.0f, -2.0f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "Prezime: Radic");
            gl.Translate(-6.0f, -0.1f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "____________");
            gl.Translate(-6.0f, -2.0f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "Sifra zad: 2.2");
            gl.Translate(-5.2f, -0.1f, 0.0f);
            gl.DrawText3D("Helvetica", 14, 1f, 0.1f, "____________");
            gl.Translate(-11.5f, -1.0f, 0.0f);
            gl.PopMatrix();
             
            /*gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(50f, (float)m_width / m_height, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            // gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Disable(OpenGL.GL_TEXTURE_2D);*/
            gl.PopMatrix();
            gl.Flush();
        }

       

        private void DrawGrid(OpenGL gl)
        {
            gl.PushMatrix();
            Grid grid = new Grid();
            gl.Translate(0f, -1f, 20f);
            gl.Rotate(90f, 0f, 0f);
            gl.Scale(30f, 30f, 30f);
            grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            gl.PopMatrix();
        }

        private void DrawPodloga(OpenGL gl)
        {
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();                
            gl.Scale(30f, 30f, 30f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.PushMatrix();
            gl.Translate(0f, 0f, 20f);
            gl.Rotate(0f, 0f, 180f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Sljunak]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0f, -1f, 0f);

           // gl.Color(1f, 1f, 1f);
            gl.TexCoord(0f, 0f);
            gl.Vertex(-300f, 0f, -300f);
            gl.TexCoord(0f, 1f);
            gl.Vertex(-300f, 0f, 300f);
            gl.TexCoord(1f, 1f);
            gl.Vertex(300f, 0f, 300f);
            gl.TexCoord(1f, 0f);
            gl.Vertex(300f, 0f, -300f);

            gl.End();

            gl.PopMatrix();
        }

        private void DrawStaza(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(0f, 1f, 20f);
            gl.Rotate(0f, 0f, 180f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Put]);
            gl.Begin(OpenGL.GL_QUADS);

            gl.Normal(0f, 5f, 0f);
           // gl.Color(0f, 1f, 0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(90f, 0f, 300f);

            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(90f, 0f, -300f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(-90f, 0f, -300f);
            gl.TexCoord(1.0f, 0f);
            gl.Vertex(-90f, 0f, 300f);
            gl.End();


            gl.PopMatrix();
        }

        private void DrawPrviZid(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-100f, 10f, 20f);
            gl.Scale(10f, 10f, 300f);
           // gl.Color(1f, 0f, 0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Metal]);
            Cube c1 = new Cube();
            c1.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        private void DrawDrugiZid(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(100f, 10f, 20f);
            gl.Scale(10f, 10f, 300f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Metal]);
            //gl.Color(1f, 0f, 0f);
            Cube c1 = new Cube();
            c1.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        private void DrawPrvaFormula(OpenGL gl) 
        {
            gl.PushMatrix();
            if (Anim == 1)
            {
                gl.Translate(0f, 0f, PomerajLeve);
            }

            gl.Translate(35f, 2f, 255f); 

            gl.Scale(0.3f, 0.3f, 0.3f);
            gl.Rotate(0.0f, -180.0f, 0.0f);
            gl.Rotate(0.0f, lb_rotate, 0.0f);

            gl.PushMatrix();
            if (novaFormulaScale != 0.0f)
            {
                gl.Scale(novaFormulaScale, novaFormulaScale, novaFormulaScale);
            }
            prva_formula.Draw();
            gl.PopMatrix();

            float[] light1pos = new float[] { 0f, 0.9f, 0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            float[] smer = { 0.0f, -1.0f, 0.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);


          //  gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD); 
           
            gl.PopMatrix();
        }

        private void DrawDrugaFormula(OpenGL gl)
        {
            if (Anim == 1)
            {
                gl.Translate(0f, 0f, PomerajDesne);
            }


            gl.PushMatrix();
            gl.Translate(-120.6f, 2f, 200f);
            gl.Translate(db_translate, 0f, 0f);
            //gl.Translate(0.0f, 0f, 0f); // 

            float[] light2pos = new float[] { 0f, 0.9f, 0f, 1.0f }; 
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, light2pos);
            float[] smer = { 0.0f, -1.0f, 0.0f };
            gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPOT_DIRECTION, smer);

            gl.Scale(2f, 2f, 2f);
           // gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            gl.Rotate(0f, -180f, 0f);
            gl.PushMatrix();
            if (novaFormulaScale != 0.0f)
            {
                gl.Scale(novaFormulaScale, novaFormulaScale, novaFormulaScale);
            }
            druga_formula.Draw();
            gl.PopMatrix();
            
            //gl.Scale(db_scale, db_scale, db_scale);
            
            
            gl.PopMatrix();
        }



        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>


        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose(bool v)
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion IDisposable metode

       
        

    }
}
