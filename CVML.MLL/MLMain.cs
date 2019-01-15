using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using CVML.Constants;
using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using java.io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using weka.classifiers;
using weka.core;
using weka.core.packageManagement;
namespace CVML.MLL
{
    public class distributionsEmotions
    {
        static public List<double> dists;
    };
    public class MLMain
    {
        private MLMain()
        {

        }
        private static MLMain instance;
        public static MLMain getInstance()
        {
            if (instance == null)
                instance = new MLMain();

            return instance;
        }
        private IKernel getKernel()
        {
           return new HistogramIntersection(1, 1);
        }

        Matrix<int> layerSize;

        private void ActivationFunctionHardFix(ANN_MLP network)
        {
            string tmpFile = "tmp.xml";
            network.Save(tmpFile); // Save current ANN network weights values 
            StreamReader reader = new StreamReader(tmpFile);
            string configContent = reader.ReadToEnd();
            reader.Close();

            configContent = configContent.Replace("<min_val>0.", "<min_val>0"); // declaration of min max values 0..1
            configContent = configContent.Replace("<max_val>0.", "<max_val>1");
            configContent = configContent.Replace("<min_val1>0.", "<min_val1>0");
            configContent = configContent.Replace("<max_val1>0.", "<max_val1>1");

            StreamWriter writer = new StreamWriter(tmpFile, false);
            writer.Write(configContent);
            writer.Close();
        }
        public bool trainingMLP(Matrix<float> inputData, Matrix<float> outputData,string modelName,int iteration= 1000,double learningRate= 0.01, int hiddenLayers=2, ANN_MLP.AnnMlpActivationFunction activationType= ANN_MLP.AnnMlpActivationFunction.SigmoidSym, double backpropWeightScale=0.1,double backpropMomentumScale= 0.2)
        {
            try
            {

                layerSize = new Matrix<int>(new int[] { inputData.Cols, hiddenLayers, 1 });// Integer vector specifying the number of neurons in each layer including the input and output layers. The very first element specifies the number of elements in the input layer. The last element - number of elements in the output layer.

                IInputArray sample_in = inputData;
                IInputArray response = outputData;



                //===========================================================
                using (ANN_MLP network = new ANN_MLP())
                {
                    network.SetActivationFunction(activationType);
                    network.SetLayerSizes(layerSize);
                    network.TermCriteria = new MCvTermCriteria(iteration, learningRate); // Number of Iteration for training
                    network.SetTrainMethod(ANN_MLP.AnnMlpTrainMethod.Backprop);
                    network.BackpropWeightScale = backpropWeightScale;
                    network.BackpropMomentumScale = backpropMomentumScale;

                    //network.Save("tmp.xml"); // Save temp weights to file for correction before training
                    
                    ActivationFunctionHardFix(network); // Fix min max values
                    network.Read(new FileStorage("tmp.xml", FileStorage.Mode.Read).GetFirstTopLevelNode()); // Read Fixed values for training
                    TrainData training = new TrainData(sample_in, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, response); // Creating training data               

                    network.Train(training); // Start Training
                    network.Save(modelName+ ".xml");

                }
                return true;
            }
            catch (Exception ee)
            {
                return false;
            }



        }
        public Matrix<float> testingMLP(Matrix<float> testData,  string modelName,int hiddenLayers = 2, ANN_MLP.AnnMlpActivationFunction activationType = ANN_MLP.AnnMlpActivationFunction.SigmoidSym)
        {
            Matrix<float> finalResult = null;
            layerSize = new Matrix<int>(new int[] { testData.Cols, hiddenLayers, 1 });
            try
            {
                using (ANN_MLP network1 = new ANN_MLP()) // Testing trainned Data
                {
                    network1.SetActivationFunction(activationType);
                    network1.SetLayerSizes(layerSize);

                    network1.Read(new FileStorage(modelName + ".xml", FileStorage.Mode.Read).GetFirstTopLevelNode()); // Load trainned ANN weights     
                    
                    IInputArray Sample_test = testData;
                    IOutputArray Result = new Matrix<float>(1, 1);

                    network1.Predict(Sample_test, Result); //Start Network prediction

                    finalResult = (Matrix<float>)Result;
                    return finalResult;
                }


            }
            catch (Exception ee)
            {

                return finalResult;
            }
        }

