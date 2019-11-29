﻿using System;
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
        public enum GameState { Menu, Reset, Player1, Player2 }
        public static GameState gameState = GameState.Menu;
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

        private void BtnRecord_Click(object sender, RoutedEventArgs e) {
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
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(3000) };
            timer.Start();
            timer.Tick += (_, args) =>
            {
                timer.Stop();
                waveIn.StopRecording();

                if (gameState == GameState.Player1) {
                    gameState = GameState.Player2;
                } else {
                    gameState = GameState.Player1;
                }
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

            Console.WriteLine(frecuenciaFundamental.ToString("n") +" Hz");

            Vector coordinates = DetermineCoordinates(frecuenciaFundamental);
            var board = (GameBoard)mainGrid.Children[0];
            board.PreviewResult(coordinates);
        }

        private Vector DetermineCoordinates(double frequency) {
            Vector coordinates = new Vector(0, 0);

            if (frequency < 100) {
                coordinates = new Vector(0, 0);
            }
            else if (frequency >= 100 && frequency < 115) {
                coordinates = new Vector(1, 0);
            }
            else if (frequency >= 115 && frequency < 130) {
                coordinates = new Vector(2, 0);
            }
            else if (frequency >= 130 && frequency < 145) {
                coordinates = new Vector(0, 1);
            }
            else if (frequency >= 145 && frequency < 160) {
                coordinates = new Vector(1, 1);
            }
            else if (frequency >= 160 && frequency < 175) {
                coordinates = new Vector(2, 1);
            }
            else if (frequency >= 175 && frequency < 190) {
                coordinates = new Vector(0, 2);
            }
            else if (frequency >= 190 && frequency < 205) {
                coordinates = new Vector(1, 2);
            }
            else {
                coordinates = new Vector(2, 2);
            }

            return coordinates;
        }
    }
}
