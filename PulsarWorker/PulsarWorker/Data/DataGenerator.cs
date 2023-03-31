using AutoBogus;
using AutoBogus.Conventions;

namespace PulsarWorker.Data
{
    public static class DataGenerator
    {
        public static TType Generate<TType>() where TType : class
        {
            // Configure globally
            AutoFaker.Configure(builder =>
            {
                builder
                    .WithRepeatCount(5) // Configures the number of items in a collection
                    .WithDataTableRowCount(5) // Configures the number of data table rows to generate
                    .WithRecursiveDepth(3) // Configures how deep nested types should recurse
                    .WithConventions()
                    .WithTreeDepth(3);
            });

            return new AutoFaker<TType>()
                .Generate();
        }
    }
}
