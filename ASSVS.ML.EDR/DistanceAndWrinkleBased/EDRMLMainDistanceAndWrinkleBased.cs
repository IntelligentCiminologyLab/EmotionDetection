using ASSVS.FAS.EDR.Shared;
using ASSVS.ML.EDR.Shared;
using CVML.Constants;
using CVML.MLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASSVS.ML.DistanceAndWrinkleBased
{
    public class EDRMLMainDistanceAndWrinkleBased
    {
        /// <summary>
        /// static object for EDRMLMainDistanceAndWrinkleBased class
        /// </summary>
        private static EDRMLMainDistanceAndWrinkleBased instance;
        /// <summary>
        /// Data base object to fetch and save data in DB
        /// </summary>
        private EDRFASMainDistanceAndWrinkleBased db = EDRFASMainDistanceAndWrinkleBased.getInstance();
        /// <summary>
        /// Constructor is private to prevent generation of EDRMLMainDistanceAndWrinkleBased class from outside
        /// </summary>
        private EDRMLMainDistanceAndWrinkleBased()
        {

        }
        /// <summary>
        /// Return single object of EDRMLMainDistanceAndWrinkleBased class and if object already exist then send same object
        /// </summary>
        /// <returns>Rerturns object of EDRMLMainDistanceAndWrinkleBased</returns>
        public static EDRMLMainDistanceAndWrinkleBased getInstance()
        {
            if (instance == null)
                instance = new EDRMLMainDistanceAndWrinkleBased();
            return instance;
        }
        /// <summary>
        /// Get calculated emotion after applying machiene learning algorithm based on wrinkles and distances
        /// </summary>
        /// <param name="emotionDistances">Distances and wrinkles features to test emotion</param>
        /// <returns>Returns a string value of emotion</returns>
        public string getCalculatedEmotionsDistanceAndWrinkleBased(EmotionTrainingMLModel emotionDistances)
        {
            string[] attributeArray = { "LEB1_CPLE1", "LEB2_CPLE2", "REB1_CPRE1", "REB2_CPRE2", "OPEN_LE1", "OPEN_LE2", "OPEN_RE1", "OPEN_RE2", "EXP_MO", "OPEN_MO1", "OPEN_MO2", "OPEN_MO3", "NS_LPLIP", "NS_RPLIP", "betweenEyesWrinkles", "noseWrinkles" };
            string[] classNames = { "anger", "smile", "sad", "surprise", "neutral", "fear", "disgust" };
            double[] dataValues = { emotionDistances.LEB1_CPLE1, emotionDistances.LEB2_CPLE2, emotionDistances.REB1_CPRE1, emotionDistances.REB2_CPRE2, emotionDistances.OPEN_LE1, emotionDistances.OPEN_LE2, emotionDistances.OPEN_RE1, emotionDistances.OPEN_RE2, emotionDistances.EXP_MO, emotionDistances.OPEN_MO1, emotionDistances.OPEN_MO2, emotionDistances.OPEN_MO3, emotionDistances.NS_LPLIP, emotionDistances.NS_RPLIP, emotionDistances.betweenEyesWrinkles, emotionDistances.noseWrinkles };
            string classHeader = "label";
            string defaultclass = "neutral";
            return MLMain.getInstance().testEmotionUsingWeka(attributeArray, classNames, dataValues, classHeader, defaultclass,(string) ConstantsLoader.getInstance().getValue(EnumConstant.emotionMethod2Model));
        }
        /// <summary>
        /// Get calculated emotion after applying machiene learning algorithm based on wrinkles and distances
        /// </summary>
        /// <param name="emotionDistances">Distances and wrinkles features to test emotion probilities</param>
        /// <returns>Returns a array probilities of emotion </returns>
        public List<double> getCalculatedEmotionsProbilitiesDistanceAndWrinkleBased(EmotionTrainingMLModel emotionDistances)
        {
            string[] attributeArray = { "LEB1_CPLE1", "LEB2_CPLE2", "REB1_CPRE1", "REB2_CPRE2", "OPEN_LE1", "OPEN_LE2", "OPEN_RE1", "OPEN_RE2", "EXP_MO", "OPEN_MO1", "OPEN_MO2", "OPEN_MO3", "NS_LPLIP", "NS_RPLIP", "betweenEyesWrinkles", "noseWrinkles" };
            string[] classNames = { "anger", "smile", "sad", "surprise", "neutral", "fear", "disgust" };
            double[] dataValues = { emotionDistances.LEB1_CPLE1, emotionDistances.LEB2_CPLE2, emotionDistances.REB1_CPRE1, emotionDistances.REB2_CPRE2, emotionDistances.OPEN_LE1, emotionDistances.OPEN_LE2, emotionDistances.OPEN_RE1, emotionDistances.OPEN_RE2, emotionDistances.EXP_MO, emotionDistances.OPEN_MO1, emotionDistances.OPEN_MO2, emotionDistances.OPEN_MO3, emotionDistances.NS_LPLIP, emotionDistances.NS_RPLIP, emotionDistances.betweenEyesWrinkles, emotionDistances.noseWrinkles };
            string classHeader = "label";
            string defaultclass = "neutral";
            return MLMain.getInstance().testMLPPredictionsUsingWeka(attributeArray, classNames, dataValues, classHeader, defaultclass,(string) ConstantsLoader.getInstance().getValue(EnumConstant.emotionMethod2Model));
        }
        /// <summary>
        /// Train machiene  based on wrinkles and distances
        /// </summary>
        /// <param name="emotionDistances">Distances and wrinkles features to train emotion</param>
        public void trainSystemForEmotionDistanceAndWrinkleBased(List<EmotionTrainingMLModel> emotionDistances)
        {
            db.saveToDbDistanceAndWrinkleBased(EDRMLUtil.getInstance().ConvertToDataModels(emotionDistances));
            List<EmotionDataModel> emotionsDistances = db.getDataFromDatabaseDistanceAndWrinkleBased();
            createWekaFileDistanceAndWrinkleBased(emotionsDistances);

            MLMain.getInstance().trainMachineForEmotionUsingWeka((string)ConstantsLoader.getInstance().getValue(EnumConstant.emotionMethod2Arff), (string)ConstantsLoader.getInstance().getValue(EnumConstant.emotionMethod2Model));
        }
        /// <summary>
        /// Create weka file of wrinkles and distances 
        /// </summary>
        public void createWekaFileDistanceAndWrinkleBased(List<EmotionDataModel> emotionsDistances)
        {
           
            List<string> features = new List<string>();
            foreach (EmotionDataModel obj in emotionsDistances)
            {

                string wekaData = obj.LEB1_CPLE1
                    + "," + obj.LEB2_CPLE2
                    + "," + obj.REB1_CPRE1
                    + "," + obj.REB2_CPRE2
                    + "," + obj.OPEN_LE1
                    + "," + obj.OPEN_LE2
                    + "," + obj.OPEN_RE1
                    + "," + obj.OPEN_RE2
                    + "," + obj.EXP_MO
                    + "," + obj.OPEN_MO1
                    + "," + obj.OPEN_MO2
                    + "," + obj.OPEN_MO3
                    + "," + obj.NS_LPLIP
                    + "," + obj.NS_RPLIP
                    + "," + obj.betweenEyesWrinkles
                    + "," + obj.noseWrinkles
                    + "," + obj.Label;
                features.Add(wekaData);
            }

            // string[] features = { "1,4.5,34,apple", "2,56,3.4,banana", "3.5,6.5,34,orange" };
            string[] lOD = { "LEB1_CPLE1 numeric", "LEB2_CPLE2 numeric", "REB1_CPRE1 numeric", "REB2_CPRE2 numeric", "OPEN_LE1 numeric", "OPEN_LE2 numeric", "OPEN_RE1 numeric", "OPEN_RE2 numeric", "EXP_MO numeric", "OPEN_MO1 numeric", "OPEN_MO2 numeric", "OPEN_MO3 numeric", "NS_LPLIP numeric", "NS_RPLIP numeric", "betweenEyesWrinkles numeric", "noseWrinkles numeric" };
            List<string> attributes = lOD.ToList<string>();
            string classHeader = "label {anger, smile, sad, surprise, neutral, fear, disgust}";
            attributes.Add(classHeader);
            MLMain.getInstance().createWekaFile("%% weka file for methodology2 emotion", "Emotion", attributes, features, (string)ConstantsLoader.getInstance().getValue(EnumConstant.emotionMethod2Arff));
        }
    }
}
