using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SnakeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private int boardSize;
        public static Game game;
        private static Random random = new Random();
        public static int points = 0;
        public static List<Record> records = new List<Record>();
        private Border[,] grdBoardBorders;
        private Label pointsLabel;
        public static string recordsFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"SnakeGame/Records.dat");
        public static string gameDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnakeGame");
        private static SettingsWindow setWindow = new SettingsWindow();

        public MainWindow()
        {
            InitializeComponent();
            InitializeContent();
        }

        private void InitializeContent()
        {
            Closed += windowClosed;
            
            // Main canvas
            Canvas cnvMain = new Canvas();
            cnvMain.Background = Brushes.Bisque;
            cnvMain.Width = winMain.Width;
            winMain.Content = cnvMain;

            // Menu
            Menu mnuTop = new Menu();
            mnuTop.Width = cnvMain.Width;
            mnuTop.Height = 20;
            Canvas.SetTop(mnuTop, 0);
            Canvas.SetLeft(mnuTop, 0);
            cnvMain.Children.Add(mnuTop);

            MenuItem mitFile = new MenuItem();
            mitFile.Header = "_File";
            mnuTop.Items.Add(mitFile);

            MenuItem mitFileNewGame = new MenuItem();
            mitFileNewGame.Header = "_New Game";
            mitFileNewGame.Click += mitFileNewGame_Click;
            mitFile.Items.Add(mitFileNewGame);

            MenuItem mitFileRecords = new MenuItem();
            mitFileRecords.Header = "_Records";
            mitFileRecords.Click += mitFileRecords_Click;
            mitFile.Items.Add(mitFileRecords);
            
            MenuItem mitFileClearRecords = new MenuItem();
            mitFileClearRecords.Header = "_Clear Records";
            mitFileClearRecords.Click += mitFileClearRecords_Click;
            mitFile.Items.Add(mitFileClearRecords);

            MenuItem mitFileExit = new MenuItem();
            mitFileExit.Header = "_Exit";
            mitFileExit.Click += mitFileExit_Click;
            mitFile.Items.Add(mitFileExit);

            MenuItem mitSettings = new MenuItem();
            mitSettings.Header = "_Settings";
            mitSettings.Click += mitFileSettings_Click;
            mnuTop.Items.Add(mitSettings); 

            // Grid with game board
            Grid grdBoard = new Grid();
            Canvas.SetTop(grdBoard, mnuTop.Height);
            Canvas.SetLeft(grdBoard, 0);
            cnvMain.Children.Add(grdBoard);

            const int rowHeight = 10;
            const int colWidth = 10;

            RowDefinition[] grdBoardRows = new RowDefinition[Game.HEIGHT];
            for(int r = 0; r < Game.HEIGHT; ++r)
            {
                grdBoardRows[r] = new RowDefinition();
                grdBoardRows[r].Height = new GridLength(rowHeight);
                grdBoard.RowDefinitions.Add(grdBoardRows[r]);
            }

            ColumnDefinition[] grdBoardCols = new ColumnDefinition[Game.WIDTH];
            for(int c = 0; c < Game.WIDTH; ++c)
            {
                grdBoardCols[c] = new ColumnDefinition();
                grdBoardCols[c].Width = new GridLength(colWidth);
                grdBoard.ColumnDefinitions.Add(grdBoardCols[c]);
            }

            grdBoardBorders = new Border[Game.HEIGHT, Game.WIDTH];
            for(int r = 0; r < Game.HEIGHT; ++r)
            {
                for(int c = 0; c < Game.WIDTH; ++c)
                {
                    grdBoardBorders[r, c] = new Border();
                    grdBoardBorders[r, c].Background = Brushes.White;
                    Grid.SetRow(grdBoardBorders[r, c], r);
                    Grid.SetColumn(grdBoardBorders[r, c], c);
                    grdBoard.Children.Add(grdBoardBorders[r, c]);
                }
            }

            //Points
            pointsLabel = new Label();
            Canvas.SetTop(pointsLabel, mnuTop.Height);
            Canvas.SetLeft(pointsLabel, Game.WIDTH * colWidth + 10);
            pointsLabel.Content = "Points: " + points;
            pointsLabel.FontSize = 52;
            cnvMain.Children.Add(pointsLabel);

            DispatcherTimer gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(250);
            gameTimer.Tick += gameTimerElaspsed; 
            gameTimer.Start();
            
            DispatcherTimer keyboardTimer = new DispatcherTimer();
            keyboardTimer.Interval = TimeSpan.FromMilliseconds(1);
            keyboardTimer.Tick += keyboardTimerElaspsed; 
            keyboardTimer.Start();

            if(!Directory.Exists(gameDir)) Directory.CreateDirectory(gameDir);
            if(File.Exists(recordsFilePath)) deserializeRecords();
        }

        private void moveSnake()
        {
            if(game != null)
            {
                List<Vector2> tempSnakeParts = new List<Vector2>();
                Vector2 tempVect = game.snakeHead.pos;
                Vector2 tempVect2;

                switch(game.snakeHead.rot)
                {
                    case 1:
                        game.snakeHead.pos.Y -= 1;
                        break;
                    case 2:
                        game.snakeHead.pos.X -= 1;
                        break;
                    case 3:
                        game.snakeHead.pos.Y += 1;
                        break;
                    case 4:
                        game.snakeHead.pos.X += 1;
                        break;
                }

                /*if(game.snakeHead.pos.X < 0 || game.snakeHead.pos.X >= 8 || game.snakeHead.pos.Y < 0 || game.snakeHead.pos.Y >= 8)
                {
                    Console.Clear();
                    Console.WriteLine("GAME OVER!");
                    Environment.Exit(0);
                }*/

                if(game.snakeHead.pos.X < 0)
                {
                    game.snakeHead.pos.X = Game.WIDTH-1;
                }
                
                if(game.snakeHead.pos.X >= Game.WIDTH)
                {
                    game.snakeHead.pos.X = 0;
                }
                
                if(game.snakeHead.pos.Y < 0)
                {
                    game.snakeHead.pos.Y = Game.HEIGHT-1;
                }
                
                if(game.snakeHead.pos.Y >= Game.HEIGHT)
                {
                    game.snakeHead.pos.Y = 0;
                }

                for(int i = 1; i <= game.snakeParts.Count; i++)
                {
                    tempVect2 = game.snakeParts.ElementAt(i-1);
                    tempSnakeParts.Add(tempVect);
                    tempVect = tempVect2;
                }

                game.snakeParts = tempSnakeParts;

                foreach(Vector2 vector in game.snakeParts)
                {
                    if(game.snakeHead.pos == vector)
                    {
                        //Console.WriteLine("GAME OVER!");
                        game = null;
                        MessageBox.Show("Game Over!\nPoints: " + points, "Snake Game");
                        records.Add(new Record("Player123", points, DateTime.Now));
                        records.Sort(new RecordsComparer());
                        while(records.Count > 10)
                        {
                            records.RemoveAt(records.Count-1);
                        }
                        points = 0;
                        pointsLabel.Content = "Points: " + points;
                        serializeRecords();
                        return;
                        //Application.Current.Shutdown();
                    }
                }

                if(game.snakeHead.pos == game.fruit)
                {
                    game.fruit = new Vector2(random.Next(1, Game.WIDTH), random.Next(1, Game.HEIGHT));
                    game.snakeParts.Add(tempVect);
                    points++;
                    pointsLabel.Content = "Points: " + points;
                }
            }
        }

        private void serializeRecords()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(recordsFilePath, FileMode.Create);
            formatter.Serialize(fileStream, records);
            fileStream.Close();
        }

        private void deserializeRecords()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(recordsFilePath, FileMode.Open);
            records = formatter.Deserialize(fileStream) as List<Record>;
            fileStream.Close();
            
        }

        private void gameTimerElaspsed(object sender, EventArgs e)
        {   
            if(game != null)
            {    
                moveSnake();

                if(game != null)
                { 
                    for(int i = 1; i <= Game.HEIGHT; i++)
                    { 
                        for(int j = 1; j <= Game.WIDTH; j++)
                        {
                            grdBoardBorders[i-1, j-1].Background = Brushes.White;
                        }
                    }
                    grdBoardBorders[(int) game.fruit.Y, (int) game.fruit.X].Background = Brushes.IndianRed;
                    grdBoardBorders[(int) game.snakeHead.pos.Y, (int) game.snakeHead.pos.X].Background = Brushes.DarkGray;
                    foreach(Vector2 vector in game.snakeParts)
                    {
                        grdBoardBorders[(int) vector.Y, (int) vector.X].Background = Brushes.Black;
                    }
                }
            }
        }

        private void keyboardTimerElaspsed(object sender, EventArgs e)
        {
            if(game != null)
            {
                if(Keyboard.IsKeyDown(Key.W)) game.snakeHead.rot = 1;
                if(Keyboard.IsKeyDown(Key.A)) game.snakeHead.rot = 2;
                if(Keyboard.IsKeyDown(Key.S)) game.snakeHead.rot = 3;
                if(Keyboard.IsKeyDown(Key.D)) game.snakeHead.rot = 4;
            }
        }

        private void mitFileNewGame_Click(object sender, RoutedEventArgs e)
        {
            game = new Game();
        }

        private void mitFileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mitFileRecords_Click(object sender, RoutedEventArgs e) 
        {
            String msgBoxText = "";
            foreach(Record record in records)
            {
                String date = "";
                if(record.time.Day < 10) date += "0" + record.time.Day; else date += record.time.Day;
                date += ".";
                if(record.time.Month < 10) date += "0" + record.time.Month; else date += record.time.Month;
                date += ".";
                date += record.time.Year;

                String time = "";
                if(record.time.Hour < 10) time += "0" + record.time.Hour; else time += record.time.Hour;
                time += ":";
                if(record.time.Minute < 10) time += "0" + record.time.Minute; else time += record.time.Minute;
                time += ":";
                if(record.time.Second < 10) time += "0" + record.time.Second; else time += record.time.Second;

                msgBoxText += record.playerName + " - " + record.points + "          " + date + " " + time + "\n";
            }
            MessageBox.Show(msgBoxText == "" ? "No Records" : msgBoxText, "Records");
        }

        private void mitFileClearRecords_Click(object sender, RoutedEventArgs e) 
        {
            records.Clear();
            serializeRecords();
        }
   
        private void mitFileSettings_Click(object sender, RoutedEventArgs e)
        {
            if(setWindow.IsActive) setWindow.Close(); else setWindow.Show();
        }
    
        private void windowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    [Serializable]
    public class Record
    {
        public string playerName;
        public int points;
        public DateTime time;

        public Record(string playerName, int points, DateTime time)
        {
            this.playerName = playerName;
            this.points = points;
            this.time = time;
        }
    }

    public class RecordsComparer : IComparer<Record>
    {
        public int Compare(Record one, Record two)
        {
            if(one.points < two.points) return 1;
            else if(one.points > two.points) return -1;
            else return 0;
        }
    }
}
