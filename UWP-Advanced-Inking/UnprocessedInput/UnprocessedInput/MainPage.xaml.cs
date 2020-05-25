//*********************************************************
//
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnprocessedInput
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();

			//// Initialize the InkCanvas
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

			Line line = ConvertStrokeToXAMLLine(stroke);
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

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			ToggleSwitch toggleSwitch = sender as ToggleSwitch;

			var p = inkCanvas.InkPresenter;
			if (toggleSwitch.IsOn == true)
			{
				// We are not in the inking or erasing mode 
				// ==> Inputs are redirected to UnprocessedInput
				// i.e. Our code will take care of the inking inputs
				p.InputProcessingConfiguration.Mode = InkInputProcessingMode.None;

				p.UnprocessedInput.PointerPressed += UnprocessedInput_PointerPressed;
				p.UnprocessedInput.PointerMoved += UnprocessedInput_PointerMoved;
				p.UnprocessedInput.PointerReleased += UnprocessedInput_PointerReleased;
			}
			else
			{
				// Go back to normal which is the inking mode
				// ==> We can draw lines on the InkCanvas
				inkCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;

				// Remove the handlers for UnprocessedInput
				p.UnprocessedInput.PointerPressed -= UnprocessedInput_PointerPressed;
				p.UnprocessedInput.PointerMoved -= UnprocessedInput_PointerMoved;
				p.UnprocessedInput.PointerReleased -= UnprocessedInput_PointerReleased;
			}
		}

		// The lasso is used to cut the lines when we switch to UnprocessedInput
		private Polyline lasso;

		private void UnprocessedInput_PointerPressed(InkUnprocessedInput sender, Windows.UI.Core.PointerEventArgs args)
		{
			lasso = new Polyline()
			{
				Stroke = new SolidColorBrush(Windows.UI.Colors.Red),
				StrokeThickness = 2,
				StrokeDashArray = new DoubleCollection() { 2, 2 }
			};

			lasso.Points.Add(args.CurrentPoint.RawPosition);
			ShapesCanvas.Children.Add(lasso);
		}

		private void UnprocessedInput_PointerMoved(InkUnprocessedInput sender, Windows.UI.Core.PointerEventArgs args)
		{
			lasso.Points.Add(args.CurrentPoint.RawPosition);
		}

private void UnprocessedInput_PointerReleased(InkUnprocessedInput sender, Windows.UI.Core.PointerEventArgs args)
{
	lasso.Points.Add(args.CurrentPoint.RawPosition);

	List<Line> linesToAdd = new List<Line>();

	foreach (Line line in ShapesCanvas.Children.OfType<Line>())
	{
		bool LineIntersection = false;
		bool SegmentIntersection = false;
		PointF IntersectionPoint;
		PointF p2;
		PointF p3;
		FindIntersection(
			// The line in the canvas
			new PointF((float)line.X1, (float)line.Y1), 
			new PointF((float)line.X2, (float)line.Y2),
					
			// The 'cutting line'
			new PointF((float)lasso.Points.First().X, (float)lasso.Points.First().Y),
			new PointF((float)lasso.Points.Last().X, (float)lasso.Points.Last().Y), 
					
			out LineIntersection,

			// Indicate if there is an intersection
			out SegmentIntersection, 
					
			// The intersection point
			out IntersectionPoint, 
					
			out p2, out p3);
				
		if (SegmentIntersection)
		{
			List<Line> lines = CutTheLine(line, IntersectionPoint);
			linesToAdd.AddRange(lines);
		}
	}



	foreach (Line line in linesToAdd)
	{
		ShapesCanvas.Children.Add(line);
	}

	ShapesCanvas.Children.Remove(lasso);
}





private List<Line> CutTheLine(Line lineToCut, PointF intersection)
{
	List<Line> lines = new List<Line>();

	var line1 = new Line();
	line1.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkOrange);
	line1.StrokeThickness = 3;
	line1.X1 = lineToCut.X1;
	line1.Y1 = lineToCut.Y1;
	line1.X2 = intersection.X;
	line1.Y2 = intersection.Y;

	var line2 = new Line();
	line2.Stroke = new SolidColorBrush(Windows.UI.Colors.DarkViolet);
	line2.StrokeThickness = 3;
	line2.X1 = intersection.X;
	line2.Y1 = intersection.Y;
	line2.X2 = lineToCut.X2;
	line2.Y2 = lineToCut.Y2;

	lines.Add(line1);
	lines.Add(line2);

	return lines;
}











// Code provided by Rod Stephens
// at http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/
// Find the point of intersection between
// the lines p1 --> p2 and p3 --> p4.
private void FindIntersection(
	PointF p1, PointF p2, PointF p3, PointF p4,
	out bool lines_intersect, out bool segments_intersect,
	out PointF intersection,
	out PointF close_p1, out PointF close_p2)
{
	// Get the segments' parameters.
	float dx12 = p2.X - p1.X;
	float dy12 = p2.Y - p1.Y;
	float dx34 = p4.X - p3.X;
	float dy34 = p4.Y - p3.Y;

	// Solve for t1 and t2
	float denominator = (dy12 * dx34 - dx12 * dy34);

	float t1 =
		((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
			/ denominator;
	if (float.IsInfinity(t1))
	{
		// The lines are parallel (or close enough to it).
		lines_intersect = false;
		segments_intersect = false;
		intersection = new PointF(float.NaN, float.NaN);
		close_p1 = new PointF(float.NaN, float.NaN);
		close_p2 = new PointF(float.NaN, float.NaN);
		return;
	}
	lines_intersect = true;

	float t2 =
		((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
			/ -denominator;

	// Find the point of intersection.
	intersection = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);

	// The segments intersect if t1 and t2 are between 0 and 1.
	segments_intersect =
		((t1 >= 0) && (t1 <= 1) &&
			(t2 >= 0) && (t2 <= 1));

	// Find the closest points on the segments.
	if (t1 < 0)
	{
		t1 = 0;
	}
	else if (t1 > 1)
	{
		t1 = 1;
	}

	if (t2 < 0)
	{
		t2 = 0;
	}
	else if (t2 > 1)
	{
		t2 = 1;
	}

	close_p1 = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);
	close_p2 = new PointF(p3.X + dx34 * t2, p3.Y + dy34 * t2);
}

}
}
