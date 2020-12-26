using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;//for working mouse and buttons
using Emgu.CV;
using Emgu.CV.Structure;

namespace BFAFv2
{

    public partial class Form1 : Form
    {
        #region options_of_program
        Stopwatch stopwatch = new Stopwatch();//debug for count
        int state_of_fishing = -1;//состояние процесса рыбалки
        /*
         * 2 - мини-игра
         */
        Point pos_of_match = new Point(0, 0);
        Point pos_of_match_poplavok = new Point(0, 0);
        bool mouse_pressed_now = false;
        int MaxPos_Of_Match_Y=0;
        bool prev_state_fishing = false;
        #endregion

        #region OPENCV_options

        //string path_for_files = "C:\\Users\\Andrey\\Desktop\\";
        string path = "D:\\";

        #endregion

        #region mouse_options
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        #endregion

        #region CV
        public void Find_matches(string where, string what, float percent)
        {
            // Запускаем внутренний таймер объекта Stopwatch
            stopwatch.Start();
            Image<Bgr, byte> source = new Image<Bgr, byte>(path + where + ".bmp"); // Image A где ищем, картинка рыбалки
            Image<Bgr, byte> template = new Image<Bgr, byte>(path + what + ".bmp"); // Image B что ищем
            Image<Bgr, byte> imageToShow = source.Copy();
            using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                // You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
                if (maxValues[0] > percent)//0.9
                {
                    // This is a match. Do something with it, for example draw a rectangle around it.
                    Rectangle match = new Rectangle(maxLocations[0], template.Size);
                    imageToShow.Draw(match, new Bgr(Color.Red), 3);
                    pos_of_match = maxLocations[0];
                    label1.Text = "Позиция совпадения: X - " + pos_of_match.X + " | Y - " + pos_of_match.Y;
                }
                else
                {
                    if (what != "fish_is_mine")
                    {
                        label1.Text = "Совпадений не найдено";
                    }
                }
                label2.Text = "Процент совпадения: " + Math.Round(maxValues[0], 3).ToString();
            }
            imageToShow.Save(path + "final.bmp");
            stopwatch.Stop();// Останавливаем внутренний таймер объекта Stopwatch
            label4.Text = "Время опознания: " + (stopwatch.ElapsedMilliseconds).ToString() + " мс";//time of exec CV
            stopwatch.Reset();
        }
        #endregion

        public Form1()
        {
            TopMost = true;
            InitializeComponent();
        }

        public void MyMouseMove(int x, int y)
        {
            PointConverter pc = new PointConverter();
            System.Drawing.Point pt = new System.Drawing.Point(x, y);
            Cursor.Position = pt;
        }

        public void Mouse_press(int state)
        {
            if (state == 0)
            {
                //Call the imported function with the cursor's current position
                uint X = (uint)Cursor.Position.X;
                uint Y = (uint)Cursor.Position.Y;
                mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
                //Thread.Sleep(time); 
                //mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                mouse_pressed_now = true;
            }
            else
            if (state == 1)
            {
                //Call the imported function with the cursor's current position
                uint X = (uint)Cursor.Position.X;
                uint Y = (uint)Cursor.Position.Y;
                mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                mouse_pressed_now = false;
            }

        }

