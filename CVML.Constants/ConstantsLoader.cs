using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml;

namespace CVML.Constants
{
    public class ConstantsLoader
    {
        private List<ConstantModel> constants;

        private static ConstantsLoader _instance;
        public static ConstantsLoader getInstance(String path = "cvmlconstants.xml")
        {
            if (_instance == null)
                _instance = new ConstantsLoader(path);
            return _instance;
        }

        private ConstantsLoader(String path)
        {
            readConfigurationFile(path);
        }

        public void readConfigurationFile(String filePath)
        {
            constants = new List<ConstantModel>();
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
      
            var xmlconstants = doc.GetElementsByTagName("constants").Item(0);
            foreach (XmlNode constant in xmlconstants.ChildNodes)
            {
                ConstantModel cobj = new ConstantModel();
                cobj.Values = new List<ConstantValue>();
                var attributes = constant.Attributes;
                foreach (XmlAttribute att in attributes)
                {
                    if (att.Name.Equals("name"))
                    {
                        cobj.Name = att.Value;
                    }
                    if (att.Name.Equals("type"))
                    {
                        cobj.DataType = att.Value;
                    }
                }
                foreach (XmlNode value in constant.ChildNodes)
                {
                    ConstantValue val = new ConstantValue();

                    foreach (XmlAttribute att in value.Attributes)
                    {
                        if (att.Name.Equals("framework"))
                        {
                            val.Framework = att.Value;
                        }
                    }
                    val.Value = value.InnerText;
                    cobj.Values.Add(val);
                }
                constants.Add(cobj);
            }
        }

