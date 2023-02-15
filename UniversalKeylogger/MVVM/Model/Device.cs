using System;
using UniversalKeylogger.Core;

namespace UniversalKeylogger.MVVM.Model
{
    public class Device : ObservableObject
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                try
                {
                    name = value;
                    OnPropertyChanged();
                }
                catch
                {

                }
            }
        }


        private string id;

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        private string barcode;

        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; OnPropertyChanged(); }
        }


        public Device()
        {
            Name = "HID KEYBOARD";
            Barcode = "";
        }

        public Device(string id)
        {
            Id = id;
            Name = "Unreadable";
            Barcode = "";
        }

        public void DeleteTrash()
        {
            try
            {
                if ((int)Barcode[0] == 16 && (int)Barcode[1] == 113)
                {
                    Barcode = Barcode.Remove(0, 4);
                }
                else if (Barcode[0] == 16 && (int)Barcode[1] == 51 ||
                    Barcode[0] == 16 && (int)Barcode[1] == 70)
                {
                    Barcode = Barcode.Remove(0, 2);
                }
            }
            catch (Exception e)
            {
                
            }


        }
    }
}