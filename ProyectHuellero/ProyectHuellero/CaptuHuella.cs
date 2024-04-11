using System;




namespace ProyectHuellero
{
    public partial class CaptuHuella : CaptureForm
    {
        public CaptuHuella()
        {
            InitializeComponent();
        }

        private void CaptuHuella_Load(object sender, EventArgs e)
        {
            
        }
        public delegate void OnTemplateEventHandler(DPFP.Template template);

        public event OnTemplateEventHandler OnTemplate;                                       //El EventHandler delegado es un delegado predefinido que representa específicamente un método de controlador de eventos para un evento que no genera datos. Si el evento genera datos, debe usar la clase delegada genérica

        private DPFP.Processing.Enrollment Enroller;                                          //declaracion creada gracias al SDK,  para leer la huella 

        protected override void Init()
        {
            base.Init();
            base.Text = "Dar de alta Huella";
            Enroller = new DPFP.Processing.Enrollment();            // Crea un registro
            UpdateStatus();
        }
        
        protected override void Process(DPFP.Sample Sample)                                         //sample huella leía
        {
            base.Process(Sample);

            // Procesa la muestra y crea un conjunto de funciones para fines de inscripción.
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);           //se extraen las features de la huella, indicando que es el proceso de (enrollment/registro)

            // Verifica la calidad de la muestra y agrega al registrador si es buena
            if (features != null) 
            {
                try
                {
                    MakeReport("Se creó el conjunto de funciones de huellas dactilares.");
                    Enroller.AddFeatures(features);     // mientras no termien las 4 muestras, le seguirá pidiendo muestras para crear un buen template 
                }
                finally
                {
                    UpdateStatus();

                    // Comprueba si se ha creado la plantilla.
                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:   // muestra que se tomo correctamente y detiene captura
                            OnTemplate(Enroller.Template);
                            SetPrompt("Haga clic en Cerrar y luego haga clic en Verificación de huellas digitales.");
                            Stop();
                            break;

                        case DPFP.Processing.Enrollment.Status.Failed:  // muestra que fallo y reinicia la captura 
                            Enroller.Clear();
                            Stop();
                            UpdateStatus();
                            OnTemplate(null);
                            Start();
                            break;
                    }

                }
            }//yo
        }

        private void UpdateStatus()
        {
            // Muestra el número de muestras necesarias, para completar el registro.
            SetStatus(String.Format("Muestras necesarias: {0}", Enroller.FeaturesNeeded));
        }//yo 8
      
    }
}
