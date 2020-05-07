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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace StrokesToShapes
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

			return line;
		}
	}
}
