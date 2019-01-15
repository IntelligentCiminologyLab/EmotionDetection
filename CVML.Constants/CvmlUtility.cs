using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVML.Constants
{
   public class CvmlUtility
    {
        private static CvmlUtility _instance;

        public static CvmlUtility getInstance()
        {
            if (_instance == null)
                _instance = new CvmlUtility();
            return _instance;
        }

        private CvmlUtility()
        {

        }

        public Mat annotate(Mat image, Rectangle face, String text, Color c)
        {

            int adjustHorizontalPoint = text.Length * 5;
            CvInvoke.Rectangle(image, face, new Bgr(c).MCvScalar, 2);
            Rectangle filled_rectangle = new Rectangle(face.X, face.Y - face.Height / 6, face.Width, face.Height / 6);
            CvInvoke.Rectangle(image, filled_rectangle, new Bgr(c).MCvScalar, -1); // filled rectangle for tag
                                                                                                  //CvInvoke.PutText(image, "Female",new Point(filled_rectangle.X + 10,filled_rectangle.Y+20),FontFace.HersheySimplex,1.0,new Bgr(255,255,255).MCvScalar);
            int thickness;
            if (filled_rectangle.Height <= 100)
            {
                thickness = 2;
            }
        
            else if (filled_rectangle.Height > 100 && filled_rectangle.Height <= 300)
            {
                thickness = 3;
            }

            else
            {
                thickness = 4;
            }
            int width = filled_rectangle.Width;
            int size = text.Length;
            int char_left = width - size;
            char_left = char_left / 2;
            CvInvoke.PutText(image, text, new Point(filled_rectangle.X + char_left / 2, filled_rectangle.Y + (int)((0.85) * filled_rectangle.Height)), FontFace.HersheySimplex, filled_rectangle.Height / 30.0, new Bgr(Color.White).MCvScalar, thickness);
            return image;
        }

    

        public void setDllSearchDirectory(String path)
        {
            AppDomainSetup app = new AppDomainSetup();
            app.PrivateBinPath = path;
            //Directory.SetCurrentDirectory(path);
           Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + path);
        }
    }
}
