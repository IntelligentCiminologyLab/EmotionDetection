using System.Collections.Generic;
using System.Linq;
using CVML.MLL;
using ASSVS.FAS.EDR.Shared;
using CVML.Constants;
using System.Runtime.InteropServices;
using System.Reflection;
using System;

namespace ASSVS.ML.EDR.Shared
{
    public class EDRMLMain
    {
        
        /// <summary>
        /// static object for EDRMLMainReducedDimentionBased class
        /// </summary>
        private static EDRMLMain instance;
        /// <summary>
        /// Common ML object to use common functionality
        /// </summary>
        private MLMain MLObject=MLMain.getInstance();
       
        /// <summary>
        /// Get reduced dimentions of features
        /// </summary>
        /// <param name="methodIndex">Method to use for dimension reduction</param>
        /// <param name="numberOfColumns">Number of dimensions</param>
        /// <param name="numberOfRows">Total features</param>
        /// <param name="features">Features need to reduce in dimensions</param>
        /// <param name="numNeighbors">Number of neighbours for dimension reduction</param>
        /// <param name="targetDimension">Output dimensions</param>
        /// <returns>Returns reduced dimensions</returns>
        [DllImport(StaticConstants.DimReductionDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetReducedDimentions(int methodIndex, int numberOfColumns, int numberOfRows, double[] features, int numNeighbors, int targetDimension);
        /// <summary>
        /// Constructor is private to prevent generation of EDRMLMain class from outside
        /// </summary>
        private EDRMLMain()
        {

        }
        /// <summary>
        /// Return single object of EDRMLMain class and if object already exist then send same object
        /// </summary>
        /// <returns>Rerturns object of EDRMLMain</returns>
        public static EDRMLMain getInstance()
        {
            if (instance == null)
                instance = new EDRMLMain();
            return instance;
        }
        /// <summary>
        /// Get reduced dimentions of features by calling dimension reduction method
        /// </summary>
        /// <param name="methodIndex">Method to use for dimension reduction</param>
        /// <param name="numberOfColumns">Number of dimensions</param>
        /// <param name="numberOfRows">Total features</param>
        /// <param name="features">Features need to reduce in dimensions</param>
        /// <param name="numNeighbors">Number of neighbours for dimension reduction</param>
        /// <param name="targetDimension">Output dimensions</param>
        /// <returns>Returns reduced dimensions</returns>
        public IntPtr getReducedDimentions(int methodIndex, int numberOfColumns, int numberOfRows, double[] features, int numNeighbors, int targetDimension)
        {
           return GetReducedDimentions(methodIndex, numberOfColumns, numberOfRows, features, numNeighbors, targetDimension);
        }
        
    }
}
