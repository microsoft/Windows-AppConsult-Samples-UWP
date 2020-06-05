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

namespace XAMLShapesAdvancedManipulations
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

			line.Tapped += Line_Tapped;

			return line;
		}

		private void Line_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// Remove the anchor from the selected line
			UnselectActiveLine();

			System.Diagnostics.Debug.WriteLine("[PoC] Line tapped");
			Line line = (Line)sender;
			line.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkRed);
			line.StrokeThickness = 10;
			line.StrokeStartLineCap = PenLineCap.Triangle;
			line.StrokeEndLineCap = PenLineCap.Triangle;


			int size_EndLines = 30;
			//TODO: [PoC] - Write some 'anchors/objects' to be able to modify the lenght of the line
			Ellipse e1 = new Ellipse
			{
				Fill = new SolidColorBrush(Windows.UI.Colors.OrangeRed),
				Height = size_EndLines,
				Width = size_EndLines
			};

			ShapesCanvas.Children.Add(e1);





			Ellipse e2 = new Ellipse
			{
				Fill = new SolidColorBrush(Windows.UI.Colors.OrangeRed),
				Height = size_EndLines,
				Width = size_EndLines
			};

			ShapesCanvas.Children.Add(e2);



			Canvas.SetLeft(e1, line.X1 - size_EndLines / 2);
			Canvas.SetLeft(e2, line.X2 - size_EndLines / 2);
			Canvas.SetTop(e1, line.Y1 - size_EndLines / 2);
			Canvas.SetTop(e2, line.Y2 - size_EndLines / 2);


			// Enable manipulation on the anchors
			//e1.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
			//e1.ManipulationStarted += E1_ManipulationStarted;
			//e1.ManipulationDelta += E1_ManipulationDelta;
			//e1.ManipulationCompleted += E1_ManipulationCompleted;

			//e2.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
			//e2.ManipulationStarted += E2_ManipulationStarted;
			//e2.ManipulationDelta += E2_ManipulationDelta;
			//e2.ManipulationCompleted += E2_ManipulationCompleted;


			// Take this line as the one to be modified with the anchors
			activeLine.line = line;
			activeLine.E1 = e1;
			activeLine.E2 = e2;
		}



		public void UnselectActiveLine()
		{
			if (activeLine.line != null)
			{
				activeLine.line.Stroke = new SolidColorBrush(Windows.UI.Colors.Green);
				activeLine.line.StrokeThickness = 6;
				ShapesCanvas.Children.Remove(activeLine.E1);
				ShapesCanvas.Children.Remove(activeLine.E2);
				ShapesCanvas.Children.Remove(activeLine.CenterEllipse);
				activeLine.bezierPath = null;
			}
		}

		public struct ActiveLine
		{
			public Line line;
			public double initialX1;
			public double initialY1;
			public double initialX2;
			public double initialY2;
			public Ellipse E1;
			public Ellipse E2;
			public Ellipse CenterEllipse;
			public double initialCenterEllipseX;
			public double initialCenterEllipseY;
			public Path bezierPath;
		}
		public ActiveLine activeLine;
	}
}
