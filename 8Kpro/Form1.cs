﻿using SignalGenCSharp;
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

namespace _8Kpro
{
    public partial class Form1 : Form
    {
		const _BMD3DPreviewFormat kDefault3DPreviewFormat = _BMD3DPreviewFormat.bmd3DPreviewFormatTopBottom;
        private Thread m_deckLinkMainThread;
        private readonly EventWaitHandle m_applicationCloseWaitHandle;

        private DeckLinkDevice m_selectedDevice;
        private DeckLinkDeviceDiscovery m_deckLinkDeviceDiscovery;
        private ProfileCallback m_profileCallback;
        private PreviewCallback m_previewCallback;

        private DeckLinkDevice[] deckLinkPortDevices;

        private int Infz = 8,
                    vrCard = 0;

        private IReadOnlyList<StringObjectPair<_BMDVideoConnection>> kInputConnectionList = new List<StringObjectPair<_BMDVideoConnection>>
        {
            new StringObjectPair<_BMDVideoConnection>("SDI",            _BMDVideoConnection.bmdVideoConnectionSDI),
            new StringObjectPair<_BMDVideoConnection>("HDMI",           _BMDVideoConnection.bmdVideoConnectionHDMI),
            new StringObjectPair<_BMDVideoConnection>("Optical SDI",    _BMDVideoConnection.bmdVideoConnectionOpticalSDI),
            new StringObjectPair<_BMDVideoConnection>("Component",      _BMDVideoConnection.bmdVideoConnectionComponent),
            new StringObjectPair<_BMDVideoConnection>("Composite",      _BMDVideoConnection.bmdVideoConnectionComposite),
            new StringObjectPair<_BMDVideoConnection>("S-Video",        _BMDVideoConnection.bmdVideoConnectionSVideo)
        };

        private IReadOnlyList<StringObjectPair<_BMDTimecodeFormat>> kTimecodeFormatList = new List<StringObjectPair<_BMDTimecodeFormat>>
        {
            new StringObjectPair<_BMDTimecodeFormat>("VITC Field 1",    _BMDTimecodeFormat.bmdTimecodeVITC),
            new StringObjectPair<_BMDTimecodeFormat>("VITC Field 2",    _BMDTimecodeFormat.bmdTimecodeVITCField2),
            new StringObjectPair<_BMDTimecodeFormat>("RP188 VITC1",     _BMDTimecodeFormat.bmdTimecodeRP188VITC1),
            new StringObjectPair<_BMDTimecodeFormat>("RP188 VITC2",     _BMDTimecodeFormat.bmdTimecodeRP188VITC2),
            new StringObjectPair<_BMDTimecodeFormat>("RP188 LTC",       _BMDTimecodeFormat.bmdTimecodeRP188LTC),
            new StringObjectPair<_BMDTimecodeFormat>("RP188 HFRTC",     _BMDTimecodeFormat.bmdTimecodeRP188HighFrameRate),
        };

        public Form1()
        {
            InitializeComponent();
            m_applicationCloseWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            // Configurar un temporizador para llamar a RenderD3DImage (Prueba)
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 100; // Intervalo en milisegundos
            timer.Tick += RenderD3DImage;
            timer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            m_deckLinkMainThread = new Thread(() => DeckLinkMainThread());
            m_deckLinkMainThread.Start();
        }
        
        private void DeckLinkMainThread()
        {
            m_previewCallback = new PreviewCallback();
            m_previewCallback.RenderFrame += RenderD3DImage;
            m_previewCallback.PreviewHelper.Set3DPreviewFormat(kDefault3DPreviewFormat);

            m_deckLinkDeviceDiscovery = new DeckLinkDeviceDiscovery();
            m_deckLinkDeviceDiscovery.DeviceArrived += AddDevice;
            m_deckLinkDeviceDiscovery.DeviceRemoved += RemoveDevice;
            m_deckLinkDeviceDiscovery.Enable();

            m_applicationCloseWaitHandle.WaitOne();

            m_previewCallback.RenderFrame -= RenderD3DImage;

            m_deckLinkDeviceDiscovery.Disable();
            m_deckLinkDeviceDiscovery.DeviceArrived -= AddDevice;
            m_deckLinkDeviceDiscovery.DeviceRemoved -= RemoveDevice;
        }

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
                //stop capture and disable input
                m_selectedDevice.StopCapture();
            }
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

        private void UpdateComboVideoModes(DeckLinkDevice selectedDevice)
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
            });

            // Bind display mode list to combo 
            comboBoxVideoFormat.Invoke((Action)(() =>
            {
                // Bind display mode list to combo 
                comboBoxVideoFormat.DataSource = displayModeEntries;
                comboBoxVideoFormat.DisplayMember = "DisplayString";
                comboBoxVideoFormat.ValueMember = "Value";

                if (comboBoxVideoFormat.Items.Count > 0)
                    comboBoxVideoFormat.SelectedIndex = 0;
            }));
        }

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

        private void UpdatePorts(DeckLinkDevice device)
        {
            vrCard++;
            Console.WriteLine(device.DisplayName.ToString() + " " + vrCard);

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

        private void CardStartup(DeckLinkDevice[] deckLinkPortDevices)
        {
            this.Invoke((Action)(() =>
            {
                Port1.Text = deckLinkPortDevices[0].DisplayName.ToString();
                Port2.Text = deckLinkPortDevices[1].DisplayName.ToString();
                Port3.Text = deckLinkPortDevices[2].DisplayName.ToString();
                Port4.Text = deckLinkPortDevices[3].DisplayName.ToString();
            }));

            startCapture(deckLinkPortDevices[0]);
        }

        // Esta función debe ser la principal
        // Revisar como pasar el device name para ver a donde debe llegar
        private void startCapture(DeckLinkDevice deckLinkPortDevices)
        {
            m_selectedDevice = deckLinkPortDevices;
            bool applyDetectedInputMode = true;
            UpdateComboVideoModes(deckLinkPortDevices);

            _BMDDisplayMode displayMode = (_BMDDisplayMode)(comboBoxVideoFormat.Invoke((Func<object>)(() => comboBoxVideoFormat.SelectedValue)) ?? _BMDDisplayMode.bmdModeUnknown);

            if (displayMode != _BMDDisplayMode.bmdModeUnknown)
                new MTAAction(() => m_selectedDevice.StartCapture(displayMode, m_previewCallback, applyDetectedInputMode));
        }

        public void RenderD3DImage(object sender, EventArgs e)
        {
            var actualWidth = preview1.Width;
            var actualHeight = preview1.Height;

            // Realiza las operaciones de representación aquí, adaptando el código original
            // para trabajar con el PictureBox en lugar de D3DImage y WPF.
            // Dibuja en un Bitmap y luego establecerlo como la imagen del PictureBox.
            Bitmap bmp = new Bitmap(actualWidth, actualHeight);

            // Simula alguna representación en el Bitmap bmp (reemplaza esto con tu lógica de representación).
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.Clear(Color.Black);
                graphics.DrawString("Hello, Windows Forms!", new Font("Arial", 12), Brushes.White, new PointF(10, 10));
                // Aquí puedes realizar tus operaciones de representación en lugar del código D3D.
            }

            // Establece el Bitmap como la imagen del PictureBox.
            preview1.Image = bmp;
        }
    }
}
