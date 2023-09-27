using SignalGenCSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using DeckLinkAPI;
using CapturePreviewCSharp;
using SharpDX.Direct3D9;
using static SharpDX.Utilities;
using System.Security.Claims;
using System.Security.Cryptography;

namespace _8Kpro
{
    public partial class Form1 : Form
    {
        private Thread m_deckLinkMainThread;
        private readonly IDeckLinkInput m_deckLinkInput;
        private readonly EventWaitHandle m_applicationCloseWaitHandle;

        // Variables de desarrollo
        private DeckLinkDeviceDiscovery m_deckLinkDeviceDiscovery; // Dispositivo encontrado
        private DeckLinkDevice m_selectedDevice;                   // Dispositivo seleccionado

        private ProfileCallback m_profileCallback;                 // Aun no se para que funciona esta variable
        private PreviewCallback m_previewCallback;

        // Lista 
        private IReadOnlyList<StringObjectPair<_BMDVideoConnection>> kInputConnectionList = new List<StringObjectPair<_BMDVideoConnection>>
        {
            new StringObjectPair<_BMDVideoConnection>("SDI",            _BMDVideoConnection.bmdVideoConnectionSDI),
            new StringObjectPair<_BMDVideoConnection>("HDMI",           _BMDVideoConnection.bmdVideoConnectionHDMI),
            new StringObjectPair<_BMDVideoConnection>("Optical SDI",    _BMDVideoConnection.bmdVideoConnectionOpticalSDI),
            new StringObjectPair<_BMDVideoConnection>("Component",      _BMDVideoConnection.bmdVideoConnectionComponent),
            new StringObjectPair<_BMDVideoConnection>("Composite",      _BMDVideoConnection.bmdVideoConnectionComposite),
            new StringObjectPair<_BMDVideoConnection>("S-Video",        _BMDVideoConnection.bmdVideoConnectionSVideo)
        };

        private DeckLinkDevice[] deckLinkPortDevices;

        private int Infz = 8,
                    vrCard = 0;

        // Estructuras
        struct DisplayModeEntry
        /// Used for putting the BMDDisplayMode value into the video format combo
        {
            private readonly string m_displayString;

            public DisplayModeEntry(IDeckLinkDisplayMode displayMode)
            {
                Value = displayMode.GetDisplayMode();
                displayMode.GetName(out m_displayString);
            }

            public _BMDDisplayMode Value { get; set; }
            public string DisplayString { get => m_displayString; }
        }

        struct StringObjectPair<T>
        {
            public StringObjectPair(string name, T value)
            {
                Name = name;
                Value = value;
            }
            public string Name { get; }
            public T Value { get; set; }
            public override string ToString() => Name;
        }

        public Form1()
        {
            InitializeComponent();

            m_applicationCloseWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            m_deckLinkMainThread = new Thread(() => DeckLinkMainThread());
            m_deckLinkMainThread.Start();
        }

        private void DeckLinkMainThread()
        {
            m_deckLinkDeviceDiscovery = new DeckLinkDeviceDiscovery();
            m_deckLinkDeviceDiscovery.DeviceArrived += AddDevice;
            m_deckLinkDeviceDiscovery.DeviceRemoved += RemoveDevice;
            m_deckLinkDeviceDiscovery.Enable();
            
            m_applicationCloseWaitHandle.WaitOne();

            m_deckLinkDeviceDiscovery.Disable();
            m_deckLinkDeviceDiscovery.DeviceArrived -= AddDevice;
            m_deckLinkDeviceDiscovery.DeviceRemoved -= RemoveDevice;
        }

        // Funciones Decklink Device Discovery
        public void AddDevice(object sender, DeckLinkDiscoveryEventArgs e)
        {
            try
            {
                DeckLinkDevice device = new DeckLinkDevice(e.deckLink, m_profileCallback);
                var deviceActive = device.IsActive;

                UpdatePorts(device);

                if (vrCard == Infz)
                {
                    CardStartup(deckLinkPortDevices);
                    UpdateVideoModes(deckLinkPortDevices[0]);
                    UpdateInputConnections(deckLinkPortDevices[0]);
                    startCapture(deckLinkPortDevices[0]);
                }
            }
            catch (DeckLinkInputInvalidException)
            {
                // Device does not support input interface
            }
        }

