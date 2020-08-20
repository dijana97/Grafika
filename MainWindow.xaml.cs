using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;



        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_world.Anim == 0)
            {
                switch (e.Key)
                {
                    case Key.F4: this.Close(); break;
                    case Key.I: m_world.RotationX -= 5.0f; break;
                    case Key.K: m_world.RotationX += 5.0f; break;
                    case Key.J: m_world.RotationY -= 5.0f; break;
                    case Key.L: m_world.RotationY += 5.0f; break;
                    case Key.Add: m_world.SceneDistance -= 10.0f; break;
                    case Key.Subtract: m_world.SceneDistance += 10.0f; break;
                    case System.Windows.Input.Key.V: m_world.animacija(); break;
                    case Key.F2:
                        OpenFileDialog opfModel = new OpenFileDialog();
                        bool result = (bool)opfModel.ShowDialog();
                        if (result)
                        {

                            try
                            {
                                // World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                                // m_world.Dispose();
                                //m_world = newWorld;
                                //m_world.Initialize(openGLControl.OpenGL);
                            }
                            catch (Exception exp)
                            {
                                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                            }
                        }
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (m_world.Anim == 0)
            {
                double d;

                if (Double.TryParse(inputText.Text, out d))
                {
                    Console.WriteLine("'{0}' --> {1}", inputText.Text, d);
                    m_world.dbTranslate += (float)d;
                }
                else
                {
                    Console.WriteLine("Unable to parse '{0}'.", inputText.Text);
                }
            }

        }

       

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_world.Anim == 0)
            {
                double d = e.NewValue;
                m_world.lbRotate = (float)90.0 + (float)d;
            }
        }

      

        private void FormulaSkalaButton(object sender, RoutedEventArgs e)
        {
           
            if (m_world.Anim == 0)
            {
                double d;

                if (Double.TryParse(FormulaSkala.Text, out d))
                {
                    Console.WriteLine("'{0}' --> {1}", FormulaSkala.Text, d);
                    m_world.FormulaScale += (float)d;
                }
                else
                {
                    Console.WriteLine("Unable to parse '{0}'.", FormulaSkala.Text);
                }
            }

        }
    }
}