        const int percentSplit = 100;
        public void trainMachineForEmotionUsingWeka(string wekaFile, string modelName,int hiddelLayers=7,double learningRate= 0.03,double momentum= 0.4,int decimalPlaces= 2,int trainingTime= 1000)
        {
            //"C:\\Users\\Gulraiz\\Desktop\\Genereted2.arff" "MLP"
            try
            {
                weka.core.Instances insts = new weka.core.Instances(new java.io.FileReader(wekaFile));
                insts.setClassIndex(insts.numAttributes() - 1);
                weka.classifiers.functions.MultilayerPerceptron cl;
                cl = new weka.classifiers.functions.MultilayerPerceptron();
                cl.setHiddenLayers(hiddelLayers.ToString());
                cl.setLearningRate(learningRate);
                cl.setMomentum(momentum);
                cl.setNumDecimalPlaces(decimalPlaces);
                cl.setTrainingTime(trainingTime);

                System.Console.WriteLine("Performing " + percentSplit + "% split evaluation.");

                //randomize the order of the instances in the dataset.
                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);
                java.io.File path = new java.io.File("/models/");
                cl.buildClassifier(train);
                saveModel(cl, modelName, path);
                #region test whole set
                //int numCorrect = 0;
                //for (int i = trainSize; i < insts.numInstances(); i++)
                //{
                //    weka.core.Instance currentInst = insts.instance(i);
                //    double predictedClass = cl.classifyInstance(currentInst);
                //    if (predictedClass == insts.instance(i).classValue())
                //        numCorrect++;
                //}

                //System.Console.WriteLine(numCorrect + " out of " + testSize + " correct (" +
                //           (double)((double)numCorrect / (double)testSize * 100.0) + "%)");
                #endregion 

            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }
        }
        public string testEmotionUsingWeka(string[] attributeArray, string[] classNames, double[] dataValues, string classHeader, string defaultclass, string modelName, int hiddelLayers = 7, double learningRate = 0.03, double momentum = 0.4, int decimalPlaces = 2, int trainingTime = 1000)
        {
            List<double> predictions=testMLPUsingWeka(attributeArray, classNames, dataValues, classHeader, defaultclass, modelName, hiddelLayers, learningRate, momentum, decimalPlaces, trainingTime);
            java.util.ArrayList classLabel = new java.util.ArrayList();
            foreach (string className in classNames)
            {
                classLabel.Add(className);
            }
            string classValueString = classLabel.get(Int32.Parse(predictions.ElementAt(0).ToString())).ToString();
            predictions.RemoveAt(0);
            distributionsEmotions.dists = new List<double>();
            distributionsEmotions.dists = predictions;
            return classValueString;
        }
        public List<double> testMLPPredictionsUsingWeka(string[] attributeArray, string[] classNames, double[] dataValues, string classHeader, string defaultclass, string modelName, int hiddelLayers = 7, double learningRate = 0.03, double momentum = 0.4, int decimalPlaces = 2, int trainingTime = 1000)
        {
            List<double> predictions = testMLPUsingWeka(attributeArray, classNames, dataValues, classHeader, defaultclass, modelName, hiddelLayers, learningRate, momentum, decimalPlaces, trainingTime);
            predictions.RemoveAt(0);
            return predictions;
        }
        weka.classifiers.functions.SMO cl;
        List<float> array = new List<float>();
        public void trainSMOUsingWeka(string wekaFile, string modelName)
        {
            try
            {
                weka.core.converters.CSVLoader csvLoader = new weka.core.converters.CSVLoader();
                csvLoader.setSource(new java.io.File(wekaFile));
                weka.core.Instances insts = csvLoader.getDataSet();
                //weka.core.Instances insts = new weka.core.Instances(new java.io.FileReader(wekaFile));
                insts.setClassIndex(insts.numAttributes() - 1);
              
                cl = new weka.classifiers.functions.SMO();
                cl.setBatchSize("100");

                cl.setCalibrator(new weka.classifiers.functions.Logistic());
                cl.setKernel(new weka.classifiers.functions.supportVector.PolyKernel());
                cl.setEpsilon(1.02E-12);
                cl.setC(1.0);
                cl.setDebug(false);
                cl.setChecksTurnedOff(false);
                cl.setFilterType(new SelectedTag(weka.classifiers.functions.SMO.FILTER_NORMALIZE, weka.classifiers.functions.SMO.TAGS_FILTER));

                System.Console.WriteLine("Performing " + percentSplit + "% split evaluation.");

                //randomize the order of the instances in the dataset.
               // weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                //myRandom.setInputFormat(insts);
               // insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);
                java.io.File path = new java.io.File("/models/");
                cl.buildClassifier(train);
                saveModel(cl, modelName, path);
                #region test whole set
                int numCorrect = 0;
                for (int i = 0; i < insts.numInstances(); i++)
                {
                    weka.core.Instance currentInst = insts.instance(i);
                    if (i == 12)
                    {
                        array = new List<float>();
                        foreach (float value in currentInst.toDoubleArray())
                        {
                            array.Add(value);
                        }
                    }
                   
                    double predictedClass = cl.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }

                System.Console.WriteLine(numCorrect + " out of " + testSize + " correct (" +
                           (double)((double)numCorrect / (double)testSize * 100.0) + "%)");
                #endregion 

            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }
        }
        public List<double> testSMOUsingWeka(string[] attributeArray, string[] classNames, float[] dataValues, string classHeader, string defaultclass, string modelName, int hiddelLayers = 7, double learningRate = 0.03, double momentum = 0.4, int decimalPlaces = 2, int trainingTime = 1000)
        {

            java.util.ArrayList classLabel = new java.util.ArrayList();
            foreach (string className in classNames)
            {
                classLabel.Add(className);
            }
            weka.core.Attribute classHeaderName = new weka.core.Attribute(classHeader, classLabel);

            java.util.ArrayList attributeList = new java.util.ArrayList();
            foreach (string attribute in attributeArray)
            {
                weka.core.Attribute newAttribute = new weka.core.Attribute(attribute);
                attributeList.Add(newAttribute);
            }
            attributeList.add(classHeaderName);
            weka.core.Instances data = new weka.core.Instances("TestInstances", attributeList, 0);
            
            data.setClassIndex(data.numAttributes() - 1);
            // Set instance's values for the attributes 
            weka.core.Instance inst_co = new DenseInstance(data.numAttributes());
            for (int i = 0; i < data.numAttributes() - 1; i++)
            {
                inst_co.setValue(i, Math.Round(dataValues.ElementAt(i),5));
            }

            inst_co.setValue(classHeaderName, defaultclass);
            data.add(inst_co);
            weka.core.Instance currentInst = data.get(0);
            int j = 0;
            //foreach (float value in dataValues)
            //{
            //    // double roundedValue = Math.Round(value);
            //    //var rounded = Math.Floor(value * 100) / 100;
            //    if (array.ElementAt(j) != value)
            //    {
            //        System.Console.WriteLine("Masla occur");
            //    }
            //    j++;
            //}
            //double predictedClass = cl.classifyInstance(data.get(0));

            weka.classifiers.functions.SMO clRead = new weka.classifiers.functions.SMO();
            try
            {
                java.io.File path = new java.io.File("/models/");
                clRead = loadSMOModel(modelName, path);
                
            }
            catch (Exception e)
            {
                //string p1 = Assembly.GetExecutingAssembly().Location;
                string ClassifierName = Path.GetFileName(Path.GetFileName(modelName));
                string Path1 = HostingEnvironment.MapPath(@"~//libs//models//"+ ClassifierName);
                //string Path1 = HostingEnvironment.MapPath(@"~//libs//models//FusionCustomized.model");
                clRead = (weka.classifiers.functions.SMO)weka.core.SerializationHelper.read(Path1);
            }
            // weka.classifiers.functions.SMO clRead = loadSMOModel(modelName, path);
            clRead.setBatchSize("100");
            
            clRead.setCalibrator(new weka.classifiers.functions.Logistic());
            clRead.setKernel(new weka.classifiers.functions.supportVector.PolyKernel());
            clRead.setEpsilon(1.02E-12);
            clRead.setC(1.0);
            clRead.setDebug(false);
            clRead.setChecksTurnedOff(false);
            clRead.setFilterType(new SelectedTag(weka.classifiers.functions.SMO.FILTER_NORMALIZE, weka.classifiers.functions.SMO.TAGS_FILTER));
            
            double classValue = clRead.classifyInstance(data.get(0));
            double[] predictionDistribution = clRead.distributionForInstance(data.get(0));
            //for (int predictionDistributionIndex = 0;
            //  predictionDistributionIndex < predictionDistribution.Count();
            //  predictionDistributionIndex++)
            //{
            //    string classValueString1 = classLabel.get(predictionDistributionIndex).ToString();
            //    double prob= predictionDistribution[predictionDistributionIndex]*100;
            //    System.Console.WriteLine(classValueString1 + ":" + prob);
            //}
            List<double> prediction = new List<double>();
            prediction.Add(classValue);
            //prediction.AddRange(predictionDistribution);
            return prediction;
        }
        public List<double> testSMOUsingWeka(string[] attributeArray, string[] classNames, double[] dataValues, string classHeader, string defaultclass, string modelName, int hiddelLayers = 7, double learningRate = 0.03, double momentum = 0.4, int decimalPlaces = 2, int trainingTime = 1000)
        {

            java.util.ArrayList classLabel = new java.util.ArrayList();

            foreach (string className in classNames)
            {
                classLabel.Add(className);
            }
            weka.core.Attribute classHeaderName = new weka.core.Attribute(classHeader, classLabel);

            java.util.ArrayList attributeList = new java.util.ArrayList();
            foreach (string attribute in attributeArray)
            {
                weka.core.Attribute newAttribute = new weka.core.Attribute(attribute);
                attributeList.Add(newAttribute);
            }
            attributeList.add(classHeaderName);
            weka.core.Instances data = new weka.core.Instances("TestInstances", attributeList, 0);

            data.setClassIndex(data.numAttributes() - 1);
            // Set instance's values for the attributes 
            weka.core.Instance inst_co = new DenseInstance(data.numAttributes());
            for (int i = 0; i < data.numAttributes() - 1; i++)
            {
                inst_co.setValue(i, Math.Round(dataValues.ElementAt(i), 5));
            }

            inst_co.setValue(classHeaderName, defaultclass);
            data.add(inst_co);
            weka.core.Instance currentInst = data.get(0);
            int j = 0;
            //foreach (float value in dataValues)
            //{
            //    // double roundedValue = Math.Round(value);
            //    //var rounded = Math.Floor(value * 100) / 100;
            //    if (array.ElementAt(j) != value)
            //    {
            //        System.Console.WriteLine("Masla occur");
            //    }
            //    j++;
            //}
            //double predictedClass = cl.classifyInstance(data.get(0));

            weka.classifiers.functions.SMO clRead = new weka.classifiers.functions.SMO();
            try
            {
                java.io.File path = new java.io.File("/models/");
                clRead = loadSMOModel(modelName, path);

            }
            catch (Exception e)
            {
                //string p1 = Assembly.GetExecutingAssembly().Location;
                string ClassifierName = Path.GetFileName(Path.GetFileName(modelName));
                string Path1 = HostingEnvironment.MapPath(@"~//libs//models//" + ClassifierName);
                //string Path1 = HostingEnvironment.MapPath(@"~//libs//models//FusionCustomized.model");
                clRead = (weka.classifiers.functions.SMO)weka.core.SerializationHelper.read(modelName);
            }
            // weka.classifiers.functions.SMO clRead = loadSMOModel(modelName, path);
            clRead.setBatchSize("100");

            clRead.setCalibrator(new weka.classifiers.functions.Logistic());
            clRead.setKernel(new weka.classifiers.functions.supportVector.PolyKernel());
            clRead.setEpsilon(1.02E-12);
            clRead.setC(1.0);
            clRead.setDebug(false);
            clRead.setChecksTurnedOff(false);
            clRead.setFilterType(new SelectedTag(weka.classifiers.functions.SMO.FILTER_NORMALIZE, weka.classifiers.functions.SMO.TAGS_FILTER));

            double classValue = clRead.classifyInstance(data.get(0));
            double[] predictionDistribution = clRead.distributionForInstance(data.get(0));
            //for (int predictionDistributionIndex = 0;
            //  predictionDistributionIndex < predictionDistribution.Count();
            //  predictionDistributionIndex++)
            //{
            //    string classValueString1 = classLabel.get(predictionDistributionIndex).ToString();
            //    double prob= predictionDistribution[predictionDistributionIndex]*100;
            //    System.Console.WriteLine(classValueString1 + ":" + prob);
            //}
            List<double> prediction = new List<double>();
            prediction.Add(classValue);
            //prediction.AddRange(predictionDistribution);
            return prediction;
        }

