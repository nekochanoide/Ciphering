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
            var bytes = Encoding.Unicode.GetBytes(Model.FirstLine);
            Model.Random.NextBytes(bytes);
            //for (int i = 0; i < bytes.Length; i++)
            //    bytes[i] >>= 1;
            Model.SecondLine = Model.Encoding.GetString(bytes);
        }

        private Random SetRandom(int seed) => Model.Random = new Random(seed);
    }
}
