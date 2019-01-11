using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfValidationSample.VM
{
    public class Model1 : INotifyPropertyChanged, IDataErrorInfo
    {
        private string ta;
        private string tb;
        private int ia;
        private int ib;

        public event PropertyChangedEventHandler PropertyChanged;

        public string TextA
        {
            set
            {
                ta = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextA"));
            }
            get
            {
                return ta;
            }
        }

        public string TextB
        {
            set
            {
                tb = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TextB"));
            }
            get
            {
                return tb;
            }
        }

        public int IntA
        {
            set
            {
                //move customized data check to IDataErrorInfo
                //throw error before set value
                //if (value > 10)
                //{
                //    throw new Exception();
                //}

                ia = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IntA"));
            }
            get
            {
                return ia;
            }
        }

        public int IntB
        {
            set
            {
                //move customized data check to IDataErrorInfo
                //throw error before set value
                //if (value > 10)
                //{
                //    throw new Exception();
                //}

                ib = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IntB"));
            }
            get
            {
                return ib;
            }
        }

        public string Error
        {
            get
            {
                return null;
            }
        }
        public string this[string propertyName]
        {
            get
            {
                string result = null;

                if (propertyName == "IntA")
                {
                    if (this.IntA > 12)
                    {
                        result = "IntA should not bigger than 12";
                    }
                }

                if (propertyName == "IntB")
                {
                    if (this.IntA < 100)
                    {
                        result = "IntB should not smaller than 100";
                    }
                }

                return result;
            }
        }

        public override string ToString()
        {
            return base.ToString() + $"[TextA:{TextA},TextB:{TextB},IntA:{IntA},IntB:{IntB}]";
        }
    }
}