        public List<double> testMLPUsingWeka(string[] attributeArray, string[] classNames, double[] dataValues, string classHeader, string defaultclass, string modelName, int hiddelLayers = 7, double learningRate = 0.03, double momentum = 0.4, int decimalPlaces = 2, int trainingTime = 1000)
        {

            java.util.ArrayList classLabel = new java.util.ArrayList();
            foreach (string className in classNames)
            {
                classLabel.Add(className);
            }
            weka.core.Attribute classHeaderName = new weka.core.Attribute(classHeader, classLabel);

            java.util.ArrayList attributeList = new java.util.ArrayList();
            foreach (string attribute in attributeArray)
            {
                weka.core.Attribute newAttribute= new weka.core.Attribute(attribute);
                attributeList.Add(newAttribute);
            }
            attributeList.add(classHeaderName);
            weka.core.Instances data = new weka.core.Instances("TestInstances", attributeList, 0);
            data.setClassIndex(data.numAttributes() - 1);
            // Set instance's values for the attributes 
            weka.core.Instance inst_co = new DenseInstance(data.numAttributes());
            for(int i=0;i< data.numAttributes()-1; i++)
            {
                inst_co.setValue(i, dataValues.ElementAt(i));
            }

            inst_co.setValue(classHeaderName, defaultclass);
            data.add(inst_co);
            
            java.io.File path = new java.io.File("/models/");
            weka.classifiers.functions.MultilayerPerceptron clRead = loadModel(modelName, path);
            clRead.setHiddenLayers(hiddelLayers.ToString());
            clRead.setLearningRate(learningRate);
            clRead.setMomentum(momentum);
            clRead.setNumDecimalPlaces(decimalPlaces);
            clRead.setTrainingTime(trainingTime);
            weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
            myRandom.setInputFormat(data);
            data = weka.filters.Filter.useFilter(data, myRandom);
            double classValue = clRead.classifyInstance(data.get(0));
            double[] predictionDistribution = clRead.distributionForInstance(data.get(0));
            List<double> predictionDistributions = new List<double>();
            for (int predictionDistributionIndex = 0;
              predictionDistributionIndex < predictionDistribution.Count();
              predictionDistributionIndex++)
            {
                string classValueString1 = classLabel.get(predictionDistributionIndex).ToString();
                double prob = predictionDistribution[predictionDistributionIndex] * 100;
                predictionDistributions.Add(prob);
            }
            List<double> prediction = new List<double>();
            prediction.Add(classValue);
            prediction.AddRange(predictionDistributions);
            return prediction;
        }

