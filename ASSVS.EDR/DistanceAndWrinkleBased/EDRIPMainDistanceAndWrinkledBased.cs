using ASSVS.FD;
using ASSVS.ML.DistanceAndWrinkleBased;
using CVML.Constants;
using CVML.ExceptionHandler;
using CVML.Logger;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Distance and wrinkled based namespace
/// </summary>
namespace ASSVS.EDR.DistanceAndWrinkleBased
{
    /// <summary>
    /// Distance and wrinkled based main class
    /// </summary>
    public class EDRIPMainDistanceAndWrinkledBased
    {
        List<KeyValuePair<string, Color>> keyPairs = new List<KeyValuePair<string, Color>>();
        /// <summary>
        /// static object for EDRIPMainDistanceAndWrinkledBased class
        /// </summary>
        private static EDRIPMainDistanceAndWrinkledBased instance;
        /// <summary>
        /// Constructor is private to prevent generation of EDRIPMainDistanceAndWrinkledBased class from outside
        /// </summary>
        private EDRIPMainDistanceAndWrinkledBased()
        {

        }
        /// <summary>
        /// Return single object of EDRIPMainDistanceAndWrinkledBased class and if object already exist then send same object
        /// </summary>
        /// <returns>Rerturns object of EDRIPMainDistanceAndWrinkledBased</returns>
        public static EDRIPMainDistanceAndWrinkledBased getInstance()
        {
            if (instance == null)
                instance = new EDRIPMainDistanceAndWrinkledBased();
            return instance;
        }
        /// <summary>
        /// Train the machiene on the basis of wrinkles and distances
        /// </summary>
        /// <param name="files">Files needs to send to train machine</param>
        public void TrainSystemForEmotionDistanceAndWrinkledBased(string[] files)
        {
             EDRMLMainDistanceAndWrinkleBased.getInstance().trainSystemForEmotionDistanceAndWrinkleBased(EDRIPMain.getInstance().TrainSystemForEmotion(files));
        }
        /// <summary>
        /// Predict emotion on the basis of wrinkles and distances 
        /// </summary>
        /// <param name="ImageMats">Input image on which emotion need to detect</param>
        /// <returns>Business model containing images of different steps and output result</returns>
        public EmotionBusinessModel DetectEmotionsDistanceAndWrinkledBased(Mat[] ImageMats, List<Rectangle> facess)
        {
            EmotionBusinessModel output = new EmotionBusinessModel();
            EmotionBusinessModel.outputmsgs = new List<string>();
            try
            {
                using (Logger logger = new Logger())
                {
                    KeyValuePair<string, Color> keyPair = new KeyValuePair<string, Color>("smile",Color.Yellow);
                    keyPairs.Add(keyPair);
                    keyPair = new KeyValuePair<string, Color>("sad", Color.Blue);
                    keyPairs.Add(keyPair);
                    keyPair = new KeyValuePair<string, Color>("surprise", Color.SlateGray);
                    keyPairs.Add(keyPair);
                    keyPair = new KeyValuePair<string, Color>("anger", Color.Red);
                    keyPairs.Add(keyPair);
                    keyPair = new KeyValuePair<string, Color>("disgust", Color.Purple);
                    keyPairs.Add(keyPair);
                    keyPair = new KeyValuePair<string, Color>("fear", Color.Black);
                    keyPairs.Add(keyPair);
                    keyPair = new KeyValuePair<string, Color>("neutral", Color.Green);
                    keyPairs.Add(keyPair);
                    Mat ImageMat = ImageMats[0];
                    logger.LogIntoFile(Log.logType.INFO, (string)ConstantsLoader.getInstance().getValue(EnumConstant.emotionDetectionMessage));
                    List<Rectangle> faces = new List<Rectangle>();
                    faces = facess;
                    //faces = FDIPMAin.DetectFace(ImageMat);
                    output.faceRect = new List<Rectangle>();
                    foreach (Rectangle face in faces)
                    {
                        List<Rectangle> fcs = new List<Rectangle>();
                        fcs.Add(face);
                        output.outputResult = EDRIPDistanceAndWrinklesBasedFeatures.getInstance().FindEmotionDistanceAndWrinkleBased(ImageMat, fcs, 0);
                        EmotionBusinessModel.outputmsgs.Add(output.outputResult);
                        output.faceRect.Add(face);
                       
                    }
                    imageModel img = new imageModel();
                    if(faces.Count>0)
                    img.image = CvmlUtility.getInstance().annotate(ImageMat, faces[0], output.outputResult, Color.Blue);

                    //img.image=drawRect(ImageMat.ToImage<Bgr, byte>(), faces[0], output.outputResult);
                    img.label = "Final emotion detected image with rectangle on face";
                    output.images.Add(img);
                    output.outputMessage = "Emotion detected successfully in given image using wrinkled and distance based approach";
                    output.success = true;
                    
                }
            }

            catch (Exception e)
            {
                string text = ExceptionHandle.GetExceptionMessage(e);
                output.success = false;
                output.outputResult = "neutral";
                output.e = e;
               
            }
            return output;
        }
        /// <summary>
        /// Predict emotion on the basis of wrinkles and distances 
        /// </summary>
        /// <param name="bmp">Input image on which emotion need to detect</param>
        /// <returns>Business model containing images of different steps and output result</returns>
        public EmotionBusinessModel DetectEmotionsDistanceAndWrinkledBased(Bitmap bmp)
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(bmp);
            Mat[] mats = new Mat[1];
            mats[0] = img.Mat;

           return  DetectEmotionsDistanceAndWrinkledBased(mats,FDIPMAin.DetectFace(mats[0]));
        }
        /// <summary>
        /// Predict emotion on the basis of wrinkles and distances 
        /// </summary>
        /// <param name="bmp">Input image on which emotion need to detect</param>
        /// <param name="face">Input face on which emotion need to detect</param>
        /// <returns>Business model containing images of different steps and output result</returns>
        public EmotionBusinessModel DetectEmotionsDistanceAndWrinkledBased(Bitmap bmp,List<Rectangle> face)
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(bmp);
            Mat[] mats = new Mat[1];
            mats[0] = img.Mat;

            return DetectEmotionsDistanceAndWrinkledBased(mats, face);
        }
        private Mat drawRect(Image<Bgr, byte> imgProc, Rectangle rect, string output)
        {
            KeyValuePair<string, Color> keyPair = keyPairs.Where(a => a.Key.Equals(output)).FirstOrDefault() ;
            Rectangle rect1 = new Rectangle(rect.X, rect.Y - 40, rect.Width, 40);
            //imgProc.Draw(rect1, new Bgr(keyPair.Value.B, keyPair.Value.G, keyPair.Value.R),-1);
           // CvInvoke.PutText(imgProc, output, new Point(rect.X,rect.Y-10), FontFace.HersheyComplex, 1.5, new MCvScalar(0, 0, 255),2);
           // imgProc.Draw(output, new Point(rect.X+17, rect.Y - 11), FontFace.HersheyComplex, 2.5, new Bgr(0, 0, 0));
            imgProc.Draw(rect, new Bgr(keyPair.Value.B,keyPair.Value.G,keyPair.Value.R), 2);
            return imgProc.Mat;
            
        }
    }
  
}
