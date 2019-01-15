using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Emotion detection IP layer namespace
/// </summary>
namespace ASSVS.EDR
{
    /// <summary>
    /// Image model that contain image and model
    /// </summary>
    public class imageModel
    {
        /// <summary>
        /// Image of single step
        /// </summary>
        public Mat image;
        /// <summary>
        /// label of coresspoinding image
        /// </summary>
        public string label;
    }
    /// <summary>
    /// Business model that contain final output result
    /// </summary>
    public class EmotionBusinessModel
    {
            /// <summary>
            /// List of images 
            /// </summary>
            public List<imageModel> images=new List<imageModel>(); 
            /// <summary>
            /// Final output result 
            /// </summary>
            public string outputResult;
            /// <summary>
            /// Output message for success or failure
            /// </summary>
            public string outputMessage;
        public List<Rectangle> faceRect;
        static public List<string> outputmsgs;
        /// <summary>
        /// Variable to show success or failure
        /// </summary>
            public bool success;
        /// <summary>
        /// Exception messgae in case of failure
        /// </summary>
            public Exception e;
        /// <summary>
        /// Constructor to generate output business model
        /// </summary>
        /// <param name="outputResult">Final output result </param>
        /// <param name="outputMessage">Output message for success or failure</param>
        /// <param name="success">Variable to show success or failure</param>
        /// <param name="e">Exception messgae in case of failure</param>
        public EmotionBusinessModel(string outputResult, string outputMessage, bool success, Exception e)
            {
            this.outputResult= outputResult;
            this.outputMessage= outputMessage;

            this.success= success;
            this.e= e;
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public EmotionBusinessModel() { }
    }
    /// <summary>
    /// Model of features
    /// </summary>
    public class EmotionTrainingDataModel : System.Attribute
    {
        /// <summary>
        /// Distance from first point of left eyebrow to first point of eye
        /// </summary>
        public double LEB1_CPLE1 { get; set; }
        /// <summary>
        /// Distance from second point of left eyebrow to second point of eye
        /// </summary>
        public double LEB2_CPLE2 { get; set; }
        /// <summary>
        /// Distance from first point of right eyebrow to first point of eye
        /// </summary>
        public double REB1_CPRE1 { get; set; }
        /// <summary>
        /// Distance from second point of right eyebrow to second point of eye
        /// </summary>
        public double REB2_CPRE2 { get; set; }
        /// <summary>
        /// Openning of left eye WRT first point
        /// </summary>
        public double OPEN_LE1 { get; set; }
        /// <summary>
        /// Openning of left eye WRT second point
        /// </summary>
        public double OPEN_LE2 { get; set; }
        /// <summary>
        /// Openning of right eye WRT first point
        /// </summary>
        public double OPEN_RE1 { get; set; }
        /// <summary>
        /// Openning of right eye WRT second point
        /// </summary>
        public double OPEN_RE2 { get; set; }
        /// <summary>
        /// Openning of mouth WRT first point
        /// </summary>
        public double OPEN_MO1 { get; set; }
        /// <summary>
        /// Openning of mouth WRT second point
        /// </summary>
        public double OPEN_MO2 { get; set; }
        /// <summary>
        /// Openning of mouth WRT third point
        /// </summary>
        public double OPEN_MO3 { get; set; }
        /// <summary>
        /// Expansion of mouth 
        /// </summary>
        public double EXP_MO { get; set; }
        /// <summary>
        /// Distance from nose middle lower point to left point of lip corner
        /// </summary>
        public double NS_LPLIP { get; set; }
        /// <summary>
        /// Distance from nose middle lower point to right point of lip corner
        /// </summary>
        public double NS_RPLIP { get; set; }
        /// <summary>
        /// Wrinkles on nose
        /// </summary>
        public double noseWrinkles { get; set; }
        /// <summary>
        /// Wrinkles between eyebrows
        /// </summary>
        public double betweenEyesWrinkles { get; set; }
        /// <summary>
        /// Label of facial expression
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Constructor for generating features model
        /// </summary>
        /// <param name="LEB1_CPLE1">Distance from first point of left eyebrow to first point of eye</param>
        /// <param name="LEB2_CPLE2">Distance from second point of left eyebrow to second point of eye</param>
        /// <param name="REB1_CPRE1">Distance from first point of right eyebrow to first point of eye</param>
        /// <param name="REB2_CPRE2">Distance from second point of right eyebrow to second point of eye</param>
        /// <param name="OPEN_LE1">Openning of left eye WRT first point</param>
        /// <param name="OPEN_LE2">Openning of left eye WRT second point</param>
        /// <param name="OPEN_RE1">Openning of right eye WRT first point</param>
        /// <param name="OPEN_RE2">Openning of right eye WRT second point</param>
        /// <param name="OPEN_MO1">Openning of mouth WRT first point</param>
        /// <param name="OPEN_MO2">Openning of mouth WRT second point</param>
        /// <param name="OPEN_MO3">Openning of mouth WRT third point</param>
        /// <param name="EXP_MO">Expansion of mouth</param>
        /// <param name="NS_LPLIP">Distance from nose middle lower point to left point of lip corner</param>
        /// <param name="NS_RPLIP">Distance from nose middle lower point to right point of lip corner</param>
        /// <param name="noseWrinkles">Wrinkles on nose</param>
        /// <param name="betweenEyesWrinkles">Wrinkles between eyebrows</param>
        /// <param name="Label">Label of facial expression</param>
        public EmotionTrainingDataModel(double LEB1_CPLE1, double LEB2_CPLE2, double REB1_CPRE1, double REB2_CPRE2, double OPEN_LE1, double OPEN_LE2, double OPEN_RE1, double OPEN_RE2, double OPEN_MO1, double OPEN_MO2, double OPEN_MO3, double EXP_MO,double NS_LPLIP,double NS_RPLIP, double noseWrinkles, double betweenEyesWrinkles, string Label)
        {
            this.LEB1_CPLE1 = LEB1_CPLE1;
            this.LEB2_CPLE2 = LEB2_CPLE2;
            this.REB1_CPRE1 = REB1_CPRE1;
            this.REB2_CPRE2 = REB2_CPRE2;
            this.OPEN_LE1 = OPEN_LE1;
            this.OPEN_LE2 = OPEN_LE2;
            this.OPEN_RE1 = OPEN_RE1;
            this.OPEN_RE2 = OPEN_RE2;
            this.OPEN_MO1 = OPEN_MO1;
            this.OPEN_MO2 = OPEN_MO2;
            this.OPEN_MO3 = OPEN_MO3;
            this.EXP_MO = EXP_MO;
            this.NS_LPLIP = NS_LPLIP;
            this.NS_RPLIP = NS_RPLIP;
            this.noseWrinkles = noseWrinkles;
            this.betweenEyesWrinkles = betweenEyesWrinkles;
            this.Label = Label;

        }
        /// <summary>
        /// Default constructor for EmotionTrainingDataModel 
        /// </summary>
        public EmotionTrainingDataModel()
        {
        }
    }
}
