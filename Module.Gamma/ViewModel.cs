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
            _random = SetRandom();
            Model = new Model();
        }
        Random _random;
        public Model Model { get; set; }

        private ICommand _generateKeyCommand;
        public ICommand GenerateKeyCommand
            => _generateKeyCommand ?? (_generateKeyCommand = new Command(GenerateKey, () => !string.IsNullOrWhiteSpace(Model.FirstLine)));

        void GenerateKey()
        {
            var bytes = Encoding.Unicode.GetBytes(Model.FirstLine);
            _random.NextBytes(bytes);
            //for (int i = 0; i < bytes.Length; i++)
            //    bytes[i] >>= 1;
            Model.SecondLine = Model.Encoding.GetString(bytes);
        }

        private Random SetRandom() => _random = new Random();
    }
}
