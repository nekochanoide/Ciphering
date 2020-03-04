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
        private byte[] firstLineBytes;
        private byte[] secondLineBytes;
        private byte[] resultLineBytes;
        private string firstBytesInterpretation;
        private string secondBytesInterpretation;
        private string resultBytesInterpretation;
        private string debugLine;
        private string groups;

        public Model()
        {
            //Encoding = Encoding.ASCII;
            Encoding = Encoding.Unicode;
            Random = new Random(19971103);
            IsKeyGenerating = true;
            FirstLine = "Hello, World!";
        }

        public string FirstLine
        {
            get => firstLine; set
            {
                SetTopLine(ref firstLine, value);
                //if (IsKeyGenerating)
                //    KeepKey();
            }
        }

        private void KeepKey()
        {
            var fBytes = Encoding.GetBytes(FirstLine);
            var sBytes = Encoding.GetBytes(SecondLine ?? "");
            var diff = fBytes.Length - sBytes.Length;
            if (fBytes.Length > sBytes.Length)
            {
                var newBytes = new byte[diff];
                Random.NextBytes(newBytes);
                if (Encoding.EncodingName == "US-ASCII")
                    for (int i = 0; i < newBytes.Length; i++)
                        newBytes[i] >>= 1;
                SecondLine += Encoding.GetString(newBytes);
            }
            else
            {
                sBytes = sBytes.Take(sBytes.Length + diff).ToArray();
                SecondLine = Encoding.GetString(sBytes);
            }
        }

        public string SecondLine { get => secondLine;
            set => SetTopLine(ref secondLine, value); }
        public string ResultLine { get => resultLine; set => SetField(ref resultLine, value); }
        public byte[] FirstLineBytes { get => firstLineBytes; set => SetTopBytes(ref firstLineBytes, value); }
        public byte[] SecondLineBytes { get => secondLineBytes; set => SetTopBytes(ref secondLineBytes, value); }
        public byte[] ResultLineBytes { get => resultLineBytes; set => SetResultBytes(ref resultLineBytes, value); }
        public int? SizeOfFirst { get => FirstLineBytes?.Length; }
        public int? SizeOfSecond { get => SecondLineBytes?.Length; }
        public int? SizeOfResult { get => ResultLineBytes?.Length; }
        public bool IsKeyGenerating { get; set; }
        public Encoding Encoding { get; set; }
        public Random Random { get; set; }
        public string DebugLine { get => debugLine; set => SetField(ref debugLine, value); }
        //public string Groups { get => groups; set => SetField(ref groups, value); }
        public string FirstBytesInterpretation
        {
            get => firstBytesInterpretation; set // how to set:
                                                 // check if string length is even,
                                                 // then validate if it fit to be a bytes,
                                                 // then get bytes from string,
                                                 // then set bytes property
            {
                if (value.Length % 2 != 0)
                    return;
                var bytes = new List<byte>();
                for (int i = 0; i < value.Length; i += 2)
                {
                    if (TryConvert.TryToByte(value.Substring(i, 2), out byte @byte))
                        bytes.Add(@byte);
                    else
                        return;
                }
                FirstLineBytes = bytes.ToArray();
                NotifyPropertyChanged();
            }
        }
        public string SecondBytesInterpretation
        {
            get => secondBytesInterpretation; set
            {
                if (value.Length % 2 != 0)
                    return;
                var bytes = new List<byte>();
                for (int i = 0; i < value.Length; i += 2)
                {
                    if (TryConvert.TryToByte(value.Substring(i, 2), out byte @byte))
                        bytes.Add(@byte);
                    else
                        return;
                }
                SecondLineBytes = bytes.ToArray();
                NotifyPropertyChanged();
            }
        }
        public string ResultBytesInterpretation
        {
            get => resultBytesInterpretation; set
            {
                resultBytesInterpretation = value;
            }
        }

        bool SetTopBytes(ref byte[] field, byte[] value, [CallerMemberName] string propertyName = null)
        {
            if (field?.SequenceEqual(value) == true)                
                return false;
            field = value;

            if (propertyName == nameof(FirstLineBytes))
            {
                FirstLine = Encoding.GetString(value);
                FirstBytesInterpretation = value.ToStr();
                if (value.Length == SecondLineBytes?.Length)
                    ResultLineBytes = DoXor(value, SecondLineBytes).ToArray();
            }
            else
            {
                SecondLine = Encoding.GetString(value);
                SecondBytesInterpretation = value.ToStr();
                if (value.Length == FirstLineBytes?.Length)
                    ResultLineBytes = DoXor(value, FirstLineBytes).ToArray();
            }

            NotifyPropertyChanged(propertyName);
            return true;
        }

        bool SetResultBytes(ref byte[] field, byte[] value, [CallerMemberName] string propertyName = null)
        {
            if (SetField(ref field, value, propertyName))
                ResultLine = Encoding.GetString(value);
            return true;
        }

        private IEnumerable<byte> DoXor(byte[] a, byte[] b)
        {
            for (int i = 0; i < a.Length; i++)
                yield return (byte)(a[i] ^ b[i]);
        }

        protected bool SetTopLine(ref string field, string value, [CallerMemberName] string propertyName = null)
        {
            if (value == field)
                return false;
            field = value;

            var input = Encoding.GetBytes(value);
            if (propertyName == nameof(FirstLine))
                FirstLineBytes = input;
            else
                SecondLineBytes = input;

            NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
