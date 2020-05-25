using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CurveALine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            DrawABezierCurve();
        }

        private BezierSegment pathSegment;
        private void DrawABezierCurve()
        {
            // Define the path properties: stroke, color, thickness
            var path = new Windows.UI.Xaml.Shapes.Path();
            path.Stroke = new SolidColorBrush(Windows.UI.Colors.Green);
            path.StrokeThickness = 6;

            // The path takes a 'GeometryGroup' for all segments of the path
            var geometryGroup = new GeometryGroup();

            // In this GeometryGroup we can add several 'PathGeometry'
            var pathGeometry = new PathGeometry();
            
            // In this PathGeometry, we have a 'Figures' property.
            // We affect a 'PathFigureCollection' to this property
            var pathFigureCollection = new PathFigureCollection();
            // The PathFigureCollection takes some 'PathFigure'
            var pathFigure = new PathFigure();
            pathFigure.StartPoint = new Windows.Foundation.Point(100, 100);
            pathFigureCollection.Add(pathFigure);
            pathGeometry.Figures = pathFigureCollection;

            // The PathFigure we created is empty; We just defined the starting point
            // We now create this PathFigure with a 'PathSegmentCollection' which takes 'PathSegment'
            
            // The PathSegment we create is a Bezier curve
            pathSegment = new BezierSegment();
            pathSegment.Point1 = new Point(100, 100);
            pathSegment.Point2 = new Point(300, 300);
            pathSegment.Point3 = new Point(500, 100);

            var pathSegmentCollection = new PathSegmentCollection();
            pathSegmentCollection.Add(pathSegment);

            // So we affect the PathSegmentCollection to the PathFigure
            pathFigure.Segments = pathSegmentCollection;

            // The PathFigure was already affected to the 'Figures' collection of
            // the PathGeometry object
            // We add this PathGeometry object to the 'GeometryGroup'
            geometryGroup.Children.Add(pathGeometry);

            // Finally, we give to the path the data corresponding to the GeometryGroup
            path.Data = geometryGroup;

            // We add the path to the Canvas
            layoutRoot.Children.Add(path);
        }

        private void curve_Click(object sender, RoutedEventArgs e)
        {
            pathSegment.Point2 = new Point(300, DateTime.Now.Millisecond);
        }
    }
}
