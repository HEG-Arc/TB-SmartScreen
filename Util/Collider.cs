using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SCE_ProductionChain.Util
{
    public static class Collider
    {
        public static bool doesCollide(Ellipse Ellipse, Point location)
        {
            Point center = new Point(
                  Canvas.GetLeft(Ellipse) + (Ellipse.Width / 2),
                  Canvas.GetTop(Ellipse) + (Ellipse.Height / 2));

            double _xRadius = Ellipse.Width / 2;
            double _yRadius = Ellipse.Height / 2;


            if (_xRadius <= 0.0 || _yRadius <= 0.0)
                return false;

            Point normalized = new Point(location.X - center.X,
                                         location.Y - center.Y);

            return ((double)(normalized.X * normalized.X)
                     / (_xRadius * _xRadius)) + ((double)(normalized.Y * normalized.Y) / (_yRadius * _yRadius))
                <= 1.0;
        }
    }
}
