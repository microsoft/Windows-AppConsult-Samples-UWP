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

namespace XAMLShapesManipulations
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
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


			// We use the manipulation events in order to move the shapes
			line.ManipulationMode = ManipulationModes.TranslateX |
				ManipulationModes.TranslateY;

			line.ManipulationStarted += Line_ManipulationStarted;
			line.ManipulationStarted += Line_ManipulationStarted;
			line.ManipulationDelta += Line_ManipulationDelta;
			line.ManipulationCompleted += Line_ManipulationCompleted; ;

			return line;
		}

		// XAML Shapes manipulations
		private TranslateTransform dragTranslation;

		private void Line_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
		{
			Line l = (Line)sender;
			// Initialize the Render transform that will be used to manipulate the shape
			dragTranslation = new TranslateTransform();
			l.RenderTransform = dragTranslation;
			l.Stroke = new SolidColorBrush(Windows.UI.Colors.Orange);
		}

		private void Line_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			dragTranslation.X += e.Delta.Translation.X;
			dragTranslation.Y += e.Delta.Translation.Y;
		}

		private void Line_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
		{
			Line l = (Line)sender;
			l.RenderTransform = null;

			// Get the cumulative move
			double x = e.Cumulative.Translation.X;
			double y = e.Cumulative.Translation.Y;
			// Change the origin (X1,Y1) and the end of the line (X2,Y2)
			l.X1 += x;
			l.X2 += x;
			l.Y1 += y;
			l.Y2 += y;

			l.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
		}
	}
}
