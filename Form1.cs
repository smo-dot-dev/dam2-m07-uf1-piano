using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toub.Sound.Midi;
using System.Threading;

namespace piano_smunoz
{
    public partial class Form1 : Form
    {
        const int canal = 1; //Canal midi, del 0-15, 15 és percusió
        byte volum = 127; //Intensitat que premem la tecla
        Dictionary<string, string> tecla_notes; //Crearem un diccionari que relaciona de tecla a nota
        List<string> tecles;//Lista de tecles premudes per evitar el ghosting o repetició de notes sostingudes
        KeysConverter kc; //Per saber el char corresponent a cada tecla
        string teclesValides = "QWERTYUIOPGHJKL1234567890";

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            MidiPlayer.OpenMidi();//Iniciem el objecte MIDI
            comboBox1.DataSource = Enum.GetValues(typeof(GeneralMidiInstruments));//Carreguem la llista de 128 instruments
            MidiPlayer.Play(new ProgramChange(0, canal, GeneralMidiInstruments.AcousticGrand));//Seleccionem el instrument per defecte del ComboBox
            tecla_notes = new Dictionary<string, string>();//Inicialitzem diccionari associatiu
            tecla_notes.Add("Q", "C4"); //Afegim les 15 notes, 2 octaves (4, 5 i Do de octava 6)
            tecla_notes.Add("W", "D4");
            tecla_notes.Add("E", "E4");
            tecla_notes.Add("R", "F4");
            tecla_notes.Add("T", "G4");
            tecla_notes.Add("Y", "A4");
            tecla_notes.Add("U", "B4");
            tecla_notes.Add("I", "C5");
            tecla_notes.Add("O", "D5");
            tecla_notes.Add("P", "E5");
            tecla_notes.Add("G", "F5");
            tecla_notes.Add("H", "G5");
            tecla_notes.Add("J", "A5");
            tecla_notes.Add("K", "B5");
            tecla_notes.Add("L", "C6");
            tecla_notes.Add("1", "C#4");
            tecla_notes.Add("2", "D#4");
            tecla_notes.Add("3", "F#4");
            tecla_notes.Add("4", "G#4");
            tecla_notes.Add("5", "A#4");
            tecla_notes.Add("6", "C#5");
            tecla_notes.Add("7", "D#5");
            tecla_notes.Add("8", "F#5");
            tecla_notes.Add("9", "G#5");
            tecla_notes.Add("0", "A#5");
            kc = new KeysConverter();//Inicialitzem parser de tecles
            //Afegim a cada botó un event MouseDown i MouseUp
            foreach (Control item in this.Controls)
            {
                if (item is Button)
                {
                    item.MouseDown += new MouseEventHandler(tocarNota_mouseDown);
                    item.MouseUp += new MouseEventHandler(soltarNota_mouseUp);
                }
            }
            tecles = new List<string>();//Inicialitzem List
        }

        //Passem una tecla, i fa servir el diccionari per passar-la a nota, per exemple Q => C4
        private void tocarNota(string tecla)
        {
            if (!tecles.Contains(tecla))
            {
                tecles.Add(tecla);
                MidiPlayer.Play(new NoteOn(0, canal, tecla_notes[tecla], volum));
            }
            
        }
        private void soltarNota(string tecla)
        {
            if (tecles.Contains(tecla))
            {
                tecles.Remove(tecla);
                MidiPlayer.Play(new NoteOff(0, canal, tecla_notes[tecla], volum));
            }
        }

        //Tocar el piano per ratolí
        private void tocarNota_mouseDown(object sender, EventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            tocarNota(tag);//Fem servir el tag corresponent al botó
        }

        private void soltarNota_mouseUp(object sender, EventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            soltarNota(tag);
        }

        //Tocar el piano per teclat
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {//Agafem el KeyData de la tecla i comprovem que estarà al diccionari
            string tecla = kc.ConvertToString(e.KeyData).ToUpper();
            if (teclesValides.Contains(tecla)) tocarNota(tecla);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            string tecla = kc.ConvertToString(e.KeyData).ToUpper();
            if (teclesValides.Contains(tecla)) soltarNota(tecla);
        }

        //Canvi de instrument al combobox
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            GeneralMidiInstruments instrument = (GeneralMidiInstruments)System.Enum.Parse(typeof(GeneralMidiInstruments), comboBox1.SelectedItem.ToString());
            MidiPlayer.Play(new ProgramChange(0, canal, instrument));
        }
    }
}
