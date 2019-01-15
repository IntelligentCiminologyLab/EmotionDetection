using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVML.Constants
{
    public class StaticConstants
    {
        public const String skinDetectorPath = @"E:\Git\FaceDetectionApi\libs\skinDetector\skinDetector.dll";
        public const String FaceLandmarksDllName = "ExportedLib.dll";
        public const String BioInspiredDllName = "ageBio.dll";
        public const String CNNDllname = "classification.dll"; 
        public const String DimReductionDllName = "Isomap.dll";
        public const String landmarkDllName = "ExportedLib.dll";
        public const string  YoloDllName =@"yolo_cpp_dll.dll";
        public const String interpolateFile= @"..\..\..\..\..\..\libs\VSR\interpolateFile.dll";
        public static string FRtrainingPath = @"..\..\..\..\libs\FaceRecognition\Dataset\Trainings\";
        public static string FRdirectoryPath = @"..\..\..\..\libs\FaceRecognition\Dataset\Images\";
        public static string FRcameraSourceFront = "0";
        public static string FRcameraSourceRear = "1";
        public static bool FRfaceDetected = false;
        public static int FRframeCountFront = 0;
        public static int FRframeCountRear = 0;
        public const string dimentionReductonDllImportPath = @"C:\Users\Gulraiz\Documents\Visual Studio 2015\Projects\Isomap\x64\Debug\Isomap.dll";
        public static bool isFDRdictonary = true;
    }
}
