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

namespace Car_Racing_Game_MOO_ICT
{
    public partial class Form1 : Form
    {

        int roadSpeed;
        int trafficSpeed;
        int playerSpeed = 12;
        int score;
        int carImage;

        Random rand = new Random();
        Random carPosition = new Random();

        bool goleft, goright;

        DatabaseHelper db;

        // Добавьте рядом с другими полями класса, например под Random rand = new Random();
        private Label lblName;
        private TextBox txtPlayerName;


        public Form1()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(keyisdown);
            this.KeyUp += new KeyEventHandler(keyisup);

            lblName = new Label();
            lblName.Name = "lblName";
            lblName.Text = "Имя игрока:";
            lblName.AutoSize = true;
            lblName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            lblName.ForeColor = Color.White; // если фон тёмный — иначе Color.Black
            lblName.Location = new Point(10, 10); // координаты на форме
            this.Controls.Add(lblName);
            lblName.BringToFront();

            // TextBox
            txtPlayerName = new TextBox();
            txtPlayerName.Name = "txtPlayerName";
            txtPlayerName.MaxLength = 50;
            txtPlayerName.Size = new Size(160, 26);
            txtPlayerName.Location = new Point(lblName.Right + 8, 8);
            this.Controls.Add(txtPlayerName);
            txtPlayerName.BringToFront();

            // Если у вас есть btnStart — сдвиньте его, чтобы не перекрывать TextBox (пример):
            if (this.btnStart != null)
            {
                //this.btnStart.Location = new Point(txtPlayerName.Right + 12, 8);
                this.btnStart.BringToFront();
            }

            db = new DatabaseHelper();
            try
            {
                db.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось инициализировать базу данных: " + ex.Message, "DB error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ResetGame();
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goleft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = true;
            }

        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goleft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = false;
            }
        }

        private void gameTimerEvent(object sender, EventArgs e)
        {

            txtScore.Text = "Score: " + score;
            score++;


            if (goleft == true && player.Left > 10)
            {
                player.Left -= playerSpeed;
            }
            if (goright == true && player.Left < 415)
            {
                player.Left += playerSpeed;
            }

            roadTrack1.Top += roadSpeed;
            roadTrack2.Top += roadSpeed;

            if (roadTrack2.Top > 519)
            {
                roadTrack2.Top = -519;
            }
            if (roadTrack1.Top > 519)
            {
                roadTrack1.Top = -519;
            }

            AI1.Top += trafficSpeed;
            AI2.Top += trafficSpeed;


            if (AI1.Top > 530)
            {
                changeAIcars(AI1);
            }

            if (AI2.Top > 530)
            {
                changeAIcars(AI2);
            }

            if (player.Bounds.IntersectsWith(AI1.Bounds) || player.Bounds.IntersectsWith(AI2.Bounds))
            {
                gameOver();
            }

            if (score > 40 && score < 500)
            {
                award.Image = Properties.Resources.bronze;
            }


            if (score > 500 && score < 2000)
            {
                award.Image = Properties.Resources.silver;
                roadSpeed = 20;
                trafficSpeed = 22;
            }

            if (score > 2000)
            {
                award.Image = Properties.Resources.gold;
                trafficSpeed = 27;
                roadSpeed = 25;
            }


        }

        private void changeAIcars(PictureBox tempCar)
        {

            carImage = rand.Next(1, 9);

            switch (carImage)
            {

                case 1:
                    tempCar.Image = Properties.Resources.ambulance;
                    break;
                case 2:
                    tempCar.Image = Properties.Resources.carGreen;
                    break;
                case 3:
                    tempCar.Image = Properties.Resources.carGrey;
                    break;
                case 4:
                    tempCar.Image = Properties.Resources.carOrange;
                    break;
                case 5:
                    tempCar.Image = Properties.Resources.carPink;
                    break;
                case 6:
                    tempCar.Image = Properties.Resources.CarRed;
                    break;
                case 7:
                    tempCar.Image = Properties.Resources.carYellow;
                    break;
                case 8:
                    tempCar.Image = Properties.Resources.TruckBlue;
                    break;
                case 9:
                    tempCar.Image = Properties.Resources.TruckWhite;
                    break;
            }


            tempCar.Top = carPosition.Next(100, 400) * -1;


            if ((string)tempCar.Tag == "carLeft")
            {
                tempCar.Left = carPosition.Next(5, 200);
            }
            if ((string)tempCar.Tag == "carRight")
            {
                tempCar.Left = carPosition.Next(245, 422);
            }
        }

        private void gameOver()
        {
            playSound();
            gameTimer.Stop();
            explosion.Visible = true;
            player.Controls.Add(explosion);
            explosion.Location = new Point(-8, 5);
            explosion.BackColor = Color.Transparent;

            award.Visible = true;
            award.BringToFront();

            btnStart.Enabled = true;

            // Сохранение результата в БД
            try
            {
                string playerName = txtPlayerName?.Text?.Trim() ?? "Anonymous";
                db.InsertScore(playerName, score);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении результата: " + ex.Message, "DB error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Показ таблицы лидеров
            ShowLeaderboard();
        }


        private void ResetGame()
        {

            btnStart.Enabled = false;
            explosion.Visible = false;
            award.Visible = false;
            goleft = false;
            goright = false;
            score = 0;
            award.Image = Properties.Resources.bronze;

            roadSpeed = 12;
            trafficSpeed = 15;

            AI1.Top = carPosition.Next(200, 500) *-1;
            AI1.Left = carPosition.Next(5, 200);

            AI2.Top = carPosition.Next(200, 500) * -1;
            AI2.Left = carPosition.Next(245, 422);

            gameTimer.Start();

            // Убираем фокус с текстового поля — чтобы форма получала клавиши
            this.ActiveControl = null;
            this.Focus();


        }

        private void restartGame(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlayerName?.Text))
            {
                MessageBox.Show("Введите имя игрока перед стартом.", "Требуется имя", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ResetGame();
        }

        private void playSound()
        {
            System.Media.SoundPlayer playCrash = new System.Media.SoundPlayer(Properties.Resources.hit);
            playCrash.Play();
            
        }

        private void ShowLeaderboard()
        {
            DataTable dt;
            try
            {
                dt = db.GetTopScores(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении таблицы лидеров: " + ex.Message, "DB error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Формируем простое окно с DataGridView
            Form leaderForm = new Form();
            leaderForm.Text = "Таблица лидеров - Топ 10";
            leaderForm.StartPosition = FormStartPosition.CenterParent;
            leaderForm.Size = new Size(420, 360);
            leaderForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            leaderForm.MaximizeBox = false;
            leaderForm.MinimizeBox = false;

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DataSource = dt;

            // Красивые заголовки
            if (dgv.Columns["Name"] != null) dgv.Columns["Name"].HeaderText = "Игрок";
            if (dgv.Columns["Score"] != null) dgv.Columns["Score"].HeaderText = "Очки";
            if (dgv.Columns["Date"] != null) dgv.Columns["Date"].HeaderText = "Дата (UTC)";

            leaderForm.Controls.Add(dgv);
            leaderForm.ShowDialog(this);
        }

    }
}
