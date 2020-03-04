using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Module.Gamma
{
    class ViewModel
    {
        public ViewModel()
        {
            Model = new Model();
        }
        public Model Model { get; set; }

        private ICommand _generateKeyCommand;
        public ICommand GenerateKeyCommand
            => _generateKeyCommand ?? (_generateKeyCommand = new Command(GenerateKey, () => !string.IsNullOrWhiteSpace(Model.FirstLine)));

        void GenerateKey()
        {
            var bytes = Model.Encoding.GetBytes(Model.FirstLine);
            Model.Random.NextBytes(bytes);
            //if (Model.Encoding.EncodingName == "US-ASCII")
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] >>= 1;
            Model.SecondLine = Model.Encoding.GetString(bytes);
        }

        //public ICommand _packToGroupCommand;
        //public ICommand PackToGroupCommand
        //    => _packToGroupCommand ?? (_packToGroupCommand = new Command(GenerateGroup, 
        //        () => !string.IsNullOrWhiteSpace(Model.SecondLine)));

        //public ICommand _unpackFromGroupCommand;
        //public ICommand UnpackFromGroupCommand
        //    => _unpackFromGroupCommand ?? (_unpackFromGroupCommand = new Command(RegenerateKey, 
        //        () => !string.IsNullOrWhiteSpace(Model.Groups)));

        //private void GenerateGroup()
        //{
        //    var bytes = Model.Encoding.GetBytes(Model.SecondLine);
        //    bytes = PackToGroup(bytes);
        //    Model.Groups = Model.Encoding.GetString(bytes);
        //}

        //private void RegenerateKey()
        //{
        //    var bytes = Model.Encoding.GetBytes(Model.Groups);
        //    bytes = UnpackFromGroup(bytes);
        //    Model.SecondLine = Model.Encoding.GetString(bytes);
        //}

        static byte[] PackToGroup(byte[] bytes)
        {
            var r = (byte)new Random().Next(2, 9);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] += r;
                bytes[i] ^= r;
            }
            return bytes.Append(r).ToArray();
        }

        static byte[] UnpackFromGroup(byte[] bytes)
        {
            var r = bytes[bytes.Length - 1];
            bytes = bytes.Take(bytes.Length - 1).ToArray();
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= r;
                bytes[i] -= r;
            }
            return bytes;
        }

        private Random SetRandom(int seed) => Model.Random = new Random(seed);
    }
}
