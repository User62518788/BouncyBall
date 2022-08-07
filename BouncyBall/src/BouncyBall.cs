using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BouncyBalls {
	struct Ball {
		private int radius;
		private double[] position;
		private double[] velocity;
		private Color color;
		private Form form;
		
		public Ball(int radius, double[] position, double[] velocity, Color color, Form form) {
			this.radius = radius;
			this.position = position;
			this.velocity = velocity;
			this.color = color;
			this.form = form;
		}
		
		public void DrawBall(Graphics graphics) {
			Brush brush = new SolidBrush(color);
			graphics.FillEllipse(brush, (int)(position[0] - radius/2), (int)(position[1] - radius/2), radius, radius);
			
			Update();
		}
		
		private void Update() {
			//X axis Physics
			
			//Left and Right border
			if (position[0] - radius/2 <= 0 || position[0] + radius/2 >= form.Size.Width) {
				velocity[0] = -(velocity[0] * 0.99);
			}
			
			//Air resistance
			if (velocity[0] > 0) {
				velocity[0] -= 0.02;
			} else if (velocity[0] < 0) {
				velocity[0] += 0.02;
			}
			
			//Update
			
			//Remove small velocity
			if (velocity[0] < 0.025 && velocity[0] > -0.025) {
				velocity[0] = 0;
			}
			
			//Right Border
			if (this.position[0] + this.velocity[0] + this.radius/2 <= form.Size.Width) {
				this.position[0] += velocity[0];
			} else if (this.position[0] + this.velocity[0] + this.radius/2 >= form.Size.Width) {
				this.position[0] = form.Size.Width - this.radius/2;
			}
			
			//Left Border
			if (this.position[0] + this.velocity[0] - this.radius/2 >= 0) {
				this.position[0] += velocity[0];
			} else if (this.position[0] + this.velocity[0] - this.radius/2 <= 0) {
				this.position[0] = this.radius/2;
			}
			
			//Y axis Physics
			
			//Ground + Gravity
			if (position[1] + radius >= form.Size.Height) {
				velocity[1] = -((int)(velocity[1] * 0.9));
			} else {
				velocity[1] += 1;
			}
			
			//Update
			if ((this.position[1] + this.velocity[1] + 1 + this.radius) <= form.Size.Height) {
				this.position[1] += this.velocity[1];
			} else {
				this.position[1] = (form.Size.Height - this.radius);
			}
		}
	}
	
	class Display {
		private Form form;
		private string title;
		private int[] size;
		
		private Button buttonReset;
		private PictureBox pictureBox;
		
		private Thread ShowThread;
		
		private List<Ball> balls = new List<Ball>();
		private int maxBalls = 100;
		
		public Display(string title, int[] size) {
			this.title = title;
			this.size = size;
			
			InitializeComponents();
		}
		
		private void InitializeComponents() {
			//Components
			form = new Form();
			buttonReset = new Button();
			pictureBox = new PictureBox();
			
			//Properties
			form.Text = title;
			form.Size = new Size(size[0], size[1]);
			form.MaximizeBox = false;
			form.FormBorderStyle = FormBorderStyle.FixedSingle;
			
			buttonReset.Text = "Reset";
			buttonReset.Size = new Size(100, 50);
			buttonReset.Location = new Point(form.ClientSize.Width/2 - 50, 10);
			buttonReset.Font = new Font("Consolas", 12);
			
			pictureBox.Size = new Size(form.Size.Width, form.Size.Height);
			
			//Events
			buttonReset.Click += new EventHandler(ButtonReset_Click);
			pictureBox.Click += new EventHandler(PictureBox_Click);
			pictureBox.Paint += new PaintEventHandler(PictureBox_Paint);
			
			//Controls
			form.Controls.Add(buttonReset);
			form.Controls.Add(pictureBox);
		}
		
		public void Start() {
			//Create threads to prevent program stopping
			ShowThread = new Thread(Show);
			Thread RenderThread = new Thread(Render);
			
			ShowThread.Start();
			RenderThread.Start();
		}
		
		private void PictureBox_Click(object sender, EventArgs e) {
			MouseEventArgs e2 = (MouseEventArgs)e;
			
			Random random = new Random();
			Color color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
			
			int radius = 50;
			double[] position = new double[]{e2.X, e2.Y};
			double[] velocity = new double[]{0, 0};
			
			//Left or Right velocity
			if (random.Next(0, 10) > 5) {
				velocity = new double[]{random.Next(4, 8), 0};
			} else {
				velocity = new double[]{random.Next(-8, -4), 0};
			}
			
			Ball ball = new Ball(radius, position, velocity, color, form);
			balls.Add(ball);
			
			if (balls.Count > maxBalls) {
				balls.Remove(balls[0]);
			}
		}
		
		private void PictureBox_Paint(object sender, PaintEventArgs e) {
			foreach (Ball item in balls) {
				item.DrawBall(e.Graphics);
			}
		}
		
		private void ButtonReset_Click(object sender, EventArgs e) {
			balls.Clear();
		}
		
		private void Show() {
			Application.Run(form);
		}
		
		private void Render() {
			while (ShowThread.ThreadState == ThreadState.Running) {
				//Sleep to prevent render speed depending on computer run speed
				Thread.Sleep(12);
				pictureBox.Refresh();
			}
		}
	}
	
	class Program {
		private static void Main() {
			string title = "BouncyBalls";
			int[] size = {600, 600};
			
			Display display = new Display(title, size);
			display.Start();
		}
	}
}