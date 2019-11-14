using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoFinalSenales {
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl {
        public Hashtable board;
        public enum GridCellState { Empty, X, O }

        public GameBoard() {
            InitializeComponent();
            CreateBoard();

            MainWindow.gameState = MainWindow.GameState.Player1;
            AddToGridCell(new Vector(1, 1));
            AddToGridCell(new Vector(1, 2));
            AddToGridCell(new Vector(2, 2));
        }

        public void CreateBoard() {
            board = new Hashtable();
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    board.Add(new Vector(i, j), GridCellState.Empty);
                }
            }
        }

        public void AddToGridCell(Vector gridCoordinates) {
            bool canAdd = IsGridCellEmpty(gridCoordinates);

            if (canAdd) {
                Image imgGridCell = new Image();
                BitmapImage imgSource = DetermineImageToAdd();

                Grid.SetColumn(imgGridCell, (int)gridCoordinates.X);
                Grid.SetRow(imgGridCell, (int)gridCoordinates.Y);

                imgGridCell.Source = imgSource;
                boardGrid.Children.Add(imgGridCell);
            }
        }

        public bool IsGridCellEmpty(Vector gridCoordinates) {
            bool isEmpty;
            GridCellState gridCellState = (GridCellState)board[gridCoordinates];

            if (gridCellState == GridCellState.Empty) {
                isEmpty = true;
            }
            else {
                isEmpty = false;
            }

            return isEmpty;
        }

        public BitmapImage DetermineImageToAdd() {
            BitmapImage imgGridCell;

            switch (MainWindow.gameState) {
                case MainWindow.GameState.Player1:
                    imgGridCell = new BitmapImage(new Uri(
                    "/Assets/Icons/X.png", UriKind.Relative));
                    break;

                case MainWindow.GameState.Player2:
                    imgGridCell = new BitmapImage(new Uri(
                    "/Assets/Icons/O.png", UriKind.Relative));
                    break;

                default:
                    throw new Exception("Illegal Game State, this should not be possible");
            }

            return imgGridCell;
        }
    }
}