        public void RemoveDevice(object sender, DeckLinkDiscoveryEventArgs e)
        {
            if (m_selectedDevice?.DeckLink == e.deckLink)
            {
                // stop capture and disable input
                m_selectedDevice.StopCapture();
            }            
        }

        // Lista todos los puertos disponibles en las tarjetas instaladas
        private void UpdatePorts(DeckLinkDevice device)
        {
            vrCard++;

            if (deckLinkPortDevices == null)
            {
                deckLinkPortDevices = new DeckLinkDevice[] { device };
            }
            else
            {
                DeckLinkDevice[] newDevicesArray = new DeckLinkDevice[deckLinkPortDevices.Length + 1];
                deckLinkPortDevices.CopyTo(newDevicesArray, 0);
                newDevicesArray[newDevicesArray.Length - 1] = device;
                deckLinkPortDevices = newDevicesArray;
            }
        }

        // Prueba a poner los nombres de los puertos a usar
        private void CardStartup(DeckLinkDevice[] deckLinkPortDevices)
        {
            this.Invoke((Action)(() =>
            {
                Port1.Text = deckLinkPortDevices[0].DisplayName.ToString();
                //Port2.Text = deckLinkPortDevices[1].DisplayName.ToString();
                //Port3.Text = deckLinkPortDevices[2].DisplayName.ToString();
                //Port4.Text = deckLinkPortDevices[3].DisplayName.ToString();
            }));
        }

        // Función para obtener todos los modos de video disponibles por la tarjeta y puertos
        private void UpdateVideoModes(DeckLinkDevice selectedDevice)
        {
            if (selectedDevice == null)
                return;

            List<DisplayModeEntry> displayModeEntries = new List<DisplayModeEntry>();

            new MTAAction(() =>
            {
                foreach (IDeckLinkDisplayMode displayMode in selectedDevice.DisplayModes)
                {
                    displayModeEntries.Add(new DisplayModeEntry(displayMode));
                }

                Invoke(new Action(() =>
                {
                    comboBoxDisplayModes.Items.Clear();

                    comboBoxDisplayModes.DataSource = displayModeEntries;
                    comboBoxDisplayModes.DisplayMember = "DisplayString";
                    comboBoxConnection.ValueMember = "Value";
                }));
            });

            // Aqui hay que agregar 
            // Averiguar por que selecciona el 0 cuando es otro tipo de entrada
            //if (comboBoxDisplayModes.Items.Count > 0)
            //    comboBoxDisplayModes.SelectedIndex = 0;
        }

        private void UpdateInputConnections(DeckLinkDevice selectedDevice)
        {
            if (selectedDevice == null)
                return;

            Invoke(new Action(() =>
            {
                List<StringObjectPair<_BMDVideoConnection>> inputConnections = kInputConnectionList.Where(pair => selectedDevice.AvailableInputConnections.HasFlag(pair.Value)).ToList();
            
                comboBoxConnection.DataSource = inputConnections;
                comboBoxConnection.DisplayMember = "Name";
                comboBoxConnection.ValueMember = "Value";

                if (comboBoxConnection.Items.Count > 0)
                    comboBoxConnection.SelectedIndex = 0;
            }));
        }

        // Inicia la rutina para la visualización de video
        private void startCapture(DeckLinkDevice selectedDevice)
        {
            if (selectedDevice == null)
                return;

            //bool applyDetectedInputMode = true; // Para pruebas va a ser siempre true



            Invoke(new Action(() =>
            {
                
                //    _BMDDisplayMode displayMode = (_BMDDisplayMode)(comboBoxDisplayModes.SelectedValue?? _BMDDisplayMode.bmdModeUnknown);

                //    if (displayMode != _BMDDisplayMode.bmdModeUnknown)
                //        new MTAAction(() => m_selectedDevice.StartCapture(displayMode, m_previewCallback, applyDetectedInputMode));
            }));

        }

        // Cierra las instancias
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_applicationCloseWaitHandle.Set();
            m_deckLinkMainThread.Join();
        }
    }
}
