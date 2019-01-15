using ASSVS.FD;
using ASSVS.ML.DistanceAndWrinkleBased;
using ASSVS.ML.DistanceBased;
using ASSVS.ML.EDR.ReducedDimentionsBased;
using ASSVS.ML.EDR.Shared;
using CVML.Constants;
using CVML.ExceptionHandler;
using CVML.Logger;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ASSVS.EDR
{
    /// <summary>
    /// Emotion detection main class
    /// </summary>
    public class EDRIPMain
    {
        /// <summary>
        /// static object for EDRIPMain class
        /// </summary>
        private static EDRIPMain instance;
        /// <summary>
        /// Constructor is private to prevent generation of EDRIPMain class from outside
        /// </summary>
        private EDRIPMain()
        {

        }
        /// <summary>
        /// Return single object of EDRIPMain class and if object already exist then send same object
        /// </summary>
        /// <returns>Rerturns object of EDRIPMain</returns>
        public static EDRIPMain getInstance()
        {
            if (instance == null)
                instance = new EDRIPMain();

            return instance;
        }
        /// <summary>
        /// Load a model to pridict point on face
        /// </summary>
        [DllImport(StaticConstants.FaceLandmarksDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShapePredictorLoad();
        [DllImport(StaticConstants.FaceLandmarksDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ShapePredictorLoadArgument(string str);
        
        /// <summary>
        /// Load a model to pridict point on face by calling dll method 
        /// </summary>
        public void loadPredictor()
        {
            ShapePredictorLoad();
        }
        /// <summary>
        ///  Load a model to pridict point on face by calling dll method using string paramenter
        /// </summary>
        /// <param name="str">Path of dat file</param>

        public void loadPredictor(string str)
        {
            ShapePredictorLoadArgument(str);
        }
        /// <summary>
        /// List of all features for wrinkle based technique
        /// </summary>
        private static string[] listOfDistances = { "LEB1_CPLE1", "LEB2_CPLE2", "REB1_CPRE1", "REB2_CPRE2", "NS_LPLIP", "NS_RPLIP", "OPEN_LE1", "OPEN_LE2", "OPEN_RE1", "OPEN_RE2", "EXP_MO", "OPEN_MO1", "OPEN_MO2", "OPEN_MO3", "betweenEyesWrinkles", "noseWrinkles", "Label" };
        
       /// <summary>
       /// Train system using multiple files and then call ML algorithm to train
       /// </summary>
       /// <param name="files">List of file names to train machiene</param>
       /// <returns>Returns a model of ML layer</returns>
       
        public List<EmotionTrainingMLModel> TrainSystemForEmotion(string[] files)
        {
            using (Logger logger = new Logger())
            {
                List<EmotionTrainingMLModel> emotionList = new List<EmotionTrainingMLModel>();
                int index = 0;
                foreach (string file in files)
                {
                    logger.LogIntoFile(Log.logType.INFO, (string)ConstantsLoader.getInstance().getValue(EnumConstant.emotionTrainingMessage));
                    Mat image = new Mat(file);
                    List<Rectangle> faces = new List<Rectangle>();
                    faces = FDIPMAin.DetectFace(image);
                  
                    if (faces.Count > 0)
                    {
                        EDRFeatureExtraction featureExtracter = new EDRFeatureExtraction();

                        Mat shortImage = new Mat(image, faces[0]);
                        CvInvoke.Resize(shortImage, shortImage, new Size(320, 240), 0, 0, Inter.Linear);

                        faces = new List<Rectangle>();
                        faces.Add(new Rectangle(0, 0, shortImage.Width, shortImage.Height));
                        List<double> distances = featureExtracter.findDistances(shortImage, faces, index);

                        EmotionTrainingMLModel dataModel = new EmotionTrainingMLModel();
                        for (int i = 0; i < 14; i++)
                        {
                            var value = distances.ElementAt(i);
                            PropertyInfo propertyInfo = dataModel.GetType().GetProperty(listOfDistances[i]);
                            propertyInfo.SetValue(dataModel, value, null);
                        }
                        dataModel.noseWrinkles = featureExtracter.findWrinkles(shortImage, featureExtracter.getOnNose());
                        dataModel.betweenEyesWrinkles = featureExtracter.findWrinkles(shortImage, featureExtracter.getBetweenEyes());
                        dataModel.Label = Path.GetFileName(Path.GetDirectoryName(file));
                        emotionList.Add(dataModel);

                    }
                    index++;
                }
                return emotionList;
            }
        }
      
      
      
    }
}
