using System;
using System.IO;


namespace ProyectHuellero
{
    public partial class frmVerificar : CaptureForm
    {
        public frmVerificar()
        {
            context = new UsuariosDBEntities2();
            InitializeComponent();
        }

        private DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;
        private UsuariosDBEntities2 context;

        public void Verify(DPFP.Template template)
        {
            Template = template;
            ShowDialog();

        }//YO
        private void UpdateStatus(int FAR)
        {

            SetStatus(String.Format("Rango de aceptación (FAR) = {0}", FAR));   // Muestra el valor de "tasa de aceptación falsa"
        }//yo
        protected override void Init()
        {
            base.Init();
            base.Text = "Verificación de Huella Digital";
            Verificator = new DPFP.Verification.Verification();     // inicializando un verificador de plantilla de huellas dactilares.
            UpdateStatus(0);
        }//ya





        private void frmVerificar_Load(object sender, EventArgs e)
        {

        }

        protected override void Process(DPFP.Sample Sample)                              //declarar que un método en una clase derivada:override
        {
            base.Process(Sample);

            // Procesa la muestra y crea un conjunto de funciones para fines de inscripción.
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            // Verifica la calidad de la muestra e inicie la verificación si es buena
            // TODO: pasar a una tarea separada
            if (features != null)
            {
                // Compara el conjunto de funciones con nuestra plantilla
                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result(); //almacenar el resultado de la verificación de la huella digital.

                DPFP.Template template = new DPFP.Template();
                Stream stream;                                 //manejar plantillas de huellas digitales y flujos de datos respectivamente.

                foreach (var usu in context.Usuarios)
                {
                    stream = new MemoryStream(usu.Huella);
                    template = new DPFP.Template(stream);

                    Verificator.Verify(features, template, ref result);
                    UpdateStatus(result.FARAchieved);
                    if (result.Verified)
                    {
                        MakeReport("La huella dactilar fue VERIFICADA. " + usu.Nombre);
                        break;
                    }
                }

                //yo
                //van 6
            }
        }

    }
}
