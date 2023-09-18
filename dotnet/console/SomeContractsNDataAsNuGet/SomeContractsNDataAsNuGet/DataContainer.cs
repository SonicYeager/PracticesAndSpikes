using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeContractsNDataAsNuGet
{
    public struct DataPackage : IEquatable<DataPackage>
    {
        public string Name { get; set; }
        public Stream GenericData { get; set; }
        public readonly Memory<char> Key { get; init; }

        public override bool Equals(object? obj)
        {
            return obj is DataPackage package && Equals(package);
        }

        public bool Equals(DataPackage other)
        {
            return Name == other.Name &&
                   EqualityComparer<Stream>.Default.Equals(GenericData, other.GenericData) &&
                   Key.Equals(other.Key);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, GenericData, Key);
        }

        public static bool operator ==(DataPackage left, DataPackage right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataPackage left, DataPackage right)
        {
            return !(left == right);
        }
    }
}
