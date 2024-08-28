// In AssemblyInfo.cs or a separate file

[assembly: Parallelizable(ParallelScope.Children)] //enable me when tests take more than 400 ms
//[assembly: LevelOfParallelism(4)] // Adjust the number as needed