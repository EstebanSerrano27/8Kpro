﻿/* -LICENSE-START-
** Copyright (c) 2020 Blackmagic Design
**  
** Permission is hereby granted, free of charge, to any person or organization 
** obtaining a copy of the software and accompanying documentation (the 
** "Software") to use, reproduce, display, distribute, sub-license, execute, 
** and transmit the Software, and to prepare derivative works of the Software, 
** and to permit third-parties to whom the Software is furnished to do so, in 
** accordance with:
** 
** (1) if the Software is obtained from Blackmagic Design, the End User License 
** Agreement for the Software Development Kit (“EULA”) available at 
** https://www.blackmagicdesign.com/EULA/DeckLinkSDK; or
** 
** (2) if the Software is obtained from any third party, such licensing terms 
** as notified by that third party,
** 
** and all subject to the following:
** 
** (3) the copyright notices in the Software and this entire statement, 
** including the above license grant, this restriction and the following 
** disclaimer, must be included in all copies of the Software, in whole or in 
** part, and all derivative works of the Software, unless such copies or 
** derivative works are solely in the form of machine-executable object code 
** generated by a source language processor.
** 
** (4) THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
** OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
** FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT 
** SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE 
** FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE, 
** ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
** DEALINGS IN THE SOFTWARE.
** 
** A copy of the Software is available free of charge at 
** https://www.blackmagicdesign.com/desktopvideo_sdk under the EULA.
** 
** -LICENSE-END-
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DeckLinkAPI;

namespace CapturePreviewCSharp
{
	public partial class MainWindow : Window
	{
		const _BMD3DPreviewFormat kDefault3DPreviewFormat = _BMD3DPreviewFormat.bmd3DPreviewFormatTopBottom;

		private Thread m_deckLinkMainThread;
		private readonly EventWaitHandle m_applicationCloseWaitHandle;

		private DeckLinkDevice m_selectedDevice;
		private DeckLinkDeviceDiscovery m_deckLinkDeviceDiscovery;
		private ProfileCallback m_profileCallback;
		private PreviewCallback m_previewCallback;

		private IReadOnlyList<StringObjectPair<_BMDVideoConnection>> kInputConnectionList = new List<StringObjectPair<_BMDVideoConnection>>
		{
			new StringObjectPair<_BMDVideoConnection>("SDI",			_BMDVideoConnection.bmdVideoConnectionSDI),
			new StringObjectPair<_BMDVideoConnection>("HDMI",			_BMDVideoConnection.bmdVideoConnectionHDMI),
			new StringObjectPair<_BMDVideoConnection>("Optical SDI",	_BMDVideoConnection.bmdVideoConnectionOpticalSDI),
			new StringObjectPair<_BMDVideoConnection>("Component",		_BMDVideoConnection.bmdVideoConnectionComponent),
			new StringObjectPair<_BMDVideoConnection>("Composite",		_BMDVideoConnection.bmdVideoConnectionComposite),
			new StringObjectPair<_BMDVideoConnection>("S-Video",		_BMDVideoConnection.bmdVideoConnectionSVideo)
		};

		private IReadOnlyList<StringObjectPair<_BMD3DPreviewFormat>> k3DPreviewFormatList = new List<StringObjectPair<_BMD3DPreviewFormat>>
		{
			new StringObjectPair<_BMD3DPreviewFormat>("Left-Eye Frame",		_BMD3DPreviewFormat.bmd3DPreviewFormatLeftOnly),
			new StringObjectPair<_BMD3DPreviewFormat>("Right-Eye Frame",	_BMD3DPreviewFormat.bmd3DPreviewFormatRightOnly),
			new StringObjectPair<_BMD3DPreviewFormat>("Side by Side",		_BMD3DPreviewFormat.bmd3DPreviewFormatSideBySide),
			new StringObjectPair<_BMD3DPreviewFormat>("Top-Bottom",			_BMD3DPreviewFormat.bmd3DPreviewFormatTopBottom),
		};

		private IReadOnlyList<StringObjectPair<_BMDTimecodeFormat>> kTimecodeFormatList = new List<StringObjectPair<_BMDTimecodeFormat>>
		{
			new StringObjectPair<_BMDTimecodeFormat>("VITC Field 1",	_BMDTimecodeFormat.bmdTimecodeVITC),
			new StringObjectPair<_BMDTimecodeFormat>("VITC Field 2",	_BMDTimecodeFormat.bmdTimecodeVITCField2),
			new StringObjectPair<_BMDTimecodeFormat>("RP188 VITC1",		_BMDTimecodeFormat.bmdTimecodeRP188VITC1),
			new StringObjectPair<_BMDTimecodeFormat>("RP188 VITC2",		_BMDTimecodeFormat.bmdTimecodeRP188VITC2),
			new StringObjectPair<_BMDTimecodeFormat>("RP188 LTC",		_BMDTimecodeFormat.bmdTimecodeRP188LTC),
			new StringObjectPair<_BMDTimecodeFormat>("RP188 HFRTC",		_BMDTimecodeFormat.bmdTimecodeRP188HighFrameRate),
		};

		private IReadOnlyList<IMetadataItem> kMetadataItemList = new List<IMetadataItem>
		{
			new	MetadataItemEOTF("HDR Electro-Optical Transfer Function",		_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRElectroOpticalTransferFunc),
			new MetadataItemDouble("HDR Display Primaries Red X",				_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRDisplayPrimariesRedX),
			new MetadataItemDouble("HDR Display Primaries Red Y",				_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRDisplayPrimariesRedY),
			new MetadataItemDouble("HDR Display Primaries Green X",				_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRDisplayPrimariesGreenX),
			new MetadataItemDouble("HDR Display Primaries Green Y",				_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRDisplayPrimariesGreenY),
			new MetadataItemDouble("HDR Display Primaries Blue X",				_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRDisplayPrimariesBlueX),
			new MetadataItemDouble("HDR Display Primaries Blue Y",				_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRDisplayPrimariesBlueY),
			new MetadataItemDouble("HDR White Point X",							_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRWhitePointX),
			new MetadataItemDouble("HDR White Point Y",							_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRWhitePointY),
			new MetadataItemDouble("HDR Maximum Display Mastering Luminance",	_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRMaxDisplayMasteringLuminance),
			new MetadataItemDouble("HDR Minimum Display Mastering Luminance",	_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRMinDisplayMasteringLuminance),
			new MetadataItemDouble("HDR Maximum Content Light Level",			_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRMaximumContentLightLevel),
			new MetadataItemDouble("HDR Frame Average Light Level",				_BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataHDRMaximumFrameAverageLightLevel),
		};

		public class TimecodeData
		{
			public string Format { get; set; }
			public string Value { get; set; }
			public string UserBits { get; set; }
		}

		public class VANCPacketData
		{
			public uint Line { get; set; }
			public string DID { get; set; }
			public string SDID { get; set; }
			public string Data { get; set; }
		}

		public class MetadataData
		{
			public string Item { get; set; }
			public string Value { get; set; }
		}

		private static void UpdateUIElement(DispatcherObject element, Action action)
		{
			if (element != null)
			{
				if (!element.Dispatcher.CheckAccess())
					element.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
				else
					action();
			}
		}

		private void ComboBoxSelectFirstEnabledItem(ComboBox comboBox)
		{
			comboBoxDevice.SelectedItem = comboBox.Items.Cast<object>().FirstOrDefault(item => ((ComboBoxItem)item).IsEnabled);
		}

		public MainWindow()
		{
			InitializeComponent();
			m_applicationCloseWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
		}

		#region dl_events
		// All events occur in MTA threading context
		public void AddDevice(object sender, DeckLinkDiscoveryEventArgs e)
		{
			try
			{
				DeckLinkDevice device = new DeckLinkDevice(e.deckLink, m_profileCallback);
				var deviceActive = device.IsActive;

				// Update combo box with new device
				UpdateUIElement(comboBoxDevice, new Action(() => UpdateComboNewDevice(device, deviceActive)));
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

			// Remove device from combo box
			UpdateUIElement(comboBoxDevice, new Action(() => UpdateComboRemoveDevice(e.deckLink)));
		}

		public void RenderD3DImage(object sender, EventArgs e)
		{
			UpdateUIElement(d3dPreview, new Action(() =>
			{
				var actualWidth = gridPreview.RenderSize.Width;
				var actualHeight = gridPreview.RenderSize.Height;

				if (d3dPreview.IsFrontBufferAvailable)
				{
					IntPtr surface = IntPtr.Zero;
					if (actualWidth > 0 && actualHeight > 0)
					{
						new MTAAction(() =>
						{
							m_previewCallback.PreviewHelper.SetSurfaceSize((uint)actualWidth, (uint)actualHeight);
							m_previewCallback.PreviewHelper.GetBackBuffer(out surface);
						});
					}
					if (surface != IntPtr.Zero)
					{
						d3dPreview.Lock();
						d3dPreview.SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, surface);
						new MTAAction(() => m_previewCallback.PreviewHelper.Render());
						d3dPreview.AddDirtyRect(new Int32Rect(0, 0, d3dPreview.PixelWidth, d3dPreview.PixelHeight));
						d3dPreview.Unlock();
					}
				}
			}));
		}

		public void DetectedVideoFormatChanged(object sender, DeckLinkDeviceInputFormatEventArgs e)
		{
			UpdateUIElement(comboBoxVideoFormat, new Action(() => comboBoxVideoFormat.SelectedValue = e.displayMode));

			var format3DVisibility = e.dualStream3D ? Visibility.Visible : Visibility.Collapsed;
			UpdateUIElement(label3DPreview, new Action(() => label3DPreview.Visibility = format3DVisibility));
			UpdateUIElement(comboBox3DPreviewFormat, new Action(() => comboBox3DPreviewFormat.Visibility = format3DVisibility));
		}

		public void InputVideoFrameArrived(object sender, DeckLinkDeviceInputVideoFrameEventArgs e)
		{
			if (e.videoFrame.GetFlags().HasFlag(_BMDFrameFlags.bmdFrameHasNoInputSource))
				return;

			ExtractTimecodeFromFrame(e.videoFrame);
			ExtractVancFromFrame(e.videoFrame);
			ExtractMetadataFromFrame(e.videoFrame);
		}

		public void ProfileChanging(object sender, DeckLinkProfileEventArgs e)
		{
			if (m_selectedDevice == null)
				return;

			e.deckLinkProfile.GetDevice(out IDeckLink deckLink);

			if (deckLink == m_selectedDevice.DeckLink)
			{
				// Profile is change for selected device. Stop existing capture gracefully.
				m_selectedDevice.StopCapture();
			}
		}

		public void ProfileActivated(object sender, DeckLinkProfileEventArgs e)
		{
			e.deckLinkProfile.GetDevice(out IDeckLink deckLink);
			bool deviceActive = DeckLinkDeviceTools.IsDeviceActive(deckLink);

			// Update device state in combo box
			UpdateUIElement(comboBoxDevice, new Action(() => UpdateComboActiveDevice(deckLink, deviceActive)));
		}
		#endregion

		private void ExtractTimecodeFromFrame(IDeckLinkVideoInputFrame inputFrame)
		{
			List<TimecodeData> timecodeDataList = new List<TimecodeData>();

			foreach (var timecodeFormat in kTimecodeFormatList)
			{
				inputFrame.GetTimecode(timecodeFormat.Value, out IDeckLinkTimecode timecode);
				if (timecode != null)
				{
					timecode.GetString(out string timecodeString);
					timecode.GetTimecodeUserBits(out uint userBits);
					timecodeDataList.Add(new TimecodeData()
					{
						Format = timecodeFormat.ToString(),
						Value = timecodeString,
						UserBits = userBits.ToString()
					});
				}
			}
			UpdateUIElement(dataGridTimecode, new Action(() => dataGridTimecode.ItemsSource = timecodeDataList));
		}

		private void ExtractVancFromFrame(IDeckLinkVideoInputFrame inputFrame)
		{
			List<VANCPacketData> vancPacketList = new List<VANCPacketData>();

			var vancPackets = inputFrame as IDeckLinkVideoFrameAncillaryPackets;
			vancPackets.GetPacketIterator(out IDeckLinkAncillaryPacketIterator vancPacketIterator);

			while (true)
			{
				vancPacketIterator.Next(out IDeckLinkAncillaryPacket vancPacket);

				if (vancPacket == null)
					break;

				vancPacket.GetBytes(_BMDAncillaryPacketFormat.bmdAncillaryPacketFormatUInt8, out IntPtr vancDataPtr, out uint vancDataSize);
				byte[] vancData = new byte[vancDataSize];
				Marshal.Copy(vancDataPtr, vancData, 0, vancData.Length);
				vancPacketList.Add(new VANCPacketData()
				{
					Line = vancPacket.GetLineNumber(),
					DID = vancPacket.GetDID().ToString("X2"),
					SDID = vancPacket.GetSDID().ToString("X2"),
					Data = BitConverter.ToString(vancData).Replace("-", " ")
				});
			}
			UpdateUIElement(dataGridVANC, new Action(() => dataGridVANC.ItemsSource = vancPacketList));
		}

		private void ExtractMetadataFromFrame(IDeckLinkVideoInputFrame inputFrame)
		{
			List<MetadataData> metadataList = new List<MetadataData>();
			var videoFrameMetadataExt = inputFrame as IDeckLinkVideoFrameMetadataExtensions;

			// First check for colorspace metadata
			var colorspaceMetadata = new MetadataItemColorspace("Colorspace", _BMDDeckLinkFrameMetadataID.bmdDeckLinkFrameMetadataColorspace);
			metadataList.Add(new MetadataData()
			{
				Item = colorspaceMetadata.ToString(),
				Value = colorspaceMetadata.ValueToString(videoFrameMetadataExt)
			});

			if (inputFrame.GetFlags().HasFlag(_BMDFrameFlags.bmdFrameContainsHDRMetadata))
			{
				// If frame contains HDR metadata, add each to metadata list
				foreach (var metadataItem in kMetadataItemList)
				{
					string metadataValueString = metadataItem.ValueToString(videoFrameMetadataExt);

					if (metadataValueString.Length > 0)
					{
						metadataList.Add(new MetadataData()
						{
							Item = metadataItem.ToString(),
							Value = metadataValueString
						});
					}
				}
			}
			UpdateUIElement(dataGridMetadata, new Action(() => dataGridMetadata.ItemsSource = metadataList));
		}

		private void UpdateComboNewDevice(DeckLinkDevice device, bool deviceActive)
		{
			ComboBoxItem newItem = new ComboBoxItem
			{
				Content = new StringObjectPair<DeckLinkDevice>(device.DisplayName, device),
				IsEnabled = deviceActive
			};
			comboBoxDevice.Items.Add(newItem);

			// If first device, then enable capture interface
			if (comboBoxDevice.Items.Count == 1)
			{
				comboBoxDevice.SelectedIndex = 0;
			}
		}

		private void UpdateComboRemoveDevice(IDeckLink deckLink)
		{
			bool selectedDeviceRemoved = m_selectedDevice?.DeckLink == deckLink;

			// Remove the device from the device dropdown
			foreach (ComboBoxItem item in comboBoxDevice.Items)
			{
				if (((StringObjectPair<DeckLinkDevice>)item.Content).Value.DeckLink == deckLink)
				{
					comboBoxDevice.Items.Remove(item);
					break;
				}
			}

			if (comboBoxDevice.Items.Count == 0)
				m_selectedDevice = null;

			else if (selectedDeviceRemoved)
				ComboBoxSelectFirstEnabledItem(comboBoxDevice);
		}

		private void UpdateComboActiveDevice(IDeckLink deckLink, bool active)
		{
			foreach (ComboBoxItem item in comboBoxDevice.Items)
			{
				if (((StringObjectPair<DeckLinkDevice>)item.Content).Value.DeckLink == deckLink)
				{
					item.IsEnabled = active;

					if (m_selectedDevice?.DeckLink == deckLink)
					{
						if (active)
							// Trigger event to restart capture
							comboBoxDevice_SelectionChanged(comboBoxDevice, null);

						else
							ComboBoxSelectFirstEnabledItem(comboBoxDevice);
					}
					break;
				}
			}
		}

		private void UpdateComboInputConnections()
		{
			if (m_selectedDevice == null)
				return;

			// Disable selection event handler, as the selection takes the current connection setting for the selected device
			comboBoxConnection.SelectionChanged -= comboBoxConnection_SelectionChanged;

			// Bind available input connections of selected device to connections combo-box
			comboBoxConnection.ItemsSource = kInputConnectionList.Where(conn => m_selectedDevice.AvailableInputConnections.HasFlag(conn.Value));
			comboBoxConnection.DisplayMemberPath = "Name";
			comboBoxConnection.SelectedValuePath = "Value";

			var currentInputConnection = new MTAFunc<_BMDVideoConnection>(() => m_selectedDevice.CurrentVideoInputConnection);
			comboBoxConnection.SelectedValue = currentInputConnection.Value;

			// Restore selection event handler
			comboBoxConnection.SelectionChanged += comboBoxConnection_SelectionChanged;
		}

		private void UpdateComboVideoModes()
		{
			if (m_selectedDevice == null)
				return;

			List<DisplayModeEntry> displayModeEntries = new List<DisplayModeEntry>();

			new MTAAction(() =>
			{
				foreach (IDeckLinkDisplayMode displayMode in m_selectedDevice.DisplayModes)
				{
					displayModeEntries.Add(new DisplayModeEntry(displayMode));
				}
			});

			// Bind display mode list to combo 
			comboBoxVideoFormat.ItemsSource = displayModeEntries;
			comboBoxVideoFormat.DisplayMemberPath = "DisplayString";
			comboBoxVideoFormat.SelectedValuePath = "Value";

			if (comboBoxVideoFormat.Items.Count > 0)
				comboBoxVideoFormat.SelectedIndex = 0;
		}

		private void DeckLinkMainThread()
		{
			m_previewCallback = new PreviewCallback();
			m_previewCallback.RenderFrame += RenderD3DImage;
			m_previewCallback.PreviewHelper.Set3DPreviewFormat(kDefault3DPreviewFormat);

			//m_profileCallback = new ProfileCallback();
			//-m_profileCallback.ProfileChanging += ProfileChanging;
			//m_profileCallback.ProfileActivated += ProfileActivated;

			m_deckLinkDeviceDiscovery = new DeckLinkDeviceDiscovery();
			m_deckLinkDeviceDiscovery.DeviceArrived += AddDevice;
			m_deckLinkDeviceDiscovery.DeviceRemoved += RemoveDevice;
			m_deckLinkDeviceDiscovery.Enable();

			m_applicationCloseWaitHandle.WaitOne();

			m_previewCallback.RenderFrame -= RenderD3DImage;

			//m_profileCallback.ProfileChanging -= ProfileChanging;
			//m_profileCallback.ProfileActivated -= ProfileActivated;

			m_deckLinkDeviceDiscovery.Disable();
			m_deckLinkDeviceDiscovery.DeviceArrived -= AddDevice;
			m_deckLinkDeviceDiscovery.DeviceRemoved -= RemoveDevice;
		}

		private void startCapture()
		{
			if (m_selectedDevice == null)
				return;

			bool applyDetectedInputMode = checkBoxAutoDetect.IsChecked ?? false;
			_BMDDisplayMode displayMode = (_BMDDisplayMode)(comboBoxVideoFormat.SelectedValue ?? _BMDDisplayMode.bmdModeUnknown);

			if (displayMode != _BMDDisplayMode.bmdModeUnknown)
				new MTAAction(() => m_selectedDevice.StartCapture(displayMode, m_previewCallback, applyDetectedInputMode));
		}

		private void restartCapture()
		{
			if (m_selectedDevice == null)
				return;

			new MTAAction(() => m_selectedDevice.StopCapture());
			startCapture();
		}

		#region uievents
		// All UI events are in STA apartment thread context, calls to DeckLinkAPI must be performed in MTA thread context
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Bind 3D preview formats to combo box
			comboBox3DPreviewFormat.ItemsSource = k3DPreviewFormatList;
			comboBox3DPreviewFormat.DisplayMemberPath = "Name";
			comboBox3DPreviewFormat.SelectedValuePath = "Value";
			comboBox3DPreviewFormat.SelectedValue = kDefault3DPreviewFormat;
			comboBox3DPreviewFormat.SelectionChanged += comboBox3DPreviewFormat_SelectionChanged;

			m_deckLinkMainThread = new Thread(() => DeckLinkMainThread());
			m_deckLinkMainThread.SetApartmentState(ApartmentState.MTA);
			m_deckLinkMainThread.Start();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			m_applicationCloseWaitHandle.Set();
			m_deckLinkMainThread.Join();
		}

		private void comboBoxDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (m_selectedDevice != null)
			{
				m_selectedDevice.InputFormatChanged -= DetectedVideoFormatChanged;
				m_selectedDevice.VideoFrameArrived -= InputVideoFrameArrived;
				new MTAAction(() => m_selectedDevice.StopCapture());
			}

			m_selectedDevice = null;

			// Reset connection and video format combo box source
			comboBoxConnection.ItemsSource = null;
			comboBoxVideoFormat.ItemsSource = null;

			if (comboBoxDevice.SelectedIndex < 0)
				return;
		
			m_selectedDevice = ((StringObjectPair<DeckLinkDevice>)((ComboBoxItem)comboBoxDevice.SelectedItem).Content).Value;

			if (m_selectedDevice != null)
			{
				m_selectedDevice.InputFormatChanged += DetectedVideoFormatChanged;
				m_selectedDevice.VideoFrameArrived += InputVideoFrameArrived;
			}

			UpdateComboInputConnections();

			UpdateComboVideoModes();

			bool selectedDeviceSupportsAutoDetection = m_selectedDevice?.SupportsFormatDetection ?? false;
			checkBoxAutoDetect.IsChecked = selectedDeviceSupportsAutoDetection;

			var autoDetectVisibility = selectedDeviceSupportsAutoDetection ? Visibility.Visible : Visibility.Collapsed;
			labelAutoDetect.Visibility = autoDetectVisibility;
			checkBoxAutoDetect.Visibility = autoDetectVisibility;
		}

		private void comboBoxConnection_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((m_selectedDevice == null) || (comboBoxConnection.SelectedIndex < 0))
				return;

			var videoInputConnection = (_BMDVideoConnection)comboBoxConnection.SelectedValue;
			new MTAAction(() => m_selectedDevice.CurrentVideoInputConnection = videoInputConnection);

			UpdateComboVideoModes();
		}

		private void comboBoxVideoFormat_SelectionChanged(object sender, RoutedEventArgs e)
		{
			if (m_selectedDevice == null)
				return;

			bool autoDetectModeChange = checkBoxAutoDetect.IsChecked ?? false;
			if (!autoDetectModeChange)
				new MTAAction(() => m_selectedDevice.StopCapture());

			if (!m_selectedDevice.IsCapturing)
				startCapture();
		}

		private void checkBoxAutoDetect_CheckedChanged(object sender, RoutedEventArgs e)
		{
			comboBoxVideoFormat.IsEnabled = !(checkBoxAutoDetect.IsChecked ?? false);
			if (m_selectedDevice?.IsCapturing ?? false)
				restartCapture();
		}

		private void comboBox3DPreviewFormat_SelectionChanged(object sender, RoutedEventArgs e)
		{
			if (comboBox3DPreviewFormat.SelectedIndex < 0)
				return;

			// Preview format combo box was updated, set preview helper 3D format
			var previewFormat = (_BMD3DPreviewFormat)comboBox3DPreviewFormat.SelectedValue;
			new MTAAction(() => m_previewCallback?.PreviewHelper.Set3DPreviewFormat(previewFormat));
		}
		#endregion

		#region combotypes
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

		/// Used for putting other object types into combo boxes.
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

		public interface IMetadataItem
		{
			string ValueToString(IDeckLinkVideoFrameMetadataExtensions extensions);
		}
		public abstract class MetadataItem<T> : IMetadataItem
			/// Used to create mapping of metadata string to the get* function 
		{
			public MetadataItem(string name, _BMDDeckLinkFrameMetadataID id)
			{
				Name = name;
				ID = id;
			}
			public string Name { get; }
			public _BMDDeckLinkFrameMetadataID ID { get; }
			protected abstract T Value(IDeckLinkVideoFrameMetadataExtensions extensions);
			public string ValueToString(IDeckLinkVideoFrameMetadataExtensions extensions)
			{
				return Value(extensions).ToString();
			}
			public override string ToString() => Name;
		}

		public class MetadataItemDouble : MetadataItem<double?>
		{
			public MetadataItemDouble(string name, _BMDDeckLinkFrameMetadataID id) : base(name, id) { }

			protected override double? Value(IDeckLinkVideoFrameMetadataExtensions extensions)
			{
				try
				{
					extensions.GetFloat(ID, out double value);
					return value;
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		public class MetadataItemEOTF : MetadataItem<string>
		{
			private readonly IReadOnlyDictionary<long, string> kEOTFDictionary = new Dictionary<long, string>
			{
				[0] = "SDR",
				[1] = "HDR",
				[2] = "PQ (ST 2084)",
				[3] = "HLG"
			};
			
			public MetadataItemEOTF(string name, _BMDDeckLinkFrameMetadataID id) : base(name, id) { }

			protected override string Value(IDeckLinkVideoFrameMetadataExtensions extensions)
			{
				extensions.GetInt(ID, out long intValue);
				try
				{
					return kEOTFDictionary[intValue];
				}
				catch (KeyNotFoundException)
				{
					return "Unknown EOTF";
				}
			}
		}

		public class MetadataItemColorspace : MetadataItem<string>
		{
			private readonly IReadOnlyDictionary<_BMDColorspace, string> kColorspaceDictionary = new Dictionary<_BMDColorspace, string>
			{
				[_BMDColorspace.bmdColorspaceRec601] = "Rec.601",
				[_BMDColorspace.bmdColorspaceRec709] = "Rec.709",
				[_BMDColorspace.bmdColorspaceRec2020] = "Rec.2020",
			};

			public MetadataItemColorspace(string name, _BMDDeckLinkFrameMetadataID id) : base(name, id) { }

			protected override string Value(IDeckLinkVideoFrameMetadataExtensions extensions)
			{
				extensions.GetInt(ID, out long intValue);
				try
				{
					return kColorspaceDictionary[(_BMDColorspace)intValue];
				}
				catch (KeyNotFoundException)
				{
					return "Unknown Colorspace";
				}
			}
		}
		#endregion
	}
}
