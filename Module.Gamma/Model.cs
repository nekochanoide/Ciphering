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

        public string FirstLine
        {
            get => firstLine; set
            {
                SetLine(ref firstLine, x => x.FirstLineBytes, value);
                if (IsKeyGenerating)
                    KeepKey();
            }
        }

        private void KeepKey()
        {
            while (true)
            {
                var fBytes = Encoding.GetBytes(FirstLine);
                var sBytes = Encoding.GetBytes(SecondLine);
                var diff = fBytes.Length - sBytes.Length;
                if (fBytes.Length > sBytes.Length)
                {
                    var newBytes = new byte[diff];
                    Random.NextBytes(newBytes);
                    SecondLine += Encoding.GetString(newBytes);
                }
                else
                {
                    sBytes = sBytes.Take(sBytes.Length + diff).ToArray();
                    SecondLine = Encoding.GetString(sBytes);
                    break;
                }
            }
        }

        public string SecondLine { get => secondLine; set => SetLine(ref secondLine, x => x.SecondLineBytes, value); }
        public string ResultLine { get => resultLine; set => SetField(ref resultLine, value); }
        public byte[] FirstLineBytes { get => firstLineBytes; set => SetTopBytes(ref firstLineBytes, value); }
        public byte[] SecondLineBytes { get => secondLineBytes; set => SetTopBytes(ref secondLineBytes, value); }
        public byte[] ResultLineBytes { get => resultLineBytes; set => SetResultBytes(ref resultLineBytes, value); }
        public bool IsKeyGenerating { get; set; }
        public Encoding Encoding { get; set; } = Encoding.Unicode;
        public Random Random { get; set; } = new Random();

        bool SetTopBytes(ref byte[] field, byte[] value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<byte[]>.Default.Equals(field, value))
                return false;
            field = value;

            if (propertyName == nameof(FirstLineBytes))
            {
                if (value.Length == SecondLineBytes.Length)
                    ResultLineBytes = DoXor(value, SecondLineBytes).ToArray();
            }
            else if (value.Length == FirstLineBytes.Length)
                ResultLineBytes = DoXor(value, FirstLineBytes).ToArray();

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

        protected bool SetLine<T>(ref T field, Expression<Func<Model, byte[]>> expression, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;

            var expr = (MemberExpression)expression.Body;
            var prop = (PropertyInfo)expr.Member;
            var input = Encoding.GetBytes(value.ToString());
            prop.SetValue(this, input, null);

            NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
