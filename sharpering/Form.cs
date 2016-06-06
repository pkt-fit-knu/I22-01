using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace sharpening
{
    class Cover
    {
        private int[,] array;
        public Cover(int[,] array)
        {
            this.array = array;
        }

        public int GetCell(int x, int y)
        {
            return this.array[x, y];
        }
    }
    class Laplacian
    {
        private static int[,] pixelArray;

        private static int width;
        private static int height;

        public Bitmap sharpening(Bitmap bitmap, Cover cover)
        {
            GetIntensities(bitmap);

            createImage(cover);

            delErrorInImage();

            return returnImage();
        }

        private void GetIntensities(Bitmap bitmap)
        {
            const double r_c = 0.5;
            const double g_c = 0.70;
            const double b_c = 0.15;
            pixelArray = new int[bitmap.Width + 4, bitmap.Height + 4];

            width = pixelArray.GetLength(0);
            height = pixelArray.GetLength(1);

            for (int x = 3; x < width - 3; x++)
            {
                for (int y = 3; y < height - 3; y++)
                {
                    pixelArray[x, y] = (int)(r_c * bitmap.GetPixel(x - 3, y - 3).R +
                                             g_c * bitmap.GetPixel(x - 3, y - 3).G +
                                             b_c * bitmap.GetPixel(x - 3, y - 3).B);
                }
            }
        }

        private void createImage(Cover cover)
        {
            int[,] imageWithCover = new int[width, height];

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    imageWithCover[x, y] = pixelArray[x - 1, y - 1] * cover.GetCell(0, 0) +
                                          pixelArray[x, y - 1] * cover.GetCell(1, 0) +
                                          pixelArray[x + 1, y - 1] * cover.GetCell(2, 0) +
                                          pixelArray[x - 1, y] * cover.GetCell(0, 1) +
                                          pixelArray[x, y] * cover.GetCell(1, 1) +
                                          pixelArray[x + 1, y] * cover.GetCell(2, 1) +
                                          pixelArray[x - 1, y + 1] * cover.GetCell(0, 2) +
                                          pixelArray[x, y + 1] * cover.GetCell(1, 2) +
                                          pixelArray[x + 1, y + 1] * cover.GetCell(2, 2);
                }
            }

       
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixelArray[x, y] -= imageWithCover[x, y];
                }
            }
        }

        private void delErrorInImage()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (pixelArray[x, y] < 0)
                    {
                        pixelArray[x, y] = 0;
                    }

                    if (pixelArray[x, y] > 255)
                    {
                        pixelArray[x, y] = 255;
                    }
                }
            }
        }

        private Bitmap returnImage()
        {
            Bitmap bitmap = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int color = pixelArray[x, y];

                    bitmap.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }

            return bitmap;
        }
    }
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            string pathName = "1462873261.jpg";
            Bitmap image = new Bitmap(pathName);
            pictureBox1.Image = image;
            Cover cover = new Cover(new int[,] { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } });
            image = new Laplacian().sharpening(image, cover);
            pictureBox2.Image = image;
        }
    }
}
