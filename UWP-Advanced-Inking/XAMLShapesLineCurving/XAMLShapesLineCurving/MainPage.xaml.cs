//*********************************************************
//
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Path = Windows.UI.Xaml.Shapes.Path;

namespace XAMLShapesLineCurving
{

	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();

			// Initialize the InkCanvas
			inkCanvas.InkPresenter.InputDeviceTypes =
				Windows.UI.Core.CoreInputDeviceTypes.Mouse |
				Windows.UI.Core.CoreInputDeviceTypes.Pen |
				Windows.UI.Core.CoreInputDeviceTypes.Touch;

			// When the user finished to draw something on the InkCanvas
			inkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
		}

		private void InkPresenter_StrokesCollected(
			Windows.UI.Input.Inking.InkPresenter sender,
			Windows.UI.Input.Inking.InkStrokesCollectedEventArgs args)
		{
			InkStroke stroke = inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Last();

			// Action 1 = We use a function that we will implement just after to create the XAML Line
			Line line = ConvertStrokeToXAMLLine(stroke);
			// Action 2 = We add the Line in the second Canvas
			ShapesCanvas.Children.Add(line);

			// We delete the InkStroke from the InkCanvas
			stroke.Selected = true;
			inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
		}

		private Line ConvertStrokeToXAMLLine(InkStroke stroke)
		{
			var line = new Line();
			line.Stroke = new SolidColorBrush(Windows.UI.Colors.Green);
			line.StrokeThickness = 6;
			// The origin = (X1, Y1)
			line.X1 = stroke.GetInkPoints().First().Position.X;
			line.Y1 = stroke.GetInkPoints().First().Position.Y;
			// The end = (X2, Y2)
			line.X2 = stroke.GetInkPoints().Last().Position.X;
			line.Y2 = stroke.GetInkPoints().Last().Position.Y;

			// Draw the anchors and initiate the advanced drag & drop functionalities
			line.Tapped += Line_Tapped;

			return line;
		}

		// XAML Shapes curving
		private TranslateTransform curveTranslation;

		private void Line_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// Remove the anchor from the selected line
			UnselectActiveLine();

			Line line = (Line)sender;
			line.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkRed);
			line.StrokeThickness = 10;




			// Display an anchor for curving
			int size_centerLine = 45;
			Ellipse curvingEllipse = new Ellipse
			{
				Fill = new SolidColorBrush(Windows.UI.Colors.OrangeRed),
				Height = size_centerLine,
				Width = size_centerLine
			};

			ShapesCanvas.Children.Add(curvingEllipse);

			// Calculate the points for the center of the ellipse
			double centerX = line.X1 + (line.X2 - line.X1) / 2;
			double centerY = line.Y1 + (line.Y2 - line.Y1) / 2;

			Canvas.SetLeft(curvingEllipse, centerX - size_centerLine / 2);
			Canvas.SetTop(curvingEllipse, centerY - size_centerLine / 2);

			// Enable manipulation on the anchor
			curvingEllipse.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
			curvingEllipse.ManipulationStarted += CurvingEllipse_ManipulationStarted;
			curvingEllipse.ManipulationDelta += CurvingEllipse_ManipulationDelta;
			curvingEllipse.ManipulationCompleted += CurvingEllipse_ManipulationCompleted;



			InitializeActiveLine(line, curvingEllipse);
		}





		private void CurvingEllipse_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
		{
			Path path = activeLine.bezierPath;

			if (activeLine.bezierPath == null)
			{
				path = DrawABezierCurve(
					activeLine.initialX1, activeLine.initialX2,
					activeLine.initialY1, activeLine.initialY2);

				// We add the path to the Canvas
				ShapesCanvas.Children.Add(path);

				// We keep track of the bezier segment
				activeLine.bezierPath = path;
				activeLine.initialCenterEllipseX = activeLine.initialX1 + (activeLine.initialX2 - activeLine.initialX1) / 2;
				activeLine.initialCenterEllipseY = activeLine.initialY1 + (activeLine.initialY2 - activeLine.initialY1) / 2;

				// We remove the line
				// i.e. the path will "replace" the line
				ShapesCanvas.Children.Remove(activeLine.line);
			}

			// Initialize the transforms that will be used to manipulate the shape
			curveTranslation = new TranslateTransform();
			Ellipse el = (Ellipse)sender;
			el.RenderTransform = curveTranslation;
			el.Fill = new SolidColorBrush(Windows.UI.Colors.Orange);
		}



		private void CurvingEllipse_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			double x = e.Delta.Translation.X;
			double y = e.Delta.Translation.Y;
			curveTranslation.X += x;
			curveTranslation.Y += y;


			GeometryCollection gc = (GeometryCollection)activeLine.bezierPath.Data.GetValue(GeometryGroup.ChildrenProperty);
			PathGeometry pg = (PathGeometry)gc[0];
			PathSegmentCollection psc = (PathSegmentCollection)pg.Figures[0].Segments;
			BezierSegment bs = (BezierSegment)psc[0];
			bs.Point2 = new Point(bs.Point2.X + x, bs.Point2.Y + y);
		}




		private void CurvingEllipse_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
		{

			Ellipse el = (Ellipse)sender;
			el.RenderTransform = null;
			double x = e.Cumulative.Translation.X;
			double y = e.Cumulative.Translation.Y;
			Canvas.SetLeft(el, Canvas.GetLeft(el) + x);
			Canvas.SetTop(el, Canvas.GetTop(el) + y);

			el.Fill = new SolidColorBrush(Windows.UI.Colors.Black);


			GeometryCollection gc = (GeometryCollection)activeLine.bezierPath.Data.GetValue(GeometryGroup.ChildrenProperty);
			PathGeometry pg = (PathGeometry)gc[0];
			PathSegmentCollection psc = (PathSegmentCollection)pg.Figures[0].Segments;
			BezierSegment bs = (BezierSegment)psc[0];
			bs.Point2 = new Point(activeLine.initialCenterEllipseX + x, activeLine.initialCenterEllipseY + y);

			activeLine.initialCenterEllipseX += x;
			activeLine.initialCenterEllipseY += y;
		}


		private Windows.UI.Xaml.Shapes.Path DrawABezierCurve(
			double X1, double X2,
			double Y1, double Y2)
		{
			// Define the path properties: stroke, color, thickness
			var path = new Windows.UI.Xaml.Shapes.Path();
			path.Stroke = new SolidColorBrush(Windows.UI.Colors.IndianRed);
			path.StrokeThickness = 10;

			// The path takes a 'GeometryGroup' for all segments of the path
			var geometryGroup = new GeometryGroup();

			// In this GeometryGroup we can add several 'PathGeometry'
			var pathGeometry = new PathGeometry();

			// In this PathGeometry, we have a 'Figures' property.
			// We affect a 'PathFigureCollection' to this property
			var pathFigureCollection = new PathFigureCollection();
			// The PathFigureCollection takes some 'PathFigure'
			var pathFigure = new PathFigure();
			pathFigure.StartPoint = new Windows.Foundation.Point(X1, Y1);
			pathFigureCollection.Add(pathFigure);
			pathGeometry.Figures = pathFigureCollection;

			// The PathFigure we created is empty; We just defined the starting point
			// We now create this PathFigure with a 'PathSegmentCollection' which takes 'PathSegment'

			// The PathSegment we create is a Bezier curve
			var pathSegment = new BezierSegment();
			pathSegment.Point1 = new Point(X1, Y1);
			pathSegment.Point2 = new Point(X1 + (X2 - X1) / 2, Y1 + (Y2 - Y1) / 2);
			pathSegment.Point3 = new Point(X2, Y2);

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

			return path;
		}




		/// <summary>
		/// Take this line as the one to be modified with the anchors
		/// Selecting the line is:
		///   - get the line object
		///   - keep track of its coordinates
		///   - get the 2 anchors objects
		/// </summary>
		private void InitializeActiveLine(Line line, Ellipse curvingEllipse)
		{
			activeLine.line = line;
			activeLine.initialX1 = line.X1;
			activeLine.initialY1 = line.Y1;
			activeLine.initialX2 = line.X2;
			activeLine.initialY2 = line.Y2;
			activeLine.CenterEllipse = curvingEllipse;
		}


		/// <summary>
		/// Before doing manipulations on the newly selected line, we put back the origina color
		/// of the previous line and we remove its the anchors from the screen
		/// </summary>
		public void UnselectActiveLine()
		{
			if (activeLine.line != null
				&& activeLine.CenterEllipse != null)
			{
				activeLine.line.Stroke = new SolidColorBrush(Windows.UI.Colors.Green);
				activeLine.line.StrokeThickness = 6;
				ShapesCanvas.Children.Remove(activeLine.CenterEllipse);
				activeLine.bezierPath = null;
			}
		}

		/// <summary>
		/// We doing manipulations on the anchors of a line, we move the line position
		/// in real time, so we have to keep track about the line's original position
		/// in order to do the definitive move when the manipulation is completed.
		/// We also move the anchors during the manipulation.
		/// </summary>
		private struct ActiveLine
		{
			public Line line;
			public double initialX1;
			public double initialY1;
			public double initialX2;
			public double initialY2;
			public Ellipse CenterEllipse;
			public double initialCenterEllipseX;
			public double initialCenterEllipseY;
			public Path bezierPath;
		}

		private ActiveLine activeLine;
	}
}
