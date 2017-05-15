using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace POC_GestureNavigation
{
    public class CustomImage : Image
    {
        private Point position;

        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public bool isGrabbed(Point pointerPosition)
        {
            return (
                    (pointerPosition.X > this.position.X && pointerPosition.X < this.position.X + this.Width) &&
                    (pointerPosition.Y > this.position.Y && pointerPosition.Y < this.position.Y + this.Height)
                   );
        }

    }
}
