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

namespace SnakeWPF
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private TextBox boardWidthInput;
        private TextBox boardHeightInput;
        private TextBox playerNameInput;

        public SettingsWindow()
        {
            InitializeComponent();
            InitializeContent();
        }

        private void InitializeContent()
        {
            // Main canvas
            Canvas cnvMain = new Canvas();
            cnvMain.Background = Brushes.Bisque;
            setMain.Content = cnvMain;

            //Settings Label
            /*Label settingsLabel = new Label();
            settingsLabel.Content = "Settings";
            settingsLabel.FontSize = 52;
            Canvas.SetTop(settingsLabel, 0);
            Canvas.SetLeft(settingsLabel, 100);
            cnvMain.Children.Add(settingsLabel);

            //Board Size - Width
            Label boardWidth = new Label();
            boardWidth.Content = "Board Width";
            boardWidth.FontSize = 20;
            Canvas.SetTop(boardWidth, 70);
            Canvas.SetLeft(boardWidth, 10);
            cnvMain.Children.Add(boardWidth);

            boardWidthInput = new TextBox();
            //boardWidthInput. = "Board Width";
            boardWidthInput.TextChanged += boardWidthInput_TextChanged;
            //boardWidthInput.FontSize = 20;
            Canvas.SetTop(boardWidthInput, 80);
            Canvas.SetLeft(boardWidthInput, 100);
            //Canvas.SetRight(boardWidthInput, 140);
            cnvMain.Children.Add(boardWidthInput);*/

            int rowsAmount = 5;
            int colsAmount = 2;

            Grid grdSet = new Grid();
            Canvas.SetTop(grdSet, 5);
            Canvas.SetLeft(grdSet, 5);
            cnvMain.Children.Add(grdSet);

            RowDefinition[] rows = new RowDefinition[rowsAmount];
            for(int i = 0; i < rowsAmount; i++)
            {
                rows[i] = new RowDefinition();
                rows[i].Height = new GridLength(26);
                grdSet.RowDefinitions.Add(rows[i]);
            }
            
            ColumnDefinition[] cols = new ColumnDefinition[colsAmount];
            for(int i = 0; i < colsAmount; i++)
            {
                cols[i] = new ColumnDefinition();
                cols[i].Width = new GridLength(185);
                grdSet.ColumnDefinitions.Add(cols[i]);
            }

            Label lblBoardWidth = new Label();
            lblBoardWidth.Content = "Board Width";
            Grid.SetRow(lblBoardWidth, 0);
            Grid.SetColumn(lblBoardWidth, 0);
            grdSet.Children.Add(lblBoardWidth);

            boardWidthInput = new TextBox();
            boardWidthInput.Text = Game.WIDTH.ToString();
            boardWidthInput.TextChanged += boardWidthInput_TextChanged;
            boardWidthInput.Height = 20;
            Grid.SetRow(boardWidthInput, 0);
            Grid.SetColumn(boardWidthInput, 1);
            grdSet.Children.Add(boardWidthInput);
            
            Label lblBoardHeight = new Label();
            lblBoardHeight.Content = "Board Height";
            Grid.SetRow(lblBoardHeight, 1);
            Grid.SetColumn(lblBoardHeight, 0);
            grdSet.Children.Add(lblBoardHeight);

            boardHeightInput = new TextBox();
            boardHeightInput.Text = Game.HEIGHT.ToString();
            boardHeightInput.TextChanged += boardHeightInput_TextChanged;
            boardHeightInput.Height = 20;
            Grid.SetRow(boardHeightInput, 1);
            Grid.SetColumn(boardHeightInput, 1);
            grdSet.Children.Add(boardHeightInput);
            
            Label lblPlayerName = new Label();
            lblPlayerName.Content = "Player Name";
            Grid.SetRow(lblPlayerName, 2);
            Grid.SetColumn(lblPlayerName, 0);
            grdSet.Children.Add(lblPlayerName);

            playerNameInput = new TextBox();
            playerNameInput.Text = MainWindow.playerName;
            playerNameInput.TextChanged += playerNameInput_TextChanged;
            playerNameInput.Height = 20;
            Grid.SetRow(playerNameInput, 2);
            Grid.SetColumn(playerNameInput, 1);
            grdSet.Children.Add(playerNameInput);
        }

        private void boardWidthInput_TextChanged(object sender, RoutedEventArgs e)
        {
            Int16 parseOut = new Int16();
            Int16.TryParse(boardWidthInput.Text, out parseOut);
            Game.WIDTH = parseOut;
            MainWindow.reInitGrid();
            MainWindow.serializeSettings();
        }
        
        private void boardHeightInput_TextChanged(object sender, RoutedEventArgs e)
        {
            Int16 parseOut = new Int16();
            Int16.TryParse(boardWidthInput.Text, out parseOut);
            Game.HEIGHT = parseOut;
            MainWindow.reInitGrid();
            MainWindow.serializeSettings();
        }

        private void playerNameInput_TextChanged(object sender, RoutedEventArgs e)
        {
            MainWindow.playerName = playerNameInput.Text;
            MainWindow.serializeSettings();
        }
    }
}
