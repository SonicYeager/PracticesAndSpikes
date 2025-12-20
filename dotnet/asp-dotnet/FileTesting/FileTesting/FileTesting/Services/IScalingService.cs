namespace FileTesting.Services;

public interface IScalingService
{
    (byte[] ImageBytes, int Width, int Height, string Method) ScaleImageAsync(byte[] inputBytes, int maxWidth, int maxHeight);
}