        private static weka.classifiers.functions.SMO loadSMOModel(String name, java.io.File path)
        {

            // weka.classifiers.bayes.HMM .HMMEstimator.packageManagement.Package p=new ;
            weka.classifiers.functions.SMO classifier;
            FileInputStream fis = new FileInputStream(name);
            ObjectInputStream ois = new ObjectInputStream(fis);
            classifier = (weka.classifiers.functions.SMO)ois.readObject();
            ois.close();

            return classifier;
        }

        private static weka.classifiers.functions.MultilayerPerceptron loadModel(String name, java.io.File path)
        {

           // weka.classifiers.bayes.HMM .HMMEstimator.packageManagement.Package p=new ;
            weka.classifiers.functions.MultilayerPerceptron classifier;
            FileInputStream fis = new FileInputStream((string)ConstantsLoader.getInstance().getValue(EnumConstant.libsPath)+"\\models\\" + "models"+ name + ".model");
            ObjectInputStream ois = new ObjectInputStream(fis);
            classifier = (weka.classifiers.functions.MultilayerPerceptron)ois.readObject();
            ois.close();

            return classifier;
        }

        private static void saveModel(Classifier c, String name, java.io.File fileName)
        {


            ObjectOutputStream oos = null;
            try
            {
                oos = new ObjectOutputStream(
                        new FileOutputStream("..\\..\\..\\..\\libs\\models\\" + "models" + name + ".model"));

            }
            catch (java.io.FileNotFoundException e1)
            {
                e1.printStackTrace();
            }
            catch (java.io.IOException e1)
            {
                e1.printStackTrace();
            }
            oos.writeObject(c);
            oos.flush();
            oos.close();

        }

