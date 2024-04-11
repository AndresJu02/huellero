using System;

using System.Data;

using System.Linq; //INQ (Language Integrated Query) es una característica de .NET que permite escribir consultas directamente en código C# (u otros lenguajes .NET) para interactuar con colecciones de objetos, bases de datos y otros tipos de datos. 

using System.Windows.Forms;

namespace ProyectHuellero
{
    public partial class frmRegistrar : Form
    {
        private DPFP.Template Template;
        private UsuariosDBEntities2 context;                           //acceso a la base de datos
        public frmRegistrar()
        {
            InitializeComponent();
        }

        private void frmRegistrar_Load(object sender, EventArgs e)
        {
            context = new UsuariosDBEntities2();                                //inicializamos la base de datos
            list();
        }

        
        private void Limpiar()
        {
            txtNombre.Text = "";
        }
        



        private void list()                                                         //actualizamos la base de datos
        {
            try
            {
                var usuarios = from usu in context.Usuarios
                               select new
                               {
                                   ID = usu.Id,
                                   USUARIO = usu.Nombre
                               };
                if (usuarios != null)                                        
                {
                    dgvList.DataSource = usuarios.ToList();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }//YO


        private void btnRegistrarHuella_Click_1(object sender, EventArgs e)
        {
            CaptuHuella capturar = new CaptuHuella();
            capturar.OnTemplate += this.OnTemplate;                      //aqui es cuendo se lee y se dispara este evento
            capturar.ShowDialog();
        }
        //ya
        private void OnTemplate(DPFP.Template template)
        {
            this.Invoke(new Function(delegate ()
            {
                Template = template;
                btnAgregar.Enabled = (Template != null);
                if (Template != null)
                {
                    MessageBox.Show("La plantilla de huellas dactilares está lista para la verificación de huellas dactilares.", "Registro de huellas dactilares");
                    txtHuella.Text = "Huella capturada correctamente";
                }
                else
                {
                    MessageBox.Show("La plantilla de huellas digitales no es válida. Repita el registro de huellas digitales.", "Registro de huellas digitales");
                }
            }));
        }

        private void txtHuella_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] streamHuella = Template.Bytes;                                            //traemos los bytes desde la variable de Template del evento OnTemplate
                Usuario usuario = new Usuario()
                {
                    Nombre = txtNombre.Text,
                    Huella = streamHuella
                };
                context.Usuarios.Add(usuario);                                                  //agregamos-Add una nueva entidad en la base de datos 
                context.SaveChanges();
                MessageBox.Show("Registro agregado a la BD correctamente.");
                Limpiar();
                list();
                Template = null;
                btnAgregar.Enabled = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            } // YO
        }
    }
}
