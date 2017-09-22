using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool[,] struArr;
        private bool[,] struArr2;
        private Color blockColor = Color.Black;
        Thread t;
        bool isMouseDown=false;
        int oldx, oldy, boldx, boldy;
        int area = 11;
        int height = 40,width=60;
        Graphics Gpicbox;
        Rectangle Recpicbox;
        int interval;
        Random r = new Random();
        /*
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            gp.Clear(Color.Black);
            Pen p = new Pen(Color.White);
            SolidBrush s = new SolidBrush(Color.White);
            for (int i = area; i < 551; i = i + area)
                gp.DrawLine(p, 1, i, 550, i);
            for (int i = area; i < 551; i = i + area)
                gp.DrawLine(p, i, 1, i, 550);
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            interval = 1000 / Convert.ToInt16(textBox1.Text);
            if (button1.Text == "开始")
            {
                button2.Enabled = false;
                textBox1.ReadOnly = true;
                t = new Thread(new ThreadStart(run));
                if (t.ThreadState == System.Threading.ThreadState.Unstarted)
                    t.Start();
                button1.Text = "停止";
            }
            else
            {
                button2.Enabled = true;
                textBox1.ReadOnly = false;
                t.Abort();
                button1.Text = "开始";
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int xPos, yPos;
            xPos = e.X / area;
            yPos = e.Y / area;
            if (xPos > width-1 || xPos < 0 || yPos > height-1 || yPos < 0)
                return;
            struArr[xPos, yPos] = !struArr[xPos, yPos];
            bool b = struArr[xPos, yPos];
            SolidBrush s = new SolidBrush(b ? blockColor : Color.White);
            Gpicbox.FillRectangle(s, area * xPos + 1, area * yPos + 1, area-1, area-1);
        }


        void run()
        {
            while (true)
            {
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        struArr2[i, j] = struArr[i, j];

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        int count = 0;
                        if (struArr[i - 1==-1?width-1:i-1, j - 1==-1?height-1:j-1] == true) { count++; }
                        if (struArr[i - 1==-1?width-1:i-1, j] == true) { count++; }
                        if (struArr[i - 1==-1?width-1:i-1, j + 1==height?0:j+1] == true) { count++; }
                        if (struArr[i, j - 1==-1?height-1:j-1] == true) { count++; }
                        if (struArr[i, j + 1==height?0:j+1] == true) { count++; }
                        if (struArr[i + 1==width?0:i+1, j - 1==-1?height-1:j-1] == true) { count++; }
                        if (struArr[i + 1==width?0:i+1, j] == true) { count++; }
                        if (struArr[i + 1==width?0:i+1, j + 1==height?0:j+1] == true) { count++; }
                        switch (count)
                        {
                            case 3:
                                struArr2[i, j] = true;
                                break;
                            case 2:
                                if (struArr[i, j] == true)
                                { struArr2[i, j] = true; }
                                break;
                            default:
                                struArr2[i, j] = false;
                                break;
                        }
                    }
                using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(Gpicbox, Recpicbox))
                using(Graphics g=bg.Graphics)
                {
                    g.FillRectangle(Brushes.Silver, Recpicbox);
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                        {
                            using (SolidBrush s = new SolidBrush(struArr2[i, j] ? blockColor : Color.White))
                            //using (SolidBrush s = new SolidBrush(struArr2[i, j] ? Color.FromArgb(r.Next(255),r.Next(255),r.Next(255)) : Color.White))
                            {
                                g.FillRectangle(s, area * i + 1, area * j + 1, area - 1, area - 1);
                            }
                        }
                    bg.Render(Gpicbox);
                }
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        struArr[i, j] = struArr2[i, j];
                Thread.Sleep(interval);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader("config.txt");
            string[] tempstr=sr.ReadLine().Split(',');
            sr.Close();
            height = Convert.ToInt16(tempstr[0]);
            width = Convert.ToInt16(tempstr[1]);
            area = Convert.ToInt16(tempstr);
            timer2.Interval = 1;
            timer2.Start();
            pictureBox1.Size = new Size(width * area + 5, height * area + 5);
            button1.Location = new Point((width * area -130) / 2, height * area + 20);
            button2.Location = new Point(button1.Location.X + 150, button1.Location.Y);
            label1.Location = new Point(button1.Location.X - 90, button1.Location.Y + 9);
            textBox1.Location = new Point(button1.Location.X - 55, button1.Location.Y + 6);
            this.Size = new Size(width * area + 25, height * area + 90);
            struArr = new bool[width, height];
            struArr2 = new bool[width, height];
            Gpicbox = pictureBox1.CreateGraphics();
            Recpicbox = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1_Load(null, null);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                int xPos, yPos;
                xPos = e.X / area;
                yPos = e.Y / area;
                if (xPos == oldx && yPos == oldy)
                    return;
                if (xPos > width - 1 || xPos < 0 || yPos > height - 1 || yPos < 0)
                    return;
                oldx = xPos;
                oldy = yPos;
                struArr[xPos, yPos] = !struArr[xPos, yPos];
                bool b = struArr[xPos, yPos];
                Graphics gp = pictureBox1.CreateGraphics();
                SolidBrush s = new SolidBrush(b ? blockColor : Color.White);
                gp.FillRectangle(s, area * xPos + 1, area * yPos + 1, area - 1, area - 1);
                gp.Dispose();
            }
            else
            {
                Graphics gp = pictureBox1.CreateGraphics();
                Pen p = new Pen(Color.Black);
                Pen clearp = new Pen(Color.Silver,1f);
                int xPos, yPos;
                xPos = e.X / area;
                yPos = e.Y / area;
                if(xPos!=boldx || yPos!=boldy)
                    gp.DrawRectangle(clearp, area * boldx, area * boldy, area, area);
                boldx = xPos;
                boldy = yPos;
                if (xPos > width - 1 || xPos < 0 || yPos > height - 1 || yPos < 0)
                    return;
                gp.DrawRectangle(p, area * xPos, area * yPos, area, area);
                gp.Dispose();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    struArr2[i, j] = false;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    SolidBrush s = new SolidBrush(Color.White);
                    Gpicbox.FillRectangle(s, area * i + 1, area * j + 1, area - 1, area - 1);
                }
            timer2.Stop();
        }
    }
}
