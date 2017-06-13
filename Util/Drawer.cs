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

        public static readonly SolidColorBrush[] BodyColors =
        {
            new SolidColorBrush(Color.FromRgb(255,0,0)),
            new SolidColorBrush(Color.FromRgb(0,255,0)),
            new SolidColorBrush(Color.FromRgb(0,0,255)),
            new SolidColorBrush(Color.FromRgb(255,0,255)),
            new SolidColorBrush(Color.FromRgb(255,255,0)),
            new SolidColorBrush(Color.FromRgb(255,255,255))
        };

        private readonly static SolidColorBrush TooFarBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        private readonly static SolidColorBrush DistanceOKBrush = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        public static void DrawHeadRectangle(Joint headJoint, Point headPosition, User user, int userIndex, Nullable<bool> isAtTheRightDistance,
                                             Canvas canvas, double color_scale_ratio)
        {
            Rectangle headRect = new Rectangle() { StrokeThickness = HEAD_RECTANGLE_THICKNESS };
            TextBlock headUsernameInfo = new TextBlock() { FontSize = HEAD_INFO_FONT_SIZE };

            if (userIndex >= 0)
            {
                headRect.Stroke = BodyColors[userIndex];
                headUsernameInfo.Foreground = BodyColors[userIndex];

                if (user != null)
                    headUsernameInfo.Text = user.Username;
            }
            else
            {
                if(isAtTheRightDistance == null || !(bool)isAtTheRightDistance)
                {
                    headRect.Stroke = TooFarBrush;
                    headUsernameInfo.Foreground = TooFarBrush;
                }
                else
                {
                    headRect.Stroke = DistanceOKBrush;
                    headUsernameInfo.Foreground = DistanceOKBrush;
                }
                headUsernameInfo.Text = Properties.Resources.UnidentifiedUser;
            }
            headRect.Height = headRect.Width = (HEAD_RECTANGLE_SIZE / color_scale_ratio) / headJoint.Position.Z;

            if (((headPosition.Y - headRect.Height / 2) + headRect.Height + (HEAD_INFO_MARGIN_TOP / color_scale_ratio)) > canvas.Height)
                headPosition.Y = canvas.Height - headRect.Height;
            Canvas.SetLeft(headRect, headPosition.X - headRect.Width / 2);
            Canvas.SetTop(headRect, headPosition.Y - headRect.Height / 2);

            Canvas.SetLeft(headUsernameInfo, headPosition.X - headRect.Width / 2);
            Canvas.SetTop(headUsernameInfo, (headPosition.Y - headRect.Height / 2) + headRect.Height + (HEAD_INFO_MARGIN_TOP / color_scale_ratio));

            canvas.Children.Add(headRect);
            canvas.Children.Add(headUsernameInfo);
        }
    }
}
