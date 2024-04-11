using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProyectHuellero
{
    delegate void Function();

    public partial class CaptureForm : Form, DPFP.Capture.EventHandler
	{
		public CaptureForm()
		{
			InitializeComponent();
		}

		protected virtual void Init()                 //indica que este método puede ser sobrescrito por las clases que heredan de esta clase, virtual.
		{
            try
            {
                Capturer = new DPFP.Capture.Capture();              // crea una captura

				if (null != Capturer)
				{
					Capturer.EventHandler = this;
				}
				else
				{
					SetPrompt("No se pudo iniciar la operación de captura");
				}
            }
            catch
            {               
                MessageBox.Show("No se pudo iniciar la operación de captura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);            
            }//yo9
		}

		protected virtual void Process(DPFP.Sample Sample)
		{
			// dibujar una imagen de la muestra de la huella digital.
			DrawPicture(ConvertSampleToBitmap(Sample));
		}//yo

		protected void Start()
		{
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                    SetPrompt("Escanea tu huella usando el lector");
                }
                catch
                {
                    SetPrompt("No se puede iniciar la captura");
                }
            }
		}

		protected void Stop()
		{
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                    SetPrompt("No se puede terminar la captura");
                }
            }
		}
		
	

		private void CaptureForm_Load(object sender, EventArgs e)
		{
			Init();
			Start();                                                // Iniciar operación de captura
		}//yo10

		private void CaptureForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Stop();
		}//yo11


		public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
		{
			MakeReport("La muestra ha sido capturada");
			SetPrompt("Escanea tu misma huella otra vez");
			Process(Sample);
		}

		public void OnFingerGone(object Capture, string ReaderSerialNumber)
		{
			MakeReport("La huella fue removida del lector");
		}

		public void OnFingerTouch(object Capture, string ReaderSerialNumber)
		{
			MakeReport("El lector fue tocado");
		}

		public void OnReaderConnect(object Capture, string ReaderSerialNumber)
		{
			MakeReport("El Lector de huellas ha sido conectado");
		}

		public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
		{
			MakeReport("El Lector de huellas ha sido desconectado");
		}

		public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
		{
			if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
				MakeReport("La calidad de la muestra es BUENA");
			else
				MakeReport("La calidad de la muestra es MALA");
		}
	

		protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
		{
			DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();  // Crea un convertidor de muestra
			Bitmap bitmap = null;                                                           // el tamaño no importa
			Convertor.ConvertToPicture(Sample, ref bitmap);                                 // devolver mapa de bits como resultado
			return bitmap;
		}//yo12

		protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
		{
			DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();  // Crear un extractor de funciones
			DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
			DPFP.FeatureSet features = new DPFP.FeatureSet();
			Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);			
			if (feedback == DPFP.Capture.CaptureFeedback.Good)
				return features;
			else
				return null;
		}//yo13


		protected void SetStatus(string status)
		{
			this.Invoke(new Function(delegate() {
				StatusLine.Text = status;                   // actualiza el texto de Estado linea
			}));
		}//14

		protected void SetPrompt(string prompt)
		{
			this.Invoke(new Function(delegate() {
				Prompt.Text = prompt;                                     
			}));
		}//15
		protected void MakeReport(string message)
		{
			this.Invoke(new Function(delegate() {
				StatusText.AppendText(message + "\r\n");   //este método es parte de la API de Windows Forms y está diseñado para ser utilizado en aplicaciones de interfaz de usuario
			}));
		}//16

		private void DrawPicture(Bitmap bitmap)
		{
			this.Invoke(new Function(delegate() {                     //Invoke  se utiliza para asegurarse de que el código que actualiza el control PictureBox se ejecute en el subproceso de la interfaz de usuario.
				Picture.Image = new Bitmap(bitmap, Picture.Size);   // encajar la imagen en el cuadro de imagen
			}));
		}//17

		private DPFP.Capture.Capture Capturer;

        private void StatusLabel_Click(object sender, EventArgs e)
        {

        }

        private void StatusLine_Click(object sender, EventArgs e)
        {

        }
    }
}