        /// <summary>
        /// Creates .arff file which can be used in Weka Classifier testing
        /// </summary>
        /// <param name="comments"> Comments to be added on top of .arff file</param>
        /// <param name="relation"> relation in .arff file </param>
        /// <param name="attributes">Attributes with which data can be columized</param>
        /// <param name="features">Feature set related to attributes</param>
        /// <param name="destinationAddress">Destination address alongwith file name of .arff file</param>
        /// 
        public void createWekaFile(string comments, string relation, List<string> attributes, List<string> features, string destinationAddress)
        {
            StreamWriter sw = new StreamWriter(destinationAddress);

            string fullDataToWrite = string.Empty;

            string allAttributes = string.Empty;

            if (attributes == null || attributes.Count == 0)
            {
                //for (int i = 0; i < attributes.Length; i++)
                //{
                //    allAttributes = allAttributes + "@attribute" + "" + Environment.NewLine;
                //}
            }
            else
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    allAttributes = allAttributes + "@attribute " + attributes[i] + System.Environment.NewLine;
                }
            }

            string allFeatures = string.Empty;
            for (int i = 0; i < features.Count; i++)
            {
                 allFeatures = i== features.Count-1? allFeatures + features[i] : allFeatures + features[i] + System.Environment.NewLine;
            }

            string dataStr = comments + System.Environment.NewLine + System.Environment.NewLine;
            if (comments == string.Empty || string.IsNullOrEmpty(comments))
            {
                dataStr = ""; //removing extra lines from top of file due to null comments
            }

            dataStr = dataStr + "@relation " + relation + "" + System.Environment.NewLine;
            dataStr = dataStr + allAttributes + System.Environment.NewLine;
            dataStr = dataStr + "@data" + System.Environment.NewLine;
            dataStr = dataStr + allFeatures;

            sw.Write(dataStr);
            sw.Flush();
            sw.Close();

        }
        public bool trainingNaiveBayes(Matrix<float> inputData, Matrix<int> outputData, string modelName)
        {
            try
            {
                using (NormalBayesClassifier classifier = new NormalBayesClassifier())
                {
                    TrainData training = new TrainData(inputData, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, outputData); // Creating training data               
                    classifier.Train(training);
                    String fileName =modelName+".xml";
                    classifier.Save(fileName);
                }
                return true;
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
        public float testingNaiveBayes(Matrix<float> testData, string modelName)
        {
            float response= 0;
            //response = null;
            try
            {
                using (NormalBayesClassifier classifier = new NormalBayesClassifier()) // Testing trainned Data
                {
                    classifier.Read(new FileStorage(modelName + ".xml", FileStorage.Mode.Read).GetFirstTopLevelNode());
                    response= classifier.Predict(testData);
                    return response;
                }
            }
            catch (Exception ee)
            {

                return response;
            }
        }
        /// <summary>
        /// Trains SVM using Bio inspired features.
        /// </summary>
        /// <param name="trainData"> Feature vectors for training. </param>
        /// <param name="trainClasses"> Class vector of instances for training. </param>
        /// <param name="saveModelName"> saved Model name using  </param>
        /// <param name="svm_type"> Svm type. </param>
        /// <param name="C"> Cost. </param>
        /// <param name="coef0"> Coeff. </param>
        /// <param name="degree"> Degree. </param>
        /// <param name="eps"> Eps for termination criteria. </param>
        /// <param name="gamma"> Gamma. </param>
        /// <param name="kernel_type"> Kernel Type of svm. </param>
        /// <param name="nu"> Nu. </param>
        /// <param name="maxIter"> Maximun number of iterations. </param>
        /// <param name="termCritType"> Termiantion Criteria Type. </param>
        /// <returns></returns>
        public bool TrainSVM(Matrix<float> trainData,Matrix<int> trainClasses, string saveModelName, SVM.SvmType svm_type = SVM.SvmType.CSvc, int kFold = 10, double C = 1.0, double coef0 = 0.1, int degree = 3, double eps = 0.001, double gamma = 1.0, SVM.SvmKernelType kernel_type = SVM.SvmKernelType.Rbf, double nu = 0.5, int maxIter = 500, Emgu.CV.CvEnum.TermCritType termCritType = Emgu.CV.CvEnum.TermCritType.Eps)
        {
            var svmModel = new SVM();
            var termCriteria = new Emgu.CV.Structure.MCvTermCriteria();
            var trainSampleCount = trainData.Rows;
            svmModel.C = C;
            svmModel.Coef0 = coef0;
            svmModel.Degree = degree;
            svmModel.Gamma = gamma;
            svmModel.Nu = nu;
            svmModel.Type = svm_type;
            termCriteria.Epsilon = eps;
            termCriteria.MaxIter = maxIter;
            termCriteria.Type = termCritType;
            svmModel.TermCriteria = termCriteria;
            svmModel.P = 1;
            kernel_type = SVM.SvmKernelType.Linear; //for training of CNN feature data
            svmModel.SetKernel(kernel_type);
            bool trained;
            try
            {
                    using (svmModel)
                    {
                        TrainData td = new TrainData(trainData, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample,trainClasses);
                        trained = svmModel.TrainAuto(td,kFold);
                        svmModel.Save(saveModelName);
                       
                    }

                }
            catch(Exception esvm)
            {
                throw esvm;
            }
          return trained;

            
        }

        private static weka.classifiers.meta.Bagging loadBaggingModel(String name, java.io.File path)
        {

            weka.classifiers.meta.Bagging classifier;

            FileInputStream fis = new FileInputStream("..\\..\\..\\..\\libs\\models\\" + "models" + name + ".model");
            ObjectInputStream ois = new ObjectInputStream(fis);

            classifier = (weka.classifiers.meta.Bagging)ois.readObject();
            ois.close();

            return classifier;
        }

 public string testHybridEmotionUsingWeka(string[] attributeArray, string[] classNames, double[] dataValues, string classHeader, string defaultclass, string modelName)
        {

            java.util.ArrayList classLabel = new java.util.ArrayList();
            foreach (string className in classNames)
            {
                classLabel.Add(className);
            }
            weka.core.Attribute classHeaderName = new weka.core.Attribute(classHeader, classLabel);

            java.util.ArrayList attributeList = new java.util.ArrayList();
            foreach (string attribute in attributeArray)
            {
                weka.core.Attribute newAttribute = new weka.core.Attribute(attribute);
                attributeList.Add(newAttribute);
            }
            attributeList.add(classHeaderName);
            weka.core.Instances data = new weka.core.Instances("TestInstances", attributeList, 0);
            data.setClassIndex(data.numAttributes() - 1);
            // Set instance's values for the attributes 
            weka.core.Instance inst_co = new DenseInstance(data.numAttributes());
            for (int i = 0; i < data.numAttributes() - 1; i++)
            {
                inst_co.setValue(i, dataValues.ElementAt(i));
            }

            inst_co.setValue(classHeaderName, defaultclass);
            data.add(inst_co);

            java.io.File path = new java.io.File("/models/");
            weka.classifiers.meta.Bagging clRead = loadBaggingModel(modelName, path);
            weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
            myRandom.setInputFormat(data);
            data = weka.filters.Filter.useFilter(data, myRandom);
            double classValue = clRead.classifyInstance(data.get(0));
            string classValueString = classLabel.get(Int32.Parse(classValue.ToString())).ToString();
            return classValueString;
        }
        

 public void trainMachineForHybridUsingWeka(string wekaFile, string modelName)
        {
            weka.core.Instances insts = new weka.core.Instances(new java.io.FileReader(wekaFile));
            insts.setClassIndex(insts.numAttributes() - 1);
            weka.classifiers.Classifier bagging = new weka.classifiers.meta.Bagging();

            System.Console.WriteLine("Performing " + percentSplit + "% split evaluation.");

            //randomize the order of the instances in the dataset.
            weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
            myRandom.setInputFormat(insts);
            insts = weka.filters.Filter.useFilter(insts, myRandom);

            int trainSize = insts.numInstances() * percentSplit / 100;
            int testSize = insts.numInstances() - trainSize;
            weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);
            java.io.File path = new java.io.File("/models/");
            bagging.buildClassifier(train);
            saveModel(bagging, modelName, path);

        }
        #region testModel
        /// <summary>
        /// Predicts the age of the person.
        /// </summary>
        /// <param name="ageML"> Feature vector for predicting age. </param>
        /// <param name="trainedModelName"> Full name of svm model. </param>
        /// <returns> the age of the person as baby, child, adult or old. </returns>
        public float TestSVM(Matrix<float> testInstance, string trainedModelName)
        {
            float result;
            var svmModel = new SVM();
            var featureCount = testInstance.Rows;
            var savedModel = new FileStorage(trainedModelName, FileStorage.Mode.Read);
            svmModel.Read(savedModel.GetFirstTopLevelNode());
            result = svmModel.Predict(testInstance);
            return result;
        }
        #endregion
        public bool accordTrainingSVM(double[][] inputs, int[] outputs,string modelName)
        {

            int classes = outputs.Distinct().Count();

            var kernel = getKernel();
            try
            {
                // Create the Multi-class Support Vector Machine using the selected Kernel
                MulticlassSupportVectorMachine ksvm = new MulticlassSupportVectorMachine(inputs[0].Length, kernel, classes);

                // Create the learning algorithm using the machine and the training data
                MulticlassSupportVectorLearning ml = new MulticlassSupportVectorLearning(ksvm, inputs, outputs);

                //// Extract training parameters from the interface
                // complexity;// = (double)numComplexity.Value;
                // double tolerance; //= (double)numTolerance.Value;
                // cacheSize; //= (int)numCache.Value;
                //SelectionStrategy strategy; // cbStrategy.SelectedItem;
                
                double complexity = 100.00000;
                double tolerance = 0.001;
                int cacheSize = 500;
                SelectionStrategy strategy = (SelectionStrategy.Sequential);
                // Configure the learning algorithm

                ml.Algorithm = (svm, classInputs, classOutputs, i, j) =>
                {
                    return new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                    {
                        //complexity,
                        //tolerance ,
                        //cacheSize,
                        //strategy ,
                    };
                };
               // double error = ml.Run();
                ksvm.Save(modelName);
                return true;
            }
            catch(Exception eeAccord)
            {
                return false;
            }      
        }
        public int accordTestingSVM(double[] features,string modelName)
        {
            try
            {
                MulticlassSupportVectorMachine ksvm1 = MulticlassSupportVectorMachine.Load(modelName);
                int actuallabel = ksvm1.Decide(features);
                return actuallabel;
            }
            catch(Exception eeTestAccord )
            {
                throw eeTestAccord;
            }
        }
        public void MultinomialLogisticRegressionAnalysisTraining (double[][] input,int [] output,string modelName)
        {
            MultinomialLogisticRegressionAnalysis maxEnt = new MultinomialLogisticRegressionAnalysis();
            maxEnt.Learn(input, output);
            

        }
        public double MultinomialLogisticRegressionAnalysisTesting (double[] input)
        {
            double prediction = 0;
            MultinomialLogisticRegressionAnalysis maxEnt =   new MultinomialLogisticRegressionAnalysis();
           // maxEnt.Transform()
            return prediction;
        }
    }
}
