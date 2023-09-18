using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomDataGenerator;

namespace SomeContractsNDataAsNuGet
{
    public class DapperUser
    {
        public DapperUser()
        {
        }
        
        public string RandomText(int length)
        {
            var gen = new RandomDataGenerator.Randomizers.RandomizerEmailAddress(new RandomDataGenerator.FieldOptions.FieldOptionsEmailAddress());
            var text = gen.Generate();
            return text;
        }
    }
}
