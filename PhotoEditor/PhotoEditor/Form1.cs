using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PhotoEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Graphics GraphicPicture;
        Bitmap SourceImage;
        List<Layer> Layers = new List<Layer>();
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            GraphicPicture = Graphics.FromImage(pictureBox1.Image);
            GraphicPicture.Clear(Color.White);
            SourceImage = new Bitmap(pictureBox1.Image);

            ReDraw();
            
        }
        public void AddLayer(Bitmap ImageAdd, Point Offset, Point[] FillPoints)
        {
           
            Layers.Add(new Layer
            {
                Id = AllIndex,
                LayerImage = ImageAdd,
                PointsFill = FillPoints,
                Offset = Offset
            });
            AllIndex++;
            ReDraw();
        }
        public void AddLayer(Bitmap ImageAdd)
        {
            List<Point> Points = new List<Point>();
            for(int y = 0; y < ImageAdd.Height; y++)
            {
                for(int x = 0; x< ImageAdd.Width; x++)
                {
                    if(ImageAdd.GetPixel(x,y).A != 0)
                    {
                        Points.Add(new Point(x, y));
                    }
                }
            }
            Layers.Add(new Layer
            {
                Id = Layers.Count + 1,
                LayerImage = ImageAdd,
                PointsFill = Points.ToArray(),
                Offset = new Point(0, 0)
            });
            ReDraw();
        }
        public Bitmap CreateNewLayer()
        {
            return new Bitmap(100, 100);
        }
        public void ReDraw()
        {
            pictureBox1.Image = new Bitmap(SourceImage);
            Graphics gr = Graphics.FromImage(pictureBox1.Image);
            foreach(var layer in Layers)
            {
                gr.DrawImage(layer.LayerImage, layer.Offset.X, layer.Offset.Y);
            }
             gr.Dispose();
        }
        public struct Layer
        {
            public Point[] PointsFill;
            public Bitmap LayerImage;
            public Point Offset;
            public int Id;
        }
        private bool SetIndexObject(MouseEventArgs e)
        {
            for (int i = 0; i< Layers.Count; i++)
            {
                Layer layer = Layers[i];
                if (layer.PointsFill != null)
                {

                    foreach (var point in layer.PointsFill)
                    {

                        if (e.X == point.X && e.Y == point.Y)
                        {
                            CurrentIndexObject = i;
                            Text = layer.Id.ToString();
                            goto LoopEnd;
                        }

                    }
                }
            }
            return false;
        LoopEnd:
            return true;
        }
        private void RemoveLayerFromId(int id)
        {
            for(int i = 0; i < Layers.Count; i++)
            {
                if(Layers[i].Id == id)
                {
                    Layers.RemoveAt(i);
                    break;
                }
            }
        }
        public Point GetOffsetToBoxFromObject(Layer layer)
        {
            return new Point(layer.Offset.X - 30, layer.Offset.Y - 30);
        }
        public Size GetSizeToBoxFromObject(Layer layer)
        {
            return new Size(layer.LayerImage.Width + 50,layer.LayerImage.Height + 50);
        }
        public bool HaveBox()
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Id == -2)
                {
                    return true;
                }
            }
            return false;
        }
        Point[] CordsEditUp;
        Point[] CordsEditDown;
        Point[] CordsEditLeft;
        Point[] CordsEditRight;

        bool EditingRight;
        bool EditingLeft;
        bool EditingUp;
        bool EditingDown;
        public void UpdateBox()
        {
            Layer CurrentLayer = Layers[CurrentIndexObject];

            Layer Box = new Layer();
            Box.Id = -2;
            Box.Offset = GetOffsetToBoxFromObject(CurrentLayer);
            Bitmap BoxImage = new Bitmap(GetSizeToBoxFromObject(CurrentLayer).Width, GetSizeToBoxFromObject(CurrentLayer).Height);
            var gr = Graphics.FromImage(BoxImage);

            Size BoxSize = new Size(BoxImage.Width - 30, BoxImage.Height - 30);
            CordsEditUp = new Point[16 * 16];
            CordsEditDown = new Point[16 * 16];
            CordsEditLeft = new Point[16 * 16];
            CordsEditRight = new Point[16 * 16];

            gr.DrawRectangle(new Pen(Brushes.Red), new Rectangle(new Point(19, 19), BoxSize));
            int Index = 0;
            for(int i = 0; i < 15; i++)
            {
                for(int y = 0; y< 15; y++)
                {
                    CordsEditUp[Index] = new Point(Box.Offset.X + BoxSize.Width / 2 + 11 + i, Box.Offset.Y + 9+y);
                    Index++;
                }
            }
            Index = 0;
            gr.FillEllipse(Brushes.Green, new Rectangle(new Point(BoxSize.Width/2+11, 9), new Size(16, 16)));
            for (int i = 0; i < 16; i++)
            {
                for (int y = 0; y < 16; y++)
                {
                    CordsEditDown[Index] = new Point(Box.Offset.X + BoxSize.Width / 2 + 11 + i, Box.Offset.Y + BoxSize.Height + 19 - 8 + y);
                    Index++;
                }
            }
            Index = 0;
            gr.FillEllipse(Brushes.Green, new Rectangle(new Point(BoxSize.Width / 2 + 11, BoxSize.Height +19-8), new Size(16, 16)));
            for (int i = 0; i < 16; i++)
            {
                for (int y = 0; y < 16; y++)
                {
                    CordsEditLeft[Index] = new Point(Box.Offset.X + BoxImage.Width - BoxSize.Width - 19 + i, Box.Offset.Y + BoxSize.Height / 2 + 19 - 8 + y);
                    Index++;
                }
            }
            Index = 0;
            gr.FillEllipse(Brushes.Green, new Rectangle(new Point(BoxImage.Width - BoxSize.Width - 19 , BoxSize.Height /2 + 19 - 8), new Size(16, 16)));
            for (int i = 0; i < 16; i++)
            {
                for (int y = 0; y < 16; y++)
                {
                    CordsEditRight[Index] = new Point(Box.Offset.X + BoxSize.Width + 19 - 8 + i, Box.Offset.Y + BoxSize.Height / 2 + 19 - 8 + y);
                    Index++;
                }
            }
            Index = 0;
            gr.FillEllipse(Brushes.Green, new Rectangle(new Point(BoxSize.Width +19 - 8, BoxSize.Height / 2 + 19 - 8), new Size(16, 16)));

            Box.LayerImage = BoxImage;
            Box.PointsFill = null;
            RemoveLayerFromId(-2);
            Layers.Add(Box);
        }
        Point StartPosToEdit;
        bool IsEditingObject = false;
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
    
           
        }
        bool IsClick = false;
        Point StartPosition;
        bool IsPainting = false;
        Point StartPointPaint;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {


            if (IsYes(CordsEditUp, e))
            {
                StartPosToEdit = e.Location;
                IsEditingObject = true;

                EditingDown = false;
                EditingUp = true;
                EditingRight = false;
                EditingLeft = false;
                return;
            }
            else if(IsYes(CordsEditLeft, e))
            {
                StartPosToEdit = e.Location;
                IsEditingObject = true;

                EditingDown = false;
                EditingUp = false;
                EditingRight = false;
                EditingLeft = true;
                return;
            }
            else if (IsYes(CordsEditRight, e))
            {
                StartPosToEdit = e.Location;
                IsEditingObject = true;

                EditingDown = false;
                EditingUp = false;
                EditingRight = true;
                EditingLeft = false;
                return;
            }
            else if (IsYes(CordsEditDown, e))
            {
                StartPosToEdit = e.Location;
                IsEditingObject = true;

                EditingDown = true;
                EditingUp = false;
                EditingRight = false;
                EditingLeft = false;
                return;
            }
            if (SetIndexObject(e))
            {
                UpdateBox();
                ReDraw();
            }
            else
            {
                RemoveLayerFromId(-2); //-2  - Box ID
                ReDraw();
            }
            if (!SetIndexObject(e))
            {
                IsPainting = true;
                StartPointPaint = e.Location;
                TempImage = new Bitmap(pictureBox1.Image);
             
            }
            StartPosition = e.Location;
            IsClick = true;
        }
        Image TempImage;
        int AllIndex = 0;
        public bool IsYes(Point[] Cords1, MouseEventArgs e)
        {
            if(Cords1 == null) { return false; }
            foreach (var Cord in Cords1)
            {
                if (Cord.X == e.X && Cord.Y == e.Y)
                {
                    return true;
                }
            }
            return false;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (IsEditingObject)
            {

                int DeltaY = e.Y - StartPosToEdit.Y ;

                int DeltaX = StartPosToEdit.X - e.X;
                if (EditingLeft || EditingRight) DeltaY = 0;
                if (EditingDown || EditingUp) DeltaX = 0;

                Layer CurrentLayer1 = Layers[CurrentIndexObject];
                if (CurrentLayer1.LayerImage.Height + DeltaY <= 0 || CurrentLayer1.LayerImage.Width + DeltaX <= 0 || CurrentLayer1.LayerImage.Width - DeltaX <= 0)
                {
                    return;
                }
                Bitmap NewLayerImage = null;
                if (EditingDown)
                {
                    NewLayerImage = new Bitmap(CurrentLayer1.LayerImage.Width, CurrentLayer1.LayerImage.Height + DeltaY);

                }
                else if (EditingUp)
                {
                    DeltaY = StartPosToEdit.Y - e.Y;
                    if (CurrentLayer1.LayerImage.Height + DeltaY <= 0) return;
                    NewLayerImage = new Bitmap(CurrentLayer1.LayerImage.Width, CurrentLayer1.LayerImage.Height + DeltaY);
                     CurrentLayer1.Offset.Y -= DeltaY;
                }
                else if (EditingLeft)
                {
                    NewLayerImage = new Bitmap(CurrentLayer1.LayerImage.Width + DeltaX, CurrentLayer1.LayerImage.Height);
                    CurrentLayer1.Offset.X -= DeltaX;
                }
                else if (EditingRight)
                {
                    NewLayerImage = new Bitmap(CurrentLayer1.LayerImage.Width - DeltaX, CurrentLayer1.LayerImage.Height);

                }
                Graphics.FromImage(NewLayerImage).Clear(Color.Black);
                CurrentLayer1.LayerImage = NewLayerImage;

              
           
                Layers[CurrentIndexObject] = CurrentLayer1;
                UpdateBox();
                StartPosToEdit = e.Location;

                ReDraw();
                return;
            }
            
            if (!IsClick) return;
            if (IsPainting)
            {
                int Width = Math.Max(StartPointPaint.X, e.X) - Math.Min(StartPointPaint.X, e.X);
                int Height = Math.Max(StartPointPaint.Y, e.Y) - Math.Min(StartPointPaint.Y, e.Y);
                pictureBox1.Image = new Bitmap(TempImage);
                GraphicPicture = Graphics.FromImage(pictureBox1.Image);
                GraphicPicture.FillRectangle(Brushes.Black, new Rectangle(new Point(Math.Min(StartPointPaint.X, e.X), Math.Min(StartPointPaint.Y, e.Y)), new Size(Width, Height)));
             
                return;
            }
            if(CurrentIndexObject < 0) { return; }
            var CurrentLayer = Layers[CurrentIndexObject];
            Point OffsetLocal = new Point(StartPosition.X - CurrentLayer.PointsFill[0].X, StartPosition.Y - CurrentLayer.PointsFill[0].Y);
            CurrentLayer.Offset = new Point(e.X - OffsetLocal.X,e.Y -OffsetLocal.Y);
            
            Layers[CurrentIndexObject] = CurrentLayer;
            if (HaveBox())
            {
                UpdateBox();
            }
            ReDraw();
        }
        int CurrentIndexObject = -1;

        /// <summary>
        /// Жрёт очень много ресурсов! Не вызывать много раз подряд!
        /// </summary>
        /// <param name="SourceLayer"></param>
        public void UpdatePoints(Layer SourceLayer)
        {
            List<Point> Points = new List<Point>();
            for (int y = 0; y < SourceLayer.LayerImage.Height; y++)
            {
                for (int x = 0; x < SourceLayer.LayerImage.Width; x++)
                {
                    if (SourceLayer.LayerImage.GetPixel(x, y).A != 0)
                    {
                        Points.Add(new Point(x+SourceLayer.Offset.X, y+SourceLayer.Offset.Y));
                    }
                }
            }
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Id == SourceLayer.Id)
                {
                    Layers[i] = new Layer
                    {
                        LayerImage = SourceLayer.LayerImage,
                        Offset = SourceLayer.Offset,
                        Id = SourceLayer.Id,
                        PointsFill = Points.ToArray()
                    };
                    break;
                }
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            IsClick = false;

            if (IsEditingObject)
            {


                int DeltaY = StartPosToEdit.Y - e.Y;
                int DeltaX = StartPosToEdit.X - e.X;
                if (EditingLeft || EditingRight) DeltaY = 0;
                if (EditingDown || EditingUp) DeltaX = 0;

                Layer CurrentLayer1 = Layers[CurrentIndexObject];
                if (CurrentLayer1.LayerImage.Height + DeltaY <= 0 || CurrentLayer1.LayerImage.Width + DeltaX <= 0)
                {
                    return;
                }
                Bitmap NewLayerImage = new Bitmap(CurrentLayer1.LayerImage.Width + DeltaX, CurrentLayer1.LayerImage.Height + DeltaY);
                Graphics.FromImage(NewLayerImage).Clear(Color.Black);
                CurrentLayer1.LayerImage = NewLayerImage;

                if (EditingUp) CurrentLayer1.Offset.Y -= DeltaY;
                if (EditingDown) CurrentLayer1.Offset.Y += DeltaY;
                if (EditingRight) CurrentLayer1.Offset.X -= DeltaX;
                if (EditingLeft) CurrentLayer1.Offset.X += DeltaX;
                UpdatePoints(CurrentLayer1);
                UpdateBox();

                StartPosToEdit = e.Location;

                ReDraw();
                IsEditingObject = false;
                return;
            }

            if (IsPainting)
            {
                List<Point> FillPoints = new List<Point>();
                int Width = Math.Max(StartPointPaint.X, e.X) - Math.Min(StartPointPaint.X, e.X);
                int Height = Math.Max(StartPointPaint.Y, e.Y) - Math.Min(StartPointPaint.Y, e.Y);
                if(Width == 0 || Height == 0) { return; }
                var ImageR = new Bitmap(Width, Height);
                for(int x = 0; x < Width; x++)
                {
                    for(int y = 0; y < Height; y++)
                    {
                        FillPoints.Add(new Point(x + Math.Min(StartPointPaint.X, e.X), y + Math.Min(StartPointPaint.Y, e.Y)));
                    }
                }
                Graphics.FromImage(ImageR).Clear(Color.Black);
                AddLayer(ImageR, new Point(Math.Min(StartPointPaint.X, e.X), Math.Min(StartPointPaint.Y, e.Y)), FillPoints.ToArray());
                IsPainting = false;
                return;
            }
            if (CurrentIndexObject != -1)
            {
                UpdatePoints(Layers[CurrentIndexObject]);
            }
            ReDraw();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Bitmap layer = CreateNewLayer();
            GraphicPicture = Graphics.FromImage(layer);
            GraphicPicture.FillRectangle(Brushes.Black, new Rectangle(0, 0, 100, 100));
            AddLayer(layer);
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            GraphicPicture = Graphics.FromImage(pictureBox1.Image);
            GraphicPicture.Clear(Color.White);
            SourceImage = new Bitmap(pictureBox1.Image);

            ReDraw();
        }
    }
    

}
