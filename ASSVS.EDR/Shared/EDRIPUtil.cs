using ASSVS.ML.EDR.Shared;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Drawing;

namespace ASSVS.EDR
{
    /// <summary>
    /// Utility class that contain helper functions
    /// </summary>
    public class EDRIPUtil
    {
        /// <summary>
        /// Convert from business model to ML model
        /// </summary>
        /// <param name="emotion">IP layer model</param>
        /// <returns>ML layer model</returns>
        public EmotionTrainingMLModel ConvertFromBusinessObject(EmotionTrainingDataModel emotion)
        {
            EmotionTrainingMLModel E = new EmotionTrainingMLModel(emotion.LEB1_CPLE1, emotion.LEB2_CPLE2, emotion.REB1_CPRE1, emotion.REB2_CPRE2, emotion.OPEN_LE1, emotion.OPEN_LE2, emotion.OPEN_RE1, emotion.OPEN_RE2, emotion.OPEN_MO1, emotion.OPEN_MO2, emotion.OPEN_MO3, emotion.EXP_MO,emotion.NS_LPLIP,emotion.NS_RPLIP,emotion.noseWrinkles,emotion.betweenEyesWrinkles,emotion.Label);
            return E;
        }
        /// <summary>
        /// Convert from business models to ML models
        /// </summary>
        /// <param name="emotions">IP layer models</param>
        /// <returns>ML layer models</returns>
        public List<EmotionTrainingMLModel> ConvertFromBusinessObjects(List<EmotionTrainingDataModel> emotions)
        {
            List<EmotionTrainingMLModel> Emotions = new List<EmotionTrainingMLModel>();
            foreach (EmotionTrainingDataModel e in emotions)
                Emotions.Add(ConvertFromBusinessObject(e));
            return Emotions;
        }
    }
}
