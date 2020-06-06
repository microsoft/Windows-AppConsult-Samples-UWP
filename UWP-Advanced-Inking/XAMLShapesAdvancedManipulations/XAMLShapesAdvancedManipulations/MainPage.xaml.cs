//*********************************************************
//
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Linq;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

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

			// Draw the anchors and initiate the advanced drag & drop functionalities
			line.Tapped += Line_Tapped;

			return line;
		}

		// XAML Shapes manipulations
		private TranslateTransform dragTranslation;

		private void Line_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// Remove the anchor from the selected line
			UnselectActiveLine();

			Line line = (Line)sender;
			line.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkRed);
			line.StrokeThickness = 10;

			int size_EndLines = 25;
			// Create 2 circles for the ends of the line
			Ellipse anchorOrigin = new Ellipse
			{
				Fill = new SolidColorBrush(Windows.UI.Colors.OrangeRed),
				Height = size_EndLines,
				Width = size_EndLines
			};
			ShapesCanvas.Children.Add(anchorOrigin);

			Ellipse anchorEnd = new Ellipse
			{
				Fill = new SolidColorBrush(Windows.UI.Colors.OrangeRed),
				Height = size_EndLines,
				Width = size_EndLines
			};
			ShapesCanvas.Children.Add(anchorEnd);

			// Put the anchors at the origin and at the end of the line
			Canvas.SetLeft(anchorOrigin, line.X1 - size_EndLines / 2);
			Canvas.SetLeft(anchorEnd, line.X2 - size_EndLines / 2);
			Canvas.SetTop(anchorOrigin, line.Y1 - size_EndLines / 2);
			Canvas.SetTop(anchorEnd, line.Y2 - size_EndLines / 2);


			// Enable manipulations on the anchors
			anchorOrigin.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
			anchorOrigin.ManipulationStarted += Anchor_ManipulationStarted;
			anchorOrigin.ManipulationDelta += Anchor_Origin_ManipulationDelta;
			anchorOrigin.ManipulationCompleted += Anchor_Origin_ManipulationCompleted;

			anchorEnd.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
			anchorEnd.ManipulationStarted += Anchor_ManipulationStarted;
			anchorEnd.ManipulationDelta += Anchor_End_ManipulationDelta;
			anchorEnd.ManipulationCompleted += Anchor_End_ManipulationCompleted;

			InitializeActiveLine(line, anchorOrigin, anchorEnd);
		}

		private void Anchor_ManipulationStarted(object sender,
			ManipulationStartedRoutedEventArgs e)
		{
			Ellipse anchor = (Ellipse)sender;

			// Initialize the transforms that will be used to manipulate the shape
			dragTranslation = new TranslateTransform();
			anchor.RenderTransform = dragTranslation;
			anchor.Fill = new SolidColorBrush(Windows.UI.Colors.Orange);
		}

		private void Anchor_Origin_ManipulationDelta(object sender,
			ManipulationDeltaRoutedEventArgs e)
		{
			AnchorManipulationDelta(sender, e, true);
		}

		private void Anchor_End_ManipulationDelta(object sender,
			ManipulationDeltaRoutedEventArgs e)
		{
			AnchorManipulationDelta(sender, e, false);
		}


		private void AnchorManipulationDelta(object sender,
			ManipulationDeltaRoutedEventArgs e,
			bool OriginofLine)
		{
			double x = e.Delta.Translation.X;
			double y = e.Delta.Translation.Y;
			dragTranslation.X += x;
			dragTranslation.Y += y;

			if (OriginofLine)
			{
				activeLine.line.X1 += x;
				activeLine.line.Y1 += y;
			}
			else
			{
				activeLine.line.X2 += x;
				activeLine.line.Y2 += y;
			}
		}

		private void Anchor_Origin_ManipulationCompleted(object sender,
			ManipulationCompletedRoutedEventArgs e)
		{
			AnchorManipulationCompleted(sender, e, true);
		}

		private void Anchor_End_ManipulationCompleted(object sender,
			ManipulationCompletedRoutedEventArgs e)
		{
			AnchorManipulationCompleted(sender, e, false);
		}


		private void AnchorManipulationCompleted(object sender,
			ManipulationCompletedRoutedEventArgs e,
			bool OriginofLine)
		{
			Ellipse anchor = (Ellipse)sender;
			anchor.RenderTransform = null;
			double x = e.Cumulative.Translation.X;
			double y = e.Cumulative.Translation.Y;
			Canvas.SetLeft(anchor, Canvas.GetLeft(anchor) + x);
			Canvas.SetTop(anchor, Canvas.GetTop(anchor) + y);

			anchor.Fill = new SolidColorBrush(Windows.UI.Colors.Black);

			if (OriginofLine)
			{
				activeLine.line.X1 = activeLine.initialX1 + x;
				activeLine.line.Y1 = activeLine.initialY1 + y;

				activeLine.initialX1 = activeLine.line.X1;
				activeLine.initialY1 = activeLine.line.Y1;
			}
			else
			{
				activeLine.line.X2 = activeLine.initialX2 + x;
				activeLine.line.Y2 = activeLine.initialY2 + y;

				activeLine.initialX2 = activeLine.line.X2;
				activeLine.initialY2 = activeLine.line.Y2;
			}
		}

		/// <summary>
		/// Take this line as the one to be modified with the anchors
		/// Selecting the line is:
		///   - get the line object
		///   - keep track of its coordinates
		///   - get the 2 anchors objects
		/// </summary>
		private void InitializeActiveLine(Line line, Ellipse origin, Ellipse end)
		{
			activeLine.line = line;
			activeLine.initialX1 = line.X1;
			activeLine.initialY1 = line.Y1;
			activeLine.initialX2 = line.X2;
			activeLine.initialY2 = line.Y2;
			activeLine.AnchorOrigin = origin;
			activeLine.AnchorEnd = end;
		}


		/// <summary>
		/// Before doing manipulations on the newly selected line, we put back the origina color
		/// of the previous line and we remove its the anchors from the screen
		/// </summary>
		public void UnselectActiveLine()
		{
			if (activeLine.line != null
				&& activeLine.AnchorOrigin != null
				&& activeLine.AnchorEnd != null)
			{
				activeLine.line.Stroke = new SolidColorBrush(Windows.UI.Colors.Green);
				activeLine.line.StrokeThickness = 6;
				ShapesCanvas.Children.Remove(activeLine.AnchorOrigin);
				ShapesCanvas.Children.Remove(activeLine.AnchorEnd);
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
			public Ellipse AnchorOrigin;
			public Ellipse AnchorEnd;
		}

		private ActiveLine activeLine;
	}
}
