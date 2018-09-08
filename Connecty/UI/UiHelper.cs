
using System.Windows;

namespace Connecty
{

    /// <summary>
    /// UI Helper class for equal Behaivor on all UI Elements
    /// </summary>
    static class UiHelper
    {


        static public Point PointWithScalingDependencies(PresentationSource uiSource, Point point)
        {

            // Get the Scaling Factor of the 
            double scaleX = 1, scaleY = 1;
            if (uiSource != null)
            {
                scaleX = uiSource.CompositionTarget.TransformToDevice.M11;
                scaleY = uiSource.CompositionTarget.TransformToDevice.M22;
            }

            // They'll both be 1.25 for 125%, etc
            Point _pointWithScaleDipendences = new Point((point.X / scaleX), (point.Y / scaleY));
            return _pointWithScaleDipendences;

        }



    }
}