        private void Get_screenshot(int x1, int y1, int x2, int y2)
        {
            Bitmap printscreen = new Bitmap(x2 - x1, y2 - y1/*Screen.PrimaryScreen.Bounds.Height*/);

            Graphics graphics = Graphics.FromImage(printscreen as Image);

            graphics.CopyFromScreen(x1, y1, 0, 0, printscreen.Size);

            printscreen.Save(path + "screenshot.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //Close();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (state_of_fishing == 1)
            {

            }
            else
            {
                timer1.Enabled = false;
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (state_of_fishing == 2)
            {

            }
            else
            {
                timer2.Enabled = false;
            }
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            if (state_of_fishing == 3)
            {

            }
            else
            {
                timer3.Enabled = false;
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (state_of_fishing == 4)
            {

            }
            else
            {
                timer4.Enabled = false;
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            pos_of_match.X = 0;
            pos_of_match.Y = 0;

            if (state_of_fishing >= 1)
            {
                #region 1poplavok
                Get_screenshot(1010, 130, 1120, 220);                   //скрин поплавка - есть ли он вообще
                Find_matches("screenshot", "poplavok_new", 0.5f);
                //Close();
                if (pos_of_match.X > 0)                                 //если нашел поплавок
                {
                    if (pos_of_match.Y>MaxPos_Of_Match_Y)//если сейчас поплавок ниже, то записываем это значение в "максимальное"
                    {
                        MaxPos_Of_Match_Y = pos_of_match.Y;
                        label5.Text = MaxPos_Of_Match_Y.ToString();

                    }
                    pos_of_match_poplavok.X = pos_of_match.X;           //найден поплавок
                    if (checkBox1.Checked == false)
                    {
                        checkBox1.Checked = true;                       //зажигаем лампочку
                    }
                    //state_of_fishing = 1;
                    prev_state_fishing = true;//мы только что ловили рыбу

                }
                else//иначе если не нашел поплавок
                {
                    if (checkBox1.Checked == true)
                    {
                        checkBox1.Checked = false;                      //тушим лампочку
                    }
                    pos_of_match_poplavok.X = 0;                        //поплавок мы не нашли
                    prev_state_fishing = false;//мы не ловили рыбу
                }
                pos_of_match.X = 0;
                #endregion

                #region 2klev
                Get_screenshot(1010, 130, 1120, 220);                   //скрин клёва
                Find_matches("screenshot", "poplavok_new", 0.7f);
                //if (pos_of_match.Y > 25 /*&& pos_of_match_poplavok.X == 0 && prev_state_fishing == true*/) //если точно не увидел в прошлую проверку и точно увидел сейчас, значит ушел под воду!!!
                //{
                if (MaxPos_Of_Match_Y - pos_of_match.Y > 13)
                {
                    
                    if (checkBox2.Checked == false)
                    {
                        checkBox2.Checked = true;
                    }
                    state_of_fishing = 1;
                    prev_state_fishing = false;
                }
                else
                {
                    if (checkBox2.Checked == true)
                    {
                        checkBox2.Checked = false;
                    }
                }
                pos_of_match.X = 0;
                #endregion

                #region 3mini-game
                Get_screenshot(850, 520, 1070, 570);//скрин мини-игры
                Find_matches("screenshot", "mini-game", 0.7f);
                //Close();
                if (pos_of_match.X > 0)
                {
                    if (checkBox3.Checked == false)
                    {
                        checkBox3.Checked = true;
                    }
                    state_of_fishing = 1;
                }
                else
                {
                    if (checkBox3.Checked == true)
                    {
                        checkBox3.Checked = false;
                    }
                }
                pos_of_match.X = 0;
                #endregion

                #region 4finish
                Get_screenshot(767, 66, 1151, 88);//скрин выигрыша
                Find_matches("screenshot", "fish_is_mine", 0.7f);
                //Close();
                if (pos_of_match.X > 0)
                {
                    if (checkBox4.Checked == false)
                    {
                        checkBox4.Checked = true;
                    }
                    state_of_fishing = 1;
                }
                else
                {
                    if (checkBox4.Checked == true)
                    {
                        checkBox4.Checked = false;
                    }
                }
                pos_of_match.X = 0;
                #endregion
            }
            else
            {
                timer5.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start_fishing();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Stop_fishing();
        }

        private void Start_fishing()
        {
            state_of_fishing = 5;//fishing_state        start
            timer5.Enabled = true;//fishing process     start 
            button1.Enabled = false;//button start      false
            button2.Enabled = true;//button stop        true
        }

        private void Stop_fishing()
        {
            MaxPos_Of_Match_Y = 0;
            timer5.Enabled = false;
            //timer1.Enabled = false;//cursor pos         stop
            state_of_fishing = 0;//proc of fish        stop
            button1.Enabled = true;//but start          enabled
            button2.Enabled = false;//but stop          disable
            //timer3.Enabled = false;//timer of fishing   stop
            label6.Text = "Ждем...";
        }
    }
}
