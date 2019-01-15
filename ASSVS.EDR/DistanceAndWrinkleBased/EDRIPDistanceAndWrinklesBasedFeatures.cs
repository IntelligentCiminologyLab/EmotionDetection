using ASSVS.ML.DistanceAndWrinkleBased;
using ASSVS.ML.EDR.Shared;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASSVS.EDR.DistanceAndWrinkleBased
{
    /// <summary>
    /// Distance and wrinkled based namespace
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }
    /// <summary>
    /// Distance and wrinkled based feature class
    /// </summary>
    public class EDRIPDistanceAndWrinklesBasedFeatures
    {
        /// <summary>
        /// static object for EDRIPDistanceAndWrinklesBasedFeatures class
        /// </summary>
        private static EDRIPDistanceAndWrinklesBasedFeatures instance;
        /// <summary>
        /// constructor is private to prevent generation of EDRIPDistanceAndWrinklesBasedFeatures class from outside
        /// </summary>
        private EDRIPDistanceAndWrinklesBasedFeatures()
        {

        }
        /// <summary>
        /// Return single object of EDRIPDistanceAndWrinklesBasedFeatures class and if object already exist then send same object
        /// </summary>
        /// <returns>Rerturns object of EDRIPDistanceAndWrinklesBasedFeatures</returns>
        public static EDRIPDistanceAndWrinklesBasedFeatures getInstance()
        {
            if (instance == null)
                instance = new EDRIPDistanceAndWrinklesBasedFeatures();
            return instance;
        }
        /// <summary>
        /// Find features of face for distance and wrinkles based
        /// </summary>
        /// <param name="ImageMat">Input image for features detection</param>
        /// <param name="faces">List of facse rectangles onm face</param>
        /// <param name="index">Index of image in case of multiple images</param>
        /// <returns>Return the predicted emotions based on distances and wrinkles</returns>
        public string FindEmotionDistanceAndWrinkleBased(Mat ImageMat, List<Rectangle> faces, int index)
        {
            EmotionTrainingMLModel dataModel = new EmotionTrainingMLModel();
            EDRFeatureExtraction obj = new EDRFeatureExtraction();
            dataModel = obj.FindEmotions(ImageMat, faces, index);
            return EDRMLMainDistanceAndWrinkleBased.getInstance().getCalculatedEmotionsDistanceAndWrinkleBased(dataModel);
        }

        
    }
}
