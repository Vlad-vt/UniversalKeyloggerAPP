using Linearstar.Windows.RawInput;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using UniversalKeylogger.Core;
using UniversalKeylogger.KeyInput;
using UniversalKeylogger.MVVM.Model;

namespace UniversalKeylogger.MVVM.ViewModel
{
    internal class MainWindowViewModel : ObservableObject
    {

        public RelayCommand ExitProgram { get; set; }

        public RelayCommand HideProgram { get; set; }

        public RelayCommand MaximizeProgram { get; set; }

        private ObservableCollection<Device> _devices;

        public ObservableCollection<Device> Devices
        {
            get { return _devices; }
            set { _devices = value; OnPropertyChanged(); }
        }

        private List<string> _repeatDataScan;

        private Queue<int> _readyToSentBarcode;

        private HttpListener _httpListener = new HttpListener();

        public MainWindowViewModel()
        {

            #region DefaultButtons commands
            ExitProgram = new RelayCommand(o => { Application.Current.Shutdown(); });
            HideProgram = new RelayCommand(o => { Application.Current.MainWindow.WindowState = WindowState.Minimized; });
            MaximizeProgram = new RelayCommand(o =>
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                else
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
            });
            #endregion

            try
            {
                _httpListener.Prefixes.Add("http://localhost:7000/keyloggerdata/");
                _httpListener.Start();
            }
            catch(Exception e)
            {

            }

            _readyToSentBarcode = new Queue<int>();


            Thread serverThread = new Thread(ResponseThread);
            serverThread.IsBackground = true;
            serverThread.Start();

            Devices = new ObservableCollection<Device>();
            _repeatDataScan = new List<string>();

            Thread keyloggerThread = new Thread(delegate ()
            {
                RawInputDevice[] devices = new RawInputDevice[1];
                IEnumerable<RawInputKeyboard> keyboards = new RawInputKeyboard[1];
                try
                {
                    devices = RawInputDevice.GetDevices();
                    keyboards = devices.OfType<RawInputKeyboard>();
                }
                catch (Exception e)
                {
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\log.txt", e.Message + "\n");
                }
                var n = 0;
                foreach (var device in keyboards)
                {
                    try
                    {
                        Devices.Add(new Device(device.Handle.ToString()));
                        _repeatDataScan.Add("");
                        Devices[n].Name = device.ManufacturerName;
                    }
                    catch(Exception e)
                    {
                        Devices.RemoveAt(n);
                        _repeatDataScan.RemoveAt(n);
                        n--;
                    }
                    finally
                    {
                        n++;
                    }
                }
                var window = new RawInputReceiverWindow();
                window.Input += (_sender, _e) =>
                {
                    if (_e != null && _e.Data.Device != null)
                    {
                        try
                        {
                            for (int i = 0; i < Devices.Count; i++)
                            {
                                if (_e.Data.Device.Handle.ToString() == Devices[i].Id)
                                {
                                    if (_repeatDataScan[i] == null || _repeatDataScan[i] != _e.Data.ToString())
                                    {
                                        if((_e.Data as RawInputKeyboardData).Keyboard.Flags == Linearstar.Windows.RawInput.Native.RawKeyboardFlags.None)
                                        {
                                            if((_e.Data as RawInputKeyboardData).Keyboard.VirutalKey == 191 || (_e.Data as RawInputKeyboardData).Keyboard.VirutalKey == 189)
                                            {

                                                Devices[i].Barcode += @"/";
                                                _repeatDataScan[i] = @"/";
                                            }
                                            else
                                            {
                                                Devices[i].Barcode += _e.Data.ToString().ToLower();
                                                _repeatDataScan[i] = _e.Data.ToString().ToLower();
                                            }
                                        }
                                        else if((_e.Data as RawInputKeyboardData).Keyboard.VirutalKey == 191 || (_e.Data as RawInputKeyboardData).Keyboard.VirutalKey == 189)
                                        {

                                        }
                                    }
                                    else
                                        _repeatDataScan[i] = "";
                                }
                            }
                        }
                        catch (ArgumentOutOfRangeException e) { File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\log.txt", e.Message + "\n"); }
                        catch (NullReferenceException e) { File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\log.txt", e.Message + "\n"); }
                    }
                };
                try
                {
                    Thread.Sleep(1000);
                    RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard, RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, window.Handle);
                    System.Windows.Forms.Application.Run();
                }
                finally
                {
                    RawInputDevice.UnregisterDevice(HidUsageAndPage.Keyboard);
                }

            });
            keyloggerThread.IsBackground = true;
            keyloggerThread.Start();


            List<Thread> checkBarcodes = new List<Thread>();
            Thread.Sleep(3000);
            for (int i=0; i < Devices.Count; i++ )
            {
                checkBarcodes.Add(new Thread(() =>
                {
                    Thread.Sleep(4000);
                    var num = i - 1;
                    string barcodeDevice = string.Empty;
                    while (true)
                    {
                        try
                        {
                            if (barcodeDevice.Length != Devices[num].Barcode.Length)
                            {
                                barcodeDevice = Devices[num].Barcode;
                            }
                            else if (barcodeDevice.Length != 0)
                            {
                                Devices[num].DeleteTrash();
                                _readyToSentBarcode.Enqueue(num);
                                barcodeDevice = "";
                            }
                        }
                        catch (Exception e)
                        {
                           
                        }
                        Thread.Sleep(500);
                    }
                }));
                checkBarcodes[i].IsBackground = true;
                checkBarcodes[i].Start();
                Thread.Sleep(4000);
            }

        }

        private void ResponseThread()
        {
            while (true)
            {
                HttpListenerContext context = _httpListener.GetContext(); // get a context
                context.Response.ContentType = "text/plain, charset=UTF-8";
                context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                context.Response.ContentEncoding = System.Text.Encoding.UTF8;                                                        // Now, you'll find the request URL in context.Request.Url
                switch (context.Request.RawUrl)
                {
                    case "/keyloggerdata":
                        SendFileToBrowser(ref context);
                        break;
                }
                context.Response.KeepAlive = false;
                context.Response.Close();
            }
        }

        private void SendFileToBrowser(ref HttpListenerContext response)
        {
            if (_readyToSentBarcode.Count > 0)
            {
                string json = "";
                using (var reader = new StreamReader(response.Request.InputStream))
                {
                    switch (reader.ReadToEnd())
                    {
                        case "getdata":
                            var elemNumber = _readyToSentBarcode.Peek();
                            json = JsonConvert.SerializeObject(new BarcodeJSON(Devices[elemNumber].Barcode), Formatting.Indented);
                            break;
                        case "gotit":
                            json = "dataget";
                            var elemNumber2 = _readyToSentBarcode.Dequeue();
                            Devices[elemNumber2].Barcode = "";
                            break;
                    }
                }
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
                response.Response.ContentLength64 = buffer.Length;
                response.Response.OutputStream.Write(buffer, 0, buffer.Length);

            }
        }

    }

}
