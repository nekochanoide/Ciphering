using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Module.Gamma
{
    class ViewModel
    {
        public ViewModel() {
            Model = new Model();
        }

        public Model Model { get; set; }

        private ICommand _generateKeyCommand;
        public ICommand GenerateKeyCommand
            => _generateKeyCommand ?? (_generateKeyCommand = new Command(GenerateKey, () => !string.IsNullOrWhiteSpace(Model.FirstLine)));

        void GenerateKey() {
            var bytes = Model.Encoding.GetBytes(Model.FirstLine);
            Model.Random.NextBytes(bytes);
            if (Model.Encoding.EncodingName == "US-ASCII")
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] >>= 1;
            Model.SecondLine = Model.Encoding.GetString(bytes);
        }

        private ICommand _byteViewCommand;
        public ICommand ByteViewCommand
            => _byteViewCommand ?? (_byteViewCommand = new Command(ToByteView, () => true));

        private void ToByteView() {
            var fBytes = Model.Encoding.GetBytes(Model.FirstLine);
            Model.FirstBytesInterpretation = fBytes.ToStr();
            var sBytes = Model.Encoding.GetBytes(Model.SecondLine);
            Model.SecondBytesInterpretation = sBytes.ToStr();
            var rBytes = Model.Encoding.GetBytes(Model.ResultLine);
            Model.ResultBytesInterpretation = rBytes.ToStr();
        }

        private ICommand _stringViewCommand;
        public ICommand StringViewCommand
            => _stringViewCommand ?? (_stringViewCommand = new Command(ToStringView, () => true));

        private void ToStringView() {
            var fBytes = Model.FirstBytesInterpretation.ReadBytes();
            Model.FirstLine = Model.Encoding.GetString(fBytes);
            var sBytes = Model.SecondBytesInterpretation.ReadBytes();
            Model.SecondLine = Model.Encoding.GetString(sBytes);
            var rBytes = Model.ResultBytesInterpretation.ReadBytes();
            Model.ResultLine = Model.Encoding.GetString(rBytes);
        }

        private ICommand _decipherCommand;
        public ICommand DecipherCommand
            => _decipherCommand ?? (_decipherCommand = new Command(Decipher, () => true));

        private void Decipher() {
            var fBytes = Model.FirstBytesInterpretation.ReadBytes();
            var sBytes = Model.SecondBytesInterpretation.ReadBytes();
            if (fBytes.Length != sBytes.Length) {
                Model.DebugLine = "different length!";
                return;
            }
            var rBytes = DoXor(fBytes, sBytes).ToArray();
            Model.ResultLine = Model.Encoding.GetString(rBytes);
            Model.ResultBytesInterpretation = rBytes.ToStr();
        }

        private IEnumerable<byte> DoXor(byte[] a, byte[] b) {
            for (int i = 0; i < a.Length; i++)
                yield return (byte)(a[i] ^ b[i]);
        }

        private ICommand _groupCommand;
        public ICommand GroupCommand
            => _groupCommand ?? (_groupCommand = new Command(Group, () => true));

        void Group() {
            var keyBytes = Model.SecondBytesInterpretation.ReadBytes();
            var keyFileString = new StringBuilder();
            for (int i = 0; i < 10; i++) {
                var e = PackToGroup(keyBytes);
                keyFileString.AppendLine(e.ToStr());
            }
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            assemblyLocation = Path.GetDirectoryName(assemblyLocation);
            var name = Path.Combine(assemblyLocation, "key.txt");
            using (StreamWriter file = new StreamWriter(name, false))
                file.Write(keyFileString.ToString());            
            Process.Start(name);
        }

        public ICommand _unpackFromGroupCommand;
        public ICommand UnpackFromGroupCommand
            => _unpackFromGroupCommand ?? (_unpackFromGroupCommand = new Command(RegenerateKey, () => !string.IsNullOrWhiteSpace(Model.Groups)));

        void RegenerateKey() {
            var bytes = Model.Groups.ReadBytes();
            var key = UnpackFromGroup(bytes);
            Model.SecondBytesInterpretation = key.ToStr();
        }

        byte[] PackToGroup(byte[] bytes) {
            var r = (byte)Model.Random.Next(2, 200);
            byte[] clone = (byte[])bytes.Clone();
            for (int i = 0; i < clone.Length; i++) {
                clone[i] += r;
            }
            return clone.Append(r).ToArray();
        }

        static byte[] UnpackFromGroup(byte[] bytes) {
            var r = bytes[bytes.Length - 1];
            bytes = bytes.Take(bytes.Length - 1).ToArray();
            for (int i = 0; i < bytes.Length; i++) {
                bytes[i] -= r;
            }
            return bytes;
        }
    }
}
