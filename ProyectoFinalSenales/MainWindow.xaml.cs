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

using NAudio;
using NAudio.Wave;
using NAudio.Dsp;
using System.Windows.Threading;

namespace ProyectoFinalSenales {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public enum GameState { Menu, Reset, Player1, Player2, Xwin, Owin, Draw }
        public static GameState gameState = GameState.Menu;
        public List<Vector> currentCoordinates;
        WaveIn waveIn; //Conexion con microfono
        WaveFormat formato; //Formato de audio

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
                    polPlayer1.Visibility = Visibility.Visible;
                    break;


                case GameState.Player1:
                    HideUI();
                    btnRecord.Visibility = Visibility.Visible;
                    lblCurrentTurn.Visibility = Visibility.Visible;
                    lblPlayer1.Visibility = Visibility.Visible;
                    lblPlayer2.Visibility = Visibility.Visible;
                    polPlayer1.Visibility = Visibility.Visible;
                    break;

                case GameState.Player2:
                    HideUI();
                    btnRecord.Visibility = Visibility.Visible;
                    lblCurrentTurn.Visibility = Visibility.Visible;
                    lblPlayer1.Visibility = Visibility.Visible;
                    lblPlayer2.Visibility = Visibility.Visible;
                    polPlayer2.Visibility = Visibility.Visible;
                    break;

                case GameState.Xwin:
                    HideUI();
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new XwinScreen());

                    var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5000) };
                    timer.Start();
                    timer.Tick += (_, args) => {
                        timer.Stop();
                        gameState = GameState.Reset;
                        Update();
                    };
                    break;

                case GameState.Owin:
                    HideUI();
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new OwinScreen());

                    var timer2 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5000) };
                    timer2.Start();
                    timer2.Tick += (_, args) => {
                        timer2.Stop();
                        gameState = GameState.Reset;
                        Update();
                    };
                    break;

                case GameState.Draw:
                    HideUI();
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new DrawScreen());

                    var timer3 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5000) };
                    timer3.Start();
                    timer3.Tick += (_, args) => {
                        timer3.Stop();
                        gameState = GameState.Reset;
                        Update();
                    };
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

        private void BtnRecord_Click(object sender, RoutedEventArgs e) {
            btnRecord.Visibility = Visibility.Hidden;
            lblRecording.Visibility = Visibility.Visible;
            currentCoordinates = new List<Vector>();

            //Inicializar la conexion
            waveIn = new WaveIn();

            //Establecer el formato
            waveIn.WaveFormat =
                new WaveFormat(44100, 16, 1);
            formato = waveIn.WaveFormat;

            //Duracion del buffer
            waveIn.BufferMilliseconds = 250;

            //Con que funcion respondemos
            //cuando se llena el buffer
            waveIn.DataAvailable += WaveIn_DataAvailable;

            waveIn.StartRecording();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(4000) };
            timer.Start();
            timer.Tick += (_, args) =>
            {
                timer.Stop();
                waveIn.StopRecording();
                bool retry = false;

                int n = currentCoordinates.Count;
                var board = (GameBoard)mainGrid.Children[0];
                for (int i = n - 1; i >= 0; i--) {
                    bool couldAdd = board.AddToGridCell(currentCoordinates[i]);

                    if (couldAdd) {
                        break;
                    }

                    if (i == 0) {
                        retry = true;
                    }
                }

                if (!retry && (gameState == GameState.Player1 || gameState == GameState.Player2)) {
                    if (gameState == GameState.Player1) {
                        gameState = GameState.Player2;
                    }
                    else {
                        gameState = GameState.Player1;
                    }
                }

                Update();
            };
        }

        private void WaveIn_DataAvailable(object sender,
            WaveInEventArgs e) {
            byte[] buffer = e.Buffer;
            int bytesGrabados = e.BytesRecorded;

            int numMuestras = bytesGrabados / 2;

            int exponente = 0;
            int numeroBits = 0;

            do {
                exponente++;
                numeroBits = (int)
                    Math.Pow(2, exponente);
            } while (numeroBits < numMuestras);
            exponente -= 1;
            numeroBits = (int)
                Math.Pow(2, exponente);
            Complex[] muestrasComplejas =
                new Complex[numeroBits];

            for (int i = 0; i < bytesGrabados; i += 2) {
                short muestra =
                    (short)(buffer[i + 1] << 8 | buffer[i]);
                float muestra32bits =
                    (float)muestra / 32768.0f;
                if (i / 2 < numeroBits) {
                    muestrasComplejas[i / 2].X = muestra32bits;
                }

            }

            FastFourierTransform.FFT(true,
                exponente, muestrasComplejas);

            float[] valoresAbsolutos =
                new float[muestrasComplejas.Length];

            for (int i = 0; i < muestrasComplejas.Length;
                i++) {
                valoresAbsolutos[i] = (float)
                    Math.Sqrt(
                    (muestrasComplejas[i].X * muestrasComplejas[i].X) +
                    (muestrasComplejas[i].Y * muestrasComplejas[i].Y));

            }

            var mitadValoresAbsolutos =
                valoresAbsolutos.Take(valoresAbsolutos.Length / 2).ToList();

            int indiceValorMaximo =
                mitadValoresAbsolutos.IndexOf(
                mitadValoresAbsolutos.Max());

            float frecuenciaFundamental =
               (float)(indiceValorMaximo * formato.SampleRate)
               / (float)valoresAbsolutos.Length;

            Console.WriteLine(frecuenciaFundamental.ToString("n") + " Hz");

            Vector coordinates = DetermineCoordinates(frecuenciaFundamental);
            currentCoordinates.Add(coordinates);

            var board = (GameBoard)mainGrid.Children[0];
            board.PreviewResult(coordinates);
        }

        private Vector DetermineCoordinates(double frequency) {
            Vector coordinates = new Vector(0, 0);

            int s = 110; // Starting frequency
            int f = 240;
            int i = (f - s) / 7;

            if (frequency < s) {
                coordinates = new Vector(0, 0);
            }
            else if (frequency >= s && frequency < s + (i * 1)) {
                coordinates = new Vector(1, 0);
            }
            else if (frequency >= s + (i * 1) && frequency < s + (i * 2)) {
                coordinates = new Vector(2, 0);
            }
            else if (frequency >= s + (i * 2) && frequency < s + (i * 3)) {
                coordinates = new Vector(0, 1);
            }
            else if (frequency >= s + (i * 3) && frequency < s + (i * 4)) {
                coordinates = new Vector(1, 1);
            }
            else if (frequency >= s + (i * 4) && frequency < s + (i * 5)) {
                coordinates = new Vector(2, 1);
            }
            else if (frequency >= s + (i * 5) && frequency < s + (i * 6)) {
                coordinates = new Vector(0, 2);
            }
            else if (frequency >= s + (i * 6) && frequency < s + (i * 7)) {
                coordinates = new Vector(1, 2);
            }
            else {
                coordinates = new Vector(2, 2);
            }

            return coordinates;
        }
    }
}
