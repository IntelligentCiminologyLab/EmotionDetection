using ASSVS.ML.DistanceAndWrinkleBased;
using ASSVS.ML.DistanceBased;
using ASSVS.ML.EDR.Shared;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CVML.Constants;
using CVML.Logger;
using ASSVS.ML.EDR.ReducedDimentionsBased;

namespace ASSVS.EDR
{
    /// <summary>
    /// Emotion detection IP layer namespace
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }
    /// <summary>
    /// Feature extraction class
    /// </summary>
    public class EDRFeatureExtraction
    {
        /// <summary>
        /// Rectangle part on nose
        /// </summary>
        private Rectangle onNose;
        /// <summary>
        /// Rectangle between eyes
        /// </summary>
       private Rectangle betweenEyes;
        /// <summary>
        /// List of all features for wrinkle based technique
        /// </summary>
        private static string[] listOfDistances = { "LEB1_CPLE1", "LEB2_CPLE2", "REB1_CPRE1", "REB2_CPRE2", "NS_LPLIP", "NS_RPLIP", "OPEN_LE1", "OPEN_LE2", "OPEN_RE1", "OPEN_RE2", "EXP_MO", "OPEN_MO1", "OPEN_MO2", "OPEN_MO3", "betweenEyesWrinkles", "noseWrinkles", "Label" };
        /// <summary>
        /// Get facial points by giving an image and face recangle
        /// </summary>
        /// <param name="width">Widht of Mat image</param>
        /// <param name="height">Height of Mat image</param>
        /// <param name="pArray">Pointer to the image on which facial points need to find</param>
        /// <param name="r">Rectangle of face</param>
        [DllImport(StaticConstants.FaceLandmarksDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Get_FaceLandMarksOnFace(int width, int height, IntPtr pArray, Rectangle r);
        /// <summary>
        /// Get x-coordinates of facial points
        /// </summary>
        /// <returns>X-coordinatres of facial landmarks points</returns>
        [DllImport(StaticConstants.FaceLandmarksDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDllXPoints();
        /// <summary>
        /// Get Y-coordinates of facial points
        /// </summary>
        /// <returns>Y-coordinatres of facial landmarks points</returns>
        [DllImport(StaticConstants.FaceLandmarksDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDllYPoints();
        /// <summary>
        /// Release the memory taken by x-coordinates and y-coordinates
        /// </summary>
        /// <param name="ptr">Pointer of the memory</param>
        /// <returns>return success</returns>
        [DllImport(StaticConstants.FaceLandmarksDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReleaseMemory(IntPtr ptr);
        /// <summary>
        /// Get nose rectangle
        /// </summary>
        /// <returns>Returns rectangle of nose</returns>
        public Rectangle getOnNose()
        {
            return onNose;
        }
        /// <summary>
        /// Get rectangle between eye brows
        /// </summary>
        /// <returns>Returns rectangle between eye brows</returns>
        public Rectangle getBetweenEyes()
        {
            return betweenEyes;
        }
        /// <summary>
        /// Find distances between face points
        /// </summary>
        /// <param name="ImageMat">Image on which distances need to calculate</param>
        /// <param name="faces">Rectangles of faces</param>
        /// <param name="index">Index of images in case of multiple images</param>
        /// <returns>Returns the list of distances</returns>
        public List<double>  findDistances (Mat ImageMat, List<Rectangle> faces, int index)
        {
            List<double> distances = new List<double>();
            List<Point> listOfPoints = getPointsViaStraightLines(ImageMat, faces,  index);

                if (listOfPoints.Count == 22)
                {
                distances = findAllDistances(listOfPoints);


             

            }
            else
                {

                }
            
            return distances;
        }
        /// <summary>
        /// Get emotion features using distances and wrinkles
        /// </summary>
        /// <param name="ImageMat">Image on which distances need to calculate</param>
        /// <param name="faces">Rectangles of faces</param>
        /// <param name="index">Index of images in case of multiple images</param>
        /// <returns>Returns the list of distances</returns>
        public EmotionTrainingMLModel FindEmotions( Mat ImageMat, List<Rectangle> faces, int index)
        {
            EmotionTrainingMLModel dataModel=new EmotionTrainingMLModel();
            if (faces.Count > 0)
            {
                Mat shortImage = new Mat(ImageMat, faces[0]);
                CvInvoke.Resize(shortImage, shortImage, new Size(320, 240), 0, 0, Inter.Linear);
            
                faces = new List<Rectangle>();
                faces.Add(new Rectangle(0, 0, shortImage.Width, shortImage.Height));

                List<double> distances = findDistances(shortImage, faces, index);
             
                double noseWrinkles = findWrinkles(shortImage, onNose);
              
                double betweenEyesWrinkles = findWrinkles(shortImage, betweenEyes);
                
                dataModel = new EmotionTrainingMLModel();
                for (int i = 0; i < 14; i++)
                {
                    var value = distances.ElementAt(i);
                    PropertyInfo propertyInfo = dataModel.GetType().GetProperty(listOfDistances[i]);
                    propertyInfo.SetValue(dataModel, value, null);
                }
                dataModel.noseWrinkles = noseWrinkles;
                dataModel.betweenEyesWrinkles = betweenEyesWrinkles;
                dataModel.Label = "";
                //emotions= EDRMLMain.getInstance().getCalculatedEmotions(dataModel);
                
            }
            return dataModel;
        }
        /// <summary>
        /// Find wrinkles on an image with in a recangle
        /// </summary>
        /// <param name="shortImage">Image on which wrinkles need to calculate</param>
        /// <param name="rect">Rectangles with in which wrinkles needs to calculate</param>
        /// <returns>Returns the intensity of wrinkles</returns>
        public double findWrinkles(Mat shortImage, Rectangle rect)
        {
            Image<Gray, byte> procImage = shortImage.ToImage<Gray, byte>();
            procImage.ROI = rect;
      
            float[] GrayHist;
            DenseHistogram Histo = new DenseHistogram(255, new RangeF(0, 255));

            Histo.Calculate(new Image<Gray, byte>[] { procImage }, true, null);

            ////The data is here
            GrayHist = new float[256];
            Histo.CopyTo<float>(GrayHist);
            ////Calculate Threashold on the basis of histogram of Image
            int threashHold = calculate_threashHold(GrayHist);
            procImage = procImage.Canny(threashHold, threashHold);
            return (double)CvInvoke.CountNonZero(procImage);
        }
        /// <summary>
        /// Calculate the threashold for histrogram equalization
        /// </summary>
        /// <param name="hist">intensities of histogram</param>
        /// <returns>Integer value of threashold</returns>
        private int calculate_threashHold(float[] hist)
        {
            List<float> array = new List<float>();
            array.Add(new List<float>(hist).GetRange(50, 50).ToArray().Sum());
            array.Add(new List<float>(hist).GetRange(100, 50).ToArray().Sum());
            array.Add(new List<float>(hist).GetRange(150, 50).ToArray().Sum());
            array.Add(new List<float>(hist).GetRange(200, 50).ToArray().Sum());
            int hist_Value = array.FindIndex(a => a == array.Max());
            int b = (int)(array[0] / array.Sum() * 20 + array[1] / array.Sum() * 50 + array[2] / array.Sum() * 120 + array[3] / array.Sum() * 140);

            int threashHold = hist_Value == 0 ? 20 : hist_Value == 1 ? 50 : hist_Value == 2 ? 120 : hist_Value == 3 ? 140 : 0;
            return b;
        }
        /// <summary>
        /// Get points on face
        /// </summary>
        /// <param name="matImage">Image on which points need to find</param>
        /// <param name="faces">Rectangles of faces</param>
        /// <param name="index">Index of images in case of multiple images</param>
        /// <returns>Returns the list of distances</returns>
        public List<Point> getPointsViaStraightLines(Mat matImage, List<Rectangle> faces,int index)
        {
           
            List<Point> listOfPoints = new List<Point>();
            Image<Bgr, byte> I2 = matImage.ToImage<Bgr, byte>();
            if (faces.Count > 0)
            {

                int size = Marshal.SizeOf(I2.Bytes[0]) * I2.Bytes.Length;
                IntPtr pnt = Marshal.AllocHGlobal(size);
                // Copy the array to unmanaged memory.
                Marshal.Copy(I2.Bytes, 0, pnt, I2.Bytes.Length);
                Get_FaceLandMarksOnFace(matImage.Width, matImage.Height, pnt, faces[0]);
                IntPtr pntx = GetDllXPoints();
                int[] xPoints = new int[68];
                Marshal.Copy(pntx, xPoints, 0, 68);
                IntPtr pnty = GetDllYPoints();
                int[] yPoints = new int[68];
                Marshal.Copy(pnty, yPoints, 0, 68);

                ReleaseMemory(pntx);
                ReleaseMemory(pnty);
                int[] arr = { 19, 20, 23, 24,33,33,37, 38, 40, 41, 43, 44, 46, 47, 48, 50, 51, 52, 54, 56, 57, 58 };
                for (int j = 0; j < arr.Length; j++)
                {
                    Point p = new Point();
                    p.X = xPoints[arr[j]];
                    p.Y = yPoints[arr[j]];
                    listOfPoints.Add(p);
                }
                int[] arr2 = { 54, 48, 33 };
                Point p1 = new Point(xPoints[21]-3, (yPoints[21] - (yPoints[27] - yPoints[21])));
                Point p2 = new Point(xPoints[31], yPoints[27]);
               onNose = new Rectangle(p2, new Size(xPoints[35] - p2.X, yPoints[31] - p2.Y));
               betweenEyes = new Rectangle(p1, new Size(xPoints[22] - p1.X+3, yPoints[27] - p1.Y));

                //CvInvoke.Imshow("Original", I2);

                I2.ROI = faces[0];
            }
            if (faces.Count > 0)
            {
                Emgu.CV.Image<Bgr, Byte> imageBgr = matImage.ToImage<Bgr, Byte>();
                drawPoints(imageBgr, listOfPoints);
                
            }

            return listOfPoints;
        }
        /// <summary>
        /// Draw points on image
        /// </summary>
        /// <param name="imgProc">Image on which points need to draw</param>
        /// <param name="listOfPoints">List of points that need to be drawn on image</param>
         private void drawPoints(Image<Bgr, byte> imgProc, List<Point> listOfPoints)
        {
            foreach (Point p in listOfPoints)
            {
                imgProc.Draw(new Rectangle(p.X, p.Y, 1, 1), new Bgr(60,60,60), 15);
            }
        }
        /// <summary>
        /// Find distances between multiple points
        /// </summary>
        /// <param name="listOfPoints">List of points to find distances between them</param>
        /// <returns>Return distances between points</returns>
        private List<double> findAllDistances(List<Point> listOfPoints)
        {
           
            int[] indexTrack = { 6, 6, 8,8,10,13, 3, 1, 3, 1, 4, 6, 4, 2 };
            List<double> distances = new List<double>();
            int j = 0;
            for (int i = 0; i < 18; i++)
            {
                if (i != 8 && i != 9 && i != 12 && i != 13 && i != 14)
                {
                    distances.Add(findDistanceWRTSingleAxis(listOfPoints.ElementAt(i), listOfPoints.ElementAt(indexTrack[j]+i), "y"));
                    j++;
                }
                else if (i == 14)
                {
                    distances.Add(findDistanceWRTSingleAxis(listOfPoints.ElementAt(i), listOfPoints.ElementAt(indexTrack[j]+i), "x"));
                    j++;
                }
            }
            return distances;
        }
        /// <summary>
        /// Get distance between two points
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="axis">Axis along which distance is to  calculate</param>
        /// <returns></returns>
        private double findDistanceWRTSingleAxis(Point p1,Point p2,string axis)
        {
            double distance = 0;
            if (axis.Equals("y"))
            {
                distance = Math.Abs(p1.Y - p2.Y);
            }
            else
            {
                distance = Math.Abs(p1.X - p2.X);
            }
            return distance;
        }
        /// <summary>
        /// Find middle point of two intersecting lines
        /// </summary>
        /// <param name="listOfPoints">Four points that make two intersecting lines</param>
        /// <returns>Returns a single point of intersection</returns>
        public Point findMiddlePoint(List<Point> listOfPoints)
        {
            double m1 = (double)(listOfPoints.ElementAt(0).Y - listOfPoints.ElementAt(2).Y) / (listOfPoints.ElementAt(0).X - listOfPoints.ElementAt(2).X);
            double X = listOfPoints.ElementAt(1).X;
            double m2;
            if ((listOfPoints.ElementAt(1).X - listOfPoints.ElementAt(3).X) != 0)
            {
                m2 = (double)(listOfPoints.ElementAt(1).Y - listOfPoints.ElementAt(3).Y) / (listOfPoints.ElementAt(1).X - listOfPoints.ElementAt(3).X);

                X = (listOfPoints.ElementAt(0).Y - listOfPoints.ElementAt(1).Y - (m1 * listOfPoints.ElementAt(0).X) + (m2 * listOfPoints.ElementAt(1).X)) / (m2 - m1);
            }
            double Y = m1 * (X - listOfPoints.ElementAt(0).X) + listOfPoints.ElementAt(0).Y;
         
            return new Point((int)X, (int)Y);
        }
       
       
    }

}
