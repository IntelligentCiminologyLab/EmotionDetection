using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASSVS.ML.EDR.Shared
{
    public enum DimensionReductionMethod
    {
        /** Kernel Locally Linear Embedding as described 
		 * in @cite Decoste2001 */
        KernelLocallyLinearEmbedding,
        /** Neighborhood Preserving Embedding as described 
		 * in @cite He2005 */
        NeighborhoodPreservingEmbedding,
        /** Local Tangent Space Alignment as described 
		 * in @cite Zhang2002 */
        KernelLocalTangentSpaceAlignment,
        /** Linear Local Tangent Space Alignment as described 
		 * in @cite Zhang2007 */
        LinearLocalTangentSpaceAlignment,
        /** Hessian Locally Linear Embedding as described in 
		 * @cite Donoho2003 */
        HessianLocallyLinearEmbedding,
        /** Laplacian Eigenmaps as described in 
		 * @cite Belkin2002 */
        LaplacianEigenmaps,
        /** Locality Preserving Projections as described in 
		 * @cite He2003 */
        LocalityPreservingProjections,
        /** Diffusion map as described in 
		 * @cite Coifman2006 */
        DiffusionMap,
        /** Isomap as described in 
		 * @cite Tenenbaum2000 */
        Isomap,
        /** Landmark Isomap as described in 
		 * @cite deSilva2002 */
        LandmarkIsomap,
        /** Multidimensional scaling as described in 
		 * @cite Cox2000 */
        MultidimensionalScaling,
        /** Landmark multidimensional scaling as described in 
		 * @cite deSilva2004 */
        LandmarkMultidimensionalScaling,
        /** Stochastic Proximity Embedding as described in 
		 * @cite Agrafiotis2003 */
        StochasticProximityEmbedding,
        /** Kernel PCA as described in 
		 * @cite Scholkopf1997 */
        KernelPCA,
        /** Principal Component Analysis */
        PCA,
        /** Random Projection as described in
		 * @cite Kaski1998*/
        RandomProjection,
        /** Factor Analysis */
        FactorAnalysis,
        /** t-SNE and Barnes-Hut-SNE as described in 
		 * @cite vanDerMaaten2008 and @cite vanDerMaaten2013 */
        tDistributedStochasticNeighborEmbedding,
        /** Manifold Sculpting as described in
		* @cite Gashler2007 */
        ManifoldSculpting,
        /** Passing through (doing nothing just passes the 
		 * data through) */
        PassThru
    };
    public class EmotionMLModel
    {
        public double sum;
        public string label;
        public EmotionMLModel(double sum, string label)
        {
            this.sum = sum;
            this.label = label;
        }

        public EmotionMLModel()
        {
        }
    }
  
    public class EmotionTrainingMLModel : System.Attribute
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
        /// Constructor for generating features ML model
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

        public EmotionTrainingMLModel(double LEB1_CPLE1, double LEB2_CPLE2, double REB1_CPRE1, double REB2_CPRE2, double OPEN_LE1, double OPEN_LE2, double OPEN_RE1, double OPEN_RE2, double OPEN_MO1, double OPEN_MO2, double OPEN_MO3, double EXP_MO,double NS_LPLIP, double NS_RPLIP,double noseWrinkles,double betweenEyesWrinkles, string Label)
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
        /// Default constructor for EmotionTrainingMLModel 
        /// </summary>
        public EmotionTrainingMLModel()
        {
        }
    }
   
}
