using System.Collections.Generic;
using System.Linq;

namespace ASSVS.FAS.EDR.Shared
{
    public class EDRFASMain
    {
        private static EDRFASMain instance;
        private EDRFASMain()
        {

        }
        public static EDRFASMain getInstance()
        {
            if (instance == null)
                instance = new EDRFASMain();
            return instance;
        }
        private static string[] listOfDistances = { "LEB1_CPLE1", "LEB2_CPLE2", "REB1_CPRE1", "REB2_CPRE2", "NS_LPLIP", "NS_RPLIP", "OPEN_LE1", "OPEN_LE2", "OPEN_RE1", "OPEN_RE2", "EXP_MO", "OPEN_MO1", "OPEN_MO2", "OPEN_MO3", "betweenEyesWrinkles", "noseWrinkles", "Label" };


        public List<EmotionDataModel> getDataFromDatabaseDistanceBased()
        {
            EmotionDataModel obj;
            List<EmotionDataModel> emotionList = new List<EmotionDataModel>();
            emotiondbEntities1 db = new emotiondbEntities1();
            List<Emotion_training_distances_wrinkle_free> distances= db.Emotion_training_distances_wrinkle_free.ToList();
            foreach (Emotion_training_distances_wrinkle_free model in distances)
            {
                obj= new EmotionDataModel();
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
                obj.Label = model.Label;
                emotionList.Add(obj);
            }
            return emotionList;
        }
        public void saveToDbDistanceBased(List<EmotionDataModel> distances)
        {
            Emotion_training_distances_wrinkle_free obj;
            List<Emotion_training_distances_wrinkle_free> features = new List<Emotion_training_distances_wrinkle_free>();
            emotiondbEntities1 db = new emotiondbEntities1();
            foreach (EmotionDataModel model in distances)
            {
                 obj = new Emotion_training_distances_wrinkle_free();
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
                obj.Label = model.Label;
                features.Add(obj);
            }
            db.Emotion_training_distances_wrinkle_free.AddRange(features);
            db.SaveChanges();
        }
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

            //System.Data.SqlClient.SqlConnection sqlConnection1 =
            //new System.Data.SqlClient.SqlConnection(Constant.emotionDataConnectionString);

            //System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            //cmd.CommandType = System.Data.CommandType.Text;
            //cmd.CommandText = "SELECT * FROM [Emotion_Training_distances]";
            //cmd.Connection = sqlConnection1;
            //sqlConnection1.Open();
            //using (SqlDataReader reader = cmd.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        EmotionDataModel dataModel = new EmotionDataModel();
            //        for (int i = 0; i < listOfDistances.Count(); i++)
            //        {
            //            var value = reader[listOfDistances[i]];
            //            PropertyInfo propertyInfo = dataModel.GetType().GetProperty(listOfDistances[i]);
            //            propertyInfo.SetValue(dataModel, value, null);

            //        }

            //        emotionList.Add(dataModel);
            //    }
            //}

            //sqlConnection1.Close();
            return emotionList;
        }
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


            //System.Data.SqlClient.SqlConnection sqlConnection1 =
            //new System.Data.SqlClient.SqlConnection(Constant.emotionDataConnectionString);

            //System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            //cmd.CommandType = System.Data.CommandType.Text;
            //cmd.CommandText = "INSERT Emotion_Training_distances (LEB1_CPLE1,LEB2_CPLE2 ,REB1_CPRE1,REB2_CPRE2,OPEN_LE1,OPEN_LE2,OPEN_RE1,OPEN_RE2,EXP_MO ,OPEN_MO1,OPEN_MO2,OPEN_MO3,NS_LPLIP,NS_RPLIP,betweenEyesWrinkles,noseWrinkles,Label) VALUES";
            //foreach (EmotionDataModel emotionDataObject in distances)
            //{
            //    cmd.CommandText += "(" + emotionDataObject.LEB1_CPLE1 + "," + emotionDataObject.LEB2_CPLE2 + "," + emotionDataObject.REB1_CPRE1 + "," + emotionDataObject.REB2_CPRE2 + "," + emotionDataObject.OPEN_LE1 + "," + emotionDataObject.OPEN_LE2 + "," + emotionDataObject.OPEN_RE1 + "," + emotionDataObject.OPEN_RE2 + "," + emotionDataObject.EXP_MO + "," + emotionDataObject.OPEN_MO1 + "," + emotionDataObject.OPEN_MO2 + "," + emotionDataObject.OPEN_MO3 + "," + emotionDataObject.NS_LPLIP + "," + emotionDataObject.NS_RPLIP + "," + emotionDataObject.betweenEyesWrinkles + "," + emotionDataObject.noseWrinkles + ",'" + emotionDataObject .Label+ "')";
            //    if (distances.IndexOf(emotionDataObject) < distances.Count - 1)
            //    {
            //        cmd.CommandText += ",";
            //    }
            //}


            //cmd.Connection = sqlConnection1;

            //sqlConnection1.Open();
            //cmd.ExecuteNonQuery();
            //sqlConnection1.Close();
        }

        public void trainingToDB(List<EmotionTrainingDataModel> distances)
        {
           

            //System.Data.SqlClient.SqlConnection sqlConnection1 =
            //new System.Data.SqlClient.SqlConnection(Constant.emotionDataConnectionString);

            //System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            //cmd.CommandType = System.Data.CommandType.Text;
            //cmd.CommandText = "INSERT Emotion_Training_Data (Emotion_Value,Emotion_Name) VALUES";
            //foreach (EmotionTrainingDataModel emotionDataObject in distances)
            //{
            //    cmd.CommandText += "(" + emotionDataObject.sum + ",'" + emotionDataObject.label+"')";
            //    if (distances.IndexOf(emotionDataObject) < distances.Count - 1)
            //    {
            //        cmd.CommandText += ",";
            //    }
                
            //}
            //cmd.Connection = sqlConnection1;
            //sqlConnection1.Open();
            //cmd.ExecuteNonQuery();
            //sqlConnection1.Close();
        }
    }
}
