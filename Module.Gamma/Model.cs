using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Module.Gamma
{
    class Model : NotifyPropertyChangedBase
    {
        private string firstLine;
        private string secondLine;
        private string resultLine;
        private string firstBytesInterpretation;
        private string secondBytesInterpretation;
        private string resultBytesInterpretation;
        private string debugLine;
        private string groups;

        public Model()
        {
            Encoding = Encoding.Unicode;
            Random = new Random(19971103);
            FirstLine = "Hello, World!";
            SecondLine = "";
            ResultLine = "";
            FirstBytesInterpretation = "";
            SecondBytesInterpretation = "";
            ResultBytesInterpretation = "";
        }

        public string FirstLine
        {
            get => firstLine; set
            {
                SetTopLine(ref firstLine, value);
                NotifyPropertyChanged(nameof(SizeOfFirst));
            }
        }
        public string SecondLine
        {
            get => secondLine; set
            {
                SetTopLine(ref secondLine, value);
                NotifyPropertyChanged(nameof(SizeOfSecond));
            }
        }
        public string ResultLine
        {
            get => resultLine; set
            {
                SetField(ref resultLine, value);
                NotifyPropertyChanged(nameof(SizeOfResult));
            }
        }
        //public byte[] FirstLineBytes { get => firstLineBytes; set => SetTopBytes(ref firstLineBytes, value); }
        //public byte[] SecondLineBytes { get => secondLineBytes; set => SetTopBytes(ref secondLineBytes, value); }
        //public byte[] ResultLineBytes { get => resultLineBytes; set => SetResultBytes(ref resultLineBytes, value); }
        public int SizeOfFirst { get => Encoding.GetByteCount(FirstLine); }    //NOTIFY!
        public int SizeOfSecond { get => Encoding.GetByteCount(SecondLine); }  //NOTIFY!
        public int SizeOfResult { get => Encoding.GetByteCount(ResultLine); }  //NOTIFY!
        public Encoding Encoding { get; set; }
        public Random Random { get; set; }
        public string DebugLine { get => debugLine; set => SetField(ref debugLine, value); }
        //public string Groups { get => groups; set => SetField(ref groups, value); }
        public string FirstBytesInterpretation
        {
            get => firstBytesInterpretation;
            set
            {
                SetField(ref firstBytesInterpretation, value);
                //NotifyPropertyChanged();
            }
        }
        public string SecondBytesInterpretation
        {
            get => secondBytesInterpretation;
            set
            {
                SetField(ref secondBytesInterpretation, value);
                //NotifyPropertyChanged();
            }
        }
        public string ResultBytesInterpretation
        {
            get => resultBytesInterpretation; set
            {
                SetField(ref resultBytesInterpretation, value);
                //resultBytesInterpretation = value;
            }
        }

        bool SetResultBytes(ref byte[] field, byte[] value, [CallerMemberName] string propertyName = null)
        {
            if (SetField(ref field, value, propertyName))
                ResultLine = Encoding.GetString(value);
            return true;
        }

        protected bool SetTopLine(ref string field, string value, [CallerMemberName] string propertyName = null)
        {
            if (value == field)
                return false;
            field = value;

            NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
