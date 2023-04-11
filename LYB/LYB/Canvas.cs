using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Diagnostics;

namespace LYB
{
    public class Canvas
    {
        public Bitmap bmp;
        public Graphics g;
        public int Width, Height;
        public byte[] bits;
        int pixelFormatSize, stride;
        public Bitmap backgroundImage;


        public Canvas(Size size)
        {
            Init(size);
        }

        private void Init(Size size)
        {
            PixelFormat format;
            GCHandle handle;
            IntPtr bitPtr;
            int padding;

            format = PixelFormat.Format32bppArgb;
            Width = size.Width;
            Height = size.Height;
            pixelFormatSize = Image.GetPixelFormatSize(format) / 8; // 8 bits = 1 byte
            stride = size.Width * pixelFormatSize;                  // total pixels (width) times ARGB (4)
            padding = (stride % 4);                                 // PADD = move every pixel in bytes
            stride += padding == 0 ? 0 : 4 - padding;               // pad out to multiple of 4 byte Alpha, Red, Green, Blue
            bits = new byte[stride * size.Height];
            handle = GCHandle.Alloc(bits, GCHandleType.Pinned);     // TO LOCK THE MEMORY FOR GB
            bitPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bits, 0);
            bmp = new Bitmap(size.Width, size.Height, stride, format, bitPtr);

            g = Graphics.FromImage(bmp);
            backgroundImage = new Bitmap(Resource1.space);

            
            
        }

        public void DrawMap(int[,] map, List<VPoint> balls, List<VBox> boxes)
        {
            //g.DrawImage(backgroundImage, 0, 0, Width, Height);
            boxes.Add(new VBox(650, 5, 500, 10, balls.Count)); // Techo
            balls.Add(boxes[boxes.Count - 1].a);
            balls.Add(boxes[boxes.Count - 1].b);
            balls.Add(boxes[boxes.Count - 1].c);
            balls.Add(boxes[boxes.Count - 1].d);

            boxes.Add(new VBox(900, 300, 10, 600, balls.Count)); // Pared derecha
            balls.Add(boxes[boxes.Count - 1].a);
            balls.Add(boxes[boxes.Count - 1].b);
            balls.Add(boxes[boxes.Count - 1].c);
            balls.Add(boxes[boxes.Count - 1].d);

            boxes.Add(new VBox(870, 595, 60, 10, balls.Count)); // Base pinball
            balls.Add(boxes[boxes.Count - 1].a);
            balls.Add(boxes[boxes.Count - 1].b);
            balls.Add(boxes[boxes.Count - 1].c);
            balls.Add(boxes[boxes.Count - 1].d);

           /* boxes.Add(new VBox(835, 525, 10, 150, balls.Count)); // Pilar pinball
            balls.Add(boxes[boxes.Count - 1].a);
            balls.Add(boxes[boxes.Count - 1].b);
            balls.Add(boxes[boxes.Count - 1].c);
            balls.Add(boxes[boxes.Count - 1].d);*/

            boxes.Add(new VBox(395, 300, 10, 600, balls.Count)); // Pared izquierda
            balls.Add(boxes[boxes.Count - 1].a);
            balls.Add(boxes[boxes.Count - 1].b);
            balls.Add(boxes[boxes.Count - 1].c);
            balls.Add(boxes[boxes.Count - 1].d);
            
            //Random rand = new Random();
            /*for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == 2) // Wall
                    {
                        //balls.Add(new VPoint((x * 15) + 350, y * 15, balls.Count, true));
                        boxes.Add(new VBox(x * 17, y * 18, 5, 5, balls.Count));
                        balls.Add(boxes[boxes.Count - 1].a);
                        balls.Add(boxes[boxes.Count - 1].b);
                        balls.Add(boxes[boxes.Count - 1].c);
                        balls.Add(boxes[boxes.Count - 1].d);
                    }
                    else if (map[y, x] == 3) // Pinball
                    {
                        balls.Add(new VPoint(x * 17, y * 17, balls.Count));
                    }
                    else if (map[y, x] == 4) // Bumper
                    {
                        boxes.Add(new VBox(x * 17, y * 17, rand.Next(20, 30), rand.Next(20, 30), balls.Count));
                        balls.Add(boxes[boxes.Count - 1].a);
                        balls.Add(boxes[boxes.Count - 1].b);
                        balls.Add(boxes[boxes.Count - 1].c);
                        balls.Add(boxes[boxes.Count - 1].d);
                    }
                }
            }*/
        }

        public void LessFast(List <VPoint> balls)
        {
            int div = 16;
            Parallel.For(0, bits.Length / div, i => // unrolling 
            {
                bits[(i * div) + 0] = 0;
                bits[(i * div) + 1] = 0;
                bits[(i * div) + 2] = 0;
                bits[(i * div) + 3] = 0;

                bits[(i * div) + 4] = 0;
                bits[(i * div) + 5] = 0;
                bits[(i * div) + 6] = 0;
                bits[(i * div) + 7] = 0;

                bits[(i * div) + 8] = 0;
                bits[(i * div) + 9] = 0;
                bits[(i * div) + 10] = 0;
                bits[(i * div) + 11] = 0;

                bits[(i * div) + 12] = 0;
                bits[(i * div) + 13] = 0;
                bits[(i * div) + 14] = 0;
                bits[(i * div) + 15] = 0;
            }
            );
        }

        public void FastClear()
        {
            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, (y,state) => // podría implementar unroll para agilizar y estabilizar                 
                {
                    byte* bits = PtrFirstPixel + (y* bitmapData.Stride);              
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        bits[x + 0] = 0;// (byte)Blue;
                        bits[x + 1] = 0;// (byte)Green;
                        bits[x + 2] = 0;// (byte)Red;
                        bits[x + 3] = 0;// (byte)Red;
                    }
                }
                );
                
                bmp.UnlockBits(bitmapData);
            }
        }
    }
}