        public object getValue(EnumConstant constant, EnumFramework framework = EnumFramework.Default)
        {
            var constantModel = constants.Where(a => a.Name.Equals(constant.ToString())).FirstOrDefault();
            if (constantModel != null)
            {
                String dtype = constantModel.DataType;
                var value = constantModel.Values.Where(a => a.Framework.ToLower().Equals(framework.ToString().ToLower())).FirstOrDefault();
                if (value != null)
                {

                    bool parseSuccess = false;
                    object parsedData = null;
                    try
                    {
                        var type = Type.GetType(dtype);
                        if (type.Name.ToLower().Equals("string"))
                            return value.Value;
                        parsedData = type.GetMethod("Parse", new[] { typeof(string) }).Invoke(null, new object[] { value.Value });
                        parseSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        parseSuccess = false;
                    }
                    if (parseSuccess)
                        return parsedData;
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;
        }


        public bool updateValue(EnumConstant constant, String str, EnumFramework framework = EnumFramework.Default)
        {
            var constantModel = constants.Where(a => a.Name.Equals(constant.ToString())).FirstOrDefault();
            if (constantModel != null)
            {
                String dtype = constantModel.DataType;
                var value = constantModel.Values.Where(a => a.Framework.ToLower().Equals(framework.ToString().ToLower())).FirstOrDefault();
                if (value != null)
                {
                    value.Value = str;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public static void UpdateConfigFile(String filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            String line = sr.ReadLine();
            line = sr.ReadLine();

            XmlWriter xmlWriter = XmlWriter.Create("E:/cvmlconstants.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("constants");
            while(line!=null)
            {
               
                String[] toks = line.Split(',');
                if (String.IsNullOrEmpty(toks[0]))
                    break ;
                xmlWriter.WriteStartElement("constant");
                xmlWriter.WriteAttributeString("name", toks[0]);
                xmlWriter.WriteAttributeString("type", toks[6]);
                xmlWriter.WriteAttributeString("description", toks[1]);
                xmlWriter.WriteAttributeString("module", toks[3]);
                xmlWriter.WriteAttributeString("methodology", toks[4]);
                xmlWriter.WriteAttributeString("addedby", toks[5]);
                xmlWriter.WriteStartElement("value");
                xmlWriter.WriteAttributeString("framework", "default");
                xmlWriter.WriteString(toks[2]);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                line = sr.ReadLine();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

   
        //public const int FRImageHeight = 320;
        //public const int FRImageWidth = 320;
        //public const double FRdownScale = 0.4;
        //public const string serverNameForCrystalReports = "aynulhassan.czwew4nughz4.us-east-1.rds.amazonaws.com,1433";
        //public const string databaseNameForCrystalReports = "facedb";
        //public const string userNameForCrystalReports = "aynulhassan";
        //public const string passwordForCrystalReports = "12345678";
        //public const string FRconnectionString = @"Data Source=aynulhassan.czwew4nughz4.us-east-1.rds.amazonaws.com,1433;Initial Catalog=facedb;Persist Security Info=True;User ID=aynulhassan;Password=12345678";
        //public const bool FRtryUseCuda = false;
        //public const int FRloopCount = 6;
        //public const int FRskipFrame = 5;
        //public static bool FRfaceDetected = false;
        //public static int FRframeCountFront = 0;
        //public static int FRframeCountRear = 0;
        //public const bool FRvalidateFace = false;
        //public const int FReigenNumComponents = 80;
        //public const double FReigenThreashold = 1000000;
        //public const int FRfisherNumComponents = 0;
        //public const double FRfisherThreashold = 2000000;
        //public const int FRLBPHRadius = 1;
        //public const int FRLBPHNeighbour = 8;
        //public const int FRLBPHGridX = 8;
        //public const int FRLBPHGridY = 8;
        //public const double FRLBPHThreashold = 150;
        //public static string FRtrainingPath = @"..\..\..\..\libs\FaceRecognition\Dataset\Trainings\";
        //public static string FRdirectoryPath = @"..\..\..\..\libs\FaceRecognition\Dataset\Images\";
        //public static string FRcameraSourceFront = "0";
        //public static string FRcameraSourceRear = "1";
        //public const string surfBowPathSave = @"..\\..\\..\\..\\libs\\models\\SR_Models\\SRHybridModel.xml";
        //public const string svmPathSave = @"..\\..\\..\\..\\libs\\models\\SR_Models\\SRSVMModel.xml";
        //public const string SRDatasetPath = @"D:\Datasets\Scene Datasets\Training6\";
        //public const string SRTestPath = @"D:\Datasets\Scene Datasets\Test6\";
        //public const int SRHueBinCount = 36;
        //public const int SRSaturationBinCount = 32;
        //public const int SRKnnClassifier = 10;
        //public const string SRKNNSavePath = @"..\\..\\..\\..\\libs\\models\\SR_Models\\knnClassifierModel.xml";
        //public const string savebySVMdct = @"..\\..\\..\\..\\libs\\models\\SR_Models\\SRSVMModelDCT.xml";
        //public const string savebySVMECOH = @"..\\..\\..\\..\\libs\\models\\SR_Models\\SRSVMModelECOH.xml";
        //public const int noOfFeaturesDCT = 912;
        //public const int noOfFeaturesECOH = 80;
        //public const string SRdiPOSSCID = @"D:\Documents\Visual Studio 2015\SceneDatasetTraining\SCID2_IndoorTraining\training";
        //public const string SRdiNEGSCID = @"D:\Documents\Visual Studio 2015\SceneDatasetTraining\SCID2_OutdoorTraining\training";
        //public const string SRdiPOS = @"F:\Scene Dataset\Training\Indoor\dining_room\";
        //public const string SRdiNEG = @"F:\Scene Dataset\Training\Outdoor\coast";
        //public const string aa = "how are you";
        //public const string emotionTrainingMessage = "Training Sytstem to detect emotion";
        //public const string emotionDetectionMessage = "Emotion detection function that return list of captured emotions and their percentage";
        //public const string emotionDataConnectionString = "Data Source=DESKTOP-2B6C80T; Initial Catalog=emotiondb;User id=sa;Password=g89456123;";
        //public const string genderDllImportPath = @"ExportedLib.dll";
        //public static string libsPath = @"..\..\..\..\libs";
        //public const string dimentionReductonDllImportPath = @"C:\Users\Gulraiz\Documents\Visual Studio 2015\Projects\Isomap\x64\Debug\Isomap.dll";
        //public const string genderCascadeLocation = @"C:\Users\CVML\Documents\Visual Studio 2015\Projects\babyNN\CascadeClassifiers\";
        //public static string faceCascadesPath = @"E:\CVML\SVN\SVN-SAMYAN\ASSVS-10-implementation\ASSVS\libs\cascades\haarcascade_frontalface_default.xml";
        //public static string faceCascade = @"E:\CVML\SVN\SVN-SAMYAN\ASSVS-10-implementation\ASSVS\libs\cascades\haarcascade_frontalface_default.xml";
        //public static string profileFaceCascade = @"haarcascade_profileface.xml";
        //public static string frontalFace = "haarcascade_frontalface_default.xml";
        //public const int noseFiducialPoint = 33;
        //public const int leftEyeFiducialPoint1 = 36;
        //public const int leftEyeFiducialPoint2 = 39;
        //public const int rightEyeFiducialPoint1 = 42;
        //public const int rightEyeFiducialPoint2 = 45;
        //public const int mouthFiducialPoint = 66;
        //public const int chinFiducialPoint = 8;
        //public const string GDRmlpModelName = "genderMLPModel";
        //public const string GDRmlp2ModelName = "genderNBModel";
        //public const int genderResizeHeight = 480;
        //public const int genderResizeWidth = 640;
        //public const string GDRCustomMlpModelName = "genderMLPModelCustom";
        //public const string genderSVMModel = "genderSVMModel.xml";
        //public const string abc = "BOWModelSaveLoad.xml";
        //public static string bowModelNameEmgu = @"..\..\..\..\libs\models\GDR_Models\BOWModelSaveLoad.xml";
        //public static string bowModelNameSVM = @"..\..\..\..\libs\models\GDR_Models\BOWSVMLoad.xml";
        //public const int KConstant = 1000;
        //public const string emotionMethod1Arff = "MLPDistanceBased.arff";
        //public const string emotionMethod1Model = "MLPDistanceBased";
        //public const string emotionMethod2Arff = "MLPDistanceAndWrinkleBased.arff";
        //public const string emotionMethod2Model = "MLPDistanceAndWrinkleBased";
        //public const string emotionMethod3Arff = "MLPReducedDimentions.arff";
        //public const string emotionMethod3Model = "MLPReducedDimentions";
        //public const string emotionMethod4Arff = "MLPOF.arff";
        //public const string emotionMethod4Model = "MLPOF";
        //public const string emotionHybridArff = "baggingHybrid.arff";
        //public const string emotionHybridModel = "baggingHybridModel";
        //public static string fullbodyCascade = @"..\..\..\..\libs\cascades\haarcascade_fullbody.xml";
        //public static string upperbodyCascade = @"..\..\..\..\libs\cascades\haarcascade_upperbody.xml";
        //public static string lowerbodyCascade = @"..\..\..\..\libs\cascades\haarcascade_lowerbody.xml";
        //public const int numClustersSOF = 10;
        //public const string ageCascadePath = @"..\..\..\libs\cascades\ADcascades\";
        //public const string BioInspiredDllImportPath = @"..\..\..\..\libs\agedll\ageBio.dll";
        //public const int ageBioResiseWidth = 60;
        //public const int ageBioResiseHeight = 60;
        //public const int ageOutputResiseWidth = 240;
        //public const int ageOutputResiseHeight = 320;
        //public const int ageResiseWidth = 800;
        //public const int ageResiseHeight = 800;
        //public const string adgwmlpemgumodel = @"..\..\..\..\libs\models\ageMLPEMGUModel.xml";
        //public const string adbiosvmemgumodel = @"..\..\..\..\libs\models\ageSVMBioEmgu.xml";
        //public const string adwmlpmodel = "agemodelsMLP.Model";
        //public const string actionTemplatePath = @"..\..\..\..\libs\models\ADR_Models\RunningActionTemplate.bin";
        //public const string joggActionTemplatePath = @"..\..\..\..\libs\models\ADR_Models\JoggingActionTemplate.bin";
        //public const string walkActionTemplatePath = @"..\..\..\..\libs\models\ADR_Models\WalkingActionTemplate.bin";
        //public const string walkHistogram = @"..\..\..\..\libs\models\ADR_Models\WalkingActionTemplate.bin";
        //public const string runHistogram = @"..\..\..\..\libs\models\ADR_Models\RunningActionTemplate.bin";
        //public const string joggHistogram = @"..\..\..\..\libs\models\ADR_Models\JoggingActionTemplate.bin";
        //public const string WalkingPose = @"..\\..\\..\\..\\libs\\models\\ADR_Models\\walkingModel.bin";
        //public const string RunningPose = @"..\\..\\..\\..\\libs\\models\\ADR_Models\\RunningPose.bin";
        //public const string JoggingPose = @"..\\..\\..\\..\\libs\\models\\ADR_Models\\JoggingPose.bin";
        //public const string seqOfKeyPoseModel = @"..\..\..\..\libs\models\ADR_Models\SeqOfKeyPosesModels\keyPoseModel.bin";
        //public const string xmlDocPath = @"..\..\..\..\libs\models\ADR_Models\SeqOfKeyPosesModels\xmlDoc.xml";
        //public const string joggAction = @"..\..\..\..\libs\models\ADR_Models\SeqOfKeyPosesModels\joggActionModel.bin";
        //public const string walkAction = @"..\..\..\..\libs\models\ADR_Models\SeqOfKeyPosesModels\walkActionModel.bin";
        //public const string runAction = @"..\..\..\..\libs\models\ADR_Models\SeqOfKeyPosesModels\runActionModel.bin";
        //public const string sideAction = @"..\..\..\..\libs\models\ADR_Models\SeqOfKeyPosesModels\sideActionModel.bin";
        //public const string skipAction = @"..\..\..\..\libs\models\ADR_Models\SeqOfKeyPosesModels\skipActionModel.bin";
        //public const string jumpAction = @"..\..\..\..\libs\models\ADR_Models\SeqOfKeyPosesModels\jumpActionModel.bin";
        //public const string fusionofFeaturesArff = "fusionofFeatures.arff";
        //public const string fusionofFeaturesModel1 = "fusionofFeatures1";
        //public const string fusionofFeaturesModel2 = "fusionofFeatures2";
        //public const string fusionofFeaturesModel3 = "fusionofFeatures3";

    }
}


