using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Игра_шашки
{
    public partial class Form1 : Form
    {
        #region Начало игры 
        int CurrentPlayer;
        int CurrentDamka;
        int SecondPlayer;
        int SecondDamka;
        bool moving;
        bool easy;

        const int MapSize = 8;
        int cellSize = 65;

        Button prevButtton;
        Button pressedButton;

        int[,] map = new int[MapSize, MapSize];
        Button[,] buttons = new Button[MapSize, MapSize];

        public Form1()
        {
            InitializeComponent();
            Shashki();
        }

        private void Shashki()
        {
            this.Width =67*8;
            this.Height =70*8;
            CurrentPlayer = 1;
            CurrentDamka = 3;
            SecondPlayer = 2;
            SecondDamka = 4;
            moving = false;
            prevButtton = null;
            map = new int[MapSize, MapSize]
            {
                {0,1,0,1,0,1,0,1},
                {1,0,1,0,1,0,1,0},
                {0,1,0,1,0,1,0,1},
                {0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0},
                {2,0,2,0,2,0,2,0},
                {0,2,0,2,0,2,0,2},
                {2,0,2,0,2,0,2,0}
            };

            CreateMap();
        }

        private void CreateMap()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    Button btn = new Button();
                    btn.Location = new Point(j * cellSize, i * cellSize);
                    btn.Size = new Size(cellSize, cellSize);
                    btn.Click += new EventHandler(FigurePress);
                    if (map[i, j] == 1) btn.BackgroundImage = Properties.Resources.sh2;
                    else if (map[i, j] == 2) btn.BackgroundImage = Properties.Resources.sh1;
                    if (i % 2 == 0)
                    {
                        if (j % 2 != 0)
                        {
                            btn.BackColor = Color.Gray;
                        }
                    }
                    if (i % 2 != 0)
                    {
                        if (j % 2 == 0)
                        {
                            btn.BackColor = Color.Gray;
                        }
                    }
                    buttons[i, j] = btn;
                    this.Controls.Add(btn);
                }
            }
        }
        #endregion
        #region Вспомогательные функции
        private void MoveStep1(Button btn)
        {
            btn.Enabled = true;
            btn.BackColor = Color.Yellow;
        }
        private bool CheckBorders(int i, int j)
        {
            if (i >= 8 || i <= -1 || j >= 8 || j <= -1) return false;
            else return true;
        }
        private void ChangePlayer()
        {
            bool notEatStep = true;
            moving = false;

            if (CurrentPlayer == 1) { CurrentPlayer = 2; CurrentDamka = 4; SecondPlayer = 1; SecondDamka = 3; }
            else if (CurrentPlayer == 2) { CurrentPlayer = 1; CurrentDamka = 3; SecondPlayer = 2; SecondDamka = 4; }

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    int a = buttons[i, j].Location.Y / cellSize;
                    int b = buttons[i, j].Location.X / cellSize;
                    if (map[i, j] == CurrentPlayer && ProvOnEating(a, b))
                    {
                        buttons[i, j].Enabled = true;
                        notEatStep = false;
                    }

                    if (map[i, j] == CurrentDamka)
                    {
                        List<int> listik1 = ProvNaEatForDamka(1, 1, i, j);
                        List<int> listik2 = ProvNaEatForDamka(-1, -1, i, j);
                        List<int> listik3 = ProvNaEatForDamka(-1, 1, i, j);
                        List<int> listik4 = ProvNaEatForDamka(1, -1, i, j);
                        if (listik1[2] == 1 || listik2[2] == 1 || listik3[2] == 1 || listik4[2] == 1)
                        {
                            buttons[i, j].Enabled = true;
                            notEatStep = false;
                        }
                    }
                }
            }
            if (notEatStep) ActivateAllButtons();
            ProvOnGameOver();
        }

        public void ReturnColor()//Возвращение цвета после перестановки батонов
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    Button p = buttons[i, j];

                    if ((p.Location.Y % 2) == 0)
                    {
                        if ((p.Location.X % 2) != 0)
                        {
                            p.BackColor = Color.Gray;
                        }
                        else p.BackColor = Color.White;
                    }
                    if ((p.Location.Y % 2) != 0)
                    {
                        if ((p.Location.X % 2) == 0)
                        {
                            p.BackColor = Color.Gray;
                        }
                        else p.BackColor = Color.White;
                    }

                }
            }
        }

        public void ActivateAllButtons()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    buttons[i, j].Enabled = true;
                }
            }
        }
        public void DeactivateAllButtons()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    buttons[i, j].Enabled = false;
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape && moving == true)
            {
                moving = false;
                ActivateAllButtons();
                ReturnColor();
            }
        }

        public void Perestanovka()
        {
            prevButtton = buttons[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize];
            pressedButton = null;
        }
        #endregion
        #region Общие функции движения
        private void MoveStep2(Button prevBtn, ref Button pressedBtn)
        {
            Image img = prevBtn.BackgroundImage;
            pressedBtn.BackgroundImage = img;
            prevBtn.BackgroundImage = null;

            buttons[pressedBtn.Location.Y / cellSize, pressedBtn.Location.X / cellSize].BackgroundImage = img;
            buttons[prevBtn.Location.Y / cellSize, prevBtn.Location.X / cellSize].BackgroundImage = null;

            if (map[prevBtn.Location.Y / cellSize, prevBtn.Location.X / cellSize] == CurrentDamka)
                map[pressedBtn.Location.Y / cellSize, pressedBtn.Location.X / cellSize] = CurrentDamka;
            else map[pressedBtn.Location.Y / cellSize, pressedBtn.Location.X / cellSize] = CurrentPlayer;

            map[prevBtn.Location.Y / cellSize, prevBtn.Location.X / cellSize] = 0;

            ReturnColor();
            provNADamky(pressedBtn.Location.Y / cellSize, pressedBtn.Location.X / cellSize);
            prevBtn = null;
        }
        #endregion
        #region Движения обычные 
        private bool ProvOnEating(int i, int j)
        {
            if (CheckBorders(i - 2, j - 2))//лево верх
            {
                if ((map[i - 1, j - 1] == SecondPlayer || map[i - 1, j - 1] == SecondDamka) && map[i - 2, j - 2] == 0) return true;
            }
            if (CheckBorders(i + 2, j - 2))//лево низ
            {
                if ((map[i + 1, j - 1] == SecondPlayer || map[i + 1, j - 1] == SecondDamka) && map[i + 2, j - 2] == 0) return true;
            }
            if (CheckBorders(i - 2, j + 2))//право верх
            {
                if ((map[i - 1, j + 1] == SecondPlayer || map[i - 1, j + 1] == SecondDamka) && map[i - 2, j + 2] == 0) return true;
            }
            if (CheckBorders(i + 2, j + 2))//право низ
            {
                if ((map[i + 1, j + 1] == SecondPlayer || map[i + 1, j + 1] == SecondDamka) && map[i + 2, j + 2] == 0) return true;
            }

            return false;

        }
        private void SimpleMove(Button pressedButton)
        {
            int i = pressedButton.Location.Y / cellSize;
            int j = pressedButton.Location.X / cellSize;

            if (CurrentPlayer == 1)//белые
            {
                Step(i, j, 1, -1);
                Step(i, j, 1, 1);
            }
            else //черные
            {
                Step(i, j, -1, -1);
                Step(i, j, -1, 1);
            }
        }
        private void Step(int i, int j, int a, int b)
        {
            if (CheckBorders(i + a, j + b))//вверх вправо
            {
                if (map[i + a, j + b] == 0)
                {
                    MoveStep1(buttons[i + a, j + b]);
                }
            }
        }

        private void MoveWithEating(int i, int j)
        {

            if (CheckBorders(i - 2, j - 2))//лево верх
            {
                if ((map[i - 1, j - 1] == SecondPlayer || map[i - 1, j - 1] == SecondDamka) && map[i - 2, j - 2] == 0) MoveStep1(buttons[i - 2, j - 2]);
            }
            if (CheckBorders(i + 2, j - 2))//лево низ
            {
                if ((map[i + 1, j - 1] == SecondPlayer || map[i + 1, j - 1] == SecondDamka) && map[i + 2, j - 2] == 0) MoveStep1(buttons[i + 2, j - 2]);
            }
            if (CheckBorders(i - 2, j + 2))//право верх
            {
                if ((map[i - 1, j + 1] == SecondPlayer || map[i - 1, j + 1] == SecondDamka) && map[i - 2, j + 2] == 0) MoveStep1(buttons[i - 2, j + 2]);
            }
            if (CheckBorders(i + 2, j + 2))//право низ
            {
                if ((map[i + 1, j + 1] == SecondPlayer || map[i + 1, j + 1] == SecondDamka) && map[i + 2, j + 2] == 0) MoveStep1(buttons[i + 2, j + 2]);
            }
        }

        private void Eat(Button prevButton, Button pressedButton)
        {
            int x = prevButton.Location.Y / cellSize;
            int y = prevButton.Location.X / cellSize;

            int i = pressedButton.Location.Y / cellSize - prevButton.Location.Y / cellSize;
            int j = pressedButton.Location.X / cellSize - prevButton.Location.X / cellSize;

            if (i > 0) i--;
            else i++;

            if (j > 0) j--;
            else j++;

            buttons[x + i, y + j].BackgroundImage = null;
            map[x + i, y + j] = 0;
        }
        #endregion
        #region Основная функция ходьбы
        private void FigurePress(object sender, EventArgs e)
        {
            pressedButton = sender as Button;
            int i = pressedButton.Location.Y / cellSize;
            int j = pressedButton.Location.X / cellSize;

            if (!moving && (map[i, j] == CurrentPlayer || map[i, j] == CurrentDamka))
            {
                DeactivateAllButtons();
                moving = true;
                if (map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] == CurrentDamka)
                {
                    StepDamka(pressedButton);
                    Perestanovka();
                }
                else
                {
                    if (!ProvOnEating(i, j))
                    {
                        easy = true;
                        SimpleMove(pressedButton);
                        Perestanovka();
                    }
                    else
                    {
                        easy = false;
                        MoveWithEating(i, j);
                        Perestanovka();
                    }
                }

            }
            else if (moving)
            {
                if (map[prevButtton.Location.Y / cellSize, prevButtton.Location.X / cellSize] == CurrentDamka)
                {
                    MoveStep2(prevButtton, ref pressedButton);
                    DamkaEat(prevButtton, pressedButton);
                    Perestanovka();
                    ChangePlayer();
                }
                else
                {
                    if (easy)
                    {
                        MoveStep2(prevButtton, ref pressedButton);
                        ChangePlayer();
                    }
                    else
                    {
                        MoveStep2(prevButtton, ref pressedButton);
                        Eat(prevButtton, pressedButton);
                        if (ProvOnEating(i, j))
                        {
                            MoveWithEating(i, j);
                            Perestanovka();
                        }
                        else ChangePlayer();
                    }
                }
            }
        }
        #endregion
        #region Ходы дамки
        private void provNADamky(int i, int j)
        {
            if (CurrentPlayer == 1 && i == 7)
            {
                buttons[i, j].BackgroundImage = Properties.Resources.dm2;
                map[i, j] = CurrentDamka;
            }
            else if (CurrentPlayer == 2 && i == 0)
            {
                buttons[i, j].BackgroundImage = Properties.Resources.dm1;
                map[i, j] = CurrentDamka;
            }
        }

        private void StepDamka(Button pressedbutton)
        {
            List<int> list1 = ProvNaEatForDamka(-1, -1, pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);
            List<int> list2 = ProvNaEatForDamka(1, 1, pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);
            List<int> list3 = ProvNaEatForDamka(1, -1, pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);
            List<int> list4 = ProvNaEatForDamka(-1, 1, pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);

            if ((list1[0] != -1) || (list2[0] != -1) || (list3[0] != -1) || (list4[0] != -1))
            {
                if (list1[0] != -1) XodForDamka(-1, -1, list1[0], list1[1]);
                if (list2[0] != -1) XodForDamka(1, 1, list2[0], list2[1]);
                if (list3[0] != -1) XodForDamka(1, -1, list3[0], list3[1]);
                if (list4[0] != -1) XodForDamka(-1, 1, list4[0], list4[1]);
            }
            else
            {
                XodForDamka(-1, -1, pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);
                XodForDamka(1, 1, pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);
                XodForDamka(1, -1, pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);
                XodForDamka(-1, 1, pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);
            }
        }

        private List<int> ProvNaEatForDamka(int a, int b, int i, int j)
        {
            int indexI = -1;
            int indexJ = -1;
            int prov = 0;
            while (CheckBorders(i + a, j + b) && map[i + a, j + b] != CurrentPlayer && map[i + a, j + b] != CurrentDamka && !provOnTwoSecondPlayer(a, b, i, j))
            {
                if (map[i + a, j + b] == SecondPlayer || map[i + a, j + b] == SecondDamka)
                {
                    if (CheckBorders(i + 2 * a, j + 2 * b))
                    {
                        if (map[i + 2 * a, j + 2 * b] == 0)
                        {
                            indexI = i + a;
                            indexJ = j + b;
                            prov = 1;
                        }
                    }
                }
                i = i + a; j = j + b;
            }
            List<int> listok = new List<int>();
            listok.Add(indexI);
            listok.Add(indexJ);
            listok.Add(prov);
            return listok;
        }

        private void XodForDamka(int a, int b, int i, int j)
        {
            while (CheckBorders(i + a, j + b) && map[i + a, j + b] == 0)
            {
                MoveStep1(buttons[i + a, j + b]);
                i = i + a;
                j = j + b;
            }
        }

        private bool provOnTwoSecondPlayer(int a, int b, int i, int j)
        {
            if (!CheckBorders(i + 2 * a, j + 2 * b)) return false;
            else if ((map[i + a, j + b] == SecondPlayer && map[i + 2 * a, j + 2 * b] == SecondPlayer) || (map[i + a, j + b] == SecondDamka && map[i + 2 * a, j + 2 * b] == SecondDamka)) return true;
            else return false;
        }

        private void DamkaEat(Button prevButton, Button pressedButton)
        {
            int x = prevButton.Location.Y / cellSize;
            int y = prevButton.Location.X / cellSize;

            int i = pressedButton.Location.Y / cellSize;
            int j = pressedButton.Location.X / cellSize;

            if (i - x > 0 && j - y > 0) ClearAfterDamka(i, j, -1, -1, x, y);
            else if (i - x < 0 && j - y < 0) ClearAfterDamka(i, j, 1, 1, x, y);
            else if (i - x > 0 && j - y < 0) ClearAfterDamka(i, j, -1, 1, x, y);
            else if (i - x < 0 && j - y > 0) ClearAfterDamka(i, j, 1, -1, x, y);
        }

        private void ClearAfterDamka(int i, int j, int a, int b, int x, int y)
        {
            i = i + a; j = j + b;
            while (i != x && j != y)
            {

                buttons[i, j].BackgroundImage = null;
                map[i, j] = 0;
                i = i + a; j = j + b;
            }
        }
        #endregion
        #region Конец игры
        private void ProvOnGameOver()
        {
            bool provOnBlack = false;
            bool provOnWhite = false;
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (map[i, j] == 1 || map[i, j] == 3) provOnWhite = true;
                    if (map[i, j] == 2 || map[i, j] == 4) provOnBlack = true;
                }
            }
            if (!provOnBlack || !provOnWhite)
            {
                MessageBox.Show("Игра закончена", "Шашки");
                Close();
            }
        }

        #endregion
    }
}
