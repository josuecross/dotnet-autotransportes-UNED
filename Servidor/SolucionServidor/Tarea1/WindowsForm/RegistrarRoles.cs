﻿using GUI_Servidor.src;
using Entidades.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_Servidor
{
    public partial class RegistrarRoles : Form
    {
        Menu menu;
        Role[] roles;
        public RegistrarRoles(Menu _menu, Role[] _roles)
        {
            InitializeComponent();
            this.menu = _menu;
            this.roles = _roles;
        }

        private void guardarbutton1_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(fechadateTimePicker.Text);
            DateTime departureHour = new DateTime();
            int idRuta = new int();
            int idAutobus = new int();
            string idConductor = conductortextBox.Text;

            Route ruta = new Route();
            Autobus autobus = new Autobus();
            Driver conductor = new Driver();

            //verificar que la fecha elegida sea con 2 dias de antelacion;
            DateTime dateNow = DateTime.Now;
            if(date < dateNow.AddDays(2))
            {
                MessageBox.Show("Debe elegir fechas con 2 dias de antelacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            // Obtener la hora de salida o una excepcion si no fueron ingresadas correctamente
            try
            {
                horaSalidacomboBox.BackColor = Color.White;
                departureHour = DateTime.ParseExact(horaSalidacomboBox.Text, "H:m", null); ;
            }
            catch (System.FormatException ex)
            {
                horaSalidacomboBox.BackColor = Color.LightSalmon;
                MessageBox.Show("Debe seleccionar correctamente la hora de salida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Herramientas.validarDatoNumerico(ref idRuta, idRutatextBox) && Herramientas.validarDatoNumerico(ref idAutobus, idAutobustextBox))
            {
                if(validarObjetoPorId(idRuta, idRutatextBox, ref ruta) && validarObjetoPorId(idAutobus, idAutobustextBox,ref  autobus)
                    && validarObjetoPorId(idConductor, conductortextBox, ref conductor)){
                    for (int i = 0; i < 20; i++)
                    {
                        //Si el indice i contiene un rol se procede a validarla
                        if (this.roles[i] != null)
                        {
                            //Verifica que el autobus seleccionado no este en marcha durante 2 horas siguientes a la hora seleccionada
                            if (this.roles[i].DepartureHour.AddHours(2) > departureHour && this.roles[i].Autobus.Id == autobus.Id)
                            {
                                MessageBox.Show("El autobus se encuentra en marcha, debe elegir otra hora", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            //Verifica que el conductor no este ocupando en otra ruta y que no sea supervisor
                            if ((this.roles[i].Conductor.Id.Equals(conductor.Id) && this.roles[i].DepartureHour.AddHours(2) > departureHour) || conductor.DriverSupervisor)
                            {
                                MessageBox.Show("El conductor se encuentra en marcha o es supervisor, debe elegir otra conductor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                        }
                        //Si se alcanza un indice sin ruta se procede a cargar el rol
                        if (this.roles[i] == null)
                        {
                            this.roles[i] = new Role(date, departureHour, ruta, autobus, conductor);
                            MessageBox.Show("Rol agregado correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clearFields();
                            return;
                        }
                        

                    }

                }
                else
                {
                    MessageBox.Show("Revise el id ingresado ya que no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            else
            {
                MessageBox.Show("El valor que intenta agregar no es un número.Intente de nuevo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        //Funciones con polimorfismo para validar que los distintos objetos de conductor,ruta y autobus existan
        private bool validarObjetoPorId(int id, TextBox _textboxValidar, ref Route _asignarObjeto)
        {
            _textboxValidar.BackColor = Color.White;

            for (int i = 0; i < 20; i++)
            {
                if (menu.Rutas[i].Id == id)
                {
                    _asignarObjeto = menu.Rutas[i];
                    return true;
                }
            }

            _textboxValidar.BackColor = Color.LightSalmon;
            return false;
        }
        private bool validarObjetoPorId(int id, TextBox _textboxValidar, ref Autobus _asignarObjeto)
        {
            for (int i = 0; i < 20; i++)
            {
                if (menu.Autobuses[i] != null)
                {
                    if (menu.Autobuses[i].Id == id)
                    {
                        _asignarObjeto = menu.Autobuses[i];
                        return true;
                    }
                }
                
            }
            _textboxValidar.BackColor = Color.LightSalmon;
            return false;
        }

        private bool validarObjetoPorId(string id, TextBox _textboxValidar, ref Driver _asignarObjeto)
        {
            _textboxValidar.BackColor = Color.White;
            for (int i = 0; i < 20; i++)
            {
                if(menu.Conductores[i] != null)
                {
                    if (menu.Conductores[i].Id == id)
                    {
                        _asignarObjeto = menu.Conductores[i];
                        return true;
                    }

                }
                
            }
            _textboxValidar.BackColor = Color.LightSalmon;
            return false;
        }

        private void clearFields()
        {
            fechadateTimePicker.ResetText();
            horaSalidacomboBox.ResetText();
            horaSalidacomboBox.Text = "-Seleccione";
            idRutatextBox.Clear();
            idAutobustextBox.Clear();
            conductortextBox.Clear();
            rutalabel.Text = "Ruta: " + idRutatextBox.Text + " ( no encontrado)";
            terminalOrigenlabel.Text = "Origen: ";
            terminalDestinolabel.Text = "Destino: ";
            tarifalabel.Text = "Tarifa: ";
            tipolabel.Text = "Tipo: ";

        }

        private void idRutatextBox_TextChanged(object sender, EventArgs e)
        {
            //Se obtendran los datos de la ruta cada vez que se cambie si campo id en el formulario y se mostraran
            //los datos requeridos en el mismo 
            int idAValidar = new int();
            Route ruta = new Route();
            idRutatextBox.BackColor = Color.White;
            if (Herramientas.validarDatoNumerico(ref idAValidar, idRutatextBox))
            {
                if(validarObjetoPorId(idAValidar, idRutatextBox, ref ruta))
                {
                    rutalabel.ForeColor = Color.Green;
                    rutalabel.Text = "Ruta: " + idRutatextBox.Text + " (encontrado)";
                    terminalOrigenlabel.Text = "Origen: " + ruta.DepartureTerminal;
                    terminalDestinolabel.Text = "Destino: " + ruta.ArriveTerminal;
                    tarifalabel.Text = "Tarifa: " + ruta.Rate;
                    tipolabel.Text = "Tipo: " + ruta.Type;
                }
                else
                {
                    rutalabel.ForeColor = Color.Red;
                    rutalabel.Text = "Ruta: " + idRutatextBox.Text + " ( no encontrado)";
                    terminalOrigenlabel.Text = "Origen: ";
                    terminalDestinolabel.Text = "Destino: ";
                    tarifalabel.Text = "Tarifa: ";
                    tipolabel.Text = "Tipo: ";
                }

            }
            
        }

        private void limpiarbutton_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void idAutobustextBox_TextChanged(object sender, EventArgs e)
        {
            idAutobustextBox.BackColor = Color.White;
            int idAValidar = new int();
            Autobus autobus = new Autobus();
            if (Herramientas.validarDatoNumerico(ref idAValidar, idAutobustextBox))
            {
                if (validarObjetoPorId(idAValidar, idAutobustextBox, ref autobus))
                {
                    autobuslabel.ForeColor = Color.Green;
                    autobuslabel.Text = "Autobus: " + idAutobustextBox.Text + " (encontrado)";
                    placalabel.Text = "Placa: " + autobus.PlateNumber;
                    marcalabel.Text = "Marca: " + autobus.Brand;
                    capacidadlabel.Text = "Capacidad: " + autobus.Capacidad;
                }
                else
                {
                    autobuslabel.ForeColor = Color.Red;
                    autobuslabel.Text = "Autobus: " + idAutobustextBox.Text + " (no encontrado)";
                    placalabel.Text = "Placa: " ;
                    marcalabel.Text = "Marca: " ;
                    capacidadlabel.Text = "Capacidad: ";
                }

            }
        }


        private void RegistrarRoles_Load(object sender, EventArgs e)
        {
            //Verificar que existan terminales, sino existen no se permite agregar roles
            mensajelabel1.ForeColor = Color.Black;
            labelFecha.Visible = true;
            fechadateTimePicker.Visible = true;
            horaSalidacomboBox.Visible = true;
            idRutatextBox.Visible = true;
            idAutobustextBox.Visible = true;
            conductortextBox.Visible = true;
            guardarbutton1.Visible = true;
            limpiarbutton.Visible = true;

            mensajelabel1.Text = "Por favor ingrese los datos solicitados";

            string objetosFaltantes = "";

            bool existeTerminal = true;
            bool existeAutobus = true;
            bool existeConductor = true;

            if (this.menu.Terminales[0] == null)
            {
                existeTerminal = false;
                objetosFaltantes += "\nTerminal";
            }
            if (this.menu.Autobuses[0] == null)
            {
                existeAutobus = false;
                objetosFaltantes += "\nAutobus";
            }
            if (this.menu.Conductores[0] == null)
            {
                existeConductor = false;
                objetosFaltantes += "\nConductor";
            }

            if (!existeTerminal || !existeConductor || !existeAutobus)
            {
                mensajelabel1.ForeColor = Color.Red;
                labelFecha.Visible = false;
                fechadateTimePicker.Visible = false;
                horaSalidacomboBox.Visible = false;
                idRutatextBox.Visible = false;
                idAutobustextBox.Visible = false;
                conductortextBox.Visible = false;
                guardarbutton1.Visible = false;
                limpiarbutton.Visible = false;
                mensajelabel1.Text = "DEBE INGRESAR CADA UNO DE LOS SIGUIENTES ELEMENTOS ANTES DE AGREGAR UN ROL" + objetosFaltantes;

            }

        }
    }
}
