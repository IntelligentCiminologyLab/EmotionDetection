using ASSVS.FAS.EDR.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASSVS.FAS.EDR.Shared
{
    public class EDRFASMainDistanceAndWrinkleBased
    {
        /// <summary>
        /// Static object for EDRFASMainDistanceAndWrinkleBased class
        /// </summary>
        private static EDRFASMainDistanceAndWrinkleBased instance;
        /// <summary>
        /// Constructor is private to prevent generation of EDRFASMainDistanceAndWrinkleBased class from outside
        /// </summary>
        private EDRFASMainDistanceAndWrinkleBased()
        {

        }
        /// <summary>
        /// Return single object of EDRFASMainDistanceAndWrinkleBased class and if object already exist then send same object
        /// </summary>
        /// <returns>Rerturns object of EDRFASMainDistanceAndWrinkleBased</returns>
        public static EDRFASMainDistanceAndWrinkleBased getInstance()
        {
            if (instance == null)
                instance = new EDRFASMainDistanceAndWrinkleBased();
            return instance;
        }
        /// <summary>
        /// Get data from table wrinkle and distances
        /// </summary>
        /// <returns>Return features saved in database</returns>
        public List<EmotionDataModel> getDataFromDatabaseDistanceAndWrinkleBased()
        {
            EmotionDataModel obj;
            List<EmotionDataModel> emotionList = new List<EmotionDataModel>();
            emotiondbEntities1 db = new emotiondbEntities1();
            List<Emotion_Training_distances> distances = db.Emotion_Training_distances.ToList();
            foreach (Emotion_Training_distances model in distances)
            {
                obj = new EmotionDataModel();
                obj.LEB1_CPLE1 = model.LEB1_CPLE1;
                obj.LEB2_CPLE2 = model.LEB2_CPLE2;
                obj.REB1_CPRE1 = model.REB1_CPRE1;
                obj.REB2_CPRE2 = model.REB2_CPRE2;
                obj.OPEN_LE1 = model.OPEN_LE1;
                obj.OPEN_LE2 = model.OPEN_LE2;
                obj.OPEN_RE1 = model.OPEN_RE1;
                obj.OPEN_RE2 = model.OPEN_RE2;
                obj.OPEN_MO1 = model.OPEN_MO1;
                obj.OPEN_MO2 = model.OPEN_MO2;
                obj.OPEN_MO3 = model.OPEN_MO3;
                obj.EXP_MO = model.EXP_MO;
                obj.NS_LPLIP = model.NS_LPLIP;
                obj.NS_RPLIP = model.NS_RPLIP;
                obj.betweenEyesWrinkles = model.betweenEyesWrinkles;
                obj.noseWrinkles = model.noseWrinkles;
                obj.Label = model.Label;
                emotionList.Add(obj);
            }
            return emotionList;
        }
        /// <summary>
        /// Save wrinkles and distnces in database
        /// </summary>
        /// <param name="distances">Wrinkles and distances</param>
        public void saveToDbDistanceAndWrinkleBased(List<EmotionDataModel> distances)
        {
            Emotion_Training_distances obj;
            List<Emotion_Training_distances> features = new List<Emotion_Training_distances>();
            emotiondbEntities1 db = new emotiondbEntities1();
            foreach (EmotionDataModel model in distances)
            {
                obj = new Emotion_Training_distances();
                obj.LEB1_CPLE1 = model.LEB1_CPLE1;
                obj.LEB2_CPLE2 = model.LEB2_CPLE2;
                obj.REB1_CPRE1 = model.REB1_CPRE1;
                obj.REB2_CPRE2 = model.REB2_CPRE2;
                obj.OPEN_LE1 = model.OPEN_LE1;
                obj.OPEN_LE2 = model.OPEN_LE2;
                obj.OPEN_RE1 = model.OPEN_RE1;
                obj.OPEN_RE2 = model.OPEN_RE2;
                obj.OPEN_MO1 = model.OPEN_MO1;
                obj.OPEN_MO2 = model.OPEN_MO2;
                obj.OPEN_MO3 = model.OPEN_MO3;
                obj.EXP_MO = model.EXP_MO;
                obj.NS_LPLIP = model.NS_LPLIP;
                obj.NS_RPLIP = model.NS_RPLIP;
                obj.betweenEyesWrinkles = model.betweenEyesWrinkles;
                obj.noseWrinkles = model.noseWrinkles;
                obj.Label = model.Label;
                features.Add(obj);
            }
            db.Emotion_Training_distances.AddRange(features);
            db.SaveChanges();
        }
    }
}
