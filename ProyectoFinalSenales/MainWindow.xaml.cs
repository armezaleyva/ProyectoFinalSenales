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
        public enum GameState { Menu, Reset, Await, Player1, Player2 }
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
                    break;

                case GameState.Reset:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new GameBoard());
                    break;

                default:
                    throw new Exception("Illegal Game State, this should not be possible");
            }
        }
    }
}
