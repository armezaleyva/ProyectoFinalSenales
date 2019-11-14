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

namespace ProyectoFinalSenales {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public enum GameState { Menu, Reset, Player1, Player2 }
        public static GameState gameState = GameState.Menu;

        public MainWindow() {
            InitializeComponent();
            Update();
        }

        public void Update() {
            switch(gameState) {
                case GameState.Menu:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new StartMenu());

                    HideUI();
                    break;

                case GameState.Reset:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new GameBoard());

                    HideUI();
                    btnRecord.Visibility = Visibility.Visible;
                    lblCurrentTurn.Visibility = Visibility.Visible;

                    lblPlayer1.Visibility = Visibility.Visible;
                    lblPlayer2.Visibility = Visibility.Visible;
                    polPlayer2.Visibility = Visibility.Visible;
                    break;


                case GameState.Player1:
                    HideUI();
                    lblRecording.Visibility = Visibility.Visible;
                    lblCurrentTurn.Visibility = Visibility.Visible;
                    lblPlayer1.Visibility = Visibility.Visible;
                    lblPlayer2.Visibility = Visibility.Visible;
                    polPlayer1.Visibility = Visibility.Visible;
                    break;

                case GameState.Player2:
                    HideUI();
                    lblRecording.Visibility = Visibility.Visible;
                    lblCurrentTurn.Visibility = Visibility.Visible;
                    lblPlayer1.Visibility = Visibility.Visible;
                    lblPlayer2.Visibility = Visibility.Visible;
                    polPlayer2.Visibility = Visibility.Visible;
                    break;

                default:
                    throw new Exception("Illegal Game State, this should not be possible");
            }
        }

        public void HideUI() {
            lblRecording.Visibility = Visibility.Hidden;
            btnRecord.Visibility = Visibility.Hidden;
            polPlayer1.Visibility = Visibility.Hidden;
            polPlayer2.Visibility = Visibility.Hidden;
            lblPlayer1.Visibility = Visibility.Hidden;
            lblPlayer2.Visibility = Visibility.Hidden;
            lblCurrentTurn.Visibility = Visibility.Hidden;
        }
    }
}
