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
        public int currentTurn = 0;

        public GameBoard() {
            InitializeComponent();
            CreateBoard();

            MainWindow.gameState = MainWindow.GameState.Player2;
            //AddToGridCell(new Vector(1, 1));
            AddToGridCell(new Vector(0, 0));
            AddToGridCell(new Vector(0, 2));
            AddToGridCell(new Vector(2, 1));

            MainWindow.gameState = MainWindow.GameState.Player1;
            AddToGridCell(new Vector(1, 1));
            AddToGridCell(new Vector(1, 2));
            AddToGridCell(new Vector(2, 2));
            AddToGridCell(new Vector(1, 0));
        }

        public void CreateBoard() {
            board = new Hashtable();
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    board.Add(new Vector(i, j), GridCellState.Empty);
                }
            }

            currentTurn = 0;
        }

        public void AddToGridCell(Vector gridCoordinates) {
            GridCellState gridCellState = (GridCellState)board[gridCoordinates];
            bool canAdd = IsGridCellEmpty(gridCoordinates);

            if (canAdd) {
                Image imgGridCell = new Image();
                BitmapImage imgSource = DetermineImageAndUpdateState(gridCoordinates);

                Grid.SetColumn(imgGridCell, (int)gridCoordinates.X);
                Grid.SetRow(imgGridCell, (int)gridCoordinates.Y);

                imgGridCell.Source = imgSource;
                boardGrid.Children.Add(imgGridCell);

                currentTurn++;

                if (currentTurn >= 6) {
                    DetermineIfGameOver(gridCoordinates);
                }
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

        public bool IsGridCellX(Vector gridCoordinates) {
            bool isX;
            GridCellState gridCellState = (GridCellState)board[gridCoordinates];

            if (gridCellState == GridCellState.X) {
                isX = true;
            }
            else {
                isX = false;
            }

            return isX;
        }

        public bool IsGridCellO(Vector gridCoordinates) {
            bool isO;
            GridCellState gridCellState = (GridCellState)board[gridCoordinates];

            if (gridCellState == GridCellState.O) {
                isO = true;
            }
            else {
                isO = false;
            }

            return isO;
        }

        public BitmapImage DetermineImageAndUpdateState(Vector gridCoordinates) {
            BitmapImage imgGridCell;

            switch (MainWindow.gameState) {
                case MainWindow.GameState.Player1:
                    imgGridCell = new BitmapImage(new Uri(
                    "/Assets/Icons/X.png", UriKind.Relative));
                    board[gridCoordinates] = GridCellState.X;
                    break;

                case MainWindow.GameState.Player2:
                    imgGridCell = new BitmapImage(new Uri(
                    "/Assets/Icons/O.png", UriKind.Relative));
                    board[gridCoordinates] = GridCellState.O;
                    break;

                default:
                    throw new Exception("Illegal Game State, this should not be possible");
            }

            return imgGridCell;
        }

        public void DetermineIfGameOver(Vector moveMade) {
            int x = (int)moveMade.X;
            int y = (int)moveMade.Y;

            if (MainWindow.gameState == MainWindow.GameState.Player1) {
                // Check Xs
                for (int i = 0; i < 3; i++) {
                    Vector coordinates = new Vector(i, y);
                    if (!IsGridCellX(coordinates)) {
                        break;
                    }
                    if (i == 2) {
                        Console.WriteLine("X wins");
                    }
                }

                // Check Ys
                for (int j = 0; j < 3; j++) {
                    Vector coordinates = new Vector(x, j);
                    if (!IsGridCellX(coordinates)) {
                        break;
                    }
                    if (j == 2) {
                        Console.WriteLine("X wins");
                    }
                }

                // Diagonal
                if (x == y) {
                    for (int k = 0; k < 3; k++) {
                        Vector coordinates = new Vector(k, k);
                        if (!IsGridCellX(coordinates)) {
                            break;
                        }
                        if (k == 2) {
                            Console.WriteLine("X wins");
                        }
                    }
                }

                // Other diagonal
                else if (x + y == 2 || (x == 1 && y == 1)) {
                    for (int l = 0; l < 3; l++) {
                        Vector coordinates = new Vector(l, 2 - l);
                        if (!IsGridCellX(coordinates)) {
                            break;
                        }
                        if (l == 2) {
                            Console.WriteLine("X wins");
                        }
                    }
                }
            }

            else {
                // Check Xs
                for (int i = 0; i < 3; i++) {
                    Vector coordinates = new Vector(i, y);
                    if (!IsGridCellO(coordinates)) {
                        break;
                    }
                    if (i == 2) {
                        // O wins
                    }
                }

                // Check Ys
                for (int j = 0; j < 3; j++) {
                    Vector coordinates = new Vector(x, j);
                    if (!IsGridCellO(coordinates)) {
                        break;
                    }
                    if (j == 2) {
                        // O wins
                    }
                }

                // Diagonal
                if (x == y) {
                    for (int k = 0; k < 3; k++) {
                        Vector coordinates = new Vector(k, k);
                        if (!IsGridCellO(coordinates)) {
                            break;
                        }
                        if (k == 2) {
                            // O wins
                        }
                    }
                }

                // Other diagonal
                else if (x + y == 2 || (x == 1 && y == 1)) {
                    for (int l = 0; l < 3; l++) {
                        Vector coordinates = new Vector(l, 2 - l);
                        if (!IsGridCellO(coordinates)) {
                            break;
                        }
                        if (l == 2) {
                            // O wins
                        }
                    }
                }
            }
        }
    }
}
