using Microsoft.Kinect;
using SCE_ProductionChain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SCE_ProductionChain.Util
{
    public class Drawer
    {
        private const int HEAD_RECTANGLE_SIZE = 350;
        private const int HEAD_RECTANGLE_THICKNESS = 5;

        private const int HEAD_INFO_MARGIN_TOP = 10;
        private const int HEAD_INFO_FONT_SIZE = 20;
        private const string HEAD_INFO_DEFAULT_TEXT = "non identifié";

        public static readonly SolidColorBrush[] BodyColors =
        {
            new SolidColorBrush(Color.FromRgb(255,0,0)),
            new SolidColorBrush(Color.FromRgb(0,255,0)),
            new SolidColorBrush(Color.FromRgb(0,0,255)),
            new SolidColorBrush(Color.FromRgb(255,0,255)),
            new SolidColorBrush(Color.FromRgb(255,255,0)),
            new SolidColorBrush(Color.FromRgb(255,255,255))
        };

        private readonly static SolidColorBrush DefaultBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));

        public static void DrawHeadRectangle(Joint headJoint, Point headPosition, User user, int userIndex,
                                             Canvas canvas, double color_scale_ratio)
        {
            Rectangle headRect = new Rectangle() { StrokeThickness = HEAD_RECTANGLE_THICKNESS };
            TextBlock headInfos = new TextBlock() { FontSize = HEAD_INFO_FONT_SIZE };

            if (userIndex >= 0)
            {
                headRect.Stroke = BodyColors[userIndex];
                headInfos.Foreground = BodyColors[userIndex];
            }
            else
            {
                headRect.Stroke = DefaultBrush;
                headInfos.Foreground = DefaultBrush;
            }
            headRect.Height = headRect.Width = (HEAD_RECTANGLE_SIZE / color_scale_ratio) / headJoint.Position.Z;

            if (((headPosition.Y - headRect.Height / 2) + headRect.Height + (HEAD_INFO_MARGIN_TOP / color_scale_ratio)) > canvas.Height)
                headPosition.Y = canvas.Height - headRect.Height;
            Canvas.SetLeft(headRect, headPosition.X - headRect.Width / 2);
            Canvas.SetTop(headRect, headPosition.Y - headRect.Height / 2);

            if (user != null)
                headInfos.Text = user.Username;
            else
                headInfos.Text = HEAD_INFO_DEFAULT_TEXT;
            Canvas.SetLeft(headInfos, headPosition.X - headRect.Width / 2);
            Canvas.SetTop(headInfos, (headPosition.Y - headRect.Height / 2) + headRect.Height + (HEAD_INFO_MARGIN_TOP / color_scale_ratio));

            canvas.Children.Add(headRect);
            canvas.Children.Add(headInfos);
        }
    }
}
