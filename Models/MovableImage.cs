using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace POC_GestureNavigation
{
    public class MovableImage : Image
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

        /// <summary>
        /// Retroune un boolean qui indique si la position du point passé en paramètre est sur l'image ou non.
        /// </summary>
        /// <param name="pointerPosition"></param>
        /// <returns></returns>
        public bool IsGrabbed(Point pointerPosition)
        {
            return (
                    (pointerPosition.X > this.position.X && pointerPosition.X < this.position.X + this.Width) &&
                    (pointerPosition.Y > this.position.Y && pointerPosition.Y < this.position.Y + this.Height)
                   );
        }

        /// <summary>
        /// Cette méthode permet de cloner (faire une copie) d'un objet MovableImage dans une nouvelle instance.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static MovableImage Clone(MovableImage image)
        {
            MovableImage clone = new MovableImage();
            clone.Source = image.Source;
            clone.Height = image.Height;
            clone.Width = image.Width;
            clone.HorizontalAlignment = image.HorizontalAlignment;
            clone.VerticalAlignment = image.VerticalAlignment;
            clone.position.X = image.Position.X;
            clone.position.Y = image.Position.Y;
            return clone;
        }
    }
}