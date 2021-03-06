﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CourseworkOfTheThirdSemester.IImpactPoint;

namespace CourseworkOfTheThirdSemester
{
    public partial class Form1 : Form
    {
        List<Emitter> emitters = new List<Emitter>();
        Emitter emitter;

        bool radarActive = false;

        private int MousePositionX = 0;
        private int MousePositionY = 0;

        

        ParticleRadar radarParticle = null; 


        public Form1()
        {
            InitializeComponent();

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            this.emitter = new Emitter 
            {
                Direction = 90,
                Spreading = 10,
                SpeedMin = 10,
                SpeedMax = 15,
                ColorFrom = Color.Blue,
                ColorTo = Color.FromArgb(0, Color.Red),
                ParticlesPerTick = 10,
                X = pictureBox1.Width / 2,
                Y = pictureBox1.Height / 2,
            };
            emitters.Add(this.emitter);


            radarParticle = new ParticleRadar
            {
                color = Color.YellowGreen
            };

            lblRadarStatus.ForeColor = Color.Red;

            lblRadarStatus.Text = "Не активен";


        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            int[] counter = new int[4]; //количество частиц из всех эммитеров, которые попали в область действия радара
            emitter.UpdateState();

            using (var g = Graphics.FromImage(pictureBox1.Image))
            {
                g.Clear(Color.Black);
                foreach (var emit in emitters)
                {
                    emit.Render(g);
                    counter = emit.CounterActiveRadar(); //подсчёт частиц, которые попали в область действия радара                 
                }

                var stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                if (counter[0]+counter[1] + counter[2] + counter[3]!=0)
                {   //рисование чисел в области радара (количество частиц попавших в его область действия)
                    g.DrawString($"{counter[0]}\n Большие {counter[3]}\n Средние {counter[2]}\n Маленькие {counter[1]} ",
                    new Font("Verdana", 10),
                    new SolidBrush(Color.White),
                    MousePositionX,
                    MousePositionY,
                    stringFormat);
                }
            }
            pictureBox1.Invalidate();
        }


       

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) 
        {
            emitter.MousePositionX = e.X;
            emitter.MousePositionY = e.Y;

            MousePositionX = e.X;
            MousePositionY = e.Y;

            foreach (var emitter in emitters)
            {
                emitter.MousePositionX = e.X;
                emitter.MousePositionY = e.Y;
            }


            radarParticle.X = e.X;
            radarParticle.Y = e.Y;
        }

        private void tbDirection_Scroll(object sender, EventArgs e)
        {
            emitter.Direction = tbDirection.Value;
            lblDirection.Text = $"{tbDirection.Value}°"; // добавил вывод значения
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            emitter.Spreading = trackBar1.Value;
            lblSpreading.Text = $"{trackBar1.Value}°"; // добавил вывод значения
        }


        private void btnActivateRadar_Click(object sender, EventArgs e)
        {
            radarActive = !radarActive;
           
            if (radarActive)
            {
                lblRadarStatus.ForeColor = Color.Green;
                lblRadarStatus.Text = "Активен";
                foreach (var emit in emitters)
                    emit.impactPoints.Add(radarParticle); //при активации радара, он добавляется
                                                         
            }
            else
            {
                lblRadarStatus.ForeColor = Color.Red;
                lblRadarStatus.Text = "Не активен";
                foreach (var emit in emitters)
                {
                    int index = (-1);
                    for (int i = 0; (i < emit.impactPoints.Count) && (index < 0); i++)
                    {
                        if (emit.impactPoints[i] is ParticleRadar)
                            index = i;
                    }

                    if (index >= 0)
                    {
                        emit.impactPoints.RemoveAt(index);
                    }
                    emit.NoActiveParticle();
                }
            }
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (radarActive)
            {
                int number = e.Delta * SystemInformation.MouseWheelScrollLines / 30;
                
                if ((radarParticle.Power + number) >= 0)
                    radarParticle.Power += number;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
