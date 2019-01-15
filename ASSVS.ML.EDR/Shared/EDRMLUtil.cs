using ASSVS.FAS.EDR;
using ASSVS.FAS.EDR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASSVS.ML.EDR.Shared
{
    public class EDRMLUtil
    {
        /// <summary>
        /// static object for EDRMLUtil class
        /// </summary>
        private static EDRMLUtil instance;
        /// <summary>
        /// Constructor is private to prevent generation of EDRMLUtil class from outside
        /// </summary>
        private EDRMLUtil()
        {

        }
        /// <summary>
        /// Return single object of EDRMLMainDistanceAndWrinkleBased class and if object already exist then send same object
        /// </summary>
        /// <returns>Rerturns object of EDRMLMainDistanceAndWrinkleBased</returns>
        public static EDRMLUtil getInstance()
        {
            if (instance == null)
                instance = new EDRMLUtil();
            return instance;
        }
        /// <summary>
        /// Convert from ML model to DB model
        /// </summary>
        /// <param name="emotion">ML layer model</param>
        /// <returns>DB layer model</returns>
        public EmotionTrainingDataModel ConvertToDataObject(EmotionMLModel emotion)
        {
            EmotionTrainingDataModel E = new EmotionTrainingDataModel(emotion.sum, emotion.label);
            return E;
        }
        /// <summary>
        /// Convert from ML model to DB model
        /// </summary>
        /// <param name="emotion">ML layer model</param>
        /// <returns>DB layer model</returns>
        public EmotionDataModel ConvertToDataModel(EmotionTrainingMLModel emotion)
        {
            EmotionDataModel E = new EmotionDataModel(emotion.LEB1_CPLE1, emotion.LEB2_CPLE2,emotion.REB1_CPRE1,emotion.REB2_CPRE2,emotion.OPEN_LE1,emotion.OPEN_LE2,emotion.OPEN_RE2,emotion.OPEN_RE2,emotion.OPEN_MO1,emotion.OPEN_MO2,emotion.OPEN_MO3,emotion.EXP_MO,emotion.NS_LPLIP,emotion.NS_RPLIP,emotion.noseWrinkles,emotion.betweenEyesWrinkles,emotion.Label);
            return E;
        }
        /// <summary>
        /// Convert from ML models to DB models
        /// </summary>
        /// <param name="emotion">ML layer model</param>
        /// <returns>DB layer models</returns>
        public List<EmotionDataModel> ConvertToDataModels(List<EmotionTrainingMLModel> emotions)
        {
            List<EmotionDataModel> Emotions = new List<EmotionDataModel>();
            foreach (EmotionTrainingMLModel e in emotions)
                Emotions.Add(ConvertToDataModel(e));
            return Emotions;
        }
    }
}
